using Dotnetydd.QuartzJob.Model;
using Newtonsoft.Json;
using Quartz;

namespace Dotnetydd.QuartzJob.Stores
{
    public class JsonFileJobInfoStore : IJobInfoStore
    {

        private readonly string _defaultDir;
        private readonly string _defaultJobFile;


        public JsonFileJobInfoStore()
        {
            _defaultDir = Path.Combine(Environment.CurrentDirectory, "JobData");
            _defaultJobFile = Path.Combine(_defaultDir, "Jobs.json");
            CreateDirectoryIfNotExists();

            var setting = Path.Combine(Environment.CurrentDirectory, "jobsetting.json");
            ReadOrInitFromFile(setting);
        }

        IEnumerable<JobInfoModel> StaticJobs(string path)
        {
            var text = string.Empty;
            if (File.Exists(path))
            {
                text = File.ReadAllText(path);
            }

            if (!string.IsNullOrEmpty(text))
            {
                var settings = JsonConvert.DeserializeObject<List<JobSetting>>(text);
                foreach (var job in settings)
                {
                    yield return new JobInfoModel
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        TaskType = EnumTaskType.StaticExecute,
                        TaskName = job.Name,
                        DisplayName = job.DisplayName,
                        GroupName = job.GroupName,
                        Interval = job.Cron,
                        Describe = job.Description,
                        Status = job.IsOpen ? EnumJobStates.Normal : EnumJobStates.Stopped
                    };
                }
            }
        }


        private void ReadOrInitFromFile(string path)
        {
            if (!File.Exists(_defaultJobFile))
            {
                using (File.CreateText(_defaultJobFile))
                { }
            }
            else
            {
                var text = File.ReadAllText(_defaultJobFile);
                if (!string.IsNullOrEmpty(text))
                {
                    var jobs = JsonConvert.DeserializeObject<List<JobInfoModel>>(text);
                    _jobs.AddRange(jobs);
                }
            }

            var staticJobs = StaticJobs(path)?.ToList();
            if (staticJobs != null && staticJobs.Any())
                _jobs.AddRange(staticJobs);
        }


        private void CreateDirectoryIfNotExists()
        {
            if (!Directory.Exists(_defaultDir))
            {
                Directory.CreateDirectory(_defaultDir);
            }
        }


        private static readonly List<JobInfoModel> _jobs = new List<JobInfoModel>();



        /// <inheritdoc />
        public Task<int> CountAsync()
        {
            return Task.FromResult(_jobs.Count);
        }

        /// <inheritdoc />
        public async Task<List<JobInfoModel>> GetListAsync()
        {
            return await Task.FromResult(_jobs);
        }

        /// <inheritdoc />
        public async Task<JobInfoModel> GetAsync(string id)
        {
            JobInfoModel model = _jobs.FirstOrDefault(x => x.Id == id);
            return await Task.FromResult(model);
        }

        /// <inheritdoc />
        public async Task<JobInfoModel> GetAsync(JobKey job)
        {
            JobInfoModel model = _jobs.FirstOrDefault(x => x.GroupName == job.Group && x.TaskName == job.Name);
            return await Task.FromResult(model);
        }

        /// <inheritdoc />
        public async Task AddAsync(JobInfoModel job)
        {
            if (_jobs.Any(x => x.GroupName == job.GroupName && x.TaskName == job.TaskName))
            {
                return;
                //throw new InvalidOperationException("任务已存在");
            }

            _jobs.Add(job);
            await SaveAllAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(JobInfoModel job)
        {
            if (!_jobs.Any(x => x.GroupName == job.GroupName && x.TaskName == job.TaskName))
            {
                throw new InvalidOperationException("任务不存在");
            }

            JobInfoModel old = _jobs.FirstOrDefault(x => x.GroupName == job.GroupName && x.TaskName == job.TaskName);
            _jobs.Remove(old);
            _jobs.Add(job);
            await SaveAllAsync();
        }

        /// <inheritdoc />
        public async Task RemoveAsync(string groupName, string jobName)
        {
            JobInfoModel old = _jobs.FirstOrDefault(x => x.GroupName == groupName && x.TaskName == jobName);
            _jobs.Remove(old);
            await SaveAllAsync();
        }

        /// <inheritdoc />
        public async Task PauseAsync(string groupName, string jobName)
        {
            JobInfoModel old = _jobs.FirstOrDefault(x => x.GroupName == groupName && x.TaskName == jobName);
            if (old == null)
            {
                throw new InvalidOperationException("任务不存在");
            }

            old.Status = EnumJobStates.Pause;
            JobInfoModel @new = old;
            _jobs.Remove(old);
            _jobs.Add(@new);
            await SaveAllAsync();
        }

        /// <inheritdoc />
        public async Task SaveAllAsync()
        {
            var saveto = _jobs.Where(x => x.TaskType != EnumTaskType.StaticExecute);
            if (!saveto.Any())
            {
                return;
            }

#if NET6_0
            await using (var sw = File.CreateText(_defaultJobFile))
            {
                await sw.WriteAsync(JsonConvert.SerializeObject(saveto));
            }
#endif

#if NET46
            using (var sw = File.CreateText(_defaultJobFile))
            {
                await sw.WriteAsync(JsonConvert.SerializeObject(saveto));
            }
#endif

        }
    }
}


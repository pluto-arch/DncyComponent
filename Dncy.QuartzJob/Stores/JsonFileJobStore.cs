using System.Diagnostics;
using System.Net.Mime;
using Dncy.QuartzJob.Model;
using Quartz;
using System.Text;
using System.Text.Json;

namespace Dncy.QuartzJob.Stores
{
    public class JsonFileJobStore : IJobInfoStore
    {

        private readonly string _defaultDir;
        private readonly string _defaultJobFile;

        public JsonFileJobStore()
        {
            _defaultDir = Path.Combine(Environment.CurrentDirectory, "Data");
            _defaultJobFile = Path.Combine(_defaultDir, "Jobs.json");
            CreateDirectoryIfNotExists();

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Console.CancelKeyPress += Console_CancelKeyPress;

            ReadOrInitFromFile();
        }

        private void ReadOrInitFromFile()
        {
            if (!File.Exists(_defaultJobFile))
            {
                File.Create(_defaultJobFile);
            }
            else
            {
                var text = File.ReadAllText(_defaultJobFile);
                if (!string.IsNullOrEmpty(text))
                {
                    var jobs = JsonSerializer.Deserialize<List<JobInfoModel>>(text);
                    jobs.RemoveAll(x => x.TaskType == EnumTaskType.StaticExecute);
                    jobs.AddRange(jobs);
                }
            }
        }


        private void CreateDirectoryIfNotExists()
        {
            if (!Directory.Exists(_defaultDir))
            {
                Directory.CreateDirectory(_defaultDir);
            }
        }


        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
           
            try
            {
                using (var sw=File.CreateText(_defaultJobFile))
                {
                    sw.Write(JsonSerializer.Serialize(jobs));
                }
            }
            finally
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                using (var sw=File.CreateText(_defaultJobFile))
                {
                    sw.Write(JsonSerializer.Serialize(jobs));
                }
            }
            finally
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            try
            {
                using (var sw=File.CreateText(_defaultJobFile))
                {
                    sw.Write(JsonSerializer.Serialize(jobs));
                }
            }
            finally
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        private static readonly List<JobInfoModel> jobs = new();



        /// <inheritdoc />
        public Task<int> CountAsync()
        {
            return Task.FromResult(jobs.Count);
        }

        /// <inheritdoc />
        public async Task<List<JobInfoModel>> GetListAsync()
        {
            return await Task.FromResult(jobs);
        }

        /// <inheritdoc />
        public async Task<JobInfoModel> GetAsync(string id)
        {
            JobInfoModel model = jobs.FirstOrDefault(x => x.Id == id);
            return await Task.FromResult(model);
        }

        /// <inheritdoc />
        public async Task<JobInfoModel> GetAsync(JobKey job)
        {
            JobInfoModel model = jobs.FirstOrDefault(x => x.GroupName == job.Group && x.TaskName == job.Name);
            return await Task.FromResult(model);
        }

        /// <inheritdoc />
        public async Task AddAsync(JobInfoModel job)
        {
            if (jobs.Any(x => x.GroupName == job.GroupName && x.TaskName == job.TaskName))
            {
                throw new InvalidOperationException("任务已存在");
            }

            jobs.Add(job);
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(JobInfoModel job)
        {
            if (!jobs.Any(x => x.GroupName == job.GroupName && x.TaskName == job.TaskName))
            {
                throw new InvalidOperationException("任务不存在");
            }

            JobInfoModel old = jobs.FirstOrDefault(x => x.GroupName == job.GroupName && x.TaskName == job.TaskName);
            jobs.Remove(old);
            jobs.Add(job);
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task RemoveAsync(string groupName, string jobName)
        {
            JobInfoModel old = jobs.FirstOrDefault(x => x.GroupName == groupName && x.TaskName == jobName);
            jobs.Remove(old);
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task PauseAsync(string groupName, string jobName)
        {
            JobInfoModel old = jobs.FirstOrDefault(x => x.GroupName == groupName && x.TaskName == jobName);
            if (old == null)
            {
                throw new InvalidOperationException("任务不存在");
            }

            old.Status = EnumJobStates.Pause;
            JobInfoModel @new = old;
            jobs.Remove(old);
            jobs.Add(@new);
            await Task.CompletedTask;
        }
    }
}


using Dotnetydd.QuartzJob.Model;
using Dotnetydd.QuartzJob.Utils;
using Quartz;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnetydd.QuartzJob.Stores
{
    public class InMemoryJobLogStore : IJobLogStore
    {
        private const int QUEUE_LENGTH = 20;
        private static readonly Dictionary<string, FixLengthQueue> JobLog = new Dictionary<string, FixLengthQueue>();
        public Task RecordAsync(JobKey job, JobLogModel model)
        {
            string key = $"{job.Group}_{job.Name}";
            if (!JobLog.ContainsKey(key))
            {
                JobLog[key] = new FixLengthQueue(QUEUE_LENGTH);
            }

            JobLog[key].Enqueue(model);
            return Task.CompletedTask;
        }

        public Task<List<JobLogModel>> GetListAsync(JobKey job, int count = 20)
        {
            string key = $"{job.Group}_{job.Name}";
            if (!JobLog.ContainsKey(key))
            {
                return Task.FromResult(new List<JobLogModel>());
            }

            object[] logs = JobLog[key].ToArray();
            List<JobLogModel> res = logs.OrderByDescending(x => ((JobLogModel)x)?.Time).Take(count)
                .Select(x => (JobLogModel)x).ToList();
            return Task.FromResult(res);
        }

        /// <inheritdoc />
        public Task<List<JobLogModel>> GetListAsync(JobKey job, int pageNo = 1, int count = 20)
        {
            string key = $"{job.Group}_{job.Name}";
            if (!JobLog.ContainsKey(key))
            {
                return Task.FromResult(new List<JobLogModel>());
            }

            object[] logs = JobLog[key].ToArray();
            List<JobLogModel> res = logs.OrderByDescending(x => ((JobLogModel)x)?.Time).Skip((pageNo - 1) * count).Take(count)
                .Select(x => (JobLogModel)x).ToList();
            return Task.FromResult(res);
        }
    }


    public class NullJobLogStore : IJobLogStore
    {
        public Task RecordAsync(JobKey job, JobLogModel model)
        {
            return Task.CompletedTask;
        }

        public Task<List<JobLogModel>> GetListAsync(JobKey job, int count = 20)
        {
            return Task.FromResult(new List<JobLogModel>());
        }

        /// <inheritdoc />
        public Task<List<JobLogModel>> GetListAsync(JobKey job, int pageNo = 1, int count = 20)
        {
            return Task.FromResult(new List<JobLogModel>());
        }


        public static NullJobLogStore Instrance => new NullJobLogStore();
    }

}


using Dotnetydd.QuartzJob.Model;
using Quartz;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnetydd.QuartzJob.Stores
{
    public interface IJobLogStore
    {
        /// <summary>
        ///     记录日志
        /// </summary>
        /// <returns></returns>
        Task RecordAsync(JobKey job, JobLogModel model);

        /// <summary>
        ///     获取日志
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<List<JobLogModel>> GetListAsync(JobKey job, int count = 20);


        /// <summary>
        ///     获取日志
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<List<JobLogModel>> GetListAsync(JobKey job, int pageNo = 1, int count = 20);
    }

}


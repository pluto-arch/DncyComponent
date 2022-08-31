using Dncy.QuartzJob.Model;
using Quartz;

namespace Dncy.QuartzJob.Stores;

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
}
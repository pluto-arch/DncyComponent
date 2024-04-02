using Dotnetydd.QuartzHost.Models;
using Quartz;

namespace Dotnetydd.QuartzHost.Storage;

public interface IJobLogStore
{
    /// <summary>
    ///     记录日志
    /// </summary>
    /// <returns></returns>
    Task AddAsync(JobLogModel model);

    /// <summary>
    ///     获取日志
    /// </summary>
    /// <returns></returns>
    Task<List<JobLogModel>> GetListAsync(string jobKey, int count = 20);


    /// <summary>
    ///     获取日志
    /// </summary>
    /// <returns></returns>
    Task<List<JobLogModel>> GetListAsync(string jobKey, int pageNo = 1, int count = 20);
}




public class NullJobLogStore : IJobLogStore
{
    public Task AddAsync(JobLogModel model)
    {
        return Task.CompletedTask;
    }

    public Task<List<JobLogModel>> GetListAsync(string jobKey, int count = 20)
    {
        return Task.FromResult(new List<JobLogModel>());
    }

    /// <inheritdoc />
    public Task<List<JobLogModel>> GetListAsync(string jobKey, int pageNo = 1, int count = 20)
    {
        return Task.FromResult(new List<JobLogModel>());
    }


    public static NullJobLogStore Instrance => new NullJobLogStore();
}
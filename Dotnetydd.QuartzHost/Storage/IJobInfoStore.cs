using Dotnetydd.QuartzHost.Models;
using Quartz;

namespace Dotnetydd.QuartzHost.Storage;

public interface IJobInfoStore
{
    /// <summary>
    ///     总数
    /// </summary>
    /// <returns></returns>
    Task<int> CountAsync();

    /// <summary>
    ///     获取job 列表
    /// </summary>
    /// <returns></returns>
    Task<List<JobInfoModel>> GetListAsync();


    /// <summary>
    ///     获取job 列表
    /// </summary>
    /// <returns></returns>
    Task<JobInfoModel> GetAsync(string id);


    /// <summary>
    ///     获取job 列表
    /// </summary>
    /// <returns></returns>
    Task<JobInfoModel> GetAsync(JobKey job);


    /// <summary>
    ///     添加job
    /// </summary>
    /// <returns></returns>
    Task AddAsync(JobInfoModel job);


    /// <summary>
    ///     添加job
    /// </summary>
    /// <returns></returns>
    Task UpdateAsync(JobInfoModel job);


    /// <summary>
    ///     移除job
    /// </summary>
    /// <returns></returns>
    Task RemoveAsync(string groupName, string jobName);


    /// <summary>
    ///     暂停
    /// </summary>
    /// <returns></returns>
    Task PauseAsync(string groupName, string jobName);

    /// <summary>
    /// 保存全部
    /// </summary>
    /// <returns></returns>
    Task SaveAllAsync();
}
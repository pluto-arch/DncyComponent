using Dotnetydd.QuartzHost.Models;

namespace Dotnetydd.QuartzHost.Storage;

public interface IJobInfoStore
{
    /// <summary>
    ///     获取job 列表
    /// </summary>
    /// <returns></returns>
    Task<List<JobInfoModel>> GetListAsync();
}
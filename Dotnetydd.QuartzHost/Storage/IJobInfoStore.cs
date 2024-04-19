using Dotnetydd.QuartzHost.Models;
using Quartz;

namespace Dotnetydd.QuartzHost.Storage;

public interface IJobInfoStore
{
    Task<int> CountAsync();

    Task<List<JobInfoModel>> GetListAsync();


    Task<JobInfoModel> GetAsync(string id);


    Task<JobInfoModel> GetAsync(JobKey job);

    JobInfoModel Get(JobKey job);


   
    Task AddAsync(JobInfoModel job);


   
    Task UpdateAsync(JobInfoModel job);


    
    Task RemoveAsync(string groupName, string jobName);


   
    Task PauseAsync(string groupName, string jobName);

  
    Task SaveAllAsync();
}
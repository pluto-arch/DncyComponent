using Dotnetydd.QuartzHost.Models;
using Dotnetydd.QuartzHost.Storage;
using Microsoft.AspNetCore.Components;
using Quartz;
using System.Collections.Concurrent;
using System;
using System.Linq.Expressions;

namespace Dotnetydd.QuartzHost.Components.Pages;

public partial class Dashboard: ComponentBase, IDisposable
{
    
    [Inject]
    public required DataRepository Repository { get; init; }
    
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    private Subscription _appSubscription;
    private int count = 0;
    

    private readonly ConcurrentDictionary<string, JobInfoModel> _JobByName = new(JobInfoModel.StringComparers.ResourceName);

    private IQueryable<JobInfoModel> FilterJobs => _JobByName.Values.OrderBy(e => e.Status).ThenBy(e => e.TaskName).AsQueryable();

    private JobInfoModel SelectJob { get; set; }


    private int TotalJobCount { get; set; }

    private int RunningJobCount { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await UpdateApplications();
        _appSubscription = Repository.OnNewApplications(async (_) =>
        {
            await UpdateApplications();
            await InvokeAsync(StateHasChanged);
        });
    }

    private async Task UpdateApplications()
    {
        var jobs = await Repository.GetJobsAsync();
        foreach (var job in jobs)
        {
            _JobByName.TryAdd(job.Id, job);
        }
        TotalJobCount = _JobByName.Count;
        RunningJobCount=_JobByName.Values.Count(e=>e.Status==EnumJobStates.Normal);
    }


    private Expression<Func<JobInfoModel,string>> _typeNameExpress = e => GetJobType(e.TaskType);
    private Expression<Func<JobInfoModel,string>> _statusNameExpress = e => GetJobStatus(e.Status);
    private static string GetJobStatus(EnumJobStates jobStatus)
    {
        return jobStatus switch
        {
            EnumJobStates.Normal => "����",
            EnumJobStates.Blocked => "����",
            EnumJobStates.Pause => "��ͣ",
            EnumJobStates.Completed => "���",
            EnumJobStates.Exception => "�쳣",
            EnumJobStates.Stopped => "ֹͣ",
            _ => "None"
        };  
    }

    private static string GetJobType(EnumTaskType jobType)
    {
        return jobType switch
        {
            EnumTaskType.StaticExecute => "��ִ̬��",
            EnumTaskType.RpcExecute => "Զ�̵���",
            _ => "None"
        };  
    }

    private void ClearSelectedJob()
    {
        SelectJob = null;
    }


    public void Dispose()
    {
        _appSubscription.Dispose();
    }


    private string GetRowClass(JobInfoModel job)
        => job == SelectJob ? "selected-row" : null;
}
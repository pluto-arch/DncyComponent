using Dotnetydd.QuartzHost.Models;
using Dotnetydd.QuartzHost.Storage;
using Microsoft.AspNetCore.Components;
using Quartz;

namespace Dotnetydd.QuartzHost.Components.Pages;

public partial class Dashboard: ComponentBase, IDisposable
{
    
    [Inject]
    public required DataRepository Repository { get; init; }
    
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    private Subscription _appSubscription;

    private int count = 0;
    

    protected override async Task OnInitializedAsync()
    {
        await UpdateApplications();
        _appSubscription = Repository.OnNewApplications(async () =>
        {
            await UpdateApplications();
            await InvokeAsync(StateHasChanged);
        });
    }

    private async Task UpdateApplications()
    {
        count = (await Repository.GetJobsAsync()).Count;
    }
    
    public void Inc()
    {
        Repository.AddJob(new JobInfoModel
        {
            Id = Guid.NewGuid().ToString("N"),
            TaskType = EnumTaskType.StaticExecute,
            TaskName = $"AAA{Random.Shared.Next(1,22)}",
            DisplayName = $"AAA{Random.Shared.Next(1,22)}",
            GroupName = "Default",
            Interval = "0/2 * * * * ?",
            Describe = $"AAA{Random.Shared.Next(1,22)}",
            Status = EnumJobStates.Normal
        });
    }
    
    public void Dispose()
    {
        _appSubscription.Dispose();
    }
}
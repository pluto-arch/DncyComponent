using Dotnetydd.QuartzHost.Models;
using Dotnetydd.QuartzHost.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static Quartz.Logging.OperationName;

namespace Dotnetydd.QuartzHost.Components.Pages;

public sealed partial class ConsoleLog: ComponentBase, IDisposable
{
    [Parameter]
    public string jobId { get; set; }

    private JobInfoModel job { get; set; }
    private bool _applicationChanged;

    [Inject]
    public required DataRepository TelemetryRepository { get; set; }

    [Inject] 
    public IJobInfoStore _JobInfoStore { get; set; }


    private Subscription _logSubscription;

    private CircularBuffer<JobLogModel> _logs = new(100);

    private CircularBuffer<string> _appRunninglogs = new(100);


    protected override async Task OnInitializedAsync()
    {
        job = await _JobInfoStore.GetAsync(jobId);
        if (job is null)
        {
            await base.OnInitializedAsync();
            return;
        }
        UpdateSubscription();
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_applicationChanged)
        {
            await JS.InvokeVoidAsync("resetContinuousScrollPosition");
            _applicationChanged = false;
        }
        if (firstRender)
        {
            await JS.InvokeVoidAsync("initializeContinuousScroll");
        }
    }


    private void UpdateSubscription()
    {
        var currentKey = $"{job.GroupName}:{job.TaskName}";

        if (_logSubscription is null || _logSubscription.ApplicationId != jobId)
        {
            _logSubscription?.Dispose();
            _logSubscription = TelemetryRepository.OnNewLogs(jobId, SubscriptionType.Read, async (context) =>
            {
                if (context.Data is JobLogModel log)
                {
                    if (log.JobKey==currentKey)
                    {
                        _logs.Add(log);
                        await InvokeAsync(StateHasChanged);
                    }
                }

                if (context.Data is string appRunningLog)
                {
                    _appRunninglogs.Add(appRunningLog);
                    await InvokeAsync(StateHasChanged);
                }

                _applicationChanged = true;
            });
        }
    }



    public void Dispose()
    {
        _logSubscription?.Dispose();
    }
}
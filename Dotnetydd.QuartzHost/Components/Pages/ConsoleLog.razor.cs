using Dotnetydd.QuartzHost.Models;
using Dotnetydd.QuartzHost.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static Quartz.Logging.OperationName;

namespace Dotnetydd.QuartzHost.Components.Pages;

public sealed partial class ConsoleLog: ComponentBase, IDisposable
{
    private bool _applicationChanged;

    [Inject]
    public required DataRepository TelemetryRepository { get; set; }


    private Subscription _logSubscription;

    private CircularBuffer<JobLogModel> _logs = new(100);

    private CircularBuffer<string> _appRunninglogs = new(100);


    protected override void OnInitialized()
    {
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
        _logSubscription?.Dispose();
        _logSubscription = TelemetryRepository.OnNewApplications(async (context) =>
        {
            if (context.Data is string appRunningLog)
            {
                _appRunninglogs.Add(appRunningLog);
                await InvokeAsync(StateHasChanged);
            }

            _applicationChanged = true;
        });
    }



    public void Dispose()
    {
        _logSubscription?.Dispose();
    }
}
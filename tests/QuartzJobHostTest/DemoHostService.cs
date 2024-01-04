using Dotnetydd.QuartzHost;

namespace QuartzJobHostTest;

public class DemoHostService: IHostedLifecycleService
{
    private readonly ILoggerFactory _loggerFactory;

    private readonly QuartzDashboardWebApplication _dashboard;

    public DemoHostService(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        var dashboardLogger = _loggerFactory.CreateLogger<QuartzDashboardWebApplication>();
        _dashboard = new QuartzDashboardWebApplication(dashboardLogger);
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _dashboard.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _dashboard.StopAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task StartedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StartingAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
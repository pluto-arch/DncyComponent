using Dotnetydd.QuartzHost;
using Microsoft.Extensions.Logging.Abstractions;
using QuartzJobHostTest.Jobs;

namespace QuartzJobHostTest;

public class DemoHostService: IHostedLifecycleService
{
    private readonly QuartzDashboardWebApplication _dashboard;

    public DemoHostService(
        ILoggerFactory loggerFactory=null,
        Action<IServiceCollection> configServices=null)
    {
        loggerFactory??=NullLoggerFactory.Instance;
        _dashboard = new QuartzDashboardWebApplication(loggerFactory, service =>
        {
            service.AddDncyQuartzJobCore();
            service.AddStaticJobDefined(typeof(UserJob).Assembly);
            service.AddInMemoryJobInfoStore();
            configServices?.Invoke(service);
        });
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
using Dotnetydd.QuartzHost;
using Quartz;
using QuartzJobHostTest.Etl.DataSources;
using QuartzJobHostTest.Etl.Fetcher;
using QuartzJobHostTest.Etl.Models;

namespace QuartzJobHostTest.Jobs;

[DisallowConcurrentExecution]
public class DemoJob:QuartzJob<DemoJob>
{
    private readonly ILogger<DemoJob> _logger;
    private readonly FetcherContextAccessor _contextAccessor;


    public DemoJob(ILogger<DemoJob> logger,FetcherContextAccessor contextAccessor): base(logger)
    {
        _logger = logger;
        _contextAccessor = contextAccessor;
    }

    
    /// <inheritdoc />
    public override async Task ExecuteAsync(IJobExecutionContext context)
    {
        var paramters =new Dictionary<string, dynamic>
        {
            { "files", new string[] { "202404181000", "202404181010" } },
            { "start", DateTime.UtcNow },
            { "end", DateTime.UtcNow.AddMinutes(10) }
        };

        using (_contextAccessor.Begin("ICTS3LOG",paramters))
        {
            var dateSource = _contextAccessor.CurrentFetcherContext.Resolve<IDataSource<List<ICTS3LogDataModel>>>();
            _ = await dateSource.FetcherDataAsync();
        }
    }
}
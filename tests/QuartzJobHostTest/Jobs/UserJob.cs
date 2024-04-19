using Dotnetydd.QuartzHost;
using Quartz;
using QuartzJobHostTest.Etl.DataSources;
using QuartzJobHostTest.Etl.Fetcher;
using QuartzJobHostTest.Etl.Models;
using QuartzJobHostTest.Services;

namespace QuartzJobHostTest.Jobs;

[DisallowConcurrentExecution]
public class UserJob: QuartzJob<UserJob>
{
    private readonly ILogger<UserJob> _logger;
    private readonly FetcherContextAccessor _contextAccessor;


    public UserJob(ILogger<UserJob> logger,FetcherContextAccessor contextAccessor): base(logger)
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

        using (_contextAccessor.Begin("CTC",paramters))
        {
            var dateSource = _contextAccessor.CurrentFetcherContext.Resolve<IDataSource<List<CTCDataModel>>>();
            _ = await dateSource.FetcherDataAsync();




        }
    }
}
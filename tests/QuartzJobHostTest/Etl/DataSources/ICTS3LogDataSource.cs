using QuartzJobHostTest.Etl.Fetcher;
using QuartzJobHostTest.Etl.Models;

namespace QuartzJobHostTest.Etl.DataSources;

/// <summary>
/// ICT S3 log sheet DATASOURCE
/// </summary>
public class ICTS3LogDataSource : IDataSource<List<ICTS3LogDataModel>>
{
    private readonly FetcherContextAccessor _accessor;
    private readonly ILogger<ICTS3LogDataSource> _logger;

    public ICTS3LogDataSource(FetcherContextAccessor accessor,ILogger<ICTS3LogDataSource> logger)
    {
        _accessor = accessor;
        _logger = logger;
    }

    /// <inheritdoc />
    public string Name => "ICTS3LOG";

    /// <inheritdoc />
    public List<ICTS3LogDataModel> FetcherData()
    {
        var ctx = _accessor.CurrentFetcherContext.SourceName;
        _logger.LogInformation("ICTS3LogDataSource worked for {SourceName}",ctx);
        return default;
    }

    /// <inheritdoc />
    public Task<List<ICTS3LogDataModel>> FetcherDataAsync()
    {
        var ctx = _accessor.CurrentFetcherContext.SourceName;
        _logger.LogInformation("ICTS3LogDataSource worked for {SourceName}",ctx);
        return Task.FromResult(default(List<ICTS3LogDataModel>));
    }
}
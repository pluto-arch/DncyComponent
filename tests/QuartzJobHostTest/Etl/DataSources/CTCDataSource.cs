using QuartzJobHostTest.Etl.Fetcher;
using QuartzJobHostTest.Etl.Models;

namespace QuartzJobHostTest.Etl.DataSources;

public class CTCDataSource : IDataSource<List<CTCDataModel>>
{
    private readonly FetcherContextAccessor _accessor;
    private readonly ILogger<CTCDataSource> _logger;

    public CTCDataSource(FetcherContextAccessor accessor,ILogger<CTCDataSource> logger)
    {
        _accessor = accessor;
        _logger = logger;
    }

    /// <inheritdoc />
    public string Name => "CTC";

    /// <inheritdoc />
    public List<CTCDataModel> FetcherData()
    {
        var ctx = _accessor.CurrentFetcherContext.SourceName;
        _logger.LogInformation("CTCDataSource worked for {SourceName}",ctx);
        return default;
    }

    /// <inheritdoc />
    public Task<List<CTCDataModel>> FetcherDataAsync()
    {
        var ctx = _accessor.CurrentFetcherContext.SourceName;
        _logger.LogInformation("CTCDataSource worked for {SourceName}",ctx);
        return Task.FromResult(default(List<CTCDataModel>));
    }
}
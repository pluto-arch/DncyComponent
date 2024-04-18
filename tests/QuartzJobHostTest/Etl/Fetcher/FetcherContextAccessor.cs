namespace QuartzJobHostTest.Etl.Fetcher;

public class FetcherContextAccessor
{
    private readonly IServiceProvider _service;
    private readonly AsyncLocal<FetcherContext> _currentContext = new AsyncLocal<FetcherContext>();

    public FetcherContext CurrentFetcherContext
    {
        get => _currentContext.Value;
        set => _currentContext.Value = value;
    }


    public FetcherContextAccessor(IServiceProvider service)
    {
        _service = service;
    }


    /// <summary>
    /// 开始一个新的数据抓取上下文
    /// </summary>
    /// <returns></returns>
    public IDisposable Begin(string sourceName,Dictionary<string,dynamic> paramters)
    {
        var parentScope = CurrentFetcherContext;
        var context= new FetcherContext(sourceName,paramters,_service.CreateScope());
        CurrentFetcherContext = context;
        return new DisposeAction(() =>
        {
            CurrentFetcherContext = parentScope;
            context?.Dispose();
        });
    }
}
namespace QuartzJobHostTest.Etl.Fetcher;

public class FetcherContext:IDisposable
{

    public FetcherContext()
    {
        Paramters=new ();
    }

    public FetcherContext(string sourceName):this()
    {
        SourceName=sourceName;
    }

    public FetcherContext(string sourceName, Dictionary<string, dynamic> paramters, IServiceScope scoped):this(sourceName)
    {
        Paramters=paramters;
        _scoped= scoped;
    }


    /// <summary>
    /// 数据源名称
    /// </summary>
    public string SourceName { get; set; }

    private readonly IServiceScope _scoped;
    public IServiceProvider ServiceProvider =>_scoped.ServiceProvider;

    /// <summary>
    /// 参数
    /// </summary>
    public Dictionary<string,dynamic> Paramters {get;set;}

    /// <summary>
    /// 添加参数信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddParamter(string key,dynamic value)
    {
        Paramters.Add(key,value);
    }


    public T Resolve<T>()
    {
        return ServiceProvider.GetService<T>();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _scoped?.Dispose();
    }
}
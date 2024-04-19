using QuartzJobHostTest.Etl.Fetcher;

namespace QuartzJobHostTest.Etl.DataSources;



public interface IDataSource
{
    string Name {get;}
}


/// <summary>
/// 数据源接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IDataSource<T> : IDataSource
{
    T FetcherData();
    Task<T> FetcherDataAsync();
}
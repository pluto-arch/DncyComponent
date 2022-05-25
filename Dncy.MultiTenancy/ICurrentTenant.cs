namespace Dncy.MultiTenancy;

public interface ICurrentTenant
{
    bool IsAvailable { get; }

    string Name { get; }

    string Id { get; }

    /// <summary>
    /// 切换租户
    /// </summary>
    /// <returns></returns>
    IDisposable Change(string id, string name = null);
}
using Dncy.MultiTenancy.Model;

namespace Dncy.MultiTenancy;

public class CurrentTenant : ICurrentTenant
{
    private readonly ICurrentTenantAccessor _currentTenantAccessor;

    public CurrentTenant(ICurrentTenantAccessor currentTenantAccessor)
    {
        _currentTenantAccessor = currentTenantAccessor;
    }


    /// <inheritdoc />
    public virtual bool IsAvailable => !string.IsNullOrEmpty(Id) && !string.IsNullOrWhiteSpace(Id);

    /// <inheritdoc />
    public virtual string Name => _currentTenantAccessor.CurrentTenantInfo?.Name;

    /// <inheritdoc />
    public virtual string Id => _currentTenantAccessor.CurrentTenantInfo?.Id;


    /// <summary>
    /// 切换租户
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public IDisposable Change(string id, string name = null)
    {
        return SetCurrent(id, name);
    }

    private IDisposable SetCurrent(string tenantId, string name = null)
    {
        var parentScope = _currentTenantAccessor.CurrentTenantInfo;
        _currentTenantAccessor.CurrentTenantInfo = new TenantInfo(tenantId, name);
        return new DisposeAction(() =>
        {
            _currentTenantAccessor.CurrentTenantInfo = parentScope;
        });
    }

}
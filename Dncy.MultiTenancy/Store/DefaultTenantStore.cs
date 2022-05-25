using Dncy.MultiTenancy.ConnectionStrings;
using Dncy.MultiTenancy.Model;
using Microsoft.Extensions.Options;

namespace Dncy.MultiTenancy.Store;

public class DefaultTenantStore : ITenantStore
{
    private readonly TenantConfiguration[] _tenants;

    public DefaultTenantStore(IOptions<TenantConfigurationOptions> options)
    {
        _ = options ?? throw new ArgumentNullException(nameof(options));
        _tenants = options.Value?.Tenants;
    }


    /// <inheritdoc />
    public Task<TenantConfiguration> FindAsync(string name)
    {
        if (IsNullOrEmpty())
            return Task.FromResult<TenantConfiguration>(default);
        var t = _tenants.FirstOrDefault(x => x.TenantName == name);
        if (t == null)
            return Task.FromResult<TenantConfiguration>(default);
        return Task.FromResult(new TenantConfiguration(t.TenantId, name,t.ConnectionStrings));
    }

    /// <inheritdoc />
    public Task<TenantConfiguration> FindAsync<Tkey>(Tkey id)
    {
        if (IsNullOrEmpty())
            return Task.FromResult<TenantConfiguration>(default);
        var t = _tenants.FirstOrDefault(x => x.TenantId == id.ToString());
        if (t == null)
            return Task.FromResult<TenantConfiguration>(default);
        return Task.FromResult(new TenantConfiguration(t.TenantId, t.TenantName,t.ConnectionStrings));
    }

    /// <inheritdoc />
    public TenantConfiguration Find(string name)
    {
        if (IsNullOrEmpty())
            return null;
        var t = _tenants.FirstOrDefault(x => x.TenantName == name);
        if (t == null)
            return null;
        return new TenantConfiguration(t.TenantId, t.TenantName,t.ConnectionStrings);
    }

    /// <inheritdoc />
    public TenantConfiguration Find<Tkey>(Tkey id)
    {
        if (IsNullOrEmpty())
            return null;
        var t = _tenants.FirstOrDefault(x => x.TenantId == id.ToString());
        if (t == null)
            return null;
        return new TenantConfiguration(t.TenantId, t.TenantName,t.ConnectionStrings);
    }

    private bool IsNullOrEmpty()
    {
        if (_tenants == null || !_tenants.Any())
        {
            return true;
        }

        return false;
    }
}
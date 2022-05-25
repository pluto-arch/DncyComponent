using Dncy.MultiTenancy.ConnectionStrings;
using Dncy.MultiTenancy.Model;

namespace Dncy.MultiTenancy.Store;

public interface ITenantStore
{
    Task<TenantConfiguration> FindAsync(string name);

    Task<TenantConfiguration> FindAsync<Tkey>(Tkey id);

    TenantConfiguration Find(string name);

    TenantConfiguration Find<Tkey>(Tkey id);
}
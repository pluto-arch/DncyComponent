using System.Threading.Tasks;
using Dncy.MultiTenancy.ConnectionStrings;

namespace Dncy.MultiTenancy.Store
{
    public interface ITenantStore
    {
        Task<TenantConfiguration> FindAsync(string name);

        Task<TenantConfiguration> FindAsync<Tkey>(Tkey id);

        TenantConfiguration Find(string name);

        TenantConfiguration Find<Tkey>(Tkey id);
    }
}


using Dotnetydd.MultiTenancy.Model;
using System.Threading.Tasks;

namespace Dotnetydd.MultiTenancy.Store
{
    public interface ITenantStore
    {
        Task<TenantInfo> FindAsync(string name);

        Task<TenantInfo> FindAsync<Tkey>(Tkey id);

        TenantInfo Find(string name);

        TenantInfo Find<Tkey>(Tkey id);
    }
}


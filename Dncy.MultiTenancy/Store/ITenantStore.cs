using System.Threading.Tasks;
using Dncy.MultiTenancy.ConnectionStrings;
using Dncy.MultiTenancy.Model;

namespace Dncy.MultiTenancy.Store
{
    public interface ITenantStore
    {
        Task<TenantInfo> FindAsync(string name);

        Task<TenantInfo> FindAsync<Tkey>(Tkey id);

        TenantInfo Find(string name);

        TenantInfo Find<Tkey>(Tkey id);
    }
}


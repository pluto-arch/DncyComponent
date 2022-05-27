using Dncy.MultiTenancy.Model;

namespace Dncy.MultiTenancy
{
    public interface ICurrentTenantAccessor
    {
        TenantInfo CurrentTenantInfo { get; set; }
    }
}


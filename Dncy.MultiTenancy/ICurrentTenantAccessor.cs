using Dotnetydd.MultiTenancy.Model;

namespace Dotnetydd.MultiTenancy
{
    public interface ICurrentTenantAccessor
    {
        TenantInfo CurrentTenantInfo { get; set; }
    }
}


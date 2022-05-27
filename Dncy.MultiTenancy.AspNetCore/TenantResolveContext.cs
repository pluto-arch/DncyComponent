using System;

namespace Dncy.MultiTenancy.AspNetCore
{
    public class TenantResolveContext:ITenantResolveContext
    {
        public IServiceProvider ServiceProvider { get; }

        public string TenantIdOrName { get; set; }

        public bool Handled { get; set; }

        public bool HasResolvedTenantOrHost()
        {
            return Handled || !string.IsNullOrEmpty(TenantIdOrName);
        }

        public TenantResolveContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}


using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dncy.MultiTenancy.AspNetCore;

public class TenantResolver:ITenantResolver
{
    private readonly IEnumerable<ITenantConstruct> _tenantConstructs;
    private readonly IServiceProvider _serviceProvider;

    public TenantResolver(IEnumerable<ITenantConstruct> tenantConstructs,IServiceProvider serviceProvider)
    {
        _tenantConstructs = tenantConstructs;
        _serviceProvider = serviceProvider;
    }


    /// <inheritdoc />
    public string ResolveTenantIdOrName()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new TenantResolveContext(serviceScope.ServiceProvider);

            foreach (var tenantResolver in _tenantConstructs)
            {
                tenantResolver.Resolve(context);
                if (context.HasResolvedTenantOrHost())
                {
                    return context.TenantIdOrName;
                }
            }
        }

        return null;
    }
}
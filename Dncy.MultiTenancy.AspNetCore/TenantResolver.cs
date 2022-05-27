using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dncy.MultiTenancy.AspNetCore
{
    public class TenantResolver:ITenantResolver
    {
        private readonly IEnumerable<ITenantConstruct> _tenantConstructs;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TenantResolver> _logger;

        public TenantResolver(IEnumerable<ITenantConstruct> tenantConstructs,IServiceProvider serviceProvider, ILogger<TenantResolver> logger)
        {
            _tenantConstructs = tenantConstructs;
            _serviceProvider = serviceProvider;
            _logger = logger;
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
                        _logger.LogInformation($"Tenant successfully resolved from : {tenantResolver.Name}！");
                        return context.TenantIdOrName;
                    }
                }
            }

            return null;
        }
    }
}


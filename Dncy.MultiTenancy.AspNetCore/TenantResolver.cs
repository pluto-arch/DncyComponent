using Dotnetydd.MultiTenancy.AspNetCore.TenantIdentityParse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;

namespace Dotnetydd.MultiTenancy.AspNetCore
{
    public class TenantResolver : ITenantResolver
    {
        private readonly IEnumerable<ITenantIdentityParse> _tenantIdentityParses;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TenantResolver> _logger;

        public TenantResolver(
            IEnumerable<ITenantIdentityParse> tenantIdentityParses,
            IServiceProvider serviceProvider,
            ILogger<TenantResolver> logger = null)
        {
            _tenantIdentityParses = tenantIdentityParses;
            _serviceProvider = serviceProvider;
            _logger = logger ?? NullLogger<TenantResolver>.Instance;
        }


        /// <inheritdoc />
        public string ResolveTenantIdOrName()
        {
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var context = new TenantResolveContext(serviceScope.ServiceProvider);

                foreach (var tenantResolver in _tenantIdentityParses)
                {
                    tenantResolver.Resolve(context);
                    if (context.HasResolvedTenantOrHost())
                    {
                        _logger.LogDebug("Tenant successfully resolved from : {@tenantResolver}. The tenant is {tenantIdOrName}", tenantResolver.Name, context.TenantIdOrName);
                        return context.TenantIdOrName;
                    }
                }
            }

            return null;
        }
    }
}


using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Dotnetydd.MultiTenancy.AspNetCore.TenantIdentityParse
{
    /// <summary>
    /// Http的租户构造基础
    /// </summary>
    public abstract class HttpTenantIdentityParseBase : ITenantIdentityParse
    {
        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public void Resolve(ITenantResolveContext context)
        {
            var httpContext = context.ServiceProvider?.GetRequiredService<IHttpContextAccessor>()?.HttpContext;
            if (httpContext == null)
            {
                return;
            }

            try
            {
                ResolveFromHttpContext(context, httpContext);
            }
            catch (Exception e)
            {
                context.ServiceProvider?.GetRequiredService<ILogger<HttpTenantIdentityParseBase>>()?
                    .LogWarning(e, e.Message);
            }
        }

        protected virtual void ResolveFromHttpContext(ITenantResolveContext context, HttpContext httpContext)
        {
            var tenantIdOrName = GetTenantIdOrNameFromHttpContextOrNull(context, httpContext);
            if (!string.IsNullOrEmpty(tenantIdOrName))
            {
                context.TenantIdOrName = tenantIdOrName;
            }
        }

        protected abstract string GetTenantIdOrNameFromHttpContextOrNull([NotNull] ITenantResolveContext context, [NotNull] HttpContext httpContext);
    }
}



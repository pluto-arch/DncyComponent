using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dncy.MultiTenancy.AspNetCore;


/// <summary>
/// Http的租户构造基础
/// </summary>
public abstract class HttpTenantConstructBase:ITenantConstruct
{
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
            ResolveFromHttpContext(context,httpContext);
        }
        catch (Exception e)
        {
            context.ServiceProvider?.GetRequiredService<ILogger<HttpTenantConstructBase>>()?
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
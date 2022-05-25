using Dncy.MultiTenancy.ConnectionStrings;
using Dncy.MultiTenancy.Model;
using Dncy.MultiTenancy.Store;
using Microsoft.AspNetCore.Http;

namespace Dncy.MultiTenancy.AspNetCore
{
    public class MultiTenancyMiddleware:IMiddleware
    {
        private readonly ITenantResolver _tenantResolver;
        private readonly ITenantStore _tenantStore;
        private readonly ICurrentTenant _currentTenant;

        public MultiTenancyMiddleware(ITenantResolver tenantResolver, ITenantStore tenantStore, ICurrentTenant currentTenant)
        {
            _tenantResolver = tenantResolver;
            _tenantStore = tenantStore;
            _currentTenant = currentTenant;
        }


        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var resolveResult = _tenantResolver.ResolveTenantIdOrName();
            TenantConfiguration tenant = null;
            if (resolveResult!=null&&!string.IsNullOrEmpty(resolveResult))
            {
                tenant = await FindAndCheckTenantAsync(resolveResult);
                if (tenant == null)
                {
                    await next(context);
                }
            }

            using (_currentTenant.Change(tenant?.TenantId,tenant?.TenantName))
            {
                await next(context);
            }
        }

        /// <summary>
        /// 查找并检查租户租户
        /// </summary>
        /// <param name="tenantIdOrName"></param>
        /// <returns></returns>
        protected virtual async Task<TenantConfiguration> FindAndCheckTenantAsync(string tenantIdOrName)
        {
            return await _tenantStore.FindAsync<string>(tenantIdOrName);
        }
    }
}
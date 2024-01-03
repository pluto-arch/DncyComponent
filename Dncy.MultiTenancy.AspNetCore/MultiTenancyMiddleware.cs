using Dotnetydd.MultiTenancy.Model;
using Dotnetydd.MultiTenancy.Store;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Dotnetydd.MultiTenancy.AspNetCore
{
    public class MultiTenancyMiddleware : IMiddleware
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
            if (resolveResult != null && !string.IsNullOrEmpty(resolveResult))
            {
                var tenant = await FindAndCheckTenantAsync(resolveResult);
                if (tenant == null)
                {
                    await next(context);
                }
                else
                {
                    using (_currentTenant.Change(tenant))
                    {
                        await next(context);
                    }
                }
            }
            else
            {
                await next(context);
            }
        }

        /// <summary>
        /// 查找并检查租户租户
        /// </summary>
        /// <param name="tenantIdOrName"></param>
        /// <returns></returns>
        protected virtual async Task<TenantInfo> FindAndCheckTenantAsync(string tenantIdOrName)
        {
            return await _tenantStore.FindAsync<string>(tenantIdOrName);
        }
    }
}
using System;
using Microsoft.AspNetCore.Http;

namespace Dncy.MultiTenancy.AspNetCore
{
    /// <summary>
    /// 域名租户构造
    /// </summary>
    public class DomainNameTenantConstruct:HttpTenantConstructBase
    {
        private readonly Func<HostString, string> _analizeFunc;


        public DomainNameTenantConstruct(Func<HostString,string> analizeFunc)
        {
            _analizeFunc = analizeFunc;
        }


        /// <inheritdoc />
        public override string Name => "Domain Name";

        /// <inheritdoc />
        protected override string GetTenantIdOrNameFromHttpContextOrNull(ITenantResolveContext context, HttpContext httpContext)
        {
            if (httpContext.Request?.Host == null)
            {
                return null;
            }

            var extractResult = _analizeFunc(httpContext.Request.Host);
            context.Handled = true;
            return extractResult;
        }
    }
}

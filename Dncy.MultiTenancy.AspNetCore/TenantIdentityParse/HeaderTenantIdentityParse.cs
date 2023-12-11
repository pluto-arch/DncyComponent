using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Dotnetydd.MultiTenancy.AspNetCore.TenantIdentityParse
{
    public class HeaderTenantIdentityParse : HttpTenantIdentityParseBase
    {

        private readonly Func<IHeaderDictionary, string> _analizeFunc;


        public HeaderTenantIdentityParse(Func<IHeaderDictionary, string> analizeFunc)
        {
            _analizeFunc = analizeFunc;
        }


        /// <inheritdoc />
        public override string Name => "Http Header";

        /// <inheritdoc />
        protected override string GetTenantIdOrNameFromHttpContextOrNull(ITenantResolveContext context, HttpContext httpContext)
        {
            if (httpContext.Request == null || !(httpContext.Request.Headers?.Any() ?? false))
            {
                return null;
            }

            var extractResult = _analizeFunc(httpContext.Request.Headers);
            context.Handled = true;
            return extractResult;

        }
    }
}

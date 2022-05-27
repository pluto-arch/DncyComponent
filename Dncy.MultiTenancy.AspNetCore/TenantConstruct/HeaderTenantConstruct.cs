using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Dncy.MultiTenancy.AspNetCore
{
    public class HeaderTenantConstruct:HttpTenantConstructBase
    {

        private readonly Func<IHeaderDictionary, string> _analizeFunc;


        public HeaderTenantConstruct(Func<IHeaderDictionary, string> analizeFunc)
        {
            _analizeFunc = analizeFunc;
        }


        /// <inheritdoc />
        public override string Name => "Http Header";

        /// <inheritdoc />
        protected override string GetTenantIdOrNameFromHttpContextOrNull(ITenantResolveContext context, HttpContext httpContext)
        {
            if (httpContext.Request == null || !(httpContext.Request.Headers?.Any()??false))
            {
                return null;
            }

            var extractResult = _analizeFunc(httpContext.Request.Headers);
            context.Handled = true;
            return extractResult;

        }
    }
}

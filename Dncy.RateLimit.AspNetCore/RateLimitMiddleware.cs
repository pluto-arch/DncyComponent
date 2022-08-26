using System.Net;
using System.Text.Json;
using Dncy.RateLimit.Core;
using Dncy.RateLimit.Core.Algorithms;
using Microsoft.AspNetCore.Http;

namespace Dncy.RateLimit.AspNetCore
{
    public class RateLimitMiddleware:IMiddleware
    {

        private readonly IRateLimitTargetResolver _targetResolve;


        public RateLimitMiddleware(IRateLimitTargetResolver targetResolve=null)
        {
            _targetResolve = targetResolve;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (_targetResolve==null)
            {
                await next(context);
            }
            else
            {
                var target = _targetResolve.ResolveWithRule(context);
                if (string.IsNullOrEmpty(target.target)||target.algoAndRules==null||!target.algoAndRules.Any())
                {
                    await next(context);
                }
                else
                {
                    var ctx = new LimitCheckContext(target.target);
                    foreach (var algorithm in target.algoAndRules)
                    {
                        var res= algorithm.Key.Check(ctx, algorithm.Value);
                        if (res.Passed)
                        {
                            await next(context);
                        }
                        else
                        {
                            context.Response.StatusCode = (int) StatusCodes.Status429TooManyRequests;
                            await context.Response.WriteAsync(JsonSerializer.Serialize(res));
                            return;
                        }
                    }
                }
            }
        }
    }
}
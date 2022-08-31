using System.Text.Json;
using Dncy.QuartzJob.AspNetCore.Pages;
using Microsoft.AspNetCore.Http;

namespace Dncy.QuartzJob.AspNetCore
{
    public class QuartzJobUiMiddleware:IMiddleware
    {
        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.StartsWithSegments(new PathString("/quartzjob")))
            {
                await EmbeddedFilesHelper.IncludeEmbeddedFile(context, context.Request.Path);
                return;
            }

            if (context.Request.Path.StartsWithSegments(new PathString("/quartzjob-api")))
            {
                await WriteDataAsync(context.Request.Path.Value,context);
                return;
            }

            await next(context);
        }



        private async Task WriteDataAsync(string path,HttpContext context)
        {
            var route = path.Replace("/quartzjob-api", string.Empty).Split('/').Last();
            context.Response.ContentType = "application/json;charset=utf-8";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { hello="123123"}));
        }
    }
}


using Dotnetydd.QuartzJob.AspNetCore.Handlers;
using Dotnetydd.QuartzJob.AspNetCore.Models;
using Dotnetydd.QuartzJob.AspNetCore.Pages;
using System.Text.Json;

namespace Dotnetydd.QuartzJob.AspNetCore
{
    public class QuartzJobUiMiddleware : IMiddleware
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
                await WriteDataAsync(context.Request.Path.Value, context);
                return;
            }

            await next(context);
        }

        private static readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private async Task WriteDataAsync(string path, HttpContext context)
        {
            var route = path.Replace("/quartzjob-api", string.Empty).Split('/').Last();
            context.Response.ContentType = "application/json;charset=utf-8";
            var handler = context.RequestServices?.GetRequiredService<JobDataHandler>();
            if (handler == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new JobDataResult<string> { Code = -1, Msg = "404 not found" }, serializerOptions));
                return;
            }
            context.Response.StatusCode = StatusCodes.Status200OK;

            if (route == "dashboarddata")
            {
                var res = await handler.DashboardData();
                await context.Response.WriteAsync(JsonSerializer.Serialize(res, serializerOptions));
                return;
            }


            if (route == "tasks")
            {
                var res = await handler.Tasks();
                await context.Response.WriteAsync(JsonSerializer.Serialize(res, serializerOptions));
                return;
            }

            if (route == "PauseTask")
            {
                if (!context.Request.Query.ContainsKey("id"))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new JobDataResult<string> { Code = -1, Msg = "请选择任务" }, serializerOptions));
                    return;
                }

                var res = await handler.PauseTask(context.Request.Query["id"]);
                await context.Response.WriteAsync(JsonSerializer.Serialize(res, serializerOptions));
                return;
            }


            if (route == "JobLogs")
            {
                if (!context.Request.Query.ContainsKey("id"))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new JobDataResult<string> { Code = -1, Msg = "请选择任务" }, serializerOptions));
                    return;
                }

                int.TryParse(context.Request.Query["pageNo"], out int pageNo);
                var res = await handler.JobLogs(context.Request.Query["id"], pageNo <= 0 ? 1 : pageNo);
                await context.Response.WriteAsync(JsonSerializer.Serialize(res, serializerOptions));
                return;
            }

            if (route == "Execute")
            {
                if (!context.Request.Query.ContainsKey("id"))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new JobDataResult<string> { Code = -1, Msg = "请选择任务" }, serializerOptions));
                    return;
                }

                var res = await handler.Execute(context.Request.Query["id"]);
                await context.Response.WriteAsync(JsonSerializer.Serialize(res, serializerOptions));
                return;
            }

            if (route == "add" && context.Request.Method == HttpMethods.Post)
            {
                var stream = new StreamReader(context.Request.Body);
                var body = await stream.ReadToEndAsync();

                if (string.IsNullOrEmpty(body))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new JobDataResult<string> { Code = -1, Msg = "参数错误" }, serializerOptions));
                    return;
                }

                var res = await handler.AddJob(body);
                await context.Response.WriteAsync(JsonSerializer.Serialize(res, serializerOptions));
                return;
            }

            if (route == "Refire")
            {
                if (!context.Request.Query.ContainsKey("id"))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new JobDataResult<string> { Code = -1, Msg = "请选择任务" }, serializerOptions));
                    return;
                }

                var res = await handler.Refire(context.Request.Query["id"]);
                await context.Response.WriteAsync(JsonSerializer.Serialize(res, serializerOptions));
                return;
            }

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new JobDataResult<string> { Code = -1, Msg = "404 not found" }, serializerOptions));
            return;
        }

    }
}


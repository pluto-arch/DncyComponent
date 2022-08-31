using System.Net.Http;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Dncy.QuartzJob.AspNetCore.Pages
{
    internal class EmbeddedFilesHelper
    {
         static readonly Dictionary<string, string> ResponseType = new Dictionary<string, string>
        {
            { ".css","text/css"},
            { ".js","application/javascript"},
            { ".html","text/html;charset=utf-8"},
            {".woff2","font/woff2" },
            {".woff","font/woff" },
            {".ttf","application/octet-stream" },
        };


        private static readonly Assembly Assembly;

        static EmbeddedFilesHelper()
        {
            Assembly = Assembly.GetExecutingAssembly();
        }

        public static async Task IncludeEmbeddedFile(HttpContext context, string path="dashboard.html")
        {
            if (string.IsNullOrEmpty(path.Replace("/quartzjob",string.Empty)))
            {
                context.Response.StatusCode = (int) StatusCodes.Status307TemporaryRedirect;
                context.Response.Redirect("/quartzjob/dashboard.html");
                return;
            }
            context.Response.OnStarting(() =>
            {
                if (context.Response.StatusCode == (int)HttpStatusCode.OK)
                {
                    context.Response.ContentType = ResponseType[Path.GetExtension(path)];
                }

                return Task.CompletedTask;
            });

            try
            {
                path = path.Replace("/quartzjob",string.Empty).TrimStart('/').Replace("/",".");
                await using var inputStream = Assembly.GetManifestResourceStream($"Dncy.QuartzJob.AspNetCore.Pages.{path}");
                if (inputStream == null)
                {
                    throw new ArgumentException($@"Resource with name {path} not found in assembly {Assembly}.");
                }
                using var reader = new StreamReader(inputStream);
                var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync(htmlBuilder.ToString());
            }
            catch
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync($"");
            }

        }
    }
}


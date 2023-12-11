
/* 项目“Dotnetydd.QuartzJob.AspNetCore (net8.0)”的未合并的更改
在此之前:
using System.Net.Http;
using System.Net;
在此之后:
using System.Net;
using System.Net.Http;
*/

/* 项目“Dotnetydd.QuartzJob.AspNetCore (net7.0)”的未合并的更改
在此之前:
using System.Net.Http;
using System.Net;
在此之后:
using System.Net;
using System.Net.Http;
*/

/* 项目“Dotnetydd.QuartzJob.AspNetCore (net5.0)”的未合并的更改
在此之前:
using System.Net.Http;
using System.Net;
在此之后:
using System.Net;
using System.Net.Http;
*/
using System.Net;
using System.Reflection;

namespace Dotnetydd.QuartzJob.AspNetCore.Pages
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
            {".ttf","application/font-sfnt" },
            {".png","image/png" },
            {".jpeg","image/jpeg" },
            {".gif","image/gif" },
            {".svg","image/svg+xml" },
        };


        private static readonly Assembly Assembly;

        static EmbeddedFilesHelper()
        {
            Assembly = Assembly.GetExecutingAssembly();
        }

        public static async Task IncludeEmbeddedFile(HttpContext context, string path = "dashboard.html")
        {
            if (string.IsNullOrEmpty(path.Replace("/quartzjob", string.Empty)))
            {
                context.Response.StatusCode = StatusCodes.Status307TemporaryRedirect;
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
                path = path.Replace("/quartzjob", string.Empty).TrimStart('/').Replace("/", ".");
                await using var inputStream = Assembly.GetManifestResourceStream($"Dncy.QuartzJob.AspNetCore.Pages.{path}");
                if (inputStream == null)
                {
                    throw new ArgumentException($@"Resource with name {path} not found in assembly {Assembly}.");
                }
                await inputStream.CopyToAsync(context.Response.Body, 1024);
            }
            catch
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync($"出现错误");
            }

        }
    }
}


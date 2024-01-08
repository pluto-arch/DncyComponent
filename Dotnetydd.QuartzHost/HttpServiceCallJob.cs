using Dotnetydd.QuartzHost.Models;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Dotnetydd.QuartzHost;

public class HttpServiceCallJob: IBackgroundJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HttpServiceCallJob> _logger;

    public HttpServiceCallJob(IHttpClientFactory httpClientFactory, ILogger<HttpServiceCallJob> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
    /// <inheritdoc />
    public virtual async Task Execute(IJobExecutionContext context)
    {
        var jobInfo = context.Get(JobExecutionContextConstants.JobExecutionContextData_JobInfo);

        if (jobInfo == null)
        {
            _logger.LogError("{jobKey} : not found ", context.JobDetail.Key);
            return;
        }

        if (!(jobInfo is JobInfoModel _))
        {
            _logger.LogError("{jobKey} : is not JobInfoModel ", context.JobDetail.Key);
            return;
        }
        JobInfoModel model = (JobInfoModel)jobInfo;
        var client = _httpClientFactory.CreateClient(model.TaskName);
        var request = new HttpRequestMessage(HttpMethod.Get, new Uri(model.ApiUrl));
        if (!string.IsNullOrEmpty(model.AuthKey))
        {
            request.Headers.Add(model.AuthKey, model.AuthValue ?? "");
        }
        if (model.RemoteCallTimeOut > 0)
        {
            client.Timeout = TimeSpan.FromSeconds(model.RemoteCallTimeOut);
        }
        var response = await client.SendAsync(request, context.CancellationToken);
        context.Result = $"{request.Method}. {response.Version}. {response.StatusCode}";
        _logger.LogInformation("{taskName} : call apiurl [{apiUrl}] result : {statusCode}", model.TaskName, model.ApiUrl, response.StatusCode);
    }

}
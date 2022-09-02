using System;
using System.Threading.Tasks;
using Dncy.QuartzJob.Model;
using Dncy.QuartzJob.Stores;
#if NET6_0
using Microsoft.Extensions.Logging;
#endif
using Quartz;
using System.Net.Http;
using Newtonsoft.Json;
using Dncy.QuartzJob.Constants;

#if NET46
using Serilog;
using Serilog.Core;
#endif


namespace Dncy.QuartzJob
{
    public class HttpServiceCallJob:IBackgroundJob
    {
        #if NET6_0
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpServiceCallJob> _logger;

        public HttpServiceCallJob(IHttpClientFactory httpClientFactory,ILogger<HttpServiceCallJob> logger)
        {
            _httpClientFactory=httpClientFactory;
            _logger=logger;
        }
        /// <inheritdoc />
        public virtual async Task Execute(IJobExecutionContext context)
        {
            var jobInfo = context.Get(JobExecutionContextConstants.JobExecutionContextData_JobInfo);

            if (jobInfo==null)
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
                request.Headers.Add(model.AuthKey, model.AuthValue??"");
            }
            if (model.RemoteCallTimeOut>0)
            {
                client.Timeout = TimeSpan.FromSeconds(model.RemoteCallTimeOut);
            }
            var response = await client.SendAsync(request,context.CancellationToken);
            var resstring = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("{taskName} : call apiurl [{apiUrl}] result : {statusCode}", model.TaskName, model.ApiUrl,response.StatusCode);
            context.Result = $"{model.TaskName} : call apiurl [{model.ApiUrl}] result : {response.StatusCode}";
        }
        #endif

        #if NET46
        static readonly HttpClient httpClient = new HttpClient();
        /// <inheritdoc />
        public virtual async Task Execute(IJobExecutionContext context)
        {
            var jobInfo = context.Get(JobExecutionContextConstants.JobExecutionContextData_JobInfo);

            if (jobInfo==null)
            {
                Log.Logger.Warning("{jobKey} : not found ", context.JobDetail.Key);
                return;
            }

            if (!(jobInfo is JobInfoModel _))
            {
                Log.Logger.Warning("{jobKey} : is not JobInfoModel ", context.JobDetail.Key);
                return;
            }
            JobInfoModel model = (JobInfoModel)jobInfo;
            var request = new HttpRequestMessage(HttpMethod.Get, model.ApiUrl);
            if (!string.IsNullOrEmpty(model.AuthKey))
            {
                request.Headers.Add(model.AuthKey, model.AuthValue??"");
            }
            var response = await httpClient.SendAsync(request);
            Log.Logger.Information("{taskName} : call apiurl [{apiUrl}] result : {statusCode}", model.TaskName, model.ApiUrl,response.StatusCode);
            context.Result = $"{model.TaskName} : call apiurl [{model.ApiUrl}] result : {response.StatusCode}";
        }
        #endif


       
    }
}
using System.Diagnostics;
using System.Text.Json;
using Dncy.Diagnostics.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.Logging;

namespace Dncy.Diagnostics.AspNetCore
{
    public class AspNetCoreDiagnosticListener : IDiagnosticListener
    {
       
        public string Name => "Microsoft.AspNetCore";
        private readonly ILogger<AspNetCoreDiagnosticListener> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DiagnosticContextAccessor _diagnosticContextAccessor;

        public AspNetCoreDiagnosticListener(IHttpContextAccessor httpContextAccessor, ILogger<AspNetCoreDiagnosticListener> logger, DiagnosticContextAccessor diagnosticContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _diagnosticContextAccessor = diagnosticContextAccessor;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnNext(KeyValuePair<string, object> value)
        {

            if (value.Key is "Microsoft.AspNetCore.Mvc.BeforeOnActionExecuting" or "Microsoft.AspNetCore.Mvc.AfterOnActionExecuted")
            {
                if (value.Value.GetType().GetProperties().FirstOrDefault(x => x.Name == "ActionDescriptor")
                        ?.GetValue(value.Value) is ActionDescriptor actionDescriptor)
                {
                    var context=_diagnosticContextAccessor.Context;
                    if (value.Key is "Microsoft.AspNetCore.Mvc.BeforeOnActionExecuting")
                    {
                        context?.Items.Add(actionDescriptor.DisplayName+"Executing","开始执行");
                        if (context!=null)
                        {
                            context.Stopwatch.Start();
                        }
                    }
                    else
                    {
                        context?.Stopwatch?.Stop();
                        _logger.LogInformation("执行耗时：{DiagnosticContext.sp.ElapsedMilliseconds}ms",context?.Stopwatch?.ElapsedMilliseconds);
                        context?.Items.Add(actionDescriptor.DisplayName+"Executed",$"执行完毕，耗时：{context?.Stopwatch?.ElapsedMilliseconds}");
                        context?.Stopwatch?.Reset();
                        context?.Stopwatch?.Reset();
                    }
                }
            }
        }
    }
}
using System;
using System.Threading.Tasks;

namespace Dncy.PipelinePattern;

public interface IPipelineMiddleware
{
    Task InvokeAsync(DataContext context, IServiceProvider service, AsyncRequestDelegate next);
}
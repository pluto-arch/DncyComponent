using System.Threading.Tasks;

namespace Dncy.PipelinePattern;

public interface IPipelineMiddleware
{
    Task InvokeAsync(DataContext context, AsyncRequestDelegate next);
}
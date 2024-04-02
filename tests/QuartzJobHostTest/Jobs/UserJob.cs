using Dotnetydd.QuartzHost;
using Quartz;
using QuartzJobHostTest.Services;

namespace QuartzJobHostTest.Jobs;

public class UserJob: IQuartzJob
{
    private readonly DemoService _service;

    public UserJob(DemoService service)
    {
        _service = service;
    }


    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        await Task.Yield();
        context.Result = "UserJob执行完毕";
        _service.OutPutHash();
    }
}
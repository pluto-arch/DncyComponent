using Dotnetydd.QuartzHost;
using Quartz;
using QuartzJobHostTest.Services;

namespace QuartzJobHostTest.Jobs;

public class UserJob: QuartzJob<UserJob>
{
    private readonly DemoService _service;

    public UserJob(DemoService service):base(null)
    {
        _service = service;
    }


    public override async Task ExecuteAsync(IJobExecutionContext context)
    {
        await Task.Yield();
        context.Result = "UserJob执行完毕";
        _service.OutPutHash();
    }

}
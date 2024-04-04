using Dotnetydd.QuartzHost;
using Quartz;

namespace QuartzJobHostTest.Jobs;

public class DemoJob:QuartzJob<DemoJob>
{
    private readonly ILogger<DemoJob> _logger;

    public DemoJob(ILogger<DemoJob> logger): base(logger)
    {
        _logger = logger;
    }

    private static string[] log = new string[]
    {
        "E","I","W","D","T","C"
    };
    private  Random random = new Random();
    /// <inheritdoc />
    public override async Task ExecuteAsync(IJobExecutionContext context)
    {
        _logger.LogInformation("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
        var jobInfo = context.GetJobInfo();
        await Task.Delay(5000);
        var e = GetRandomLog();
        switch (e)
        {
            case "E":
                throw new ArgumentException(nameof(jobInfo));
            case "I":
                _logger.LogInformation("{Name} - 11111 {@JobInfo}", context?.FireInstanceId, jobInfo);
                break;
            case "W":
                _logger.LogWarning("{Name} - 22222 {@JobInfo}", context?.FireInstanceId, jobInfo);
                break;
            case "D":
                _logger.LogDebug("{Name} - 33333 {@JobInfo}", context?.FireInstanceId, jobInfo);
                break;
        }
    }

    string GetRandomLog()
    {
        int index = random.Next(0,99999)%log.Length;
        return log[index];
    }
}
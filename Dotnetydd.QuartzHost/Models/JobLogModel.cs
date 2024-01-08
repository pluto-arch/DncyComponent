namespace Dotnetydd.QuartzHost.Models;

public class JobLogModel
{
    public string JobKey { get; set; }

    public string Time { get; set; }

    public int RunSeconds { get; set; }

    public EnumJobStates State { get; set; }

    public string Message { get; set; }
}
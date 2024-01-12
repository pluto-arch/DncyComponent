using Dotnetydd.QuartzHost.Utils;
using Quartz;

namespace Dotnetydd.QuartzHost.Models;

public class JobInfoModel
{
    public string Id { get; set; }

    /// <summary>
    /// </summary>
    public EnumTaskType TaskType { get; set; } = EnumTaskType.StaticExecute;


    /// <summary>
    ///     显示名称
    /// </summary>
    public string DisplayName { get; set; } = "";

    /// <summary>
    ///     作业名称
    /// </summary>
    public string TaskName { get; set; } = "";

    /// <summary>
    ///     组名
    /// </summary>
    public string GroupName { get; set; } = "";

    /// <summary>
    /// </summary>
    public string Interval { get; set; } = "";

    /// <summary>
    ///     触发器名称
    /// </summary>
    public string TriggerName { get; set; } = "";


    /// <summary>
    ///     最后运行时间
    /// </summary>
    public string LastRunTime { get; set; }


    /// <summary>
    ///     调用的api地址
    /// </summary>
    public string ApiUrl { get; set; } = "";

    /// <summary>
    ///     接口授权key
    /// </summary>
    public string AuthKey { get; set; } = "";

    /// <summary>
    ///     接口授权value
    /// </summary>
    public string AuthValue { get; set; } = "";

    /// <summary>
    ///     描述
    /// </summary>
    public string Describe { get; set; } = "";

    /// <summary>
    ///     请求方式
    /// </summary>
    public string RequestType { get; set; } = "";

    /// <summary>
    /// 远程调用超时时长
    /// 默认 60s
    /// </summary>
    public int RemoteCallTimeOut { get; set; } = 60;

    /// <summary>
    ///     触发器状态
    /// </summary>
    public TriggerState TriggerStatus { get; set; } = TriggerState.Paused;

    /// <summary>
    ///     状态
    /// </summary>
    public EnumJobStates Status { get; set; } = EnumJobStates.Pause;


    public bool Selected { get; set; }

    /// <summary>
    ///     隐式数据转换
    /// </summary>
    /// <param name="geo"></param>
    public static implicit operator string(JobInfoModel geo)
    {
        return geo.ToString();
    }


    public static string GetResourceName(JobInfoModel resource, IEnumerable<JobInfoModel> allResources)
    {
        var count = 0;
        foreach (var item in allResources)
        {
            if (item.DisplayName == resource.DisplayName)
            {
                count++;
                if (count >= 2)
                {
                    return ResourceFormatter.GetName(resource.DisplayName, resource.Id);
                }
            }
        }
        return resource.DisplayName;
    }


    internal static class StringComparers
    {
        public static StringComparer ResourceName => StringComparer.Ordinal;
        public static StringComparer ResourceType => StringComparer.Ordinal;
        public static StringComparer UserTextSearch => StringComparer.CurrentCultureIgnoreCase;
    }
}
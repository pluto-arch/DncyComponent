using System;
using Quartz;

namespace Dncy.QuartzJob.Model
{
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
        public EnumJobStates Status { get; set; }= EnumJobStates.Pause;


        /// <summary>
        ///     隐式数据转换
        /// </summary>
        /// <param name="geo"></param>
        public static implicit operator string(JobInfoModel geo)
        {
            return geo.ToString();
        }

        ///// <summary>
        /////     显示数据转换
        ///// </summary>
        ///// <param name="str"></param>
        //public static explicit operator JobInfoModel(string str)
        //{
        //    if (!string.IsNullOrEmpty(str))
        //    {
        //        var strarr = str.Split('^');
        //        if (strarr==null)
        //        {
        //            return null;
        //        }
        //        return new JobInfoModel
        //        {
        //            Id = strarr[0],
        //            TaskType =(EnumTaskType)Enum.Parse(typeof(EnumTaskType),strarr[1]),
        //            DisplayName = strarr[2],
        //            TaskName = strarr[3],
        //            GroupName = strarr[4],
        //            Interval = strarr[5],
        //            TriggerName = strarr[6],
        //            ApiUrl = strarr[7],
        //            AuthKey = strarr[8],
        //            AuthValue = strarr[9],
        //            Describe = strarr[10],
        //            RequestType = strarr[11],
        //            RemoteCallTimeOut = int.Parse(strarr[12]),
        //            TriggerStatus = (TriggerState)Enum.Parse(typeof(TriggerState),strarr[13]),
        //            Status =(EnumJobStates)Enum.Parse(typeof(EnumJobStates),strarr[14]),
        //        };
        //    }

        //    return null;
        //}



        ///// <inheritdoc />
        //public override string ToString()
        //{
        //    return $"{Id}^{TaskType}^{DisplayName}^{GroupName}^{TaskName}^{Interval}^{TriggerName}^{ApiUrl}^{AuthKey}^{AuthValue}^{Describe}^{RequestType}^{RemoteCallTimeOut}^{TriggerStatus}^{Status};";
        //}

    }
}


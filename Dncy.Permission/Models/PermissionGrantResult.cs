namespace Dotnetydd.Permission.Models
{
    public enum PermissionGrantResult
    {
        /// <summary>
        /// 未定义
        /// </summary>
        Undefined = 0x01,
        /// <summary>
        /// 已授权
        /// </summary>
        Granted = 0x02,
        /// <summary>
        /// 禁止
        /// </summary>
        Prohibited = 0x03
    }
}


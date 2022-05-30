using System;

namespace Dncy.Permission
{
    public interface IPermissionGrant
    {
        /// <summary>
        ///     权限名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        ///     被授权主体提供者名称
        ///     例如：单个用户、角色、部门
        /// </summary>
        public string ProviderName { get; set; }


        /// <summary>
        ///     被授予主体的值
        ///     比如 用户id，角色id，部门id
        /// </summary>
        public string ProviderKey { get; set; }
    }
}
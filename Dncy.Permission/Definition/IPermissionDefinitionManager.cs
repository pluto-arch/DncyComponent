using Dotnetydd.Permission.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dotnetydd.Permission.Definition
{
    /// <summary>
    /// 权限定义管理器
    /// </summary>
    public interface IPermissionDefinitionManager
    {
        /// <summary>
        /// 根据权限名称获取权限定义
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">无效的权限名称</exception>
        PermissionDefinition Get([NotNull] string name);

        /// <summary>
        /// 根据权限名称获取权限定义
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        PermissionDefinition GetOrNull([NotNull] string name);

        /// <summary>
        /// 获取权限合集
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<PermissionDefinition> GetPermissions();


        /// <summary>
        /// 获取权限组
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<PermissionGroupDefinition> GetGroups();
    }
}
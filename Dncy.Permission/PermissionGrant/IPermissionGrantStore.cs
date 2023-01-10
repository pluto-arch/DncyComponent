using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dncy.Permission
{
    public interface IPermissionGrantStore
    {
        /// <summary>
        /// 获取权限授予信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="providerName"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        Task<IPermissionGrant> GetAsync(string name,string providerName, string providerKey);

        /// <summary>
        /// 获取权限的授予表
        /// </summary>
        /// <param name="providerName">权限值提供者名称。eg. role,user</param>
        /// <param name="providerKey">权限值提供者值，eg. roleid，userid</param>
        /// <returns></returns>
        Task<IEnumerable<IPermissionGrant>> GetListAsync(string providerName, string providerKey);

        /// <summary>
        /// 获取权限的授予表
        /// </summary>
        /// <param name="names">权限名称</param>
        /// <param name="providerName">权限值提供者名称。eg. role,user</param>
        /// <param name="providerKey">权限值提供者值，eg. roleid，userid</param>
        /// <returns></returns>
        Task<IEnumerable<IPermissionGrant>> GetListAsync(string[] names, string providerName, string providerKey);


        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="name">权限</param>
        /// <param name="providerName"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        Task SaveAsync(string name, string providerName, string providerKey);


        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="name">权限</param>
        /// <param name="providerName"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        Task SaveAsync(string[] name, string providerName, string providerKey);


        /// <summary>
        /// 取消授权
        /// </summary>
        /// <param name="name">权限</param>
        /// <param name="providerName"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        Task RemoveGrantAsync(string name, string providerName, string providerKey);

        /// <summary>
        /// 取消授权
        /// </summary>
        /// <param name="name">权限</param>
        /// <param name="providerName"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        Task RemoveGrantAsync(string[] name, string providerName, string providerKey);
    }
}
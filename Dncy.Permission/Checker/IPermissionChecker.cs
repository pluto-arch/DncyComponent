using Dotnetydd.Permission.Models;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dotnetydd.Permission.Checker
{
    public interface IPermissionChecker
    {
        /// <summary>
        /// 是否授权
        /// </summary>
        /// <param name="claimsPrincipal">主体</param>
        /// <param name="name">权限名称</param>
        /// <returns></returns>
        Task<bool> IsGrantedAsync([MaybeNull] ClaimsPrincipal claimsPrincipal, [NotNull] string name);


        /// <summary>
        /// 是否授权
        /// </summary>
        /// <param name="claimsPrincipal">主体</param>
        /// <param name="names">权限集合</param>
        /// <returns></returns>
        Task<MultiplePermissionGrantResult> IsGrantedAsync([MaybeNull] ClaimsPrincipal claimsPrincipal, [NotNull] string[] names);
    }
}


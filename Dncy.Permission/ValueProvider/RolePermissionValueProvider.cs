using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dncy.Permission.Models;

namespace Dncy.Permission
{
    /// <summary>
    ///     角色级别的权限值检测提供程序
    /// </summary>
    public class RolePermissionValueProvider : IPermissionValueProvider
    {
        private readonly IPermissionManager _manager;

        public RolePermissionValueProvider(IPermissionManager manager)
        {
            _manager = manager;
        }

        public string Name => "Role";


        /// <inheritdoc />
        public async Task<PermissionGrantResult> CheckAsync(ClaimsPrincipal principal, PermissionDefinition permission)
        {
            string[] roles = principal?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
            if (roles == null || !roles.Any())
            {
                return PermissionGrantResult.Prohibited;
            }

            foreach (string role in roles)
            {
                if (await _manager.IsGrantedAsync(permission.Name, Name, role))
                {
                    return PermissionGrantResult.Granted;
                }
            }

            return PermissionGrantResult.Prohibited;
        }

        /// <inheritdoc />
        public async Task<MultiplePermissionGrantResult> CheckAsync(ClaimsPrincipal principal, List<PermissionDefinition> permissions)
        {
            var permissionNames = permissions.Select(x => x.Name).ToList();
            var result = new MultiplePermissionGrantResult(permissionNames.ToArray());

            string[] roles = principal?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

            if (roles is null || !roles.Any())
            {
                return result;
            }

            foreach (string role in roles)
            {
                var multipleResult = await _manager.IsGrantedAsync(permissionNames.ToArray(), Name, role);
                var list = multipleResult.Result.Where(grantResult =>
                        result.Result.ContainsKey(grantResult.Key) &&
                        result.Result[grantResult.Key] == PermissionGrantResult.Undefined &&
                        grantResult.Value != PermissionGrantResult.Undefined);
                foreach (var grantResult in list)
                {
                    result.Result[grantResult.Key] = grantResult.Value;
                    permissionNames.RemoveAll(x => x == grantResult.Key);
                }

                if (result.AllGranted || result.AllProhibited)
                {
                    break;
                }
            }

            return result;
        }
    }
}
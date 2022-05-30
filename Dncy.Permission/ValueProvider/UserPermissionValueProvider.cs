using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dncy.Permission.Models;

namespace Dncy.Permission
{
      /// <summary>
    ///     用户级别的权限值检测提供程序
    /// </summary>
    public class UserPermissionValueProvider : IPermissionValueProvider
    {
        private readonly IPermissionManager _manager;

        public UserPermissionValueProvider(IPermissionManager manager)
        {
            _manager = manager;
        }

        public string Name => "User";


        public async Task<PermissionGrantResult> CheckAsync(ClaimsPrincipal principal, PermissionDefinition permission)
        {
            string id = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id))
            {
                return PermissionGrantResult.Prohibited;
            }

            if (await _manager.IsGrantedAsync(permission.Name, Name, id))
            {
                return PermissionGrantResult.Granted;
            }

            return PermissionGrantResult.Prohibited;
        }

        public async Task<MultiplePermissionGrantResult> CheckAsync(ClaimsPrincipal principal,
            List<PermissionDefinition> permissions)
        {
            var permissionNames = permissions.Select(x => x.Name).ToList();
            var result = new MultiplePermissionGrantResult(permissionNames.ToArray());

            string id = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(id))
            {
                return result;
            }

            var multipleResult = await _manager.IsGrantedAsync(permissionNames.ToArray(), Name, id);
            var res = multipleResult.Result.Where(grantResult =>
                    result.Result.ContainsKey(grantResult.Key) &&
                    result.Result[grantResult.Key] == PermissionGrantResult.Undefined &&
                    grantResult.Value != PermissionGrantResult.Undefined);
            foreach (var grantResult in res)
            {
                result.Result[grantResult.Key] = grantResult.Value;
                permissionNames.RemoveAll(x => x == grantResult.Key);
            }

            return result;
        }
    }
}
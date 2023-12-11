using Dotnetydd.Permission.Models;
using Dotnetydd.Permission.PermissionGrant;
using Dotnetydd.Permission.PermissionManager;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dotnetydd.Permission.ValueProvider
{
    /// <summary>
    ///     用户级别的权限值检测提供程序
    /// </summary>
    public class UserPermissionValueProvider : IPermissionValueProvider
    {
        protected readonly IPermissionGrantStore _grantStore;
        protected readonly ILogger<UserPermissionValueProvider> _logger;
        protected readonly IPermissionManager _manager;

        public UserPermissionValueProvider(
            IPermissionGrantStore grantStore,
            IPermissionManager permissionManager,
            ILogger<UserPermissionValueProvider> logger = null)
        {
            _grantStore = grantStore;
            _manager = permissionManager;
            _logger = logger ?? NullLogger<UserPermissionValueProvider>.Instance;
        }

        public string Name => "User";


        public virtual async Task<PermissionGrantResult> CheckAsync(ClaimsPrincipal principal, PermissionDefinition permission)
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

        public virtual async Task<MultiplePermissionGrantResult> CheckAsync(ClaimsPrincipal principal,
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
            foreach (var grantResult in multipleResult.Result)
            {
                if (result.Result.ContainsKey(grantResult.Key))
                {
                    if (result.Result[grantResult.Key] == PermissionGrantResult.Granted || result.Result[grantResult.Key] == PermissionGrantResult.Undefined)
                    {
                        continue;
                    }
                    result.Result[grantResult.Key] = grantResult.Value;
                }
                else
                {
                    result.Result[grantResult.Key] = grantResult.Value;
                }
            }
            return result;
        }
    }
}
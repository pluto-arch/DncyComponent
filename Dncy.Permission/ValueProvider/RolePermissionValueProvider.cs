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
    ///     角色级别的权限值检测提供程序
    /// </summary>
    public class RolePermissionValueProvider : IPermissionValueProvider
    {
        protected readonly IPermissionGrantStore _grantStore;
        protected readonly ILogger<RolePermissionValueProvider> _logger;
        protected readonly IPermissionManager _manager;

        public RolePermissionValueProvider(
            IPermissionGrantStore grantStore,
            IPermissionManager permissionManager,
            ILogger<RolePermissionValueProvider> logger = null)
        {
            _grantStore = grantStore;
            _manager = permissionManager;
            _logger = logger ?? NullLogger<RolePermissionValueProvider>.Instance;
        }

        public string Name => "Role";


        /// <inheritdoc />
        public virtual async Task<PermissionGrantResult> CheckAsync(ClaimsPrincipal principal, PermissionDefinition permission)
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
        public virtual async Task<MultiplePermissionGrantResult> CheckAsync(ClaimsPrincipal principal, List<PermissionDefinition> permissions)
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
            }

            return result;
        }
    }
}
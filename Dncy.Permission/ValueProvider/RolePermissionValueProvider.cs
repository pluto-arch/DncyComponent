using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dncy.Permission.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dncy.Permission
{
    /// <summary>
    ///     角色级别的权限值检测提供程序
    /// </summary>
    public class RolePermissionValueProvider : IPermissionValueProvider
    {
        private readonly IPermissionGrantStore _grantStore;
        private readonly ILogger<RolePermissionValueProvider> _logger;

        public RolePermissionValueProvider(
            IPermissionGrantStore grantStore, 
            ILogger<RolePermissionValueProvider> logger=null)
        {
            _grantStore = grantStore;
            _logger = logger??NullLogger<RolePermissionValueProvider>.Instance;
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
                var grantInfo = await _grantStore.GetAsync(permission.Name, Name, role);
                if (grantInfo!=null)
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
                var grantInfos = await _grantStore.GetListAsync(permissionNames.ToArray(), Name, role);
                foreach (var item in permissionNames)
                {
                    var grantInfo = grantInfos.SingleOrDefault(x=>x.Name==item);
                    result.Result.Add(item,grantInfo!=null?PermissionGrantResult.Granted:PermissionGrantResult.Prohibited);
                }

                if (result.AllGranted)
                {
                    break;
                }
            }

            return result;
        }
    }
}
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
    ///     用户级别的权限值检测提供程序
    /// </summary>
    public class UserPermissionValueProvider : IPermissionValueProvider
    {
        private readonly IPermissionGrantStore _grantStore;
        private readonly ILogger<UserPermissionValueProvider> _logger;

        public UserPermissionValueProvider(IPermissionGrantStore grantStore, ILogger<UserPermissionValueProvider> logger=null)
        {
            _grantStore = grantStore;
            _logger = logger??NullLogger<UserPermissionValueProvider>.Instance;
        }

        public string Name => "User";


        public virtual async Task<PermissionGrantResult> CheckAsync(ClaimsPrincipal principal, PermissionDefinition permission)
        {
            string id = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id))
            {
                return PermissionGrantResult.Prohibited;
            }
            var grantInfo = await _grantStore.GetAsync(permission.Name, Name, id);
            if (grantInfo!=null)
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
            var grantInfos = await _grantStore.GetListAsync(permissionNames.ToArray(), Name, id);
            foreach (var item in permissionNames)
            {
                var grant = grantInfos.SingleOrDefault(x=>x.Name==item);
                result.Result.Add(item,grant!=null?PermissionGrantResult.Granted:PermissionGrantResult.Prohibited);
            }
            return result;
        }
    }
}
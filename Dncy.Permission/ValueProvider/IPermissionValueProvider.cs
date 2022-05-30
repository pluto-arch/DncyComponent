using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Dncy.Permission.Models;

namespace Dncy.Permission
{
    public interface IPermissionValueProvider
    {
        string Name { get; }

        Task<PermissionGrantResult> CheckAsync(ClaimsPrincipal principal, PermissionDefinition permission);

        Task<MultiplePermissionGrantResult> CheckAsync(ClaimsPrincipal principal, List<PermissionDefinition> permissions);
    }
}
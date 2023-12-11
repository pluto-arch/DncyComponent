using Dotnetydd.Permission.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dotnetydd.Permission.ValueProvider
{
    public interface IPermissionValueProvider
    {
        string Name { get; }

        Task<PermissionGrantResult> CheckAsync(ClaimsPrincipal principal, PermissionDefinition permission);

        Task<MultiplePermissionGrantResult> CheckAsync(ClaimsPrincipal principal, List<PermissionDefinition> permissions);
    }
}
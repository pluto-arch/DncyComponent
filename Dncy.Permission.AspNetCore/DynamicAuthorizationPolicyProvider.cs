﻿using Dotnetydd.Permission.Definition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Dotnetydd.Permission.AspNetCore
{
    public class DynamicAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly IPermissionDefinitionManager _permissionDefinitionManager;

        /// <inheritdoc />
        public DynamicAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options,
            IPermissionDefinitionManager permissionDefinitionManager) : base(options)
        {
            _permissionDefinitionManager = permissionDefinitionManager;
        }


        /// <inheritdoc />
        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            AuthorizationPolicy policy = await base.GetPolicyAsync(policyName);

            if (policy != null)
            {
                return policy;
            }

            var permission = _permissionDefinitionManager.GetOrNull(policyName);

            if (permission != null)
            {
                AuthorizationPolicyBuilder policyBuilder = new AuthorizationPolicyBuilder(Array.Empty<string>());
                policyBuilder.Requirements.Add(new OperationAuthorizationRequirement { Name = policyName });
                return policyBuilder.Build();
            }

            return null;
        }
    }
}


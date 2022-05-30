using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Dncy.Permission.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Dncy.Permission
{
    public class DefaultPermissionDefinitionManager : IPermissionDefinitionManager
    {
        private readonly Lazy<Dictionary<string, PermissionDefinition>> _lazyPermissionDefinitions;

        private readonly Lazy<Dictionary<string, PermissionGroupDefinition>> _lazyPermissionGroupDefinitions;

        private readonly IServiceProvider _serviceProvider;

        public DefaultPermissionDefinitionManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _lazyPermissionGroupDefinitions = new Lazy<Dictionary<string, PermissionGroupDefinition>>(CreatePermissionGroupDefinitions, true);
            _lazyPermissionDefinitions = new Lazy<Dictionary<string, PermissionDefinition>>(CreatePermissionDefinitions, true);
        }


        protected IDictionary<string, PermissionGroupDefinition> PermissionGroupDefinitions => _lazyPermissionGroupDefinitions.Value;

        protected IDictionary<string, PermissionDefinition> PermissionDefinitions => _lazyPermissionDefinitions.Value;

        /// <inheritdoc />
        public virtual PermissionDefinition Get([NotNull] string name)
        {
            PermissionDefinition permission = GetOrNull(name);

            if (permission == null)
            {
                throw new InvalidOperationException($"Undefined permission {name}");
            }

            return permission;
        }

        /// <inheritdoc />
        public virtual PermissionDefinition GetOrNull([NotNull] string name)
        {
            return PermissionDefinitions.TryGetValue(name, out PermissionDefinition obj) ? obj : default;
        }

        /// <inheritdoc />
        public virtual IReadOnlyList<PermissionDefinition> GetPermissions()
        {
            return PermissionDefinitions.Values.ToImmutableList();
        }

        /// <inheritdoc />
        public virtual IReadOnlyList<PermissionGroupDefinition> GetGroups()
        {
            return PermissionGroupDefinitions.Values.ToImmutableList();
        }


        protected virtual Dictionary<string, PermissionDefinition> CreatePermissionDefinitions()
        {
            var permissions = new Dictionary<string, PermissionDefinition>();

            foreach (PermissionGroupDefinition groupDefinition in PermissionGroupDefinitions.Values)
            {
                foreach (PermissionDefinition permission in groupDefinition.Permissions)
                {
                    AddPermissionToDictionaryRecursively(permissions, permission);
                }
            }

            return permissions;
        }


        protected virtual void AddPermissionToDictionaryRecursively(Dictionary<string, PermissionDefinition> permissions, PermissionDefinition permission)
        {
            if (permissions.ContainsKey(permission.Name))
            {
                throw new InvalidOperationException($"Duplicate permission name {permission.Name}");
            }

            permissions[permission.Name] = permission;

            foreach (PermissionDefinition child in permission.Children)
            {
                AddPermissionToDictionaryRecursively(permissions, child);
            }
        }

        protected virtual Dictionary<string, PermissionGroupDefinition> CreatePermissionGroupDefinitions()
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            var context = new PermissionDefinitionContext(scope.ServiceProvider);
            var providers = _serviceProvider.GetServices<IPermissionDefinitionProvider>();
            foreach (IPermissionDefinitionProvider provider in providers)
            {
                provider.Define(context);
            }
            return context.Groups;
        }
    }
}


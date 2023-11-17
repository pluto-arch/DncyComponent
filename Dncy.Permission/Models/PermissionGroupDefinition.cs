using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Dncy.Permission.Models
{
    public class PermissionGroupDefinition
    {
        private readonly List<PermissionDefinition> _permissions = new List<PermissionDefinition>();


        protected internal PermissionGroupDefinition([NotNull] string name, string displayName = null)
        {
            Name = name;
            DisplayName = displayName;
        }

        /// <summary>
        ///     名称
        /// </summary>
        public string Name { get; }


        /// <summary>
        ///     显示名称
        /// </summary>
        public string DisplayName { get; set; }


        public IReadOnlyList<PermissionDefinition> Permissions => _permissions.ToImmutableList();


        public virtual PermissionDefinition AddPermission([NotNull] string name, string displayName = null, bool isEnabled = true)
        {
            PermissionDefinition permission = new PermissionDefinition(name, displayName, isEnabled){Group=this.Name};
            _permissions.Add(permission);
            return permission;
        }


        public virtual List<PermissionDefinition> GetPermissionsWithChildren()
        {
            List<PermissionDefinition> permissions = new List<PermissionDefinition>();

            foreach (PermissionDefinition permission in _permissions)
            {
                AddPermissionToListRecursively(permissions, permission);
            }

            return permissions;
        }

        public PermissionDefinition GetPermissionOrNull([NotNull] string name)
        {
            return GetPermissionOrNullRecursively(Permissions, name);
        }

        private void AddPermissionToListRecursively(List<PermissionDefinition> permissions, PermissionDefinition permission)
        {
            permissions.Add(permission);

            foreach (PermissionDefinition child in permission.Children)
            {
                AddPermissionToListRecursively(permissions, child);
            }
        }

        private PermissionDefinition GetPermissionOrNullRecursively(IReadOnlyList<PermissionDefinition> permissions, string name)
        {
            foreach (PermissionDefinition permission in permissions)
            {
                if (permission.Name == name)
                {
                    return permission;
                }

                PermissionDefinition childPermission = GetPermissionOrNullRecursively(permission.Children, name);
                if (childPermission != null)
                {
                    return childPermission;
                }
            }

            return null!;
        }
    }
}


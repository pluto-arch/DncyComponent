using Dotnetydd.Permission.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dotnetydd.Permission.Definition
{
    public class PermissionDefinitionContext
    {
        internal PermissionDefinitionContext()
        {
            Groups = new Dictionary<string, PermissionGroupDefinition>();
        }

        internal Dictionary<string, PermissionGroupDefinition> Groups { get; }

        public virtual PermissionGroupDefinition AddGroup(string name, string displayName = null)
        {
            if (Groups.ContainsKey(name))
            {
                throw new InvalidOperationException($"There is already an existing permission group with name: {name}");
            }

            return Groups[name] = new PermissionGroupDefinition(name, displayName);
        }


        public virtual PermissionGroupDefinition GetGroup([NotNull] string name)
        {
            PermissionGroupDefinition group = GetGroupOrNull(name);

            if (group is null)
            {
                throw new InvalidOperationException(
                    $"Could not find a permission definition group with the given name: {name}");
            }

            return group;
        }

        public virtual PermissionGroupDefinition GetGroupOrNull([NotNull] string name)
        {
            if (!Groups.ContainsKey(name))
            {
                return null;
            }

            return Groups[name];
        }


        public virtual void RemoveGroup([NotNull] string name)
        {
            if (!Groups.ContainsKey(name))
            {
                throw new InvalidOperationException($"Not found permission group with name: {name}");
            }

            Groups.Remove(name);
        }

        public virtual PermissionDefinition GetPermissionOrNull([NotNull] string name)
        {
            foreach (PermissionGroupDefinition groupDefinition in Groups.Values)
            {
                PermissionDefinition permissionDefinition = groupDefinition.GetPermissionOrNull(name);

                if (permissionDefinition != null)
                {
                    return permissionDefinition;
                }
            }

            return null;
        }
    }
}
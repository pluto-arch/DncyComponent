using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Dncy.Permission.Models
{
    public class PermissionDefinition
    {
        /// <summary>
        ///     下级权限定义
        /// </summary>
        private readonly List<PermissionDefinition> _children = new List<PermissionDefinition>();

        public PermissionDefinition()
        {
            Name = string.Empty;
        }

        public PermissionDefinition([NotNull] string name, string displayName = null, bool isEnabled = true)
        {
            Name = name;
            DisplayName = displayName;
            IsEnabled = isEnabled;
        }


        /// <summary>
        ///     名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        ///     显示名称
        /// </summary>
        public string DisplayName { get; set; }


        /// <summary>
        ///     接口地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     上级权限
        /// </summary>
        public int ParentId { get; set; }


        /// <summary>
        ///     分组id
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        ///     分组
        /// </summary>
        public PermissionGroupDefinition Group { get; set; }


        /// <summary>
        ///     上级权限
        /// </summary>
        public PermissionDefinition Parent { get; private set; }

        /// <summary>
        ///     是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     允许的提供者，eg. role/dept
        /// </summary>
        public List<string> AllowedProviders { get; set; } = new List<string>();

        public IReadOnlyList<PermissionDefinition> Children => _children.ToImmutableList();


        public virtual PermissionDefinition AddChild([NotNull] string name, string displayName = null,
            bool isEnabled = true)
        {
            PermissionDefinition child = new PermissionDefinition(name, displayName, isEnabled) { Parent = this };
            _children.Add(child);
            return child;
        }

        public virtual PermissionDefinition WithProviders(params string[] providers)
        {
            if (providers != null && providers.Any())
            {
                AllowedProviders.AddRange(providers);
            }

            return this;
        }
    }
}
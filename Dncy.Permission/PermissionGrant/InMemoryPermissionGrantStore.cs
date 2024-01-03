using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnetydd.Permission.PermissionGrant
{
    public class InMemoryPermissionGrantStore : IPermissionGrantStore
    {
        private static readonly HashSet<IPermissionGrant> _grants = new HashSet<IPermissionGrant>();

        private static readonly object _lock = new object();

        private static ImmutableList<IPermissionGrant> Query
        {
            get
            {
                lock (_lock)
                {
                    return _grants.ToImmutableList();
                }
            }
        }

        /// <inheritdoc />
        public Task<IPermissionGrant> GetAsync(string name, string providerName, string providerKey)
        {
            var res = Query.SingleOrDefault(s => s.Name == name && s.ProviderName == providerName && s.ProviderKey == providerKey);
            return Task.FromResult(res);
        }

        /// <inheritdoc />
        public Task<IEnumerable<IPermissionGrant>> GetListAsync(string providerName, string providerKey)
        {
            var res = Query.Where(s => s.ProviderName == providerName && s.ProviderKey == providerKey);
            return Task.FromResult(res);
        }

        /// <inheritdoc />
        public Task<IEnumerable<IPermissionGrant>> GetListAsync(string[] names, string providerName, string providerKey)
        {
            var res = Query.Where(s => names.Contains(s.Name) && s.ProviderName == providerName && s.ProviderKey == providerKey);
            return Task.FromResult(res);
        }

        /// <inheritdoc />
        public Task SaveAsync(string name, string providerName, string providerKey)
        {
            lock (_lock)
            {
                _grants.Add(new PermissionGrant(name, providerName, providerKey));
                return Task.CompletedTask;
            }
        }


        /// <inheritdoc />
        public Task SaveAsync(string[] name, string providerName, string providerKey)
        {
            lock (_lock)
            {
                foreach (var item in name)
                {
                    _grants.Add(new PermissionGrant(item, providerName, providerKey));
                }
                return Task.CompletedTask;
            }
        }


        /// <inheritdoc />
        public Task RemoveGrantAsync(string name, string providerName, string providerKey)
        {
            lock (_lock)
            {
                _grants.RemoveWhere(x => x.Name == name && x.ProviderKey == providerKey && x.ProviderName == providerName);
                return Task.CompletedTask;
            }

        }

        /// <inheritdoc />
        public Task RemoveGrantAsync(string[] name, string providerName, string providerKey)
        {
            lock (_lock)
            {
                foreach (var item in name)
                {
                    _grants.RemoveWhere(x => x.Name == item && x.ProviderKey == providerKey && x.ProviderName == providerName);
                }
                return Task.CompletedTask;
            }
        }

        internal struct PermissionGrant : IPermissionGrant
        {
            public PermissionGrant(string name, string providerName, string providerKey)
            {
                Name = name;
                ProviderName = providerName;
                ProviderKey = providerKey;
            }

            public string Name { get; set; }
            public string ProviderName { get; set; }
            public string ProviderKey { get; set; }
        }

    }
}
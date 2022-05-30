using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Dncy.Permission
{
    public class InMemoryPermissionGrantStore:IPermissionGrantStore
    {
        private static readonly List<IPermissionGrant> _grants = new List<IPermissionGrant>();

        private static readonly object _lock=new object();

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
        public Task GrantAsync(IPermissionGrant grant)
        {
            lock (_lock)
            {
                _grants.Add(grant);
                return Task.CompletedTask;
            }
        }

        /// <inheritdoc />
        public Task CancleGrantAsync(IPermissionGrant grant)
        {
            lock (_lock)
            {
                _grants.Remove(grant);
                return Task.CompletedTask;
            }
           
        }

        /// <inheritdoc />
        public Task CancleGrantAsync(List<IPermissionGrant> grants)
        {
            if (grants==null||!grants.Any())
            {
                return Task.CompletedTask;
            }
            lock (_lock)
            {
                foreach (var grant in grants)
                {
                    _grants.Remove(grant);
                }
                return Task.CompletedTask;
            }
        }
    }
}
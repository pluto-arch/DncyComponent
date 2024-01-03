using Dotnetydd.MultiTenancy.ConnectionStrings;
using Dotnetydd.MultiTenancy.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnetydd.MultiTenancy.Store
{
    public class DefaultTenantStore : ITenantStore
    {
        private readonly TenantConfiguration[] _tenants;

        public DefaultTenantStore(IOptions<TenantConfigurationOptions> options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            _tenants = options.Value?.Tenants;
        }


        /// <inheritdoc />
        public Task<TenantInfo> FindAsync(string name)
        {
            if (IsNullOrEmpty())
                return Task.FromResult<TenantInfo>(default);
            var t = _tenants.FirstOrDefault(x => x.TenantName == name);
            if (t == null)
                return Task.FromResult<TenantInfo>(default);
            return Task.FromResult(new TenantInfo
            {
                Id = t.TenantId,
                Name = t.TenantName,
                ConnectionStrings = t.ConnectionStrings,
                IsAvaliable = t.IsAvaliable
            });
        }

        /// <inheritdoc />
        public Task<TenantInfo> FindAsync<Tkey>(Tkey id)
        {
            if (IsNullOrEmpty())
                return Task.FromResult<TenantInfo>(default);
            var t = _tenants.FirstOrDefault(x => x.TenantId == id.ToString());
            if (t == null)
                return Task.FromResult<TenantInfo>(default);
            return Task.FromResult(new TenantInfo
            {
                Id = t.TenantId,
                Name = t.TenantName,
                ConnectionStrings = t.ConnectionStrings,
                IsAvaliable = t.IsAvaliable
            });
        }

        /// <inheritdoc />
        public TenantInfo Find(string name)
        {
            if (IsNullOrEmpty())
                return null;
            var t = _tenants.FirstOrDefault(x => x.TenantName == name);
            if (t == null)
                return null;
            return new TenantInfo
            {
                Id = t.TenantId,
                Name = t.TenantName,
                ConnectionStrings = t.ConnectionStrings,
                IsAvaliable = t.IsAvaliable
            };
        }

        /// <inheritdoc />
        public TenantInfo Find<Tkey>(Tkey id)
        {
            if (IsNullOrEmpty())
                return null;
            var t = _tenants.FirstOrDefault(x => x.TenantId == id.ToString());
            if (t == null)
                return null;
            return new TenantInfo
            {
                Id = t.TenantId,
                Name = t.TenantName,
                ConnectionStrings = t.ConnectionStrings,
                IsAvaliable = t.IsAvaliable
            };
        }

        private bool IsNullOrEmpty()
        {
            if (_tenants == null || !_tenants.Any())
            {
                return true;
            }

            return false;
        }
    }
}


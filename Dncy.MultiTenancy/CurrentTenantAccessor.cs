using Dotnetydd.MultiTenancy.Model;
using System.Threading;

namespace Dotnetydd.MultiTenancy
{
    public class CurrentTenantAccessor : ICurrentTenantAccessor
    {
        private readonly AsyncLocal<TenantInfo> _currentScope = new AsyncLocal<TenantInfo>();

        public TenantInfo CurrentTenantInfo
        {
            get => _currentScope.Value;
            set => _currentScope.Value = value;
        }
    }
}


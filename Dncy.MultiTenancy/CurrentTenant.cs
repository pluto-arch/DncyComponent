using Dotnetydd.MultiTenancy.Model;
using System;

namespace Dotnetydd.MultiTenancy
{
    public class CurrentTenant : ICurrentTenant
    {
        private readonly ICurrentTenantAccessor _currentTenantAccessor;

        public CurrentTenant(ICurrentTenantAccessor currentTenantAccessor)
        {
            _currentTenantAccessor = currentTenantAccessor;
        }


        /// <inheritdoc />
        public virtual bool IsAvailable => !string.IsNullOrEmpty(Id) && !string.IsNullOrWhiteSpace(Id);

        /// <inheritdoc />
        public virtual string Name => _currentTenantAccessor.CurrentTenantInfo?.Name;

        /// <inheritdoc />
        public virtual string Id => _currentTenantAccessor.CurrentTenantInfo?.Id;


        /// <summary>
        /// 切换租户
        /// </summary>
        /// <returns></returns>
        public IDisposable Change(TenantInfo tenant)
        {
            var parentScope = _currentTenantAccessor.CurrentTenantInfo;
            _currentTenantAccessor.CurrentTenantInfo = tenant;
            return new DisposeAction(() =>
            {
                _currentTenantAccessor.CurrentTenantInfo = parentScope;
            });
        }
    }
}


using Dotnetydd.MultiTenancy.Model;
using System;

namespace Dotnetydd.MultiTenancy
{
    public interface ICurrentTenant
    {
        bool IsAvailable { get; }

        string Name { get; }

        string Id { get; }

        /// <summary>
        /// 切换租户
        /// </summary>
        /// <returns></returns>
        IDisposable Change(TenantInfo tenant);
    }
}


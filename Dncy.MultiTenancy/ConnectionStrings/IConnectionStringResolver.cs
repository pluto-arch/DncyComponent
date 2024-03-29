﻿using System.Threading.Tasks;

namespace Dotnetydd.MultiTenancy
{
    public interface IConnectionStringResolver
    {
        /// <summary>
        /// 获取对应名称的连接字符串
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        Task<string> GetAsync(string connectionStringName = null);
    }
}


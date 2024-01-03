using System;
using System.Collections.Generic;

namespace Dotnetydd.MultiTenancy
{
    /// <summary>
    /// tenant info in configuration
    /// </summary>
    [Serializable]
    public class TenantConfiguration
    {

        public TenantConfiguration()
        {

        }

        public TenantConfiguration(string tenantId, string tenantName, Dictionary<string, string> connectionStrings = null)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            ConnectionStrings = connectionStrings;
        }


        public string TenantId { get; set; }

        public string TenantName { get; set; }

        public bool IsAvaliable { get; set; }

        public Dictionary<string, string> ConnectionStrings { get; set; }
    }


    /// <summary>
    /// read tenant from configuration
    /// </summary>
    public class TenantConfigurationOptions
    {
        public TenantConfiguration[] Tenants { get; set; }
    }
}


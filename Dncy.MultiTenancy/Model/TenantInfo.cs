using System;
using System.Collections.Generic;

namespace Dotnetydd.MultiTenancy.Model
{
    [Serializable]
    public class TenantInfo
    {
        public TenantInfo()
        {
        }
        public TenantInfo(string id, string name = null)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; set; }

        public string Name { get; set; }


        public Dictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();


        public bool IsAvaliable { get; set; }

    }
}



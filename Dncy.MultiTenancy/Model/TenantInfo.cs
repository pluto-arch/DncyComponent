using System;

namespace Dncy.MultiTenancy.Model
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
    }
}



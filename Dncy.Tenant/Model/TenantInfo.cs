namespace Dncy.MultiTenancy.Model;


public class TenantInfo
{
    public TenantInfo(string id, string name = null)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; set; }

    public string Name { get; set; }
}
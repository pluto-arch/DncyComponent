namespace Dotnetydd.MultiTenancy.AspNetCore
{
    public interface ITenantResolver
    {
        string ResolveTenantIdOrName();
    }
}


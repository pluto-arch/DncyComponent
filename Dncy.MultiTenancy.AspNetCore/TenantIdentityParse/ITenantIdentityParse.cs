namespace Dotnetydd.MultiTenancy.AspNetCore.TenantIdentityParse
{
    public interface ITenantIdentityParse
    {
        string Name { get; }
        void Resolve(ITenantResolveContext context);
    }
}


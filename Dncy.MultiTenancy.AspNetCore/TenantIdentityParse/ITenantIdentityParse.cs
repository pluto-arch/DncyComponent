namespace Dncy.MultiTenancy.AspNetCore
{
    public interface ITenantIdentityParse
    {
        string Name { get; }
        void Resolve(ITenantResolveContext context);
    }
}


namespace Dncy.MultiTenancy.AspNetCore
{
    public interface ITenantConstruct
    {
        string Name { get; }
        void Resolve(ITenantResolveContext context);
    }
}


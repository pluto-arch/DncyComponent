namespace Dncy.MultiTenancy.AspNetCore;

public interface ITenantConstruct
{
    void Resolve(ITenantResolveContext context);
}
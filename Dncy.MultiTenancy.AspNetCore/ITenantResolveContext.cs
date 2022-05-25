namespace Dncy.MultiTenancy.AspNetCore;

public interface ITenantResolveContext
{
    string TenantIdOrName { get; set; }

    bool Handled { get; set; }

    IServiceProvider ServiceProvider { get; }
}
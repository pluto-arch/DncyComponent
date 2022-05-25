using Dncy.MultiTenancy.Model;

namespace Dncy.MultiTenancy.AspNetCore;

public interface ITenantResolver
{
    string ResolveTenantIdOrName();
}
namespace Dncy.Permission.UnitTest;

public class SystemPermission:IPermissionGrant
{
    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public string ProviderName { get; set; }

    /// <inheritdoc />
    public string ProviderKey { get; set; }
}
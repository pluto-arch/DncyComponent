using Dncy.Permission.Models;

namespace Dncy.Permission.UnitTest.Definitions;

public class ProductPermission
{
    public const string GroupName = "ProductManager";


    public static class Product
    {
        public const string Default = GroupName + ".Products";

        public const string List = Default + ".List";

        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }


    public static class Device
    {
        public const string Default = GroupName + ".Devices";

        public const string List = Default + ".List";

        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
}


public class ProductPermissionDefinitionProvider : IPermissionDefinitionProvider
{
    /// <inheritdoc />
    public void Define(PermissionDefinitionContext context)
    {
        var productGroup = context.AddGroup("PM", "产品管理");
        var userPermissionManager = productGroup.AddPermission("PM.PP", "产品权限");
        userPermissionManager.AddChild("PM.PP.List", "创建产品");
        userPermissionManager.AddChild("PM.PP.Create", "创建产品");
        userPermissionManager.AddChild("PM.PP.Edit", "编辑产品");
    }
}
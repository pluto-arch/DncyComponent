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

        /*
         * can read from database
         */
        var productGroup = context.AddGroup(ProductPermission.GroupName, "产品管理");
        PermissionDefinition userPermissionManager = productGroup.AddPermission(ProductPermission.Product.Default, "产品");
        userPermissionManager.AddChild(ProductPermission.Product.List, "产品列表");
        userPermissionManager.AddChild(ProductPermission.Product.Create, "创建产品");
        userPermissionManager.AddChild(ProductPermission.Product.Edit, "编辑产品");
        userPermissionManager.AddChild(ProductPermission.Product.Delete, "删除产品");


        var rolePermissionManager = productGroup.AddPermission(ProductPermission.Device.Default, "设备");
        rolePermissionManager.AddChild(ProductPermission.Device.List, "设备列表");
        rolePermissionManager.AddChild(ProductPermission.Device.Create, "创建设备");
        rolePermissionManager.AddChild(ProductPermission.Device.Edit, "编辑设备");
        rolePermissionManager.AddChild(ProductPermission.Device.Delete, "删除设备");
    }
}
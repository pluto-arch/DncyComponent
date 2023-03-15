using Dncy.Permission.Models;

namespace Dncy.Permission.UnitTest.Definitions;

public class ProductPermission
{
    public const string GroupName = "ProductManager";


    public static class Product
    {
        public const string Default = GroupName + ".Products";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }


    public static class Device
    {
        public const string Default = GroupName + ".Devices";
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

        // 产品管理
        //   -- 产品列表
        //     -- 产品详情
        //     -- 新增产品
        //     -- 删除产品
        //     -- 编辑产品

        /*
         * can read from database
         */
        var productGroup = context.AddGroup(ProductPermission.GroupName, "ProductPermission.ProductManager");
        PermissionDefinition userPermissionManager = productGroup.AddPermission(ProductPermission.Product.Default, "ProductPermission.ProductManager.Products");
        userPermissionManager.AddChild(ProductPermission.Product.Create, "ProductPermission.ProductManager.Products.Create");
        userPermissionManager.AddChild(ProductPermission.Product.Edit, "ProductPermission.ProductManager.Products.Edit");
        userPermissionManager.AddChild(ProductPermission.Product.Delete, "ProductPermission.ProductManager.Products.Delete");


        var rolePermissionManager = productGroup.AddPermission(ProductPermission.Device.Default, "ProductPermission.ProductManager.Device");
        rolePermissionManager.AddChild(ProductPermission.Device.Create, "ProductPermission.ProductManager.Device.Create");
        rolePermissionManager.AddChild(ProductPermission.Device.Edit, "ProductPermission.ProductManager.Device.Edit");
        rolePermissionManager.AddChild(ProductPermission.Device.Delete, "ProductPermission.ProductManager.Device.Delete");
    }
}
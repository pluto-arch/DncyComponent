using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Dncy.Permission.Models;
using Dncy.Permission.UnitTest.Definitions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Dncy.Permission.UnitTest
{
    public class Tests
    {
        private IServiceProvider _serviceProvider;
        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddScoped<IPermissionChecker, DefaultPermissionChecker>();
            services.AddSingleton<IPermissionDefinitionManager, DefaultPermissionDefinitionManager>();

            services.AddTransient<IPermissionManager, InMemoryPermissionManager>();
            services.AddTransient<IPermissionGrantStore, InMemoryPermissionGrantStore>();
            services.AddSingleton<IPermissionDefinitionProvider, ProductPermissionDefinitionProvider>();

            services.AddTransient<IPermissionValueProvider, RolePermissionValueProvider>();
            services.AddTransient<IPermissionValueProvider, UserPermissionValueProvider>();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Test]
        public void PermissionDefinitionManagerTest()
        {
            var permissDefinMgr = _serviceProvider.GetService<IPermissionDefinitionManager>();
            Assert.IsNotNull(permissDefinMgr);
            var permissionGroups = permissDefinMgr.GetGroups();
            Assert.IsTrue(permissionGroups.Count==1);
        }

        [Test]
        public async Task PermissionTest()
        {
            var permissionGrant = _serviceProvider.GetService<IPermissionGrantStore>();
            Assert.IsNotNull(permissionGrant);

            // grant ProductPermission.Product.Create to role with admin
            await permissionGrant.GrantAsync(ProductPermission.Product.Create, "Role", "admin");
           
            // grant ProductPermission.Product.Edit to user with 123 identity
            await permissionGrant.GrantAsync(ProductPermission.Product.Edit, "User", "123");


            var permissionChecker = _serviceProvider.GetService<IPermissionChecker>();
            var claimsA = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
                new Claim(ClaimTypes.Role, "admin"),
            };
            var claimsIdentityA = new ClaimsIdentity(claimsA, "demo_scheme");
            var principalA = new ClaimsPrincipal(claimsIdentityA);
            var isGrantA = await permissionChecker.IsGrantedAsync(principalA, new string[] { ProductPermission.Product.Create , ProductPermission.Product.Edit });
            Assert.IsTrue(isGrantA.AllGranted);


            var claimsB = new[]
            {
                new Claim("user_name", "B"),
                new Claim(ClaimTypes.NameIdentifier, "123"),
            };
            var claimsIdentity = new ClaimsIdentity(claimsB, "demo_scheme");
            var principalB = new ClaimsPrincipal(claimsIdentity);
            var isGrantB = await permissionChecker.IsGrantedAsync(principalB, ProductPermission.Product.Edit);
            Assert.IsTrue(isGrantB);


            var claims2 = new[]
            {
                new Claim("user_name", "C"),
            };
            var claimsIdentity2 = new ClaimsIdentity(claims2, "demo_scheme");
            var principal2 = new ClaimsPrincipal(claimsIdentity2);
            var isGrant2 = await permissionChecker.IsGrantedAsync(principal2, ProductPermission.Product.Create);
            Assert.IsFalse(isGrant2);
        }

    }
}
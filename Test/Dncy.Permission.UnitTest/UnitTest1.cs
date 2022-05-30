using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
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
            await permissionGrant.GrantAsync(new SystemPermission
            {
                Name = ProductPermission.Product.Create,
                ProviderName = "Role",
                ProviderKey = "admin"
            });

            var permissionChecker = _serviceProvider.GetService<IPermissionChecker>();
            var claims = new[]
            {
                new Claim("user_name", "hello"),
                new Claim("role", "admin"),
            };
            var claimsIdentity = new ClaimsIdentity(claims, "demo_scheme");
            var principal = new ClaimsPrincipal(claimsIdentity);
            var isGrant = await permissionChecker.IsGrantedAsync(principal, ProductPermission.Product.Create);
            Assert.IsTrue(isGrant);


            var claims2 = new[]
            {
                new Claim("user_name", "hello"),
            };
            var claimsIdentity2 = new ClaimsIdentity(claims, "demo_scheme");
            var principal2 = new ClaimsPrincipal(claimsIdentity);
            var isGrant2 = await permissionChecker.IsGrantedAsync(principal, ProductPermission.Product.Create);
            Assert.IsFalse(isGrant2);
        }

    }
}
using Microsoft.AspNetCore.Authentication;

namespace PermissionTest.MyAuthentication
{
    public class MyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "MyAuthenticationScheme";
        public string TokenHeaderName { get; set; } = "key";

        public const string UserId="123";
    }
}

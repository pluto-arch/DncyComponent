using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace PermissionTest.MyAuthentication
{
    public class MyAuthenticationHandler : AuthenticationHandler<MyAuthenticationOptions>
    {
        public MyAuthenticationHandler(IOptionsMonitor<MyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        { }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(Options.TokenHeaderName))
            {
                return AuthenticateResult.NoResult();
            }

            string token = Request.Headers[Options.TokenHeaderName]!;
            if(token!="aaa"&&token!="bbb")
            {
                return AuthenticateResult.Fail($"Invalid token.");
            }
            //Success! Add details here that identifies the user

            string userId=MyAuthenticationOptions.UserId;
            if(token=="bbb")
            {
                userId="222";
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var claimsIdentity = new ClaimsIdentity(claims, this.Scheme.Name);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await Task.Delay(1);
            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal,this.Scheme.Name));
        }
    }
}

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Web;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dotnetydd.QuartzHost.Auth;

public class ValidateTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidateTokenMiddleware> _logger;

    public ValidateTokenMiddleware(RequestDelegate next, ILogger<ValidateTokenMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }


    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Equals(new PathString("/login")) && context.Request.Query.TryGetValue("t", out var value))
        {
            if (await TryAuthenticateAsync(value.ToString(), context).ConfigureAwait(false))
            {
                // Success. Redirect to the app.
                if (context.Request.Query.TryGetValue("returnUrl", out var returnUrl))
                {
                    context.Response.Redirect(returnUrl.ToString());
                }
                else
                {
                    context.Response.Redirect("/");
                }
            }
            else
            {
                context.Response.Redirect($"/AccessDenied");
            }
        }

        await _next(context).ConfigureAwait(false);
    }

    public static async Task<bool> TryAuthenticateAsync(string incomingBrowserToken, HttpContext httpContext)
    {
        if (string.IsNullOrEmpty(incomingBrowserToken))
        {
            return false;
        }

        var claimsIdentity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "Local")],
            authenticationType: CookieAuthenticationDefaults.AuthenticationScheme);
        var claims = new ClaimsPrincipal(claimsIdentity);

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims).ConfigureAwait(false);
        return true;
    }

}
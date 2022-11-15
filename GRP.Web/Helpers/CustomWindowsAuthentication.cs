using Microsoft.Owin;
using Owin;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace GRP.Web.Helpers
{
    public class CustomWindowsAuthentication: OwinMiddleware
    {
        public CustomWindowsAuthentication(OwinMiddleware next) : base(next) { }

        public async override Task Invoke(IOwinContext context)
        {
            var windowsPrincipal = context.Request.User as WindowsPrincipal;
            if (windowsPrincipal != null && windowsPrincipal.Identity.IsAuthenticated)
            {
                //if (context.Response.StatusCode == 401)
                //{
                var nameClaim = windowsPrincipal.FindFirst(ClaimTypes.Name);

                // This is the domain name
                string name = nameClaim.Value;
                var parts = name.Split(new[] { '\\' }, 2);
                string adUserName = parts.Length == 1 ? parts[0] : parts[parts.Length - 1];

                // Normalize the claims here
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, name));
                claims.Add(new Claim("uac", "test"));
                claims.Add(new Claim(ClaimTypes.Name, adUserName));
                claims.Add(new Claim(ClaimTypes.AuthenticationMethod, "Windows"));

                var identity = new ClaimsIdentity(claims, "customWinAuth");

                context.Authentication.SignIn(identity);

                await this.Next.Invoke(context);
                //context.Response.Redirect(context.Request.PathBase.Value);

                //context.Response.Redirect((context.Request.PathBase + context.Request.Path).Value);
                //}

                return;
            }
            //else
            //    context.Response.Redirect(context.Request.PathBase.Value + "/User/Login");
            await this.Next.Invoke(context);
        }
    }

    internal static class CustomWindowsAuthenticationHandler
    {
        public static IAppBuilder UseCustomWindowsAuthentication(this IAppBuilder app)
        {
            app.Use<CustomWindowsAuthentication>();
            return app;
        }
    }
}
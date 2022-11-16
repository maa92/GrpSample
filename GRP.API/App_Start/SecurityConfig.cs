using GrpSample.API.AuthProviders;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;

namespace GrpSample.API
{
    public class SecurityConfig
    {
        public static void Configure(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/api/gt"),
                //Provider = new LDAPGRPOAuthProvider(),
                AllowInsecureHttp = true,
                //AuthorizationCodeExpireTimeSpan = TimeSpan.FromMinutes(1),
                //AccessTokenExpireTimeSpan = TimeSpan.FromHours(12), //TimeSpan.FromDays(1),
                SystemClock = new SystemClock()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
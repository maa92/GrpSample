using System;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
//using Microsoft.Owin.Security.OAuth;
//using Microsoft.Owin.Security;
//using Microsoft.Owin.Extensions;

namespace GrpSample.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                //AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieHttpOnly = true,
                CookieName = "grpApp",
                LoginPath = new PathString("/User/Login"),
                LogoutPath = new PathString("/User/Logout"),
                ExpireTimeSpan = TimeSpan.FromHours(11),
                ReturnUrlParameter = "ru"//,
                //Provider = new CookieAuthenticationProvider()
                //{
                //    OnApplyRedirect = ctx =>
                //    {
                //        if (ctx.OwinContext.Response.StatusCode == 401)
                //            ctx.Response.Redirect("User/Login");    //grp is fixed
                //        //if (!IsAjaxRequest(ctx.Request))
                //        //{
                //        //    //ctx.Response.Redirect(ctx.RedirectUri);
                //        //}
                //    }
                //}
            });
            //app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions()
            //{
            //    AuthenticationType = DefaultAuthenticationTypes.ExternalBearer,
            //    AuthenticationMode = AuthenticationMode.Passive,
            //});
            //app.UseStageMarker(PipelineStage.MapHandler);
        }

        private static bool IsAjaxRequest(IOwinRequest request)
        {
            IReadableStringCollection query = request.Query;
            if ((query != null) && (query["X-Requested-With"] == "XMLHttpRequest"))
            {
                return true;
            }
            IHeaderDictionary headers = request.Headers;
            return ((headers != null) && (headers["X-Requested-With"] == "XMLHttpRequest"));
        }
    }
}
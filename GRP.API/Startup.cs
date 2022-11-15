using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GRP.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            SecurityConfig.Configure(app);
            WebApiConfig.Configure(app);

            //////Configure AutoMapper (http://automapper.codeplex.com/)
            ////Mapper.Initialize(ConfigureMapper);

            ////Configure Bearer Authentication
            //ConfigureAuth(app);

            //////Log trafic using Log4Net
            ////app.Use(typeof(Logging));

            //////Configure SignalR self host
            ////ConfigureSignalR(app);

            //var config = new HttpConfiguration();

            //////Configure AutoFac (http://autofac.org/) for DependencyResolver
            //////For more information visit http://www.asp.net/web-api/overview/extensibility/using-the-web-api-dependency-resolver
            ////ConfigureComposition(config);

            ////Configure WebApi
            //ConfigureWebApi(config);
            //app.UseWebApi(config);
        }
    }
}
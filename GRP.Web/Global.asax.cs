using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GRP.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            MvcHandler.DisableMvcResponseHeader = true;
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier; //ClaimTypes.Email;
        }

        protected void Application_Error()
        {
            // This error handling is implemented this way temporarily to handle the case when the token is expired and the GRP.Web cookies is still alive.
            // it will be further improved in the near future.
            HttpContext httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                RequestContext requestContext = ((MvcHandler)httpContext.CurrentHandler).RequestContext;
                /* when the request is ajax the system can automatically handle a mistake with a JSON response. then overwrites the default response */
                if (requestContext.HttpContext.Request.IsAjaxRequest())
                {
                    httpContext.Response.Clear();
                    httpContext.Response.End();
                    //string controllerName = requestContext.RouteData.GetRequiredString("controller");
                    //IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
                    //IController controller = factory.CreateController(requestContext, controllerName);
                    //ControllerContext controllerContext = new ControllerContext(requestContext, (ControllerBase)controller);

                    //JsonResult jsonResult = new JsonResult();
                    //jsonResult.Data = new { success = false, serverError = "500" };
                    //jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    //jsonResult.ExecuteResult(controllerContext);
                    //httpContext.Response.End();
                }
                //else
                //{
                //    httpContext.Response.Redirect("~/Error");
                //}
            }
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Set("Server", "GRP");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }
    }
}

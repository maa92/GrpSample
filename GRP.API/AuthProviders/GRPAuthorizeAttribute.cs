using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace GRP.API.AuthProviders
{
    public class GRPAuthorizeAttribute : AuthorizeAttribute
    {
        //public string GRPRoles { get; set; }
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //retrieve controller action's authorization attributes
                //var authorizeAttributes = actionContext.ActionDescriptor.GetCustomAttributes<GRPAuthorizeAttribute>();
                string clientId = (HttpContext.Current.User.Identity as ClaimsIdentity).FindFirst("cId").Value;

                //check controller and action BypassValidation value
                if (!string.IsNullOrWhiteSpace(Roles) && GRPClientSecurityManager.Create().IsClientInRole(clientId, Roles))
                {
                    return true;
                }
                //else
                //{
                //    return false;
                //}
                return base.IsAuthorized(actionContext);
            }
            return base.IsAuthorized(actionContext);
        }
    }
}
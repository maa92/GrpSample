using System;
using System.Reflection;
using System.Web.Mvc;

namespace GRP.Web.Helpers
{
    public class RequiresParameterAttribute : ActionMethodSelectorAttribute
    {

        readonly string parameterName;

        public RequiresParameterAttribute(string parameterName)
        {
            this.parameterName = parameterName;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return controllerContext.RouteData.Values[parameterName] != null;
        }
    }
}

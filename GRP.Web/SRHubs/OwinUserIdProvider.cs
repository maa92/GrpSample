using Microsoft.AspNet.SignalR;
using System.Linq;
using System.Web;

namespace GRP.Web.SRHubs
{
    public class OwinUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            return request.GetHttpContext().GetOwinContext().Authentication.User.Claims.First(c => c.Type == "uid").Value;
            //return HttpContext.Current.GetOwinContext().Authentication.User.Claims.First(c => c.Type == "uid").Value;
        }
    }
}
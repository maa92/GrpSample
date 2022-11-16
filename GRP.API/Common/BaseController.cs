using GrpSample.API.AuthProviders;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace GrpSample.API.Common
{
    [Authorize]
    public class BaseController : ApiController
    {
        private GRPClientSecurityManager _AppRoleManager = null;

        protected GRPClientSecurityManager AppRoleManager
        {
            get
            {
                return _AppRoleManager ?? GRPClientSecurityManager.Create();//Request.GetOwinContext());
            }
        }

        public string UserId { get { return (User.Identity as ClaimsIdentity).FindFirst("uId").Value; } }

        public string UserADName { get { return (User.Identity as ClaimsIdentity).FindFirst("uName").Value; } }

        public string UserLoginSerialId { get { return (User.Identity as ClaimsIdentity).FindFirst("uLogin").Value; } }

        /// <summary>
        /// User connection environment ( P => Production, T => Test )
        /// </summary>
        public string Environment { get { return (User.Identity as ClaimsIdentity).FindFirst("env").Value; } }
    }
}

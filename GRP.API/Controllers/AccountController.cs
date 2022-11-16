using System.Web.Http;
//using log4net;
using GrpSample.DataAccess.Handlers.Account;
using GrpSample.API.Common;
using GrpSample.API.AuthProviders;

namespace GrpSample.API.Controllers
{
    [GRPAuthorize(Roles = "Admin")]
    public class AccountController : BaseController
    {
        public IHttpActionResult Logout()
        {

            UserDH udh = new UserDH(Environment);
            string retVal = udh.LogoutUser(UserId, UserLoginSerialId);
            if (retVal == "1") // logout is logged in the system.
                return Ok(retVal);
            else
                return BadRequest("Error while logging out from the system.");
        }
    }

    #region Commented
    //[RoutePrefix("api/Account")]
    //public class AccountController : ApiController
    //{
    //    private readonly ILoginProvider _loginProvider = new LocalUserLoginProvider();
    //    //private static readonly ILog Log = LogManager.GetLogger(typeof(AccountController));

    //    //[HttpPost, Route("Token")]
    //    [HttpPost]
    //    public IHttpActionResult Token(LoginViewModel login)
    //    {
    //        //Log.DebugFormat("Entering Token(): User={0}", login.UserName);

    //        if (!ModelState.IsValid)
    //        {
    //            //Log.Debug("Leaving Token(): Bad request");
    //            return this.BadRequestError(ModelState);
    //        }

    //        ClaimsIdentity identity;

    //        if (!_loginProvider.ValidateCredentials(login.UserName, login.Password, out identity))
    //        {
    //            //Log.Debug("Leaving Token(): Incorrect user or password");
    //            return BadRequest("Incorrect user or password");
    //        }

    //        var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
    //        var currentUtc = new SystemClock().UtcNow;
    //        ticket.Properties.IssuedUtc = currentUtc;
    //        ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromMinutes(30));

    //        //Log.Debug("Leaving Token()");

    //        return Ok(new LoginAccessViewModel
    //        {
    //            UserName = login.UserName,
    //            AccessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket)
    //        });
    //    }

    //    //[Authorize]
    //    //[HttpGet, Route("Profile")]
    //    //public IHttpActionResult Profile()
    //    //{
    //    //    Log.DebugFormat("Entering Profile(): User={0}", User.Identity.Name);
    //    //    return Ok(new AccountProfileViewModel
    //    //    {
    //    //        UserName = User.Identity.Name
    //    //    });
    //    //}

    //    /// <summary>
    //    /// Use this action to test authentication
    //    /// </summary>
    //    /// <returns>status code 200 if the user is authenticated, otherwise status code 401</returns>
    //    [Authorize]
    //    [HttpGet, Route("Ping")]
    //    public IHttpActionResult Ping()
    //    {
    //        //Log.DebugFormat("Entering Ping(): User={0}", User.Identity.Name);
    //        return Ok();
    //    }

    //    //public AccountController(ILoginProvider loginProvider)
    //    //{
    //    //    Log.Debug("Entering AccountController()");
    //    //    _loginProvider = loginProvider;
    //    //}
    //}
    #endregion
}
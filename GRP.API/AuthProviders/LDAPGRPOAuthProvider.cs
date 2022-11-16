using GrpSample.DataAccess;
using GrpSample.API.Identity;
using GrpSample.DataAccess.Handlers;
using GrpSample.DataAccess.Handlers.Account;
using GrpSample.Models.Account;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GrpSample.API.AuthProviders
{
    public class LDAPGRPOAuthProvider : OAuthAuthorizationServerProvider
    {
        GRPClientSecurityManager _clientSecurityManager = null;
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (_clientSecurityManager == null)
                _clientSecurityManager = GRPClientSecurityManager.Create();

            string clientId;
            string clientSecret;
            if (context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                if (_clientSecurityManager.IsClientAllowed(Utils.DecodeString(clientId), clientSecret))//Utils.DecodeString(clientId) == "grp" && Utils.DecodeString(clientSecret) == "srca_grp")
                    context.Validated();
                else
                    context.SetError("invalid_client", "Client credentials are not valid.");
                //context.Rejected();
            }
            else
            {
                context.SetError("invalid_client", "Client credentials could not be retrieved through the Authorization header.");
                //context.Rejected();
            }
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                if (_clientSecurityManager == null)
                    _clientSecurityManager = GRPClientSecurityManager.Create();

                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

                bool valid;

                DomainUserLoginProvider _repo = new DomainUserLoginProvider(ConfigurationManager.AppSettings["ADDomainURL"]);
                valid = _repo.ValidateCredentials(context.UserName, context.Password);

                if (!valid)
                {
                    CommonDH udh = new CommonDH(context.Request.Headers["env"]);
                    string errorMsg = udh.GetMessageText(1);
                    context.SetError("invalid_grant", errorMsg);//"The user name or password is incorrect.");
                    //context.Rejected();
                    //context.Response.Headers.Add(Constants.OwinChallengeFlag, new[] { ((int)HttpStatusCode.Unauthorized).ToString() }); //Little trick to get this to throw 401, refer to AuthenticationMiddleware for more
                    return;
                }

                User au = AuthenticateUserInDB(context.UserName, context.Request.Headers["aui"], context.Request.Headers["env"], context.Request.Headers["dip"], context.Request.Headers["dname"]);
                if (au.userLoginFlag == "T")
                {
                    List<Claim> userClaims = new List<Claim>();

                    var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);

                    oAuthIdentity.AddClaim(new Claim("env", context.Request.Headers["env"]));
                    oAuthIdentity.AddClaim(new Claim("cId", Utils.DecodeString(context.ClientId)));
                    oAuthIdentity.AddClaim(new Claim("uName", context.UserName));
                    oAuthIdentity.AddClaim(new Claim("uId", au.userId));
                    oAuthIdentity.AddClaim(new Claim("uTp", au.userType));
                    oAuthIdentity.AddClaim(new Claim("uLogin", au.loginSerial));
                    au.userIsSuperAdmin = ConfigurationManager.AppSettings["grpAdmnAccounts"] == null ? "-1" : ConfigurationManager.AppSettings["grpAdmnAccounts"].IndexOf(context.UserName.ToLower()).ToString();
                    oAuthIdentity.AddClaim(new Claim("uIsSAdmn", au.userIsSuperAdmin));

                    AuthenticationProperties additionalData = CreateProperties(au, Utils.DecodeString(context.ClientId));
                    AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, additionalData);
                    context.Validated(ticket);
                }
                else
                {
                    context.SetError("unauthorized_client", au.loginMessage);
                    //context.Rejected();
                    //context.Response.Headers.Add(Constants.OwinChallengeFlag, new[] { ((int)HttpStatusCode.Unauthorized).ToString() }); //Little trick to get this to throw 401, refer to AuthenticationMiddleware for more
                }
            }
            catch (Exception ex)
            {
                context.SetError("authentication_error_exception", ex.Message);
                //context.Rejected();
            }
        }

        private User AuthenticateUserInDB(string AuthUserName, string AdminUserId, string Environment, string DeviceIP, string DeviceName)
        {
            User retVal = new User();

            UserDH udh = new UserDH(Environment);
            retVal = udh.AuthenticateUser(AuthUserName, AdminUserId, DeviceIP, DeviceName);

            return retVal;
        }

        private static AuthenticationProperties CreateProperties(User UserInfo, string ClientId)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userId", UserInfo.userId},
                { "userLoginSerial",UserInfo.loginSerial },
                { "userFName", UserInfo.userFullName },
                { "userDeptName", UserInfo.userDeptName },
                { "userType", UserInfo.userType },
                { "userCanChangeLogin", UserInfo.userIsSuperAdmin },
                { "clientId", ClientId }
            };
            return new AuthenticationProperties(data);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            context.Properties.ExpiresUtc = GetExpireTime(_clientSecurityManager.GetClientTokenExpirationTime(context.Properties.Dictionary["clientId"]));
            context.Properties.Dictionary.Remove("clientId"); // remove the client id to NOT return to the caller.

            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        private DateTimeOffset GetExpireTime(string ExpireTime)
        {
            DateTime retVal = DateTime.Now;

            if (ExpireTime.StartsWith("d_"))
                retVal = retVal.AddDays(Convert.ToInt32(ExpireTime.Split('_')[1]));
            else if (ExpireTime.StartsWith("mo_"))
                retVal = retVal.AddMonths(Convert.ToInt32(ExpireTime.Split('_')[1]));
            else if (ExpireTime.StartsWith("h_"))
                retVal = retVal.AddHours(Convert.ToInt32(ExpireTime.Split('_')[1]));
            else if (ExpireTime.StartsWith("m_"))
                retVal = retVal.AddMinutes(Convert.ToInt32(ExpireTime.Split('_')[1]));
            else
                retVal = retVal.AddHours(12);

            return retVal;
        }

        //public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        //{
        //    if (context.ClientId == _publicClientId)
        //    {
        //        Uri expectedRootUri = new Uri(context.Request.Uri, "/");

        //        if (expectedRootUri.AbsoluteUri == context.RedirectUri)
        //        {
        //            context.Validated();
        //        }
        //    }

        //    return Task.FromResult<object>(null);
        //}
    }
}
using GRP.Models.Security;
using GRP.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
//using System.Web.Http;
using System.Web.Mvc;
using System.Net;
using System.Configuration;
using System.Linq;
using CaptchaMvc.HtmlHelpers;
//using GRP.Web.Areas.HR.Helpers;

namespace GRP.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string ru)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "App");

            System.Web.Helpers.AntiForgeryConfig.CookieName = "grpafc";
            ViewBag.Title = "تسجيل الدخول";
            ViewBag.ReturnUrl = ru;

            // If login cookie exists pull username
            if (Request.Cookies["grplc"] != null)
            {
                HttpCookie userNameCookie = Request.Cookies["grplc"];
                if (!string.IsNullOrEmpty(userNameCookie.Values["uname"]))
                {
                    return View(new LoginViewModel
                    {
                        UserName = userNameCookie.Values["uname"],
                        SaveUName = true
                    });
                }
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string ru)
        {
            try
            {
                ViewBag.Title = "تسجيل الدخول";
                ViewBag.ReturnUrl = ru;

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "App");
                }
                // Check the anti forgery token now
                if (Session["TempRedirectUrl"] == null)
                    System.Web.Helpers.AntiForgery.Validate();

                if (!this.IsCaptchaValid("رمز خاطئ"))
                {
                    return View(model);
                }

                Dictionary<string, string> authParams = new Dictionary<string, string>(3)
                    { {"grant_type", "password"}, {"username", model.UserName}, {"password", model.Password} };

                string userHosttName = string.Empty;
                try
                {
                    userHosttName = Dns.GetHostEntry(Request.UserHostAddress).HostName.ToLower().Replace(".srca.org.sa", string.Empty).ToUpper();
                }
                catch (Exception ex)
                {
                    userHosttName = string.Empty;
                }
                Dictionary<string, string> customHeaders = new Dictionary<string, string>(3)
                    { {"env", model.SystemType}, {"dip", Request.UserHostAddress}, {"dname", userHosttName } };

                var authContent = new FormUrlEncodedContent(authParams);
                RestClient<SecurityToken> client = new RestClient<SecurityToken>();
                string headerValue = string.Format("{0}:{1}", GRPEncoding.EncodeString(ConfigurationManager.AppSettings["authHeaderUN"]), ConfigurationManager.AppSettings["authHeaderPwd"]);
                SecurityToken authResult = await client.AuthenticatePost("gt", authContent, "Basic", headerValue, customHeaders);

                if (!string.IsNullOrEmpty(authResult.error))
                {
                    if (authResult.error == "authentication_error_exception")
                    {
                        string stackTraceMsg = @"<div style=""text-align:center""><a href=""javascript:void(0)"" class=""easyui-linkbutton"" iconCls=""icon-tip"" data-err=""" +
                        authResult.error_description + @""" onclick =""showErrDetails(this)"">عرض تفاصيل الخطأ</a></div><div id=""wndLgErrDetails""></div>";
                        model.LoginErrMsg = string.Format("خطأ أثناء تسجيل الدخول|{0}", authResult.error + " <br/> " + stackTraceMsg);
                    }
                    else
                    {
                        model.LoginErrMsg = authResult.error_description;
                    }
                    return View(model);
                }

                //if (json["refresh_token"] != null)
                //    _refreshToken = json["refresh_token"].ToString();

                //SignIn with Token, SignOut and create new identity for SignIn
                //Request.Headers.Add("Authorization", authResult.token_type + " " + authResult.access_token);

                // This code is running on my local IIS 10 Express :( !!!!, BUT when uploaded to Windows Server 2012 R2 with IIS 8.5 I got ugly NullRefrenceException with
                // Exception :  GRP.Web.Controllers.UserController.<Login>d__1.MoveNext() in GRPGRP.WebControllersUserController.cs:line 69
                //var authenticateResult = await ctx.Authentication.AuthenticateAsync(DefaultAuthenticationTypes.ExternalBearer);
                //ctx.Authentication.SignOut(DefaultAuthenticationTypes.ExternalBearer);
                //List<Claim> claims = new List<Claim>(authenticateResult.Identity.Claims);
                // So, I put all variables at the TokenEndPoint in the authorization process. 

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("uat", authResult.access_token));
                claims.Add(new Claim("env", model.SystemType));
                claims.Add(new Claim("uid", authResult.userId));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, authResult.userId)); //For integration with AntiForgeryToken
                claims.Add(new Claim("uls", authResult.userLoginSerial));
                claims.Add(new Claim("ufn", authResult.userFName));
                claims.Add(new Claim("udn", authResult.userDeptName));
                claims.Add(new Claim("utp", authResult.userType));
                claims.Add(new Claim("uccl", authResult.userCanChangeLogin));

                var ctx = Request.GetOwinContext();
                var applicationCookieIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                ctx.Authentication.SignIn(applicationCookieIdentity);

                if (Session["TempRedirectUrl"] == null)
                    SetSaveUserNameCookie(model);

                //redirect back to the view which required authentication
                string decodedUrl = "";
                if (!string.IsNullOrEmpty(ru))
                    decodedUrl = Server.UrlDecode(ru);

                string redirectUrl = string.Empty;
                if (Session["TempRedirectUrl"] != null)
                {
                    redirectUrl = "#" + Session["TempRedirectUrl"].ToString();
                    Session.Remove("TempRedirectUrl");
                }

                if (Url.IsLocalUrl(decodedUrl))
                    return Redirect(decodedUrl);
                else
                    return Redirect(Url.Action("Index", "App") + redirectUrl); //return RedirectToAction("Index", "App");
            }
            catch (Exception ex)
            {
                string stackTraceMsg = @"<div style=""text-align:center""><a href=""javascript:void(0)"" class=""easyui-linkbutton"" iconCls=""icon-tip"" data-err=""" +
                    ex.StackTrace + @""" onclick =""showErrDetails(this)"">عرض تفاصيل الخطأ</a></div><div id=""wndLgErrDetails""></div>";
                model.LoginErrMsg = string.Format("خطأ أثناء تسجيل الدخول|{0}", ex.Message + " <br/> " + stackTraceMsg);
                if (ex is HttpAntiForgeryException)
                    model.LoginErrMsg = "اسم المستخدم او كلمة المرور غير صحيحه ...";
                return View(model);
            }
        }

        private void SetSaveUserNameCookie(LoginViewModel userModel)
        {
            // If Remember me then set an appropriate cookie
            if (userModel.SaveUName)
            {
                HttpCookie userNameCookie = new HttpCookie("grplc");
                Response.Cookies.Remove("grplc");
                Response.Cookies.Add(userNameCookie);
                userNameCookie.Values.Add("uname", userModel.UserName);
                Response.Cookies["grplc"].Expires = DateTime.Now.AddYears(50);
            }

            //// Set a cookie to expire after 1 second
            else
            {
                HttpCookie userNameCookie = new HttpCookie("grplc");
                Response.Cookies.Remove("grplc");
                Response.Cookies["grplc"].Expires = DateTime.Now.AddSeconds(1);
                Response.Cookies.Add(userNameCookie);
            }
        }

        [AllowAnonymous, ActionName("OSFS")]
        public async Task<ActionResult> OpenSysFeature(string sfu)
        {
            if (!string.IsNullOrEmpty(sfu))
            {
                try
                {
                    string decrEncStr = GRPEncryption.DecryptText(sfu, "srca_grp_key_2017");
                    string decoStr = GRPEncoding.DecodeString(decrEncStr);
                    string[] paramVals;
                    if (InputParamIsValid(decoStr, out paramVals))
                    {
                        var paramsObj = new
                        {
                            sysId = paramVals[1],
                            sysSecret = paramVals[2],
                            userName = paramVals[6],
                            userPassword = paramVals[7],
                            grpFeaturePath = paramVals[8]
                        };

                        if (User.Identity.IsAuthenticated) //&& (User.Identity as ClaimsIdentity).Claims.First(c => c.Type == "uid").Value != paramsObj.userName)
                        {
                            RestClient<string> soClient = new RestClient<string>(Request.GetOwinContext());
                            string signOut = await soClient.PostRequest("Account/Logout", string.Empty);
                            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        }

                        Dictionary<string, string> authParams = new Dictionary<string, string>(3)
                                    { {"grant_type", "password"}, {"username", paramsObj.userName}, {"password",paramsObj.userPassword} };

                        string userHosttName = string.Empty;
                        try
                        {
                            userHosttName = Dns.GetHostEntry(Request.UserHostAddress).HostName.ToLower().Replace(".srca.org.sa", string.Empty).ToUpper();
                        }
                        catch (Exception ex)
                        {
                            userHosttName = string.Empty;
                        }
                        Dictionary<string, string> customHeaders = new Dictionary<string, string>(3)
                                    { {"env", "P"}, {"dip", Request.UserHostAddress}, {"dname", userHosttName } };

                        var authContent = new FormUrlEncodedContent(authParams);
                        RestClient<SecurityToken> client = new RestClient<SecurityToken>();
                        string headerValue = string.Format("{0}:{1}", GRPEncoding.EncodeString(paramsObj.sysId), paramsObj.sysSecret);
                        SecurityToken authResult = await client.AuthenticatePost("gt", authContent, "Basic", headerValue, customHeaders);

                        if (!string.IsNullOrEmpty(authResult.error))
                        {
                            HttpUnauthorizedResult hur = new HttpUnauthorizedResult(authResult.error);
                            return hur;
                        }

                        List<Claim> claims = new List<Claim>();
                        claims.Add(new Claim("uat", authResult.access_token));
                        claims.Add(new Claim("env", "P"));
                        claims.Add(new Claim("uid", authResult.userId));
                        claims.Add(new Claim("uls", authResult.userLoginSerial));
                        claims.Add(new Claim("ufn", authResult.userFName));
                        claims.Add(new Claim("udn", authResult.userDeptName));
                        claims.Add(new Claim("utp", authResult.userType));

                        var ctx = Request.GetOwinContext();
                        var applicationCookieIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                        ctx.Authentication.SignIn(applicationCookieIdentity);

                        return Redirect(Url.Action("Index", "App") + "#" + GRPEncoding.EncodeString(paramsObj.grpFeaturePath));
                    }
                    else
                    {
                        HttpUnauthorizedResult hur = new HttpUnauthorizedResult("invalid_params_detected");
                        return hur;
                    }
                }
                catch (Exception ex)
                {
                    HttpUnauthorizedResult hur = new HttpUnauthorizedResult("invalid_params_detected");
                    return hur;
                }
            }
            else
            {
                HttpUnauthorizedResult hur = new HttpUnauthorizedResult("invalid_params_detected");
                return hur;
            }
        }

        [AllowAnonymous, ActionName("OELN")]
        public async Task<ActionResult> OpenEmailLink(string lnkPayVal)
        {
            if (!string.IsNullOrEmpty(lnkPayVal))
            {
                try
                {
                    string decoStr = GRPEncoding.DecodeString(lnkPayVal);
                    string[] paramVals;
                    if (EmailLinkParamIsValid(decoStr, out paramVals))
                    {
                        var paramsObj = new
                        {
                            userID = paramVals[1],
                            userAD = paramVals[2],
                            url = paramVals[4]
                        };

                        if (User.Identity.IsAuthenticated)
                        {
                            if (paramsObj.userID == (User.Identity as ClaimsIdentity).Claims.First(c => c.Type == "uid").Value)
                            {
                                return Redirect(Url.Action("Index", "App") + "#" + GRPEncoding.EncodeString(paramsObj.url));
                            }
                            else
                            {
                                RestClient<string> soClient = new RestClient<string>(Request.GetOwinContext());
                                string signOut = await soClient.PostRequest("Account/Logout", string.Empty);
                                Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                                Session["TempRedirectUrl"] = GRPEncoding.EncodeString(paramsObj.url);
                                return View("Login", new LoginViewModel
                                {
                                    UserName = paramsObj.userAD
                                });
                            }
                        }
                        else
                        {
                            Session["TempRedirectUrl"] = GRPEncoding.EncodeString(paramsObj.url);
                            return View("Login", new LoginViewModel
                            {
                                UserName = paramsObj.userAD
                            });
                        }
                    }
                    else
                    {
                        HttpUnauthorizedResult hur = new HttpUnauthorizedResult("invalid_params_detected");
                        return hur;
                    }
                }
                catch (Exception ex)
                {
                    HttpUnauthorizedResult hur = new HttpUnauthorizedResult("invalid_params_detected");
                    return hur;
                }
            }
            else
            {
                HttpUnauthorizedResult hur = new HttpUnauthorizedResult("invalid_params_detected");
                return hur;
            }
        }

        private bool EmailLinkParamIsValid(string StringVal, out string[] StringVals)
        {
            bool retVal = false;
            StringVals = new string[] { };
            if (!string.IsNullOrEmpty(StringVal))
            {
                StringVals = StringVal.Split(new string[] { "||" }, StringSplitOptions.None);
                if (StringVals[0] == "OENL" && DateTime.Now.Subtract(DateTime.ParseExact(StringVals[3], "MM/dd/yyyy hh:mm:ss tt", null)).Days <= 3)
                    retVal = true;
            }
            return retVal;
        }

        [HttpPost]
        public async Task<ActionResult> ChangeLogin(string lun)
        {
            if (!string.IsNullOrEmpty(lun) && User.Identity.IsAuthenticated && (User.Identity as ClaimsIdentity).Claims.First(c => c.Type == "uccl") != null && Convert.ToInt32((User.Identity as ClaimsIdentity).Claims.First(c => c.Type == "uccl").Value) > 0)
            {
                try
                {
                    var ctx = Request.GetOwinContext();

                    Dictionary<string, string> authParams = new Dictionary<string, string>(3)
                                    { {"grant_type", "password"}, {"username", lun}, {"password", "clu_X!$ul@"} };

                    string userHosttName = string.Empty;
                    try
                    {
                        userHosttName = Dns.GetHostEntry(Request.UserHostAddress).HostName.ToLower().Replace(".srca.org.sa", string.Empty).ToUpper();
                    }
                    catch (Exception ex)
                    {
                        userHosttName = string.Empty;
                    }
                    Dictionary<string, string> customHeaders = new Dictionary<string, string>(3)
                    {
                        { "env", ctx.Authentication.User.Claims.First(c => c.Type == "env").Value },
                        { "dip", Request.UserHostAddress}, {"dname", userHosttName },
                        { "aui", ctx.Authentication.User.Claims.First(c => c.Type == "uid").Value }
                    };

                    var authContent = new FormUrlEncodedContent(authParams);
                    RestClient<SecurityToken> client = new RestClient<SecurityToken>();
                    string headerValue = string.Format("{0}:{1}", GRPEncoding.EncodeString(ConfigurationManager.AppSettings["authHeaderUN"]), ConfigurationManager.AppSettings["authHeaderPwd"]);
                    SecurityToken authResult = await client.AuthenticatePost("gt", authContent, "Basic", headerValue, customHeaders);

                    if (!string.IsNullOrEmpty(authResult.error))
                    {
                        if (authResult.error == "authentication_error_exception")
                        {
                            string stackTraceMsg = @"<div style=""text-align:center""><a href=""javascript:void(0)"" class=""easyui-linkbutton"" iconCls=""icon-tip"" data-err=""" +
                            authResult.error_description + @""" onclick =""showErrDetails(this)"">عرض تفاصيل الخطأ</a></div><div id=""wndLgErrDetails""></div>";
                            return Json(new { res = 0, msg = string.Concat(authResult.error + " <br/> " + stackTraceMsg) });
                        }
                        else
                        {
                            return Json(new { res = 0, msg = authResult.error_description });
                        }                        
                    }

                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim("uat", authResult.access_token));
                    claims.Add(new Claim("env", ctx.Authentication.User.Claims.First(c => c.Type == "env").Value));
                    claims.Add(new Claim("uid", authResult.userId));
                    claims.Add(new Claim("uls", authResult.userLoginSerial));
                    claims.Add(new Claim("ufn", authResult.userFName));
                    claims.Add(new Claim("udn", authResult.userDeptName));
                    claims.Add(new Claim("utp", authResult.userType));
                    claims.Add(new Claim("uccl", authResult.userCanChangeLogin));
                    
                    var applicationCookieIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                    ctx.Authentication.SignIn(applicationCookieIdentity);

                    return Json(new { res = 1, msg = "success" });
                }
                catch (Exception ex)
                {
                    string stackTraceMsg = @"<div style=""text-align:center""><a href=""javascript:void(0)"" class=""easyui-linkbutton"" iconCls=""icon-tip"" data-err=""" +
                    ex.StackTrace + @""" onclick =""showErrDetails(this)"">عرض تفاصيل الخطأ</a></div><div id=""wndLgErrDetails""></div>";
                    return Json(new { res = 0, msg = string.Concat(ex.Message + " <br/> " + stackTraceMsg) });
                }
            }
            else
            {
                return new HttpUnauthorizedResult("invalid_params_detected");
            }
        }

        private bool InputParamIsValid(string StringVal,out string[] StringVals)
        {
            bool retVal = false;
            StringVals = new string[] { };
            if (!string.IsNullOrEmpty(StringVal))
            {
                StringVals = StringVal.Split(new string[] { "||" }, StringSplitOptions.None);
                if (StringVals[0] == "GRP" &&
                    StringVals[3] == DateTime.Now.Day.ToString() &&
                    StringVals[4] == DateTime.Now.Month.ToString() &&
                    StringVals[5] == DateTime.Now.Year.ToString() &&
                    !string.IsNullOrWhiteSpace(StringVals[8]))
                    retVal = true;
            }
            return retVal;
        }

        public async Task<ActionResult> Logout()
        {
            RestClient<string> client = new RestClient<string>(Request.GetOwinContext());
            string authResult = await client.PostRequest("Account/Logout", string.Empty);

            var ctx = Request.GetOwinContext();
            ctx.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "User");
        }

        /*protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            var action = filterContext.RequestContext.RouteData.Values["action"] as string;
            var controller = filterContext.RequestContext.RouteData.Values["controller"] as string;

            if ((filterContext.Exception is HttpAntiForgeryException) && action == "Login" && controller == "User")
            {
                //&&
                //filterContext.RequestContext.HttpContext.User != null &&
                //filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
                // Log the exception.
                filterContext.ExceptionHandled = true;
                // redirect/show error/
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.RawUrl); //RedirectToAction("Index", "App");//new RedirectResult("/warning");
            }
        }*/
    }

    #region commented code AntiForgery

    /*public class AntiForgeryHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is HttpAntiForgeryException)
            {
                var url = string.Empty;
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    var requestContext = new RequestContext(context.HttpContext, context.RouteData);
                    url = RouteTable.Routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { Controller = "User", action = "Login" })).VirtualPath;
                }
                else
                {
                    context.HttpContext.Response.StatusCode = 200;
                    context.ExceptionHandled = true;
                    url = GetRedirectUrl(context);
                }
                context.HttpContext.Response.Redirect(url, true);
            }
            else
            {
                base.OnException(context);
            }
        }

        private string GetRedirectUrl(ExceptionContext context)
        {
            try
            {
                var requestContext = new RequestContext(context.HttpContext, context.RouteData);
                var url = RouteTable.Routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { Controller = "User", action = "AlreadySignIn" })).VirtualPath;

                return url;
            }
            catch (Exception)
            {
                throw new NullReferenceException();
            }
        }
    }*/

    #endregion

}
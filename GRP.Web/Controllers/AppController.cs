using GrpSample.Models.System;
using GrpSample.Web.Helpers;
using Microsoft.Owin;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System;

namespace GrpSample.Web.Controllers
{
    [Authorize]
    public class AppController : Controller
    {
        public async Task<ActionResult> Index()
        {
            RestClient<SysSettings> client = new RestClient<SysSettings>(Request.GetOwinContext());
            SysSettings sysSettings = await client.GetSingleItemRequest("Sys/GetSysSettings");

            RestClient<Notification> clientN = new RestClient<Notification>(Request.GetOwinContext());
            Notification[] userNotifications = await clientN.GetMultipleItemsRequest("Sys/GetUserNotifications");

            Session["sysSettings"] = sysSettings;
            IOwinContext owinContext = Request.GetOwinContext();

            HomePage hpm = new HomePage();
            hpm.EnvironmentType = owinContext.Authentication.User.Claims.First(c => c.Type == "env").Value;
            hpm.UserFullName = owinContext.Authentication.User.Claims.First(c => c.Type == "ufn").Value;
            hpm.UserDeptName = owinContext.Authentication.User.Claims.First(c => c.Type == "udn").Value;
            if (owinContext.Authentication.User.Claims.FirstOrDefault(c => c.Type == "uccl") != null)
                hpm.UserCanChangeLogin = Convert.ToInt32(owinContext.Authentication.User.Claims.First(c => c.Type == "uccl").Value) > 0;
            hpm.UserNotifications = userNotifications;

            return View("AppHomePage", hpm);
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public async Task<JsonResult> GetRefUrl(string fpu)
        {
            try
            {
                string fup = GRPEncoding.DecodeString(fpu);

                RestClient<TreeNode> client = new RestClient<TreeNode>(Request.GetOwinContext());
                TreeNode[] allowedSystems = await client.GetMultipleItemsRequest("Sys/GetUserSys");

                TreeNode requestedNode = null;

                if (fup.IndexOf('?') == -1)     // check if it contains query string params or not.
                {
                    requestedNode = allowedSystems.Flatten().FirstOrDefault(n => n.attributes.Contains(fup));
                }
                else
                {
                    string requestedUrlActionName = fup.Substring(fup.LastIndexOf('/') + 1, fup.IndexOf('?') - fup.LastIndexOf('/') - 1);
                    requestedNode = allowedSystems.Flatten().FirstOrDefault(n => n.attributes.Contains(requestedUrlActionName));
                }

                if (requestedNode == null)
                    return Json(new { res = 0, resCon = "unauthrorized access to system feature." }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { res = 1, resCon = requestedNode.id, resUrl = fup }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { res = 0, resCon = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ContentResult GetOUrl()
        {
            SysSettings settings = Session["sysSettings"] as SysSettings;
            IOwinContext owinContext = Request.GetOwinContext();

            string userId = owinContext.Authentication.User.Claims.First(c => c.Type == "uid").Value;
            string loginSerial = owinContext.Authentication.User.Claims.First(c => c.Type == "uls").Value;
            string envType = owinContext.Authentication.User.Claims.First(c => c.Type == "env").Value;

            string ourl = string.Format("{0}config={1}&form=frm.fmx&otherparams=user_id={2}&otherparams=login_srl={3}&otherparams=form_id=frmid",
                settings.forms_path,
                envType == "T" ? ConfigurationManager.AppSettings["OraTestConfig"] : ConfigurationManager.AppSettings["OraProdConfig"],
                userId, loginSerial);

            return Content(ourl);
        }

        public async Task<JsonResult> UserSys()
        {
            RestClient<TreeNode> client = new RestClient<TreeNode>(Request.GetOwinContext());

            TreeNode[] allowedSystems = await client.GetMultipleItemsRequest("Sys/GetUserSys");

            return Json(allowedSystems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHYears()
        {
            SysSettings ss = Session["sysSettings"] as SysSettings;

            if (ss != null)
            {
                return Json(new { chd = ss.current_hDate, hyl = ss.hijriYears }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> GetSysMsg(int msgCode)
        {
            RestClient<string> client = new RestClient<string>(Request.GetOwinContext());

            string msgText = await client.PostRequest("Sys/GetMessage", msgCode);

            return Json(msgText, JsonRequestBehavior.AllowGet);
        }
    }

    public static class TreeNodeExt
    {
        public static IEnumerable<TreeNode> Flatten(this IEnumerable<TreeNode> e)
        {
            return e.SelectMany(c => c.children.Flatten()).Concat(e);
        }
    }

    #region Commented AngularJS Code

    /*requestedNode = allowedSystems.Flatten().FirstOrDefault(delegate (TreeNode node) {
        string sourceUrl = node.attributes.Split('|')[5];
        string targetUrl = fup.Split('/')[3];

        return targetUrl.Contains(sourceUrl);
    });*/

    //function compileAngularElement(elSelector)
    //{
    //    var elSelector = (typeof elSelector == 'string') ? elSelector : null;
    //    // The new element to be added
    //    if (elSelector != null)
    //    {
    //        var $div = $(elSelector);

    //        // The parent of the new element
    //        var $target = $("[ng-app]");

    //        angular.element($target).injector().invoke(['$compile', function($compile) {
    //            var $scope = angular.element($target).scope();
    //        $compile($div)($scope);
    //        // Finally, refresh the watch expressions in the new element
    //        $scope.$apply();
    //        }]);
    //    }
    //}

    //function ajCompile(content, callback)
    //{
    //    var $target = $("[ng-app]");

    //    angular.element($target).injector().invoke(['$compile', function($compile) {
    //        var $scope = angular.element($target).scope();
    //        var compiled = $compile(content)($scope);

    //    $scope.$apply();
    //        callback(compiled);
    //    }]);
    //}

    #endregion
}


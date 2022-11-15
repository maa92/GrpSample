using GRP.Models.LAW.Register.SessionAccept;
using GRP.Web.Controllers;
using GRP.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GRP.Web.Areas.LAW.Controllers
{
    [Authorize]
    public partial class RegisterController : BaseController
    {
        public async Task<ActionResult> SessionAccept()
        {
            RestClient<SessionRegForAccept> client = new RestClient<SessionRegForAccept>(OwinContext);
            SessionRegForAccept[] sessionData = await client.GetMultipleItemsRequest("SessionAccept/GetFormOpenData");
            SessionAcceptFormOpen formData = new SessionAcceptFormOpen
            {
                formUniqueId = "0",
                sessionLst = sessionData
            };
            return View("SessionAccept", formData);
        }

        public async Task<JsonResult> ReloadApproveRequests()
        {
            RestClient<SessionRegForAccept> client = new RestClient<SessionRegForAccept>(OwinContext);
            SessionRegForAccept[] sessionData = await client.GetMultipleItemsRequest("SessionAccept/GetFormOpenData");
            return Json(sessionData, JsonRequestBehavior.AllowGet);
        }

        [ActionName("SessionApproveByReqSrl")]
        public async Task<ActionResult> SessionAccept(string sSrl, string yr, string cSrl)
        {
            RestClient<SessionRegForAccept[]> client = new RestClient<SessionRegForAccept[]>(OwinContext);
            SessionRegForAccept[] sessionData = await client.PostRequest("SessionAccept/GetFormOpenSessionData", new int[] { Convert.ToInt32(sSrl), Convert.ToInt32(yr), Convert.ToInt32(cSrl) });
            SessionAcceptFormOpen formData = new SessionAcceptFormOpen
            {
                formUniqueId = string.Concat("lawAtt", sSrl),
                sessionLst = sessionData
            };
            return View("SessionAccept", formData);
        }

        [HttpPost]
        public async Task<ActionResult> SaveSessionAcceptReq(string SessionNote, SessionRegForAccept[] sessionData)
        {
            RestClient<SessionAcceptSaveResult> client = new RestClient<SessionAcceptSaveResult>(OwinContext);
            foreach (SessionRegForAccept session in sessionData)
            {
                session.SESSION_NOTE = SessionNote;
            }
            SessionAcceptSaveResult lawAcceptResult = await client.PostRequest("SessionAccept/SaveSessionAcceptReq", sessionData);
            return Json(lawAcceptResult);
        }
    }
}
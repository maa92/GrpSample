using GRP.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Web.Mvc;
using GRP.Models.LAW.Register;
using GRP.Web.Helpers;
using GRP.Models.System;
using System.Web.Script.Serialization;
using GRP.Web.Areas.LAW.Helpers;
using GRP.Web.SRHubs;

namespace GRP.Web.Areas.LAW.Controllers
{
    [Authorize]
    public partial class RegisterController : BaseController
    {
        private readonly Microsoft.AspNet.SignalR.IHubContext _notificationsHubContex =
            Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<RequestsNotificationHub>();

        //public async Task<ActionResult> SessionReg()
        //{
        //    return View();
        //}

        public ActionResult SessionReg(string wndId, string wndTitle)
        {
            ViewData["wndId"] = wndId;
            ViewData["wndTitle"] = wndTitle;

            return PartialView();
        }

        public async Task<ActionResult> LoadFormLookups()
        {
            RestClient<SessionRegFormOpen> client = new RestClient<SessionRegFormOpen>(Request.GetOwinContext());
            SessionRegFormOpen lookUpData = await client.GetSingleItemRequest("SessionReg/GetFormOpenData");
            return Json(lookUpData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SaveLawSessionRequest(LawSessionReqSaveRequest sessionReq)
        {
            RestClient<LawSessionReqSaveResult> client = new RestClient<LawSessionReqSaveResult>(Request.GetOwinContext());
            //foreach (LawSessionReqAttachments Att in sessionReq.LawSessionReqAtt)
            //{
            //    //byte[] bytes = Att.FileBytes.SelectMany(BitConverter.GetBytes).ToArray();
            //    byte[] bytes = Att.FileBytes.Cast<int>().Select(i => (byte)i).ToArray();
            //    Att.ATTACH_FILE = bytes;
            //    //sessionReq.fileBytes = bytes;
            //}

            LawSessionReqSaveResult result = await client.PostRequest("SessionReg/SaveNewLawSessionRequest", sessionReq);
            if (sessionReq.LawCaseReqDet.DF != "D" && result.res > 0 && result.msg == "")
            {
                foreach (NotifyList notify in result.notifyLst)
                {
                    Notification notification = new Notification();
                    notification.GroupName = notify.NOTICE_FLAG;
                    notification.GroupText = notify.NOTICE_NAME;
                    notification.Text = string.Format("{0} - {1}", result.SessionSerial, notify.EMPNAME);
                    notification.Params.Add("rId", SessionRegNotification.GetNotificationId(notify.NOTICE_FLAG, result.SessionSerial));
                    notification.Params.Add("tabTxt", SessionRegNotification.GetApproveNotificationTabText(notify.NOTICE_FLAG, result.SessionSerial));
                    notification.Params.Add("url", SessionRegNotification.GetApprovalnotificationUrl(notify.NOTICE_FLAG, result.SessionSerial, result.CaseYear, result.CaseSerial));

                    _notificationsHubContex.Clients.User(notify.USER_ID).addRequestNotification(notification);
                }
            }

            return Json(result);
        }

        //public async Task<ActionResult> SaveSessionFile(LawSessionAttachments sessionFile)
        //{
        //    RestClient<LawSessionFilesSaveResult> client = new RestClient<LawSessionFilesSaveResult>(Request.GetOwinContext());
        //    byte[] bytes = sessionFile.FileInInt.Cast<int>().Select(i => (byte)i).ToArray();
        //    sessionFile.AttachFile = bytes;
        //    LawSessionFilesSaveResult result = await client.PostRequest("SessionReg/SaveSessionFile", sessionFile);
        //    return Json(result);
        //}

        [HttpPost]
        public async Task<ActionResult> SaveFile(string df, string CaseYear, string CaseSerial, string SessionId, string AttachId, string AttachDate, string AttachFileName, string AttachFileType, string AttachNote, int AttachType, HttpPostedFileBase fileData)
        {
            var supportedFileExt = new[] { ".pdf", ".doc", ".docx", ".jpg" };
            var fileExt = Path.GetExtension(fileData.FileName);
            MemoryStream memory = new MemoryStream();
            fileData.InputStream.CopyTo(memory);
            byte[] bytes = memory.ToArray();
            SessionAttachment Att = new SessionAttachment()
            {
                DF = df,
                ATTACH_ID = string.Empty,
                ATTACH_DATE = AttachDate,
                ATTACH_FILE_NAME = AttachFileName,
                ATTACH_FILE_TYPE = AttachFileType,
                ATTACH_NOTES = AttachNote
            };
            LawSessionAttachments sessionFile = new LawSessionAttachments()
            {
                CaseYear = CaseYear,
                CaseSerial = Convert.ToInt32(CaseSerial),
                SessionId = Convert.ToInt32(SessionId),
                AttachFile = bytes,
                AttachType = AttachType,
                SessionAttach = Att
            };
            if (fileData.ContentLength > 1024 * 1024) //1 Megabyte
            {
                LawSessionFilesSaveResult res = new LawSessionFilesSaveResult() { msg = "حجم الملف يجب أن لا يتجاوز 1 ميجابايت" };
                return Json(res);
            }
            else if (!supportedFileExt.Contains(fileExt))
            {
                LawSessionFilesSaveResult res = new LawSessionFilesSaveResult() { msg = "الملفات المسموحة هي .pdf و .doc و .jpg" };
                return Json(res);
            }

            RestClient<LawSessionFilesSaveResult> client = new RestClient<LawSessionFilesSaveResult>(Request.GetOwinContext());
            LawSessionFilesSaveResult result = await client.PostRequest("SessionReg/SaveSessionFile", sessionFile);
            return Json(result);

        }

        [HttpPost]
        public async Task<ActionResult> DeleteFile(string df, string CaseYear, string CaseSerial, string SessionId, string AttachId, string AttachDate)
        {
            SessionAttachment Att = new SessionAttachment()
            {
                DF = df,
                ATTACH_ID = AttachId,
                ATTACH_DATE = AttachDate,
                ATTACH_FILE_NAME = null,
                ATTACH_FILE_TYPE = null,
                ATTACH_NOTES = null
            };
            LawSessionAttachments sessionFile = new LawSessionAttachments()
            {
                CaseYear = CaseYear,
                CaseSerial = Convert.ToInt32(CaseSerial),
                SessionId = Convert.ToInt32(SessionId),
                AttachFile = null,
                AttachType = null,
                SessionAttach = Att
            };
            RestClient<LawSessionFilesSaveResult> client = new RestClient<LawSessionFilesSaveResult>(Request.GetOwinContext());
            LawSessionFilesSaveResult result = await client.PostRequest("SessionReg/SaveSessionFile", sessionFile);
            return Json(result);
        }


        //[HttpPost]
        //public ActionResult UploadFile(HttpPostedFileBase file_name, string Case_No)
        //{
        //    string FilePath = @"D:\srca\Projects\InternalEServices\GRP-Dev\GRP.Web\UploadedFiles\";
        //    var supportrdTypes = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
        //    var fileExt = Path.GetExtension(file_name.FileName);
        //    try
        //    {
        //        string FileName = string.Empty;
        //        if (file_name.ContentLength > 0)
        //        {
        //            string subPath = "LAW_" + "325";
        //            bool exists = Directory.Exists(Server.MapPath("~/UploadedFiles/" + subPath));
        //            if (!exists)
        //            {
        //                Directory.CreateDirectory(FilePath + subPath);
        //            }
        //            FileName = Path.GetFileName(file_name.FileName);
        //            string path = Path.Combine(Server.MapPath("~/UploadedFiles" + "/" + subPath), FileName);
        //            file_name.SaveAs(path);
        //        }
        //        ViewBag.Message = "File Uploaded Successfully!";
        //        return Json(FileName, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {

        //        ViewBag.Message = "File upload failed:" + ex.Message;
        //        return Json(ex.Message);
        //    }
        //}
    }
}
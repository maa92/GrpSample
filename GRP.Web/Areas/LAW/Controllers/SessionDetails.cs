using GrpSample.Models.LAW.Register;
using GrpSample.Web.Controllers;
using GrpSample.Web.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace GrpSample.Web.Areas.LAW.Controllers
{
    [Authorize]
    public partial class RegisterController : BaseController
    {

        public ActionResult SessionDetails(string wndId, string wndTitle, string CaseYear, string CaseSerial, string SessionId)
        {
            ViewData["wndId"] = wndId;
            ViewData["wndTitle"] = wndTitle;
            return PartialView();
        }

        public async Task<ActionResult> LoadDetailFormLookups()
        {
            RestClient<SessionRegFormOpen> client = new RestClient<SessionRegFormOpen>(Request.GetOwinContext());
            SessionRegFormOpen lookUpData = await client.GetSingleItemRequest("SessionReg/GetFormOpenData");
            return Json(lookUpData, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SessionDetailsRet(string CaseYear, string CaseSerial, string SessionId)
        {
            RestClient<SessionDetailsRet> client = new RestClient<SessionDetailsRet>(Request.GetOwinContext());
            SessionDetailsRet sessionInfo = await client.PostRequest("SessionDetails/GetSessionInfo", new string[] { CaseYear, CaseSerial, SessionId });
            foreach (var file in sessionInfo.SessionAttachments)
            {
                //HttpPostedFileBase objFile = (HttpPostedFileBase)new CustomPostedFile(file.ATTACH_FILE, "test.pdf");
                //HttpPostedFileBase file_name = objFile;
                file.fileObj = (HttpPostedFileBase)new CustomPostedFile(file.ATTACH_FILE, file.ATTACH_FILE_NAME, file.ATTACH_FILE_TYPE);//JsonConvert.SerializeObject(file.ATTACH_FILE);//JsonConvert.SerializeObject(objFile, new HttpPostedFileConverter());  //objFile;  
                file.ATTACH_FILE = null;
            }
            JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = Int32.MaxValue;
            var jsonSerializedResult = JsonConvert.SerializeObject(sessionInfo, new HttpPostedFileConverter());
            //jsonSerializedResult = GRPEncoding.EncodeString(jsonSerializedResult);
            return Json(jsonSerializedResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SaveFileDet(string df, string CaseYear, string CaseSerial, string SessionId, string AttachId, string AttachDate, string AttachFileName, string AttachFileType, string AttachNote, int AttachType, HttpPostedFileBase fileDataDet)
        {
            var supportedFileExt = new[] { ".pdf", ".doc", ".docx", ".jpg" };
            var fileExt = Path.GetExtension(fileDataDet.FileName);
            MemoryStream memory = new MemoryStream();
            fileDataDet.InputStream.CopyTo(memory);
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
            if (fileDataDet.ContentLength > 1024 * 1024) //1 Megabyte
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
            LawSessionFilesSaveResult result = await client.PostRequest("SessionDetails/SaveSessionFileDet", sessionFile);
            return Json(result);

        }

        [HttpPost]
        public async Task<ActionResult> DeleteFileDet(string df, string CaseYear, string CaseSerial, string SessionId, string AttachId, string AttachDate)
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
            LawSessionFilesSaveResult result = await client.PostRequest("SessionDetails/SaveSessionFileDet", sessionFile);
            return Json(result);
        }
    }


}
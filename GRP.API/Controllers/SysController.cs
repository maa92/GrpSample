using GrpSample.API.AuthProviders;
using GrpSample.API.Common;
//using GrpSample.API.Controllers.HR.Helpers;
using GrpSample.DataAccess;
using GrpSample.DataAccess.Handlers;
using GrpSample.Models.System;
using GrpSample.API.Controllers.LAW.Helpers;
//using GrpSample.API.Controllers.HAJ.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;

namespace GrpSample.API.Controllers
{
    [GRPAuthorize(Roles = "Admin")]
    public class SysController : BaseController
    {
        [HttpGet]
        public IHttpActionResult GetSysSettings()
        {
            SystemDH sdh = new SystemDH(Environment);
            SysSettings settings = sdh.GetSysSettings();
            return Ok(settings);
        }

        [HttpGet]
        public IHttpActionResult GetUserSys()
        {
            SystemDH sdh = new SystemDH(Environment);
            List<SysForm> systems = sdh.GetUserSystems(Convert.ToDecimal(UserId));
            List<TreeNode> retVal = GroupingUtil.BuildFormTree(systems).ToList();

            return Ok(retVal);
        }

        //[HttpGet]
        //public IHttpActionResult GetUserNotifications()
        //{
        //    SystemDH sdh = new SystemDH(Environment);
        //    List<NotificationDb> notificationsDb = sdh.GetUserNotifications(Convert.ToDecimal(UserId));
        //    Notification notification = null;
        //    List<Notification> notifications = new List<Notification>();

        //    foreach (NotificationDb ndb in notificationsDb)
        //    {
        //        notification = new Notification()
        //        {
        //            GroupName = ndb.NTF,
        //            GroupText = ndb.NTN
        //        };
        //        notification.Text = string.Format("{0} - {1}", ndb.SL, ndb.EN);
        //        if (ndb.NTF.StartsWith("VAC"))
        //        {
        //            notification.Params["rId"] = VacationRequestNotification.GetNotificationId(ndb.NTF, ndb.SL.ToString());
        //            notification.Params["tabTxt"] = VacationRequestNotification.GetApproveNotificationTabText(ndb.NTF, ndb.SL.ToString());
        //            notification.Params["url"] = VacationRequestNotification.GetApproveNotificationUrl(ndb.NTF, ndb.SL.ToString(), ndb.YR.ToString());
        //        }
        //        else if (ndb.NTF.StartsWith("INT"))
        //        {
        //            if (ndb.NTF == "INTPAYACTIV")
        //            {
        //                notification.Text = string.Format("{0} - {1}", ndb.SL, ndb.EN);
        //                notification.Params["rId"] = "intpay" + ndb.SL;
        //                notification.Params["tabTxt"] = string.Format("{0} اعتماد مسير انتداب", ndb.SL);
        //                notification.Params["url"] = string.Format("{0}/HR/SalAndAlow/SingleSheetBySlYr?sl={1}&yr={2}", ConfigurationManager.AppSettings["notifyUrlPfx"], ndb.SL, ndb.YR);
        //            }
        //            else
        //            {
        //                if (ndb.NTF == "INTENDAPP")
        //                    notification.Text = string.Format("{0} - {1}", ndb.EC, ndb.EN);
        //                notification.Params["rId"] = IntidabRequestNotification.GetNotificationId(ndb.NTF, ndb.SL.ToString(), ndb.EC.ToString());
        //                notification.Params["tabTxt"] = IntidabRequestNotification.GetApproveNotificationTabText(ndb.NTF, ndb.SL.ToString());
        //                notification.Params["url"] = IntidabRequestNotification.GetApproveNotificationUrl(ndb.NTF, ndb.SL.ToString(), ndb.YR.ToString(), ndb.EC.ToString());
        //            }
        //        }
        //        else if (ndb.NTF.StartsWith("OPR"))
        //        {
        //            notification.Params["rId"] = "msta" + ndb.SL;
        //            notification.Params["tabTxt"] = string.Format("{0} - {1}", ndb.SL, ndb.EN);
        //            notification.Params["url"] = string.Format("{0}/OPR/Approvals/MnthTblAprvByPrms?cnt={1}&yr={2}&mn={3}&vno={4}", ConfigurationManager.AppSettings["notifyUrlPfx"], ndb.SL, ndb.YR, ndb.EC, ndb.ES);
        //        }
        //        else if (ndb.NTF.StartsWith("OVR"))
        //        {
        //            if (ndb.NTF == "OVRPAYACTIV")
        //            {
        //                notification.Text = string.Format("{0} - {1}", ndb.SL, ndb.EN);
        //                notification.Params["rId"] = "intpay" + ndb.SL;
        //                notification.Params["tabTxt"] = string.Format("{0} اعتماد مسير العمل الاضافي", ndb.SL);
        //                notification.Params["url"] = string.Format("{0}/HR/SalAndAlow/OvertimeBySlYr?sl={1}&yr={2}", ConfigurationManager.AppSettings["notifyUrlPfx"], ndb.SL, ndb.YR);
        //            }
        //            else
        //            {
        //                if (ndb.NTF == "OVRENDAPP")
        //                    notification.Text = string.Format("{0} - {1}", ndb.EC, ndb.EN);
        //                notification.Params["rId"] = OvertimeRequestNotification.GetNotificationId(ndb.NTF, ndb.SL.ToString(), ndb.EC.ToString());
        //                notification.Params["tabTxt"] = OvertimeRequestNotification.GetApproveNotificationTabText(ndb.NTF, ndb.SL.ToString());
        //                notification.Params["url"] = OvertimeRequestNotification.GetApproveNotificationUrl(ndb.NTF, ndb.SL.ToString(), ndb.YR.ToString(), ndb.EC.ToString());
        //            }
        //        }
        //        else if (ndb.NTF.StartsWith("ENDSRV"))
        //        {
        //            if (ndb.NTF == "ENDSRVAPP")
        //                notification.Text = string.Format("{0} - {1}", ndb.EC, ndb.EN);
        //            notification.Params["rId"] = EndOfServiceRequestNotification.GetNotificationId(ndb.NTF, ndb.SL.ToString());
        //            notification.Params["tabTxt"] = EndOfServiceRequestNotification.GetApproveNotificationTabText(ndb.NTF, ndb.SL.ToString());
        //            notification.Params["url"] = EndOfServiceRequestNotification.GetApproveNotificationUrl(ndb.NTF, ndb.SL.ToString(), ndb.YR.ToString(), ndb.EC.ToString());
        //        }
        //        //****Case session notification
        //        else if (ndb.NTF.StartsWith("CAS"))
        //        {
        //            if (ndb.NTF == "CASATNAPP")
        //                notification.Text = string.Format("{0} - {1}", ndb.ES + " / " + ndb.SL, ndb.EN);  //ES is SessionId
        //            notification.Params["rId"] = SessionRegNotification.GetNotificationId(ndb.NTF, ndb.ES.ToString());
        //            notification.Params["tabTxt"] = SessionRegNotification.GetApproveNotificationTabText(ndb.NTF, ndb.ES.ToString());
        //            notification.Params["url"] = SessionRegNotification.GetApprovalnotificationUrl(ndb.NTF, ndb.ES.ToString(), ndb.YR.ToString(), ndb.SL.ToString());
        //        }
        //        //******Haj Request notification
        //        else if (ndb.NTF.StartsWith("HAJ"))
        //        {
        //            if (ndb.NTF == "HAJNEWAPP")
        //                notification.Text = string.Format("{0} - {1}", ndb.ES, ndb.EN);  //ES is Request id
        //            notification.Params["rId"] = HajRequestNotification.GetNotificationId(ndb.NTF, ndb.ES.ToString());
        //            notification.Params["tabTxt"] = HajRequestNotification.GetApproveNotificationTabText(ndb.NTF, ndb.ES.ToString());
        //            notification.Params["url"] = HajRequestNotification.GetApprovalnotificationUrl(ndb.NTF, ndb.ES.ToString(), ndb.YR.ToString());
        //        }

        //        notifications.Add(notification);
        //    }

        //    return Ok(notifications);
        //}

        [HttpPost]
        public IHttpActionResult GetMessage([FromBody]int msgCode)
        {
            CommonDH udh = new CommonDH(Environment);
            string Msg = udh.GetMessageText(msgCode);

            return Ok(Msg);
        }
    }
}

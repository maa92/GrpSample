using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace GRP.API.Controllers.LAW.Helpers
{
    public class SessionRegNotification
    {
        public static string GetNotificationId(string NotifyType, /*string cSrl,*/ string sSrl)
        {
            string retVal = string.Empty;
            switch (NotifyType)
            {
                case "CASATNAPP":
                    retVal = "lawAtt" + sSrl;
                    break;
            }
            return retVal;
        }

        public static string GetApprovalnotificationUrl(string NotifyType, string sSrl, string yr, string cSrl = "")
        {
            string retVal = string.Empty;
            switch (NotifyType)
            {
                case "CASATNAPP":
                    retVal = string.Format("{0}/LAW/Register/SessionApproveByReqSrl?sSrl={1}&yr={2}&cSrl={3}",
                        ConfigurationManager.AppSettings["notifyUrlPfx"], sSrl, yr, cSrl);
                    break;
            }
            return retVal;
        }

        public static string GetApproveNotificationTabText(string NotifyType, string sSrl)
        {
            string retVal = string.Empty;
            switch (NotifyType)
            {
                case "CASATNAPP":
                    retVal = string.Format("حضور جلسة {0}", sSrl);
                    break;
            }
            return retVal;
        }
    }
}
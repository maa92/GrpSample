using GrpSample.Models.SysCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.LAW.Register
{
    public class LawSessionReqSaveResult : SaveOperationResult
    {
        public string CaseYear { get; set; }
        public string CaseSerial { get; set; }
        public string SessionSerial { get; set; }   //رقم الجلسة
        public List<NotifyList> notifyLst { get; set; }
    }

    public class LawSessionFilesSaveResult : SaveOperationResult
    {
        public string AttachmentId { get; set; }
    }

    public class NotifyList
    {
        public string USER_ID { get; set; }
        public string EMPNAME { get; set; }   //EMPNAME for notification.Text
        public string NOTICE_FLAG { get; set; }
        public string NOTICE_NAME { get; set; }
    }
}

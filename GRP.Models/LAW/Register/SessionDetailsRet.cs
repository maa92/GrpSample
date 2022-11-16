using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.LAW.Register
{
    public class SessionDetailsRet  //For SessionDetails.cshtml onOpen retrive data
    {
        public LawSessionDetailsRet LawSessionReqDet { get; set; }
        public List<SessionEmpsRet> LawSessionEmps { get; set; }   //Emps in a session
        public List<SessionAttachmentsRet> SessionAttachments { get; set; }  //Uploaded files in Session
        public string errMsg { get; set; }
    }

    public class LawSessionDetailsRet  //بيانات الجلسة 
    {
        public Int32 SESSION_ID { get; set; }
        public string SESSION_DATE_G { get; set; }
        public string SESSION_DATE_H { get; set; }
        public int SESSION_STATUS { get; set; }
        public string SESSION_STATUS_DESC { get; set; }
        public Int32 BRANCH_SRL_ID { get; set; }
        //public string THE_COURT { get; set; }
        public string SESSION_DAY { get; set; }
        public string SESSION_TIME { get; set; }
        public string SESSION_NOTES { get; set; }
        public string IS_ATTEND { get; set; }
        public String CBY { get; set; }
        public String CDT { get; set; }
        public String UBY { get; set; }
        public String UDT { get; set; }
    }

    public class SessionEmpsRet   //الموظفين المختصين
    {
        public string EMP_CODE { get; set; }
        public string EMP_NAME { get; set; }
        public string SESSION_NOTE { get; set; }
        public String CBY { get; set; }
        public String CDT { get; set; }
        public String UBY { get; set; }
        public String UDT { get; set; }

    }

    public class SessionAttachmentsRet  //المرفقات
    {
        public string ATTACH_ID { get; set; }
        public string ATTACH_DATE_G { get; set; }
        public string ATTACH_DATE_H { get; set; }
        public int ATTACH_TYPE { get; set; }  //نوع المرفق
        public string ATTACH_TYPE_DESC { get; set; }
        public string ATTACH_FILE_NAME { get; set; }  //اسم الملف
        public string ATTACH_FILE_TYPE { get; set; }  //نوع الملف
        public byte[] ATTACH_FILE { get; set; }
        public object fileObj { get; set; }  //this object is to view a file in the browser after converting from byte[]
        public string ATTACH_NOTES { get; set; }
        public String CBY { get; set; }
        public String CDT { get; set; }
        public String UBY { get; set; }
        public String UDT { get; set; }
    }
}

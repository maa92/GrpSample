using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.LAW.Register
{
    public class LawSessionReqSaveRequest
    {
        //public string CaseYear { get; set; }
        //public string CaseSerial { get; set; }
        public LawCaseReqSessionDet LawCaseReqDet { get; set; }
        //public byte[] fileBytes { get; set; }   //just try taking file alone
        public EmpsInSession[] LawSessionEmps { get; set; }   //Emps in a session
        //public LawSessionReqAttachments[] LawSessionReqAtt { get; set; }   //session attachments
    }

    public class EmpsInSession
    {
        public string DF { get; set; }
        //public string CASE_YEAR { get; set; }
        //public string CASE_SERIAL { get; set; }
        public string SESSION_ID { get; set; }
        public string EMP_CODE { get; set; }
        public string EmpName { get; set; } //for notifications only
        public string SESSION_NOTE { get; set; }
    }

    //public class LawSessionReqAttachments
    //{
    //    public string DF { get; set; }
    //    public int CASE_YEAR { get; set; }
    //    public int CASE_SERIAL { get; set; }
    //    public int SESSION_ID { get; set; }
    //    public int ATTACH_ID { get; set; }
    //    public string ATTACH_DATE { get; set; }
    //    public int[] FileBytes { get; set; }  
    //    public byte[] ATTACH_FILE { get; set; }
    //    public String ATTACH_NOTES { get; set; }

    //}
}

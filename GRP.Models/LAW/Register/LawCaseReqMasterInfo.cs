using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Models.LAW.Register
{
    public class LawCaseReqMasterInfo
    {
        public string DF { get; set; }
        public int CASE_YEAR { get; set; }
        public int? CASE_SERIAL { get; set; }
        public int COURT_TYPE { get; set; }   //نوع المحكمة
        public string CASE_NO { get; set; }    //رقم القضية
        public string CASE_NO_PLEA { get; set; }  //رقم القضية بالإستئناف
        public string CASE_NO_HIGH { get; set; }  //رقم القضية بالمحكمة العليا
        public string CASE_DATE { get; set; }
        public int CASE_TYPE { get; set; }  //النوع: ضد الهيئة - من الهيئة
        public int BRANCH_SRL_ID { get; set; }
        public int CASE_STATUS { get; set; }
        public int CASE_SECTOR { get; set; }
        public int? SECTOR_COURT { get; set; }
        public int? COURT_CITY { get; set; }
        public string DISTRICT_NAME { get; set; }  //إسم الدائرة
        public string COURT_OTHERS { get; set; }
        public string EMP_NO_AUTHORITY { get; set; }  //ممثل الهيئة
        public string JUDGEMENT_STATUS { get; set; }  //int?
        public string JUDGMENT_STATUS_OTHERS { get; set; }  //حالة الحكم - أخرى
        public int DEFENDANT_TYPE { get; set; }
        public string DEFENDANT_NAME { get; set; }
        public string CASE_NOTES { get; set; }
        public string CONNECTED_SERIAL { get; set; } //مسلسل القضية المرتبطة
        public string CONNECTED_CASE_YEAR { get; set; }  //سنة القضية المرتبطة
    }
}

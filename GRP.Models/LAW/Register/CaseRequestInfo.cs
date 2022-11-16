using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.LAW.Register
{
    public class CaseRequestInfo
    {
        public String CASE_YEAR { get; set; }  //سنة القضية
        public String CASE_NO { get; set; } //رقم القضية
        public String CASE_NO_PLEA { get; set; } //رقم القضية بالإستئناف 
        public String CASE_NO_HIGH { get; set; } //رقم القضية بالمحكمة العليا
        public String CASE_DATE_G { get; set; }
        public String CASE_DATE_H { get; set; }
        public int COURT_TYPE { get; set; }  //نوع المحكمة
        public String COURT_TYPE_DESC { get; set; }
        public Int32 CASE_TYPE { get; set; }  //النوع 
        public String CASE_TYPE_DESC { get; set; }
        public Int32 BRANCH_SRL_ID { get; set; } //الفرع
        public String BRANCH_NAME { get; set; }
        public Int32 CASE_STATUS { get; set; } //حالة القضية
        public String CASE_STATUS_DESC { get; set; }
        public String CASE_SECTOR { get; set; }  //قطاع المحكمة
        public String CASE_SECTOR_DESC { get; set; }
        public String SECTOR_COURT { get; set; }  //المحكمة
        public String SECTOR_COURT_DESC { get; set; }
        public int COURT_CITY { get; set; }  //المدينة
        public String COURT_CITY_DESC { get; set; }
        public String DISTRICT_NAME { get; set; }  //إسم الدائرة
        public String COURT_OTHERS { get; set; } //أخرى
        public Int32 EMP_NO_AUTHORITY { get; set; } //ممثل الهيئة
        public String EMP_NAME_AUTHORITY { get; set; }
        public Int32 JUDGEMENT_STATUS { get; set; } //حالة الحكم
        public String JUDGEMENT_STATUS_DESC { get; set; }
        public Int32 DEFENDANT_TYPE { get; set; } //طبيعة المدعي - المدعي عليه
        public String DEFENDANT_TYPE_DESC { get; set; }
        public String DEFENDANT_NAME { get; set; } //اسم المدعي - مدعي عليه
        public String CASE_NOTES { get; set; } //وصف القضية
        public String CONNECTED_SERIAL { get; set; } //مسلسل القضية المرتبطة
        public String CONNECTED_CASE_YEAR { get; set; }  //سنة القضية المرتبطة
        public String CONNECTED_CASE_NO { get; set; }  //رقم القضية المرتبطة
        public String CBY { get; set; }
        public String CDT { get; set; }
        public String UBY { get; set; }
        public String UDT { get; set; }
    }
}

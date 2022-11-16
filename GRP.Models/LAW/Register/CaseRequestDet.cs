using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.LAW.Register
{
    public class CaseRequestDet
    {
        public Int32 SESSION_ID { get; set; }
        public String SESSION_DATE_G { get; set; }
        public String SESSION_DATE_H { get; set; }
        public Int32 SESSION_STATUS { get; set; } //حالة الجلسة
        public String SESSION_STATUS_DESC { get; set; }
        //public String EMP_CODE { get; set; } //اسم - رقم الموظف
        public Int32 BRANCH_SRL_ID { get; set; } //الفرع
        public String THE_COURT { get; set; } //الدائرة
        public String SESSION_DAY { get; set; } //اليوم 
        public String SESSION_TIME { get; set; } //الساعة
        public String SESSION_NOTES { get; set; }  //ملاحظات
        public String IS_ATTEND { get; set; }  //حضر - لم يحضر
        public String CBY { get; set; }
        public String CDT { get; set; }
        public String UBY { get; set; }
        public String UDT { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Models.LAW.Register.SessionAccept
{
    public class SessionRegForAccept
    {
        public string df { get; set; }
        public string CASE_YEAR { get; set; }
        public string CASE_SERIAL { get; set; }
        public string SESSION_ID { get; set; }
        public string EMP_CODE { get; set; }  //for Session Accept DML
        public string SESSION_DATE { get; set; }
        public string SESSION_HDATE { get; set; }
        public string SESSION_GDATE { get; set; }
        public string SESSION_STATUS { get; set; }
        public string STATUS_DESC { get; set; }
        public string CASE_NO { get; set; }   //رقم القضية الإبتدائية
        public string BRANCH_SRL_ID { get; set; }
        public string BRANCH_NAME { get; set; }
        public string CASE_SECTOR { get; set; }
        public string CASE_SECTOR_DESC { get; set; }
        public string COURT_CITY { get; set; }
        public string CITY_NAME { get; set; }
        public string SESSION_NOTES { get; set; }  //For session notes
        public string SESSION_NOTE { get; set; }  //For emp notes, for DML purposes only
        public string IS_ATTEND { get; set; }
        public string CBY { get; set; }
        public string CDT { get; set; }
        public string UBY { get; set; }
        public string UDT { get; set; }

    }
}

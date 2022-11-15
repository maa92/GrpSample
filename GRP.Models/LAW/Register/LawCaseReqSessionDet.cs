using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Models.LAW.Register
{
    public class LawCaseReqSessionDet
    {
        public string DF { get; set; }
        public string CASE_YEAR { get; set; }
        public string CASE_SERIAL { get; set; }
        public string SESSION_ID { get; set; }
        public string SESSION_DATE { get; set; }
        public string SESSION_STATUS { get; set; }
        //public string EMP_CODE { get; set; }
        public string BRANCH_SRL_ID { get; set; }
        //public string THE_COURT { get; set; }
        public string SESSION_DAY { get; set; }
        public string SESSION_TIME { get; set; }
        public string SESSION_NOTES { get; set; }
        public string IS_ATTEND { get; set; }

    }
}

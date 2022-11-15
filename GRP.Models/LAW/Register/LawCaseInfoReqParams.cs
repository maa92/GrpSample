using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Models.LAW.Register
{
    public class LawCaseInfoReqParams  //Originally created for LAW_FRM_CASE_REQ_QUR. maybe not needed!
    {
        public int EmpCode { get; set; }
        public int? CaseYear { get; set; }
        public int? CaseSerial { get; set; }
        public int? SessionID { get; set; }  //law session
    }
}

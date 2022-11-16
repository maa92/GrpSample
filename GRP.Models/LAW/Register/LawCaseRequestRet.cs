using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.LAW.Register
{
    public class LawCaseRequestRet
    {
        public CaseRequestInfo LawReqMstr { get; set; } //بيانات القضية
        public List<CaseRequestDet> LawReqDetail { get; set; }  //بيانات الجلسة
        public string errMsg { get; set; }
    }
}

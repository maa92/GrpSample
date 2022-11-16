using GrpSample.Models.SysCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.LAW.Register
{
    public class LawCaseReqSaveResult : SaveOperationResult
    {
        public string CaseSerial { get; set; }
        public string CaseYear { get; set; }
    }
}

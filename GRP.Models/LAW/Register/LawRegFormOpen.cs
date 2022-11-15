using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Models.LAW.Register
{
    public class LawRegFormOpen
    {
        public int caseYear { get; set; }
        public List<Branches> branches { get; set; }
        public List<CaseStatus> caseStatus { get; set; }
        public List<JudgmentStatus> JdgmntStatus { get; set; }
        public List<DefendantType> DfndtType { get; set; }
        public List<GCourtNames> GCourtNames { get; set; }
        public List<JCourtNames> JCourtNames { get; set; }
        public List<CourtCity> CourtCity { get; set; }
    }

    public class Branches
    {
        public int SRL_ID { get; set; }
        public string OLD_GEN_DEPT { get; set; }
        public string DEPT_NAME { get; set; }
    }

    public class CaseStatus
    {
        public int CODE_ID { get; set; }
        public string CODE_NAME { get; set; }
    }

    public class JudgmentStatus
    {
        public int CODE_ID { get; set; }
        public string CODE_NAME { get; set; }
    }

    public class DefendantType
    {
        public int CODE_ID { get; set; }
        public string CODE_NAME { get; set; }
    }

    public class GCourtNames
    {
        public int CODE_ID { get; set; }
        public string CODE_NAME { get; set; }
    }

    public class JCourtNames
    {
        public int CODE_ID { get; set; }
        public string CODE_NAME { get; set; }
    }

    public class CourtCity
    {
        public int CODE_ID { get; set; }
        public string CODE_NAME { get; set; }
    }
}

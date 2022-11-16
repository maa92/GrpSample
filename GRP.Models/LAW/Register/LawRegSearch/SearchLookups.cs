using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.LAW.Register.LawRegSearch
{
    public class SearchLookups
    {
        public int GrgYear { get; set; }
        public int hijriYear { get; set; }
        public List<Branches> branches { get; set; }
        public List<CaseStatus> cStatus { get; set; }
        public List<JudgmentStatus> JStatus { get; set; }
        public List<DefendantType> defntType { get; set; }
        public List<GCourtNames> GCourts { get; set; }
        public List<JCourtNames> JCourts { get; set; }
        public List<CourtCity> cCity { get; set; }
    }
}

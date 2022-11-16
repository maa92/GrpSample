using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.LAW.Register.LawRegSearch
{
    public class SearchParams
    {
        public string CASE_NO { get; set; }
        public string CASE_DATE { get; set; }
        public string CASE_YEAR { get; set; }
        public string CASE_TYPE { get; set; }
        public string CASE_SERIAL { get; set; }
        public string COURT_TYPE { get; set; }
        public string CASE_STATUS { get; set; }
        public string CASE_SECTOR { get; set; }
        public string SECTOR_COURT { get; set; }
        public string COURT_CITY { get; set; }
        public string EMP_NO_AUTHORITY { get; set; }
        public string JUDGEMENT_STATUS { get; set; }
        public string DEFENDANT_TYPE { get; set; }
        public int page { get; set; }
        public int rows { get; set; }

        public bool loadManual { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.LAW.Register.LawRegSearch
{
    public class SearchResult
    {
        public string total { get; set; }
        public List<SearchResultRecord> rows { get; set; }
    }

    public class SearchResultRecord
    {
        public string CASE_YEAR { get; set; }
        public string CASE_SERIAL { get; set; }
        public string CASE_NO { get; set; }
        public string CASE_HDATE { get; set; }
        public string CASE_GDATE { get; set; }
        public string CASE_TYPE_DESC { get; set; }
        public string CASE_STATUS { get; set; }
        public string JUDGEMENT_STATUS { get; set; }
        public string DEFENDANT_TYPE { get; set; }
        //public string CASE_NOTES { get; set; }

    }
}

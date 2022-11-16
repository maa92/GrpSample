using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.HR
{
    public class EmpInfoForLookup
    {
        private string _eNm = string.Empty;
        public string eId { get; set; }
        //public string eNm { get; set; }
        public string eNm {
            get
            {
                return eId == "0" ? string.Empty : string.Format("{0} - {1}", eId, _eNm.Replace(eId + " -", string.Empty));
            }
            set
            {
                _eNm = value;
            }
        }
    }
}

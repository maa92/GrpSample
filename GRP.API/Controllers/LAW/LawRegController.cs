using GRP.API.AuthProviders;
using GRP.API.Common;
using GRP.DataAccess.Handlers.LAW;
using GRP.Models.HR;
using GRP.Models.LAW.Register;
using GRP.Models.LAW.Register.LawRegSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace GRP.API.Controllers.LAW
{
    [GRPAuthorize(Roles = "Admin")]
    public class LawRegController : BaseController
    {
        [HttpGet]
        public IHttpActionResult GetFormOpenData()
        {
            LawRegDH dah = new LawRegDH(Environment);
            LawRegFormOpen retVal = dah.GetFormOpenData(Convert.ToDecimal(UserId));

            return Ok(retVal);
        }

        [HttpPost]
        public IHttpActionResult SaveLawCaseRequest(LawCaseReqSaveRequest saveReq)
        {
            LawRegDH dah = new LawRegDH(Environment);
            LawCaseReqSaveResult retVal = dah.SaveLawCaseRequest(saveReq, Convert.ToDecimal(UserId));

            return Ok(retVal);
        }

        [HttpPost]
        public IHttpActionResult GetLawCaseInfoForReq([FromBody]int[] Case)
        {
            LawRegDH dah = new LawRegDH(Environment);
            LawCaseRequestRet retVal = dah.GetLawCaseForNewRequest(Case[0], Case[1], Convert.ToDecimal(UserId));

            return Ok(retVal);
        }

        [HttpPost]
        public IHttpActionResult SearchEmpByCodeOrName([FromBody]string srchFilter)
        {
            LawRegDH dah = new LawRegDH(Environment);
            List<EmpInfoForLookup> retVal = dah.SearchEmpByCodeOrName(Convert.ToDecimal(UserId), srchFilter);
            /*foreach (var emp in retVal)
            {
                if (emp.eId == "0")
                    emp.eNm = string.Empty;
                else
                    emp.eNm = string.Format("{0} - {1}", emp.eId, emp.eNm);
            }*/

            return Ok(retVal);
        }

        [HttpGet]
        public IHttpActionResult GetPrevCases()
        {
            LawRegDH dah = new LawRegDH(Environment);
            List<SearchResultRecord> retVal = dah.GetPrevCases();
            return Ok(retVal);
        }

        [HttpPost]
        public IHttpActionResult GetPrevCaseBySerial([FromBody]int[] pCase)
        {
            LawRegDH dah = new LawRegDH(Environment);
            string[] retVal = dah.GetPrevCaseBySerial(pCase[0], pCase[1]);

            return Ok(retVal);
        }
    }
}
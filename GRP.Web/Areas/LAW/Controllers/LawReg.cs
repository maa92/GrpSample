using GrpSample.Models.HR;
using GrpSample.Models.LAW.Register;
using GrpSample.Models.LAW.Register.LawRegSearch;
using GrpSample.Web.Controllers;
using GrpSample.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GrpSample.Web.Areas.LAW.Controllers
{
    [Authorize]
    public partial class RegisterController : BaseController
    {
        public async Task<ActionResult> LawReg()
        {
            RestClient<LawRegFormOpen> client = new RestClient<LawRegFormOpen>(Request.GetOwinContext());
            LawRegFormOpen formData = await client.GetSingleItemRequest("LawReg/GetFormOpenData");
            return View(formData);
        }

        public async Task<ActionResult> LAW_newRequest()
        {
            RestClient<LawRegFormOpen> client = new RestClient<LawRegFormOpen>(Request.GetOwinContext());
            LawRegFormOpen formData = await client.GetSingleItemRequest("LawReg/GetFormOpenData");
            formData.caseYear = DateTime.Now.Year;
            return Json(formData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SaveLawCaseRequest(LawCaseReqSaveRequest reqData)
        {
            RestClient<LawCaseReqSaveResult> client = new RestClient<LawCaseReqSaveResult>(Request.GetOwinContext());
            LawCaseReqSaveResult lawSaveResult = await client.PostRequest("LawReg/SaveLawCaseRequest", reqData);
            return Json(lawSaveResult);
        }

        public async Task<ActionResult> law_GetCaseInfoForReq(int caseYear, int caseSrl)
        {
            RestClient<LawCaseRequestRet> client = new RestClient<LawCaseRequestRet>(Request.GetOwinContext());
            LawCaseRequestRet formData = await client.PostRequest("LawReg/GetLawCaseInfoForReq", new int[] { caseYear, caseSrl });
            return Json(formData);
        }

        //[HttpPost]
        public async Task<ActionResult> law_SearchEmp(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(string.Empty);

            RestClient<EmpInfoForLookup[]> client = new RestClient<EmpInfoForLookup[]>(Request.GetOwinContext());

            EmpInfoForLookup[] employees = await client.PostRequest("LawReg/SearchEmpByCodeOrName", q);

            return Json(employees);
        }

        public async Task<ActionResult> law_GetPrevCases()
        {
            RestClient<SearchResultRecord[]> client = new RestClient<SearchResultRecord[]>(Request.GetOwinContext());

            SearchResultRecord[] Cases = await client.GetSingleItemRequest("LawReg/GetPrevCases");

            var prvCases = from Case in Cases
                           select new
                           {
                               id = Case.CASE_SERIAL + "/"  + Case.CASE_YEAR,
                               cAndt = Case.CASE_YEAR + " - " + Case.CASE_SERIAL + " - " +  Case.CASE_NO,
                               cSerial = Case.CASE_SERIAL,
                               CaseNo = Case.CASE_NO,
                               CaseYr = Case.CASE_YEAR
                           };

            return Json(prvCases, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> law_GetPrevCaseBySerial(int caseSrl, int caseYr)
        {
            RestClient<string[]> client = new RestClient<string[]>(Request.GetOwinContext());
            string[] caseObj = await client.PostRequest("LawReg/GetPrevCaseBySerial", new int[] { caseSrl, caseYr });

            return Json(caseObj);
        }
    }
}
using GRP.Models.LAW.Register.LawRegSearch;
using GRP.Web.Controllers;
using GRP.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GRP.Web.Areas.LAW.Controllers
{
    [Authorize]
    public partial class RegisterController : BaseController
    {
        public ActionResult SearchForm(string wndId, string wndTitle, string SrchGridSelect)
        {
            ViewData["wndId"] = wndId;
            ViewData["wndTitle"] = wndTitle;
            ViewData["SrchGridSelect"] = SrchGridSelect;

            return PartialView();
        }

        public async Task<ActionResult> LoadSearchFormLookups()
        {
            RestClient<SearchLookups> client = new RestClient<SearchLookups>(Request.GetOwinContext());
            SearchLookups lookUpData = await client.GetSingleItemRequest("LawRegSearch/GetFormOpenData");

            return Json(lookUpData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SearchForCases(SearchParams srchParams)
        {
            if (srchParams.loadManual)
            {
                RestClient<SearchResult> client = new RestClient<SearchResult>(Request.GetOwinContext());
                SearchResult srchResult = await client.PostRequest("LawRegSearch/SearchForCases", srchParams);

                return Json(srchResult);
            }
            else
            {
                return Json(null);
            }
        }
    }
}
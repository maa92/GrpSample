using GRP.API.AuthProviders;
using GRP.API.Common;
using GRP.DataAccess.Handlers.LAW;
using GRP.Models.LAW.Register.LawRegSearch;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace GRP.API.Controllers.LAW
{
    [GRPAuthorize(Roles = "Admin")]
    public class LawRegSearchController : BaseController
    {

        HijriCalendar hjCal = new HijriCalendar();

        [HttpGet]
        public IHttpActionResult GetFormOpenData()
        {
            LawRegSearchDH dah = new LawRegSearchDH(Environment);
            SearchLookups retVal = dah.GetFormOpenData(Convert.ToDecimal(UserId));
            retVal.GrgYear = DateTime.Now.Year;
            retVal.hijriYear = hjCal.GetYear(DateTime.Now);

            return Ok(retVal);
        }

        [HttpPost]
        public IHttpActionResult SearchForCases(SearchParams srchParams)
        {
            LawRegSearchDH dah = new LawRegSearchDH(Environment);
            SearchResult retVal = dah.SearchForCases(srchParams, Convert.ToDecimal(UserId));

            return Ok(retVal);
        }
    }
}
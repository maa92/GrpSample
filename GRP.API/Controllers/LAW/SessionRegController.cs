using GRP.API.AuthProviders;
using GRP.API.Common;
using GRP.DataAccess.Handlers.LAW;
using GRP.Models.LAW.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace GRP.API.Controllers.LAW
{
    [GRPAuthorize(Roles = "Admin")]
    public class SessionRegController : BaseController
    {
        [HttpGet]
        public IHttpActionResult GetFormOpenData()
        {
            SessionRegDH dah = new SessionRegDH(Environment);
            SessionRegFormOpen retVal = dah.SessionRegFormOpen(Convert.ToDecimal(UserId));

            return Ok(retVal);
        }

        [HttpPost]
        public IHttpActionResult SaveNewLawSessionRequest(LawSessionReqSaveRequest sessionReq)
        {
            SessionRegDH dah = new SessionRegDH(Environment);
            LawSessionReqSaveResult retVal = dah.SaveNewSessionRequest(sessionReq, Convert.ToDecimal(UserId));

            return Ok(retVal);
        }

        [HttpPost]
        public IHttpActionResult SaveSessionFile(LawSessionAttachments sessionFile)
        {
            SessionRegDH dah = new SessionRegDH(Environment);
            LawSessionFilesSaveResult retVal = dah.SaveSessionFile(sessionFile, Convert.ToDecimal(UserId));

            return Ok(retVal);
        }
    }
}
using GrpSample.API.AuthProviders;
using GrpSample.API.Common;
using GrpSample.DataAccess.Handlers.LAW;
using GrpSample.Models.LAW.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace GrpSample.API.Controllers.LAW
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
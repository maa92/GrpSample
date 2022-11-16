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
    public class SessionDetailsController : BaseController
    {
        [HttpPost]
        public IHttpActionResult GetSessionInfo([FromBody]string[] SessionInfo)
        {
            SessionDetailsDH dah = new SessionDetailsDH(Environment);
            SessionDetailsRet retVal = dah.GetSessionInfo(SessionInfo[0], SessionInfo[1], SessionInfo[2], Convert.ToDecimal(UserId));

            return Ok(retVal);
        }

        [HttpPost]
        public IHttpActionResult SaveSessionFileDet(LawSessionAttachments sessionFile)
        {
            SessionDetailsDH dah = new SessionDetailsDH(Environment);
            LawSessionFilesSaveResult retVal = dah.SaveSessionFileDet(sessionFile, Convert.ToDecimal(UserId));

            return Ok(retVal);
        }
    }
}
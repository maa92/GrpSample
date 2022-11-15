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
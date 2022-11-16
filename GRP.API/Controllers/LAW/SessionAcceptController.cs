using GrpSample.API.AuthProviders;
using GrpSample.API.Common;
using GrpSample.DataAccess.Handlers.LAW;
using GrpSample.Models.LAW.Register.SessionAccept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace GrpSample.API.Controllers.LAW
{
    [GRPAuthorize(Roles = "Admin")]
    public class SessionAcceptController : BaseController
    {
        [HttpGet]
        public IHttpActionResult GetFormOpenData()
        {
            SessionAcceptDH dah = new SessionAcceptDH(Environment);
            List<SessionRegForAccept> retVal = dah.GetFormOpenData(Convert.ToDecimal(UserId), null, null, null);

            return Ok(retVal);
        }

        [HttpPost]
        public IHttpActionResult GetFormOpenSessionData([FromBody]int[] SessionInfo)
        {
            SessionAcceptDH dah = new SessionAcceptDH(Environment);
            List<SessionRegForAccept> retVal = dah.GetFormOpenData(Convert.ToDecimal(UserId), SessionInfo[0], SessionInfo[1], SessionInfo[2]);

            return Ok(retVal);
        }

        [HttpPost]
        public IHttpActionResult SaveSessionAcceptReq(SessionRegForAccept[] sessionData)
        {
            SessionAcceptDH dah = new SessionAcceptDH(Environment);
            SessionAcceptSaveResult retVal = dah.SaveSessionAccept(sessionData, Convert.ToDecimal(UserId));

            return Ok(retVal);
        }
    }
}
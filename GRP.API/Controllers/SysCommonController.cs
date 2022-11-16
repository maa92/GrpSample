using GrpSample.API.AuthProviders;
using GrpSample.API.Common;
using GrpSample.DataAccess;
using GrpSample.DataAccess.Handlers.SysCommon;
using GrpSample.Models.SysCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace GrpSample.API.Controllers
{
    [GRPAuthorize(Roles = "Admin")]
    public class SysCommonController : BaseController
    {
        [HttpGet]
        public IHttpActionResult GetBranchesByUser(Decimal Sid,string Filter)
        {
            SysCommonDH sdh = new SysCommonDH(Environment);
            List<BranchInfo> retVal = sdh.GetBranchesByUser(Convert.ToDecimal(UserId),Sid,Filter);
          
            return Ok(retVal);
        }

        [HttpPost]
        public IHttpActionResult GetCentersByBranch([FromBody]Decimal BranchSid)
        {
            SysCommonDH sdh = new SysCommonDH(Environment);
            List<CenterInfo> retVal = sdh.GetCentersByBranch(BranchSid);
            return Ok(retVal);
        }
    }
}

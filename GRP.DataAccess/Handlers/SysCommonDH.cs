using GRP.DataAccess.DAL;
using GRP.DataAccess.DAL.Oracle;
using GRP.DataAccess.Xml;
using GRP.Models.SysCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.DataAccess.Handlers.SysCommon
{
    public class SysCommonDH : BaseDataHandler
    {
        public SysCommonDH(string EnvironmentFlagValue) : base(EnvironmentFlagValue)
        { }

        public List<BranchInfo> GetBranchesByUser(Decimal UserId,Decimal Sid,string Filter)
        {
            List<BranchInfo> retVal = new List<BranchInfo>();

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

            // In Params
            lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));
            lstParams.Add(new OracleDbParameter("I_SRL_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, Sid));
            lstParams.Add(new OracleDbParameter("I_DEPT_FILTER", OracleDataType.Varchar2, OracleDbParameterDirection.Input, Filter));

            // Out Params 
            lstParams.Add(new OracleDbParameter("O_DEPT_LIST_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));

            DALHelper dalh = new DALHelper(ConnectionStringName);
            DataSet dsResultData = dalh.GetDataMultipleResultSet("HR_FRM_INT_GET_BRANCH", OracleCommandType.StoredProcedure, lstParams);

            if (dsResultData.Tables["O_DEPT_LIST_C"].Rows.Count > 0)
                retVal = InstanceMapper<BranchInfo>.CreateList(dsResultData.Tables["O_DEPT_LIST_C"].Rows);
            

            lstParams.Clear();

            return retVal;
        }

        public List<CenterInfo> GetCentersByBranch(Decimal BranchSid)
        {
            List<CenterInfo> retVal = new List<CenterInfo>();

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

            //In Params
            lstParams.Add(new OracleDbParameter("I_SRL_ID", OracleDataType.Varchar2, OracleDbParameterDirection.Input, BranchSid));

            //Out Params
            lstParams.Add(new OracleDbParameter("O_DEPT_LIST_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));

            DALHelper dalh = new DALHelper(ConnectionStringName);
            DataTable dtResultData = dalh.GetDataResultSet("HR_FRM_INT_GET_CENTER", OracleCommandType.StoredProcedure, lstParams);

            if (dtResultData.Rows.Count > 0)
                retVal = InstanceMapper<CenterInfo>.CreateList(dtResultData.Rows);

            lstParams.Clear();

            return retVal;
        }
    }
}

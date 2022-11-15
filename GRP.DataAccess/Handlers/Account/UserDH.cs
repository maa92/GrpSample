using GRP.Models.Account;
using GRP.DataAccess.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using GRP.DataAccess.DAL.Oracle;
using System.Data;

namespace GRP.DataAccess.Handlers.Account
{
    public class UserDH : BaseDataHandler
    {
        public UserDH(string EnvironmentFlagValue) : base(EnvironmentFlagValue)
        { }

        public User AuthenticateUser(string UserName,string AdminUserId, string PCIP,string PCName)
        {
            User retVal = new User();

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

            // In Params
            lstParams.Add(new OracleDbParameter("I_AD_USERNAME", OracleDataType.Varchar2, OracleDbParameterDirection.Input, UserName));
            lstParams.Add(new OracleDbParameter("I_PC_IP", OracleDataType.Varchar2, OracleDbParameterDirection.Input, PCIP));
            lstParams.Add(new OracleDbParameter("I_PC_NAME", OracleDataType.Varchar2, OracleDbParameterDirection.Input, PCName));
            lstParams.Add(new OracleDbParameter("I_ADMIN_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, AdminUserId));

            // Out Params
            lstParams.Add(new OracleDbParameter("O_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_USER_NAME", OracleDataType.Varchar2, 500, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_USER_DEPT", OracleDataType.Varchar2, 500, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("LOGIN_PRV", OracleDataType.Varchar2, 10, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_LOGIN_SRL", OracleDataType.Decimal, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_USER_TYPE", OracleDataType.Varchar2, 10, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_ERR_MSG", OracleDataType.Varchar2, 500, OracleDbParameterDirection.Output));

            DALHelper dalh = new DALHelper(ConnectionStringName);
            DataTable dtData = dalh.GetDataResultSet("PRV_USER_LOGIN", OracleCommandType.StoredProcedure, lstParams);
            
            retVal.userId = lstParams.First(prm => prm.Name == "O_USER_ID").Value.ToString().Replace("null", string.Empty);
            retVal.userFullName = lstParams.First(prm => prm.Name == "O_USER_NAME").Value.ToString().Replace("null", string.Empty);
            retVal.userDeptName = lstParams.First(prm => prm.Name == "O_USER_DEPT").Value.ToString().Replace("null", string.Empty);
            retVal.userLoginFlag = lstParams.First(prm => prm.Name == "LOGIN_PRV").Value.ToString().Replace("null", string.Empty);
            retVal.loginSerial = lstParams.First(prm => prm.Name == "O_LOGIN_SRL").Value.ToString().Replace("null", string.Empty);
            retVal.userType = lstParams.First(prm => prm.Name == "O_USER_TYPE").Value.ToString().Replace("null", string.Empty);
            retVal.loginMessage = lstParams.First(prm => prm.Name == "O_ERR_MSG").Value.ToString().Replace("null", string.Empty);

            lstParams.Clear();

            return retVal;
        }

        public string LogoutUser(string UserId, string LoginSerialId)
        {
            string retVal = string.Empty;

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

            // In Params
            lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, Convert.ToInt32(UserId)));
            lstParams.Add(new OracleDbParameter("I_LOGIN_SRL", OracleDataType.Decimal, OracleDbParameterDirection.Input, Convert.ToInt32(LoginSerialId)));
            lstParams.Add(new OracleDbParameter("O_AFFECTED_ROWS", OracleDataType.Decimal, OracleDbParameterDirection.Output));

            DALHelper dalh = new DALHelper(ConnectionStringName);
            DataTable dt = dalh.GetDataResultSet("PRV_USER_LOGOUT", OracleCommandType.StoredProcedure, lstParams);

            retVal = lstParams.First(prm => prm.Name == "O_AFFECTED_ROWS").Value.ToString().Replace("null", string.Empty);
            lstParams.Clear();

            return retVal;
        }
    }
}

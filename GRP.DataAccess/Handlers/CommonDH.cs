using GRP.DataAccess.DAL;
using GRP.DataAccess.DAL.Oracle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.DataAccess.Handlers
{
    public class CommonDH : BaseDataHandler
    {
        public CommonDH(string EnvironmentFlagValue) : base(EnvironmentFlagValue)
        { }

        public string GetMessageText(int MessageCode)
        {
            string retVal = string.Empty;

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

            // In Params
            lstParams.Add(new OracleDbParameter("I_MSG_ID", OracleDataType.Varchar2, OracleDbParameterDirection.Input, MessageCode));

            DALHelper dalh = new DALHelper(ConnectionStringName);
            object msg = dalh.GetScalarValueByFunction("GET_MESSAGE", OracleDataType.Varchar2, 500, lstParams);

            if (msg != null)
                retVal = msg.ToString();

            lstParams.Clear();

            return retVal;
        }
    }
}

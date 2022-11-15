using GRP.DataAccess.DAL;
using GRP.DataAccess.DAL.Oracle;
using GRP.Models.System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GRP.DataAccess.Handlers
{
    public class SystemDH : BaseDataHandler
    {
        public SystemDH(string EnvironmentFlagValue) : base(EnvironmentFlagValue)
        { }

        public SysSettings GetSysSettings()
        {
            SysSettings retVal = new SysSettings();

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

            // Out Params
            lstParams.Add(new OracleDbParameter("O_DATA", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_HIJRI_YEARS", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_CURRENT_HDATE", OracleDataType.Varchar2, 20, OracleDbParameterDirection.Output));

            DALHelper dalh = new DALHelper(ConnectionStringName);
            DataSet dsSettings = dalh.GetDataMultipleResultSet("SYS_GET_GENERAL_SETUP", OracleCommandType.StoredProcedure, lstParams);

            retVal = InstanceMapper<SysSettings>.Create(dsSettings.Tables["O_DATA"].Rows[0]);
            retVal.current_hDate = lstParams.First(prm => prm.Name == "O_CURRENT_HDATE").Value.ToString().Replace("null", string.Empty);
            retVal.hijriYears = InstanceMapper<Models.SysCommon.HijriYear>.CreateList(dsSettings.Tables["O_HIJRI_YEARS"].Rows);

            lstParams.Clear();

            return retVal;
        }

        public List<SysForm> GetUserSystems(Decimal UserId)
        {
            List<SysForm> retVal = new List<SysForm>();

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

            // In Params
            lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));

            // Out Params
            lstParams.Add(new OracleDbParameter("O_USER_SYS_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));

            DALHelper dalh = new DALHelper(ConnectionStringName);
            DataTable dtUserSystemsData = dalh.GetDataResultSet("PRV_GET_USER_SYS", OracleCommandType.StoredProcedure, lstParams);

            retVal = InstanceMapper<SysForm>.CreateList(dtUserSystemsData.Rows);

            lstParams.Clear();

            return retVal;
        }

        public List<NotificationDb> GetUserNotifications(Decimal UserId)
        {
            List<NotificationDb> retVal = new List<NotificationDb>();

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

            // In Params
            lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));

            // Out Params
            lstParams.Add(new OracleDbParameter("O_NOTIFY_DATA", OracleDataType.RefCursor, OracleDbParameterDirection.Output));

            DALHelper dalh = new DALHelper(ConnectionStringName);
            DataTable dtUserNotifications = dalh.GetDataResultSet("SYS_USER_NOTIFICATION", OracleCommandType.StoredProcedure, lstParams);

            retVal = InstanceMapper<NotificationDb>.CreateList(dtUserNotifications.Rows);

            lstParams.Clear();

            return retVal;
        }
    }
}

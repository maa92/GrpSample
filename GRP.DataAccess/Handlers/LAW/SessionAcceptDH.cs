using GrpSample.DataAccess.DAL;
using GrpSample.DataAccess.DAL.Oracle;
using GrpSample.DataAccess.Xml;
using GrpSample.Models.LAW.Register.SessionAccept;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.DataAccess.Handlers.LAW
{
    public class SessionAcceptDH : BaseDataHandler
    {
        public SessionAcceptDH(string EnvironmentFlagValue) : base(EnvironmentFlagValue)
        { }

        public List<SessionRegForAccept> GetFormOpenData(Decimal UserId, int? SessionSerial, int? CaseYear, int? CaseSerial)
        {
            List<SessionRegForAccept> retVal = new List<SessionRegForAccept>();

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

            //Input Params
            lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));
            lstParams.Add(new OracleDbParameter("I_CASE_YEAR", OracleDataType.Decimal, OracleDbParameterDirection.Input, CaseYear == null ? (object)DBNull.Value : CaseYear));
            lstParams.Add(new OracleDbParameter("I_CASE_SERIAL", OracleDataType.Decimal, OracleDbParameterDirection.Input, CaseSerial == null ? (object)DBNull.Value : CaseSerial));
            lstParams.Add(new OracleDbParameter("I_SESSION_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, SessionSerial == null ? (object)DBNull.Value : SessionSerial));

            //Output Params
            lstParams.Add(new OracleDbParameter("O_CASE_SESSION_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_ERR_MSG", OracleDataType.Varchar2, 500, OracleDbParameterDirection.Output));

            DALHelper dalh = new DALHelper(ConnectionStringName);
            DataTable dtResultData = dalh.GetDataResultSet("LAW_FRM_CASE_SESSION_ATND_OUR", OracleCommandType.StoredProcedure, lstParams);


            retVal = InstanceMapper<SessionRegForAccept>.CreateList(dtResultData.Rows);

            lstParams.Clear();

            return retVal;
        }

        public SessionAcceptSaveResult SaveSessionAccept(SessionRegForAccept[] sessionList, Decimal UserId)
        {
            SessionAcceptSaveResult retVal = new SessionAcceptSaveResult();
            try
            {
                List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

                //Input Params
                string xmlData = GetSessionAcceptDataAsXml(sessionList, UserId);
                lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));
                if (string.IsNullOrEmpty(xmlData))
                    lstParams.Add(new OracleDbParameter("I_SESSION_DATA", OracleDataType.XmlType, OracleDbParameterDirection.Input, DBNull.Value));
                else
                    lstParams.Add(new OracleDbParameter("I_SESSION_DATA", OracleDataType.XmlType, OracleDbParameterDirection.Input, xmlData));

                //Output Params
                lstParams.Add(new OracleDbParameter("O_AFFECTED_ROWS", OracleDataType.Decimal, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_ERR_MSG", OracleDataType.Varchar2, 500, OracleDbParameterDirection.Output));

                DALHelper dalh = new DALHelper(ConnectionStringName);
                DataTable dtResultSet = dalh.GetDataResultSet("LAW_FRM_CASE_SESSION_ATND_DML", OracleCommandType.StoredProcedure, lstParams);

                string rs = lstParams.First(prm => prm.Name == "O_AFFECTED_ROWS").Value.ToString().Replace("null", string.Empty);
                retVal.res = string.IsNullOrEmpty(rs) ? 0 : Convert.ToInt32(rs);
                retVal.msg = lstParams.First(prm => prm.Name == "O_ERR_MSG").Value.ToString().Replace("null", string.Empty);

                lstParams.Clear();

                return retVal;

            }
            catch (Exception ex)
            {
                retVal.res = 0;
                retVal.msg = ex.Message;
                return retVal;
            }
        }

        private string GetSessionAcceptDataAsXml(SessionRegForAccept[] sessionLst, Decimal UserId)
        {
            string retVal = string.Empty;
            if (sessionLst != null && sessionLst.Length > 0)
            {
                List<XmlNodeData> nodes = new List<XmlNodeData>();
                XmlNodeData node = null;

                foreach (SessionRegForAccept session in sessionLst)
                {
                    node = new XmlNodeData("APP");
                    node.Attributes.Add("DML_FLAG", session.df);
                    node.Attributes.Add("CASE_SERIAL", session.CASE_SERIAL);
                    node.Attributes.Add("CASE_YEAR", session.CASE_YEAR);
                    node.Attributes.Add("SESSION_ID", session.SESSION_ID);
                    node.Attributes.Add("EMP_CODE", session.EMP_CODE);
                    node.Attributes.Add("IS_ATTEND", session.IS_ATTEND);
                    node.Attributes.Add("SESSION_NOTE", session.SESSION_NOTE);
                    node.Attributes.Add("USER_ID", UserId.ToString());

                    nodes.Add(node);
                }
                retVal = XmlData.CreateXmlDocument("ROOT", nodes);
            }

            return retVal;
        }


    }
}

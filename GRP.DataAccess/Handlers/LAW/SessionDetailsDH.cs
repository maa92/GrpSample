using GrpSample.DataAccess.DAL;
using GrpSample.DataAccess.DAL.Oracle;
using GrpSample.DataAccess.Xml;
using GrpSample.Models.LAW.Register;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.DataAccess.Handlers.LAW
{
    public class SessionDetailsDH : BaseDataHandler
    {
        public SessionDetailsDH(string EnvironmentFlagValue) : base(EnvironmentFlagValue)
        { }

        public SessionDetailsRet GetSessionInfo(string CaseYear, string CaseSerial, string SessionId, Decimal UserId)
        {
            SessionDetailsRet retVal = new SessionDetailsRet();

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();
            try
            {
                //Input Params
                lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));
                lstParams.Add(new OracleDbParameter("I_CASE_YEAR", OracleDataType.Decimal, OracleDbParameterDirection.Input, CaseYear));
                lstParams.Add(new OracleDbParameter("I_CASE_SERIAL", OracleDataType.Decimal, OracleDbParameterDirection.Input, CaseSerial));
                lstParams.Add(new OracleDbParameter("I_SESSION_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, SessionId));


                //Output Params
                lstParams.Add(new OracleDbParameter("O_CASE_SESSION_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_EMP_SESSION_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_SESSION_ATTACH_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_ERR_MSG", OracleDataType.Varchar2, 1000, OracleDbParameterDirection.Output));

                DALHelper dalh = new DALHelper(ConnectionStringName);
                DataSet dsResultData = dalh.GetDataMultipleResultSet("LAW_FRM_CASE_SESSION_RET_DATA", OracleCommandType.StoredProcedure, lstParams);

                retVal.LawSessionReqDet = InstanceMapper<LawSessionDetailsRet>.Create(dsResultData.Tables["O_CASE_SESSION_C"].Rows[0]);
                retVal.LawSessionEmps = InstanceMapper<SessionEmpsRet>.CreateList(dsResultData.Tables["O_EMP_SESSION_C"].Rows);
                retVal.SessionAttachments = InstanceMapper<SessionAttachmentsRet>.CreateList(dsResultData.Tables["O_SESSION_ATTACH_C"].Rows);
                retVal.errMsg = lstParams.First(prm => prm.Name == "O_ERR_MSG").Value.ToString().Replace("null", string.Empty);

                lstParams.Clear();

                return retVal;

            }
            catch (Exception ex)
            {
                retVal.errMsg = ex.Message;
            }

            return retVal;
        }

        public LawSessionFilesSaveResult SaveSessionFileDet(LawSessionAttachments sessionFile, Decimal UserId)
        {
            LawSessionFilesSaveResult retVal = new LawSessionFilesSaveResult();
            try
            {
                List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

                string xmlData = GetSessionDetAttachmentsDataAsXml(sessionFile.SessionAttach);

                //Input Params
                lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));
                lstParams.Add(new OracleDbParameter("I_CASE_YEAR", OracleDataType.Decimal, OracleDbParameterDirection.Input, sessionFile.CaseYear));
                lstParams.Add(new OracleDbParameter("I_CASE_SERIAL", OracleDataType.Decimal, OracleDbParameterDirection.Input, sessionFile.CaseSerial));
                lstParams.Add(new OracleDbParameter("I_SESSION_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, sessionFile.SessionId));
                lstParams.Add(new OracleDbParameter("I_ATTACH_FILE", OracleDataType.Blob, OracleDbParameterDirection.Input, sessionFile.AttachFile));
                lstParams.Add(new OracleDbParameter("I_ATTACH_TYPE", OracleDataType.Decimal, OracleDbParameterDirection.Input, sessionFile.AttachType));
                if (string.IsNullOrEmpty(xmlData))
                    lstParams.Add(new OracleDbParameter("I_SESSION_ATTACH", OracleDataType.XmlType, OracleDbParameterDirection.Input, DBNull.Value));
                else
                    lstParams.Add(new OracleDbParameter("I_SESSION_ATTACH", OracleDataType.XmlType, OracleDbParameterDirection.Input, xmlData));

                //Output Params
                lstParams.Add(new OracleDbParameter("O_ATTACH_ID", OracleDataType.Decimal, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_AFFECTED_ROWS", OracleDataType.Decimal, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_ERR_MSG", OracleDataType.Varchar2, 500, OracleDbParameterDirection.Output));

                DALHelper dalh = new DALHelper(ConnectionStringName);
                DataTable dtResultData = dalh.GetDataResultSet("LAW_FRM_SESSION_ADD_FILE", OracleCommandType.StoredProcedure, lstParams);

                retVal.AttachmentId = lstParams.First(prm => prm.Name == "O_ATTACH_ID").Value.ToString().Replace("null", string.Empty);
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

        private string GetSessionDetAttachmentsDataAsXml(SessionAttachment sessionAtt)
        {
            string retVal = string.Empty;
            if (sessionAtt != null)
            {
                List<XmlNodeData> nodes = new List<XmlNodeData>();
                XmlNodeData node = null;

                node = new XmlNodeData("ATCHDATA");
                node.Attributes.Add("DF", sessionAtt.DF);
                node.Attributes.Add("ATTACH_ID", sessionAtt.ATTACH_ID);
                node.Attributes.Add("ATTACH_DATE", sessionAtt.ATTACH_DATE);
                node.Attributes.Add("ATTACH_FILE_NAME", sessionAtt.ATTACH_FILE_NAME);
                node.Attributes.Add("ATTACH_FILE_TYPE", sessionAtt.ATTACH_FILE_TYPE);
                node.Attributes.Add("ATTACH_NOTES", sessionAtt.ATTACH_NOTES);
                nodes.Add(node);

                retVal = XmlData.CreateXmlDocument("ROOT", nodes);
            }

            return retVal;
        }
    }
}

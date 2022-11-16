using GrpSample.DataAccess.DAL;
using GrpSample.DataAccess.DAL.Oracle;
using GrpSample.DataAccess.Xml;
using GrpSample.Models.LAW.Register;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace GrpSample.DataAccess.Handlers.LAW
{
    public class SessionRegDH : BaseDataHandler
    {
        public SessionRegDH(string EnvironmentFlagValue) : base(EnvironmentFlagValue)
        { }

        public SessionRegFormOpen SessionRegFormOpen(Decimal UserId)
        {
            SessionRegFormOpen retVal = new SessionRegFormOpen();

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

            //Input Params
            lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));

            //Output Params
            lstParams.Add(new OracleDbParameter("O_SESSION_STATUS_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_ATTACH_TYPE_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));

            DALHelper dalh = new DALHelper(ConnectionStringName);
            DataSet dsResulSet = dalh.GetDataMultipleResultSet("LAW_FRM_CASE_SESSION_OPEN", OracleCommandType.StoredProcedure, lstParams);

            retVal.sStatus = InstanceMapper<SessionStatus>.CreateList(dsResulSet.Tables["O_SESSION_STATUS_C"].Rows);
            retVal.aType = InstanceMapper<AttachType>.CreateList(dsResulSet.Tables["O_ATTACH_TYPE_C"].Rows);

            lstParams.Clear();

            return retVal;
        }

        public LawSessionReqSaveResult SaveNewSessionRequest(LawSessionReqSaveRequest sessionReq, Decimal UserId)
        {
            LawSessionReqSaveResult retVal = new LawSessionReqSaveResult();
            try
            {
                List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

                string xmlData = GetSessionMasterDataAsXml(sessionReq.LawCaseReqDet);

                //Input Params
                lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));
                lstParams.Add(new OracleDbParameter("I_CASE_YEAR", OracleDataType.Decimal, OracleDbParameterDirection.Input, sessionReq.LawCaseReqDet.CASE_YEAR));
                lstParams.Add(new OracleDbParameter("I_CASE_SERIAL", OracleDataType.Decimal, OracleDbParameterDirection.Input, sessionReq.LawCaseReqDet.CASE_SERIAL));
                //lstParams.Add(new OracleDbParameter("I_SESSION_ATT", OracleDataType.Blob, OracleDbParameterDirection.Input, sessionReq.fileBytes));


                if (string.IsNullOrEmpty(xmlData))
                    lstParams.Add(new OracleDbParameter("I_SESSION_DATA", OracleDataType.XmlType, OracleDbParameterDirection.Input, DBNull.Value));
                else
                    lstParams.Add(new OracleDbParameter("I_SESSION_DATA", OracleDataType.XmlType, OracleDbParameterDirection.Input, xmlData));

                xmlData = GetSessionEmpsDataAsXml(sessionReq.LawSessionEmps);
                if (string.IsNullOrEmpty(xmlData))
                    lstParams.Add(new OracleDbParameter("I_EMP_SESSION", OracleDataType.XmlType, OracleDbParameterDirection.Input, DBNull.Value));
                else
                    lstParams.Add(new OracleDbParameter("I_EMP_SESSION", OracleDataType.XmlType, OracleDbParameterDirection.Input, xmlData));

                //xmlData = GetSessionAttachmentsDataAsXml(sessionReq.LawSessionReqAtt);
                //if (string.IsNullOrEmpty(xmlData))
                //    lstParams.Add(new OracleDbParameter("I_SESSION_ATTACH", OracleDataType.XmlType, OracleDbParameterDirection.Input, DBNull.Value));
                //else
                //    lstParams.Add(new OracleDbParameter("I_SESSION_ATTACH", OracleDataType.XmlType, OracleDbParameterDirection.Input, xmlData));

                //Output Params
                lstParams.Add(new OracleDbParameter("O_NEXT_NOTIFY", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_CASE_YEAR", OracleDataType.Decimal, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_CASE_SERIAL", OracleDataType.Decimal, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_SESSION_ID", OracleDataType.Decimal, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_AFFECTED_ROWS", OracleDataType.Decimal, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_ERR_MSG", OracleDataType.Varchar2, 500, OracleDbParameterDirection.Output));

                DALHelper dalh = new DALHelper(ConnectionStringName);
                DataSet dtResultData = dalh.GetDataMultipleResultSet("LAW_FRM_CASE_SESSION_DML", OracleCommandType.StoredProcedure, lstParams);

                if (dtResultData.Tables["O_NEXT_NOTIFY"].Rows.Count > 0)
                {
                    retVal.notifyLst = InstanceMapper<NotifyList>.CreateList(dtResultData.Tables["O_NEXT_NOTIFY"].Rows);
                }
                else
                {
                    retVal.notifyLst = new List<NotifyList>();
                }
                retVal.CaseYear = lstParams.First(prm => prm.Name == "O_CASE_YEAR").Value.ToString().Replace("null", string.Empty);
                retVal.CaseSerial = lstParams.First(prm => prm.Name == "O_CASE_SERIAL").Value.ToString().Replace("null", string.Empty);
                retVal.SessionSerial = lstParams.First(prm => prm.Name == "O_SESSION_ID").Value.ToString().Replace("null", string.Empty);
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

        public LawSessionFilesSaveResult SaveSessionFile(LawSessionAttachments sessionFile, Decimal UserId)
        {
            LawSessionFilesSaveResult retVal = new LawSessionFilesSaveResult();
            try
            {
                List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

                string xmlData = GetSessionAttachmentsDataAsXml(sessionFile.SessionAttach);

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

        private string GetSessionMasterDataAsXml(LawCaseReqSessionDet sessionDet)
        {
            string retVal = string.Empty;
            if (sessionDet != null)
            {
                List<XmlNodeData> nodes = new List<XmlNodeData>();
                XmlNodeData node = new XmlNodeData("DTLTDATA");

                node.Attributes.Add("DF", sessionDet.DF);
                node.Attributes.Add("CASE_YEAR", sessionDet.CASE_YEAR);
                node.Attributes.Add("CASE_SERIAL", sessionDet.CASE_SERIAL);
                node.Attributes.Add("SESSION_ID", sessionDet.SESSION_ID);
                node.Attributes.Add("SESSION_DATE", sessionDet.SESSION_DATE);
                node.Attributes.Add("SESSION_STATUS", sessionDet.SESSION_STATUS);
                node.Attributes.Add("BRANCH_SRL_ID", sessionDet.BRANCH_SRL_ID);
                //node.Attributes.Add("THE_COURT", sessionDet.THE_COURT);
                node.Attributes.Add("SESSION_DAY", sessionDet.SESSION_DAY);
                node.Attributes.Add("SESSION_TIME", sessionDet.SESSION_TIME);
                node.Attributes.Add("SESSION_NOTES", sessionDet.SESSION_NOTES);
                node.Attributes.Add("IS_ATTEND", sessionDet.IS_ATTEND);

                nodes.Add(node);
                retVal = XmlData.CreateXmlDocument("ROOT", nodes);
            }
            return retVal;
        }

        private string GetSessionEmpsDataAsXml(EmpsInSession[] sessionEmps)
        {
            string retVal = string.Empty;
            if (sessionEmps != null && sessionEmps.Length > 0)
            {
                List<XmlNodeData> nodes = new List<XmlNodeData>();
                XmlNodeData node = null;
                foreach (EmpsInSession emp in sessionEmps)
                {
                    node = new XmlNodeData("EMPDATA");
                    node.Attributes.Add("DF", emp.DF);
                    node.Attributes.Add("SESSION_ID", emp.SESSION_ID);
                    node.Attributes.Add("EMP_CODE", emp.EMP_CODE);
                    node.Attributes.Add("SESSION_NOTE", emp.SESSION_NOTE);
                    nodes.Add(node);
                }
                retVal = XmlData.CreateXmlDocument("ROOT", nodes);
            }
            return retVal;
        }

        private string GetSessionAttachmentsDataAsXml(SessionAttachment sessionAtt)
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

        //private string CreateXmlDoc(LawSessionReqAttachments[] sessionAtt)
        //{
        //    string retVal = string.Empty;
        //    XmlDocument doc = new XmlDocument();
        //    XmlElement root = doc.CreateElement("ROOT");
        //    List<XmlNodeData> nodes = new List<XmlNodeData>();
        //    XmlNodeData node = null;
        //    foreach (LawSessionReqAttachments Att in sessionAtt)
        //    {
        //        node = new XmlNodeData("ATCHDATA");
        //        node.Attributes.Add("DF", Att.DF);
        //        node.Attributes.Add("SESSION_ID", Att.SESSION_ID.ToString());
        //        node.Attributes.Add("ATTACH_ID", Att.ATTACH_ID.ToString());
        //        node.Attributes.Add("ATTACH_DATE", Att.ATTACH_DATE);
        //        //node.Attributes.Add("ATTACH_FILE", Convert.ToBase64String(Att.ATTACH_FILE));
        //        node.byteAttributes.Add("ATTACH_FILE", Att.ATTACH_FILE);  //when retrieving file use Convert.FromBase64String(string) / Convert.ToBase64String(Att.ATTACH_FILE)
        //        node.Attributes.Add("ATTACH_NOTES", Att.ATTACH_NOTES);
        //        nodes.Add(node);

        //        XmlSerializer serializer = new XmlSerializer(node.GetType());
        //        MemoryStream stream = new MemoryStream();
        //        serializer.Serialize(stream, node);
        //        stream.Position = 0;
        //        doc.Load(stream);
        //        retVal = doc.OuterXml;
        //        //return retVal;
        //    }

        //    return retVal;
        //}
    }
}

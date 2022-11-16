using GrpSample.DataAccess.DAL;
using GrpSample.DataAccess.DAL.Oracle;
using GrpSample.DataAccess.Xml;
using GrpSample.Models.HR;
using GrpSample.Models.LAW.Register;
using GrpSample.Models.LAW.Register.LawRegSearch;
using GrpSample.Models.LAW.Register;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.DataAccess.Handlers.LAW
{
    public class LawRegDH : BaseDataHandler
    {
        public LawRegDH(string EnvironmentFlagValue) : base(EnvironmentFlagValue)
        { }

        public LawRegFormOpen GetFormOpenData(Decimal UserId)
        {
            LawRegFormOpen retVal = new LawRegFormOpen();

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

            //Input Params
            lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));

            //Output Params
            lstParams.Add(new OracleDbParameter("O_BRANCH_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_CASE_STATUS_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_JDGMNT_STATUS_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_DFNDT_TYPE_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_COURT_GNAME_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_COURT_JNAME_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_CASE_CITYS_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));

            DALHelper dalh = new DALHelper(ConnectionStringName);
            DataSet dsResultData = dalh.GetDataMultipleResultSet("LAW_FRM_CASE_REQ_OPEN", OracleCommandType.StoredProcedure, lstParams);

            retVal.branches = InstanceMapper<Branches>.CreateList(dsResultData.Tables["O_BRANCH_C"].Rows);
            retVal.caseStatus = InstanceMapper<CaseStatus>.CreateList(dsResultData.Tables["O_CASE_STATUS_C"].Rows);
            retVal.JdgmntStatus = InstanceMapper<JudgmentStatus>.CreateList(dsResultData.Tables["O_JDGMNT_STATUS_C"].Rows);
            retVal.DfndtType = InstanceMapper<DefendantType>.CreateList(dsResultData.Tables["O_DFNDT_TYPE_C"].Rows);
            retVal.GCourtNames = InstanceMapper<GCourtNames>.CreateList(dsResultData.Tables["O_COURT_GNAME_C"].Rows);
            retVal.JCourtNames = InstanceMapper<JCourtNames>.CreateList(dsResultData.Tables["O_COURT_JNAME_C"].Rows);
            retVal.CourtCity = InstanceMapper<CourtCity>.CreateList(dsResultData.Tables["O_CASE_CITYS_C"].Rows);

            lstParams.Clear();

            return retVal;
        }

        public LawCaseRequestRet GetLawCaseForNewRequest(int? caseYear, int? caseSrl, Decimal UserId)
        {
            LawCaseRequestRet retVal = new LawCaseRequestRet();
            try
            {
                List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

                //Input Params
                lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));
                //lstParams.Add(new OracleDbParameter("I_EMP_CODE", OracleDataType.Int64, OracleDbParameterDirection.Input, ReqParams.EmpCode));
                lstParams.Add(new OracleDbParameter("I_CASE_YEAR", OracleDataType.Decimal, OracleDbParameterDirection.Input, caseYear == null ? (object)DBNull.Value : caseYear));
                lstParams.Add(new OracleDbParameter("I_CASE_SERIAL", OracleDataType.Decimal, OracleDbParameterDirection.Input, caseSrl == null ? (object)DBNull.Value : caseSrl));

                //Output Params
                lstParams.Add(new OracleDbParameter("O_CASE_MASTER_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_CASE_DETAIL_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
                //lstParams.Add(new OracleDbParameter("O_CASE_ATTACH_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_ERR_MSG", OracleDataType.Varchar2, 500, OracleDbParameterDirection.Output));

                DALHelper dalh = new DALHelper(ConnectionStringName);
                DataSet dsResultSet = dalh.GetDataMultipleResultSet("LAW_FRM_CASE_REQ_QUR", OracleCommandType.StoredProcedure, lstParams);

                retVal.LawReqMstr = InstanceMapper<CaseRequestInfo>.Create(dsResultSet.Tables["O_CASE_MASTER_C"].Rows[0]);
                retVal.LawReqDetail = InstanceMapper<CaseRequestDet>.CreateList(dsResultSet.Tables["O_CASE_DETAIL_C"].Rows);


                retVal.errMsg = lstParams.First(prm => prm.Name == "O_ERR_MSG").Value.ToString().Replace("null", string.Empty);

                lstParams.Clear();

            }
            catch (Exception ex)
            {
                retVal.errMsg = string.Format("خطأ أثناء استرجاع البيانات <br/> {0}", ex.Message);
            }
            return retVal;
        }

        public LawCaseReqSaveResult SaveLawCaseRequest(LawCaseReqSaveRequest saveReq, Decimal UserId)
        {
            LawCaseReqSaveResult retVal = new LawCaseReqSaveResult();
            try
            {
                List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

                string xmlData = GetLawCaseMasterDataAsXml(saveReq.LawCaseReqMstr);

                //Input Params
                lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));
                if (string.IsNullOrEmpty(xmlData))
                    lstParams.Add(new OracleDbParameter("I_MASTER_DATA", OracleDataType.XmlType, OracleDbParameterDirection.Input, DBNull.Value));
                else
                    lstParams.Add(new OracleDbParameter("I_MASTER_DATA", OracleDataType.XmlType, OracleDbParameterDirection.Input, xmlData));

                //Output Params
                lstParams.Add(new OracleDbParameter("O_CASE_YEAR", OracleDataType.Decimal, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_CASE_SERIAL", OracleDataType.Decimal, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_AFFECTED_ROWS", OracleDataType.Decimal, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_ERR_MSG", OracleDataType.Varchar2, 500, OracleDbParameterDirection.Output));

                DALHelper dalh = new DALHelper(ConnectionStringName);
                DataTable dtResultData = dalh.GetDataResultSet("LAW_FRM_CASE_REQ_DML", OracleCommandType.StoredProcedure, lstParams);

                retVal.CaseYear = lstParams.First(prm => prm.Name == "O_CASE_YEAR").Value.ToString().Replace("null", string.Empty);
                retVal.CaseSerial = lstParams.First(prm => prm.Name == "O_CASE_SERIAL").Value.ToString().Replace("null", string.Empty);
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

        public List<EmpInfoForLookup> SearchEmpByCodeOrName(Decimal UserId, String SearchFilter)
        {
            List<EmpInfoForLookup> retVal = new List<EmpInfoForLookup>();

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

            // In Params
            lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));
            lstParams.Add(new OracleDbParameter("I_EMP_FILTER", OracleDataType.Varchar2, OracleDbParameterDirection.Input, SearchFilter));

            // Out Params 
            lstParams.Add(new OracleDbParameter("O_EMP_LIST_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));

            DALHelper dalh = new DALHelper(ConnectionStringName);
            // var stopwatch = new Stopwatch();
            // stopwatch.Start();
            DataTable dtResultData = dalh.GetDataResultSet("LAW_FRM_VR_EMP_RET", OracleCommandType.StoredProcedure, lstParams);
            // stopwatch.Stop();
            // var psed_time = (stopwatch.ElapsedMilliseconds / 1000) / 60;
            retVal = InstanceMapper<EmpInfoForLookup>.CreateList(dtResultData.Rows);

            lstParams.Clear();

            return retVal;
        }

        public List<SearchResultRecord> GetPrevCases()
        {
            List<SearchResultRecord> retVal = new List<SearchResultRecord>();
            try
            {

                List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

                // Out Params
                lstParams.Add(new OracleDbParameter("O_PREV_CASES", OracleDataType.RefCursor, OracleDbParameterDirection.Output));

                DALHelper dalh = new DALHelper(ConnectionStringName);
                DataTable dtResultData = dalh.GetDataResultSet("LAW_GET_PREV_CASES", OracleCommandType.StoredProcedure, lstParams);

                retVal = InstanceMapper<SearchResultRecord>.CreateList(dtResultData.Rows);

                lstParams.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retVal;
        }

        public string[] GetPrevCaseBySerial(int caseSrl, int caseYear)
        {
            string[] retVal;

            try
            {
                List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

                //In Params
                lstParams.Add(new OracleDbParameter("I_CASE_SERIAL", OracleDataType.Decimal, OracleDbParameterDirection.Input, caseSrl));
                lstParams.Add(new OracleDbParameter("I_CASE_YEAR", OracleDataType.Decimal, OracleDbParameterDirection.Input, caseYear));

                // Out Params
                lstParams.Add(new OracleDbParameter("O_CASE_INFO", OracleDataType.RefCursor, OracleDbParameterDirection.Output));

                DALHelper dalh = new DALHelper(ConnectionStringName);
                DataTable dtResultData = dalh.GetDataResultSet("LAW_GET_PREV_CASE_BY_SERIAL", OracleCommandType.StoredProcedure, lstParams);

                //retVal = dtResultData.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();
                retVal = new string[] { dtResultData.Rows[0].ItemArray[0].ToString(),
                                        dtResultData.Rows[0].ItemArray[1].ToString(),
                                        dtResultData.Rows[0].ItemArray[2].ToString() };

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retVal;

        }

        private string GetLawCaseMasterDataAsXml(LawCaseReqMasterInfo lawCaseMasterInfo)
        {
            string retVal = string.Empty;
            if (lawCaseMasterInfo != null)
            {
                List<XmlNodeData> nodes = new List<XmlNodeData>();
                XmlNodeData node = new XmlNodeData("MSTRDATA");

                node.Attributes.Add("DF", lawCaseMasterInfo.DF);
                node.Attributes.Add("CASE_YEAR", lawCaseMasterInfo.CASE_YEAR.ToString());
                node.Attributes.Add("CASE_SERIAL", lawCaseMasterInfo.CASE_SERIAL.ToString());
                node.Attributes.Add("COURT_TYPE", lawCaseMasterInfo.COURT_TYPE.ToString());
                node.Attributes.Add("CASE_NO", lawCaseMasterInfo.CASE_NO);
                node.Attributes.Add("CASE_NO_PLEA", lawCaseMasterInfo.CASE_NO_PLEA);
                node.Attributes.Add("CASE_NO_HIGH", lawCaseMasterInfo.CASE_NO_HIGH);
                node.Attributes.Add("CASE_DATE", lawCaseMasterInfo.CASE_DATE);
                node.Attributes.Add("CASE_TYPE", lawCaseMasterInfo.CASE_TYPE.ToString());
                node.Attributes.Add("BRANCH_SRL_ID", lawCaseMasterInfo.BRANCH_SRL_ID.ToString());
                node.Attributes.Add("CASE_STATUS", lawCaseMasterInfo.CASE_STATUS.ToString());
                node.Attributes.Add("CASE_SECTOR", lawCaseMasterInfo.CASE_SECTOR.ToString());
                node.Attributes.Add("SECTOR_COURT", lawCaseMasterInfo.SECTOR_COURT.ToString());
                node.Attributes.Add("COURT_CITY", lawCaseMasterInfo.COURT_CITY.ToString());
                node.Attributes.Add("DISTRICT_NAME", lawCaseMasterInfo.DISTRICT_NAME == null ? DBNull.Value.ToString(): lawCaseMasterInfo.DISTRICT_NAME);
                node.Attributes.Add("COURT_OTHERS", lawCaseMasterInfo.COURT_OTHERS);
                node.Attributes.Add("EMP_NO_AUTHORITY", lawCaseMasterInfo.EMP_NO_AUTHORITY);
                node.Attributes.Add("JUDGEMENT_STATUS", lawCaseMasterInfo.JUDGEMENT_STATUS);
                node.Attributes.Add("JUDGMENT_STATUS_OTHERS", lawCaseMasterInfo.JUDGMENT_STATUS_OTHERS == null ? DBNull.Value.ToString() : lawCaseMasterInfo.JUDGMENT_STATUS_OTHERS);
                node.Attributes.Add("DEFENDANT_TYPE", lawCaseMasterInfo.DEFENDANT_TYPE.ToString());
                node.Attributes.Add("DEFENDANT_NAME", lawCaseMasterInfo.DEFENDANT_NAME);
                node.Attributes.Add("CASE_NOTES", lawCaseMasterInfo.CASE_NOTES);
                node.Attributes.Add("CONNECTED_SERIAL", lawCaseMasterInfo.CONNECTED_SERIAL == null ? DBNull.Value.ToString() : lawCaseMasterInfo.CONNECTED_SERIAL);
                node.Attributes.Add("CONNECTED_CASE_YEAR", lawCaseMasterInfo.CONNECTED_CASE_YEAR == null ? DBNull.Value.ToString() : lawCaseMasterInfo.CONNECTED_CASE_YEAR);

                nodes.Add(node);
                retVal = XmlData.CreateXmlDocument("ROOT", nodes);

            }
            return retVal;
        }

    }
}

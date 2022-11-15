using GRP.DataAccess.DAL;
using GRP.DataAccess.DAL.Oracle;
using GRP.DataAccess.Xml;
using GRP.Models.LAW.Register;
using GRP.Models.LAW.Register.LawRegSearch;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.DataAccess.Handlers.LAW
{
    public class LawRegSearchDH : BaseDataHandler
    {
        public LawRegSearchDH(string EnvironmentFlagValue) : base(EnvironmentFlagValue)
        { }

        public SearchLookups GetFormOpenData(Decimal UserId)
        {
            SearchLookups retVal = new SearchLookups();

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
            DataSet dsResulSet = dalh.GetDataMultipleResultSet("LAW_FRM_CASE_SEARCH_OPEN", OracleCommandType.StoredProcedure, lstParams);

            retVal.branches = InstanceMapper<Branches>.CreateList(dsResulSet.Tables["O_BRANCH_C"].Rows);
            retVal.cStatus = InstanceMapper<CaseStatus>.CreateList(dsResulSet.Tables["O_CASE_STATUS_C"].Rows);
            retVal.JStatus = InstanceMapper<JudgmentStatus>.CreateList(dsResulSet.Tables["O_JDGMNT_STATUS_C"].Rows);
            retVal.defntType = InstanceMapper<DefendantType>.CreateList(dsResulSet.Tables["O_DFNDT_TYPE_C"].Rows);
            retVal.GCourts = InstanceMapper<GCourtNames>.CreateList(dsResulSet.Tables["O_COURT_GNAME_C"].Rows);
            retVal.JCourts = InstanceMapper<JCourtNames>.CreateList(dsResulSet.Tables["O_COURT_JNAME_C"].Rows);
            retVal.cCity = InstanceMapper<CourtCity>.CreateList(dsResulSet.Tables["O_CASE_CITYS_C"].Rows);

            lstParams.Clear();

            return retVal;
        }

        public SearchResult SearchForCases(SearchParams srchParams, Decimal UserId)
        {
            SearchResult retVal = new SearchResult();

            List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

            //Input Params
            string xmlData = GetCaseSearchParamsAsXml(srchParams, UserId);
            if (string.IsNullOrEmpty(xmlData))
                lstParams.Add(new OracleDbParameter("I_SEARCH_PARAMS", OracleDataType.XmlType, OracleDbParameterDirection.Input, DBNull.Value));
            else
                lstParams.Add(new OracleDbParameter("I_SEARCH_PARAMS", OracleDataType.XmlType, OracleDbParameterDirection.Input, xmlData));

            //Output Params
            lstParams.Add(new OracleDbParameter("O_CASE_REQ_C", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
            lstParams.Add(new OracleDbParameter("O_ROW_COUNT", OracleDataType.Decimal, OracleDbParameterDirection.Output));

            DALHelper dalh = new DALHelper(ConnectionStringName);
            DataTable dtResultData = dalh.GetDataResultSet("LAW_FRM_CASE_SEARCH_RESULT", OracleCommandType.StoredProcedure, lstParams);

            retVal.rows = InstanceMapper<SearchResultRecord>.CreateList(dtResultData.Rows);
            retVal.total = lstParams.First(prm => prm.Name == "O_ROW_COUNT").Value.ToString().Replace("null", string.Empty);

            lstParams.Clear();

            return retVal;
        }

        private string GetCaseSearchParamsAsXml(SearchParams srchParams, Decimal UserId)
        {
            List<XmlNodeData> nodes = new List<XmlNodeData>();

            XmlNodeData nd = new XmlNodeData("CASE");

            nd.Attributes.Add("USER_ID", UserId.ToString());
            nd.Attributes.Add("CASE_NO", srchParams.CASE_NO);
            nd.Attributes.Add("CASE_DATE", srchParams.CASE_DATE);
            nd.Attributes.Add("CASE_YEAR", srchParams.CASE_YEAR);
            nd.Attributes.Add("CASE_TYPE", srchParams.CASE_TYPE);
            nd.Attributes.Add("CASE_SERIAL", srchParams.CASE_SERIAL);
            nd.Attributes.Add("COURT_TYPE", srchParams.COURT_TYPE);
            nd.Attributes.Add("CASE_STATUS", srchParams.CASE_STATUS);
            nd.Attributes.Add("CASE_SECTOR", srchParams.CASE_SECTOR);
            nd.Attributes.Add("SECTOR_COURT", srchParams.SECTOR_COURT);
            nd.Attributes.Add("COURT_CITY", srchParams.COURT_CITY);
            nd.Attributes.Add("EMP_NO_AUTHORITY", srchParams.EMP_NO_AUTHORITY);
            nd.Attributes.Add("JUDGEMENT_STATUS", srchParams.JUDGEMENT_STATUS);
            nd.Attributes.Add("DEFENDANT_TYPE", srchParams.DEFENDANT_TYPE);
            nd.Attributes.Add("PAGENUMBER", srchParams.page.ToString());
            nd.Attributes.Add("PAGESIZE", srchParams.rows.ToString());

            nodes.Add(nd);

            return XmlData.CreateXmlDocument("ROOT", nodes);
        }
    }
}

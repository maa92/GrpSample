using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace GRP.DataAccess.DAL.Oracle
{
    public enum OracleDbParameterDirection
    {
        Input = 1,
        Output = 2,
        InputOutput = 3,
        ReturnValue = 6,
    }

    public enum OracleDataType
    {
        BFile = 101,
        Blob = 102,
        Byte = 103,
        Char = 104,
        Clob = 105,
        Date = 106,
        Decimal = 107,
        Double = 108,
        Long = 109,
        LongRaw = 110,
        Int16 = 111,
        Int32 = 112,
        Int64 = 113,
        IntervalDS = 114,
        IntervalYM = 115,
        NClob = 116,
        NChar = 117,
        NVarchar2 = 119,
        Raw = 120,
        RefCursor = 121,
        Single = 122,
        TimeStamp = 123,
        TimeStampLTZ = 124,
        TimeStampTZ = 125,
        Varchar2 = 126,
        XmlType = 127,
        Array = 128,
        Object = 129,
        Ref = 130,
        BinaryDouble = 132,
        BinaryFloat = 133,
        Boolean = 134,
    }

    public enum OracleCommandType
    {
        Text = 1,
        StoredProcedure = 4,
        TableDirect = 512,
    }

    public class OracleDbParameter
    {
        public string Name { get; set; }
        public OracleDataType DataType { get; set; }
        public OracleDbParameterDirection Direction { get; set; }
        public int Size { get; set; }
        public object Value { get; set; }

        public OracleDbParameter() { }
        public OracleDbParameter(string ParameterName, OracleDataType DataType, OracleDbParameterDirection ParameterDirection, object ParameterValue)
        {
            this.Name = ParameterName;
            this.DataType = DataType;
            this.Direction = ParameterDirection;
            this.Value = ParameterValue;
        }
        public OracleDbParameter(string ParameterName, OracleDataType DataType, OracleDbParameterDirection ParameterDirection)
        {
            this.Name = ParameterName;
            this.DataType = DataType;
            this.Direction = ParameterDirection;
        }
        public OracleDbParameter(string ParameterName, OracleDataType DataType, int Size, OracleDbParameterDirection ParameterDirection)
        {
            this.Name = ParameterName;
            this.DataType = DataType;
            this.Direction = ParameterDirection;
            this.Size = Size;
        }
        public OracleDbParameter(string ParameterName, OracleDataType DataType, OracleDbParameterDirection ParameterDirection, int Size, object ParameterValue)
        {
            this.Name = ParameterName;
            this.DataType = DataType;
            this.Direction = ParameterDirection;
            this.Size = Size;
            this.Value = ParameterValue;
        }
    }

    public class DALHelper
    {
        private string _connectionStringName = string.Empty;

        public DALHelper(string ConnectionStringName)
        {
            _connectionStringName = ConnectionStringName;
        }        

        // New Implementations.

        public DataSet GetDataMultipleResultSet(string SQLTextOrSPName,OracleCommandType CommandType, List<OracleDbParameter> InAndOutParameters = null)
        {
            DataSet dsRetVal = new DataSet();
            DataTable dtData = null;
            OracleConnection con = null;
            OracleCommand cmd = null;
            OracleDataReader dr = null;

            Dictionary<string, object> dLog = new Dictionary<string, object>();

            try
            {
                con = new OracleConnection(ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString);
                cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandType = (System.Data.CommandType)CommandType;
                cmd.CommandText = SQLTextOrSPName;
                cmd.BindByName = true;

                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    foreach (OracleDbParameter prm in InAndOutParameters)
                    {
                        if (prm.Size == 0)
                            cmd.Parameters.Add(prm.Name, (OracleDbType)prm.DataType, prm.Value, (ParameterDirection)prm.Direction);
                        else
                            cmd.Parameters.Add(prm.Name, (OracleDbType)prm.DataType, prm.Size, prm.Value, (ParameterDirection)prm.Direction);
                    }
                }

                con.Open();
                //OracleDataAdapter adapter = new OracleDataAdapter(cmd);

                // Return the data result set, AND Fill the output Parameters
                //adapter.Fill(dsRetVal);

                cmd.ExecuteNonQuery();

                foreach (OracleDbParameter prm in InAndOutParameters.Where(prm => prm.DataType == OracleDataType.RefCursor && prm.Direction == OracleDbParameterDirection.Output))
                {
                    if (cmd.Parameters[prm.Name].Status == OracleParameterStatus.Success)
                    {
                        dr = ((OracleRefCursor)cmd.Parameters[prm.Name].Value).GetDataReader();
                        if (dr != null)
                        {
                            dtData = new DataTable(prm.Name);
                            dtData.Load(dr);
                            dsRetVal.Tables.Add(dtData);
                        }
                    }
                }

                // Return Output values if exists.
                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    foreach (OracleDbParameter prm in InAndOutParameters.Where(prm => prm.Direction == OracleDbParameterDirection.Output))
                    {
                        prm.Value = cmd.Parameters[prm.Name].Value;
                    }
                }

                return dsRetVal;
            }
            catch (Exception ex)
            {
                dLog.Add("SQLTextOrSPName", SQLTextOrSPName);
                foreach (OracleDbParameter param in InAndOutParameters)
                {
                    dLog.Add(param.Name, param.Value);
                }               
                Utils.LogErrorInFile(".... Method Exception.... : " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, dLog);
                throw ex;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();

                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Dispose();
                }
            }
        }

        public DataTable GetDataResultSet(string SQLTextOrSPName, OracleCommandType CommandType, List<OracleDbParameter> InAndOutParameters = null)
        {
            DataTable dtRetVal = new DataTable();
            OracleConnection con = null;
            OracleCommand cmd = null;

            Dictionary<string, object> dLog = new Dictionary<string, object>();

            try
            {
                con = new OracleConnection(ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString);
                cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandType = (System.Data.CommandType)CommandType;
                cmd.CommandText = SQLTextOrSPName;
                cmd.BindByName = true;

                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    foreach (OracleDbParameter prm in InAndOutParameters)
                    {
                        if (prm.Size == 0)
                            cmd.Parameters.Add(prm.Name, (OracleDbType)prm.DataType, prm.Value, (ParameterDirection)prm.Direction);
                        else
                            cmd.Parameters.Add(prm.Name, (OracleDbType)prm.DataType, prm.Size, prm.Value, (ParameterDirection)prm.Direction);
                    }
                }

                con.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                // Return the data result set.
                dtRetVal.Load(reader);

                // Return Output values if exists.
                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    foreach (OracleDbParameter prm in InAndOutParameters.Where(prm => prm.Direction == OracleDbParameterDirection.Output))
                    {
                        prm.Value = cmd.Parameters[prm.Name].Value;
                    }
                }

                return dtRetVal;
            }
            catch (Exception ex)
            {
                dLog.Add("SQLTextOrSPName", SQLTextOrSPName);
                foreach (OracleDbParameter param in InAndOutParameters)
                {
                    dLog.Add(param.Name, param.Value);
                }
                Utils.LogErrorInFile(".... Method Exception.... : " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, dLog);
                throw ex;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();

                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Dispose();
                }
            }
        }
       
        public object GetScalarValueByFunction(string FunctionName, OracleDataType ReturnValueType, int ReturnValueSize, List<OracleDbParameter> InAndOutParameters = null)
        {
            OracleConnection con = null;
            OracleCommand cmd = null;

            try
            {
                con = new OracleConnection(ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString);
                cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = FunctionName;
                cmd.BindByName = true;

                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    cmd.Parameters.Add("Return_Value", (OracleDbType)ReturnValueType, ReturnValueSize, DBNull.Value, ParameterDirection.ReturnValue);
                    foreach (OracleDbParameter prm in InAndOutParameters)
                    {
                        if (prm.Size == 0)
                            cmd.Parameters.Add(prm.Name, (OracleDbType)prm.DataType, prm.Value, (ParameterDirection)prm.Direction);
                        else
                            cmd.Parameters.Add(prm.Name, (OracleDbType)prm.DataType, prm.Size, prm.Value, (ParameterDirection)prm.Direction);
                    }
                }

                con.Open();

                object retVal = cmd.ExecuteScalar();
                //OracleDataReader reader = cmd.ExecuteReader();

                return cmd.Parameters["Return_Value"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();

                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Dispose();
                }
            }
        }

        public int ExecuteCUDOperation(string SQLTextOrSPName, OracleCommandType CommandType, List<OracleDbParameter> InAndOutParameters = null)
        {
            int retVal = 0;
            OracleConnection con = null;
            OracleCommand cmd = null;

            try
            {
                con = new OracleConnection(ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString);
                cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandType = (System.Data.CommandType)CommandType;
                cmd.CommandText = SQLTextOrSPName;
                cmd.BindByName = true;

                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    foreach (OracleDbParameter prm in InAndOutParameters)
                    {
                        if (prm.Size == 0)
                            cmd.Parameters.Add(prm.Name, (OracleDbType)prm.DataType, prm.Value, (ParameterDirection)prm.Direction);
                        else
                            cmd.Parameters.Add(prm.Name, (OracleDbType)prm.DataType, prm.Size, prm.Value, (ParameterDirection)prm.Direction);
                    }
                }

                con.Open();

                retVal = cmd.ExecuteNonQuery();

                // Return Output values if exists.
                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    foreach (OracleDbParameter prm in InAndOutParameters.Where(prm => prm.Direction == OracleDbParameterDirection.Output))
                    {
                        prm.Value = cmd.Parameters[prm.Name].Value;
                    }
                }

                return retVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();

                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Dispose();
                }
            }
        }

        public object GetScalarValueByStatment(string statment)
        {
            OracleConnection con = null;
            OracleCommand cmd = null;

            try
            {
                con = new OracleConnection(ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString);
                cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = statment;
                cmd.BindByName = true;

                con.Open();

                object retVal = cmd.ExecuteScalar();
                //OracleDataReader reader = cmd.ExecuteReader();

                return retVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();

                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Dispose();
                }
            }
        }
    }
}

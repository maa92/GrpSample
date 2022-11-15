using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;

namespace GRP.DataAccess.DAL.Sql
{
    public enum SqlDbParameterDirection
    {
        Input = 1,
        Output = 2,
        InputOutput = 3,
        ReturnValue = 6,
    }

    public enum SqlDataType
    {
        // Summary:
        //     System.Int64. A 64-bit signed integer.
        BigInt = 0,
        //
        // Summary:
        //     System.Array of type System.Byte. A fixed-length stream of binary data ranging
        //     between 1 and 8,000 bytes.
        Binary = 1,
        //
        // Summary:
        //     System.Boolean. An unsigned numeric value that can be 0, 1, or null.
        Bit = 2,
        //
        // Summary:
        //     System.String. A fixed-length stream of non-Unicode characters ranging between
        //     1 and 8,000 characters.
        Char = 3,
        //
        // Summary:
        //     System.DateTime. Date and time data ranging in value from January 1, 1753
        //     to December 31, 9999 to an accuracy of 3.33 milliseconds.
        DateTime = 4,
        //
        // Summary:
        //     System.Decimal. A fixed precision and scale numeric value between -10 38
        //     -1 and 10 38 -1.
        Decimal = 5,
        //
        // Summary:
        //     System.Double. A floating point number within the range of -1.79E +308 through
        //     1.79E +308.
        Float = 6,
        //
        // Summary:
        //     System.Array of type System.Byte. A variable-length stream of binary data
        //     ranging from 0 to 2 31 -1 (or 2,147,483,647) bytes.
        Image = 7,
        //
        // Summary:
        //     System.Int32. A 32-bit signed integer.
        Int = 8,
        //
        // Summary:
        //     System.Decimal. A currency value ranging from -2 63 (or -9,223,372,036,854,775,808)
        //     to 2 63 -1 (or +9,223,372,036,854,775,807) with an accuracy to a ten-thousandth
        //     of a currency unit.
        Money = 9,
        //
        // Summary:
        //     System.String. A fixed-length stream of Unicode characters ranging between
        //     1 and 4,000 characters.
        NChar = 10,
        //
        // Summary:
        //     System.String. A variable-length stream of Unicode data with a maximum length
        //     of 2 30 - 1 (or 1,073,741,823) characters.
        NText = 11,
        //
        // Summary:
        //     System.String. A variable-length stream of Unicode characters ranging between
        //     1 and 4,000 characters. Implicit conversion fails if the string is greater
        //     than 4,000 characters. Explicitly set the object when working with strings
        //     longer than 4,000 characters. Use System.Data.SqlDbType.NVarChar when the
        //     database column is nvarchar(max).
        NVarChar = 12,
        //
        // Summary:
        //     System.Single. A floating point number within the range of -3.40E +38 through
        //     3.40E +38.
        Real = 13,
        //
        // Summary:
        //     System.Guid. A globally unique identifier (or GUID).
        UniqueIdentifier = 14,
        //
        // Summary:
        //     System.DateTime. Date and time data ranging in value from January 1, 1900
        //     to June 6, 2079 to an accuracy of one minute.
        SmallDateTime = 15,
        //
        // Summary:
        //     System.Int16. A 16-bit signed integer.
        SmallInt = 16,
        //
        // Summary:
        //     System.Decimal. A currency value ranging from -214,748.3648 to +214,748.3647
        //     with an accuracy to a ten-thousandth of a currency unit.
        SmallMoney = 17,
        //
        // Summary:
        //     System.String. A variable-length stream of non-Unicode data with a maximum
        //     length of 2 31 -1 (or 2,147,483,647) characters.
        Text = 18,
        //
        // Summary:
        //     System.Array of type System.Byte. Automatically generated binary numbers,
        //     which are guaranteed to be unique within a database. timestamp is used typically
        //     as a mechanism for version-stamping table rows. The storage size is 8 bytes.
        Timestamp = 19,
        //
        // Summary:
        //     System.Byte. An 8-bit unsigned integer.
        TinyInt = 20,
        //
        // Summary:
        //     System.Array of type System.Byte. A variable-length stream of binary data
        //     ranging between 1 and 8,000 bytes. Implicit conversion fails if the byte
        //     array is greater than 8,000 bytes. Explicitly set the object when working
        //     with byte arrays larger than 8,000 bytes.
        VarBinary = 21,
        //
        // Summary:
        //     System.String. A variable-length stream of non-Unicode characters ranging
        //     between 1 and 8,000 characters. Use System.Data.SqlDbType.VarChar when the
        //     database column is varchar(max).
        VarChar = 22,
        //
        // Summary:
        //     System.Object. A special data type that can contain numeric, string, binary,
        //     or date data as well as the SQL Server values Empty and Null, which is assumed
        //     if no other type is declared.
        Variant = 23,
        //
        // Summary:
        //     An XML value. Obtain the XML as a string using the System.Data.SqlClient.SqlDataReader.GetValue(System.Int32)
        //     method or System.Data.SqlTypes.SqlXml.Value property, or as an System.Xml.XmlReader
        //     by calling the System.Data.SqlTypes.SqlXml.CreateReader() method.
        Xml = 25,
        //
        // Summary:
        //     A SQL Server user-defined type (UDT).
        Udt = 29,
        //
        // Summary:
        //     A special data type for specifying structured data contained in table-valued
        //     parameters.
        Structured = 30,
        //
        // Summary:
        //     Date data ranging in value from January 1,1 AD through December 31, 9999
        //     AD.
        Date = 31,
        //
        // Summary:
        //     Time data based on a 24-hour clock. Time value range is 00:00:00 through
        //     23:59:59.9999999 with an accuracy of 100 nanoseconds. Corresponds to a SQL
        //     Server time value.
        Time = 32,
        //
        // Summary:
        //     Date and time data. Date value range is from January 1,1 AD through December
        //     31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999 with an
        //     accuracy of 100 nanoseconds.
        DateTime2 = 33,
        //
        // Summary:
        //     Date and time data with time zone awareness. Date value range is from January
        //     1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through
        //     23:59:59.9999999 with an accuracy of 100 nanoseconds. Time zone value range
        //     is -14:00 through +14:00.
        DateTimeOffset = 34,
    }

    public enum SqlCommandType
    {
        Text = 1,
        StoredProcedure = 4,
        TableDirect = 512,
    }

    public class SqlDbParameter
    {
        public string Name { get; set; }
        public SqlDataType DataType { get; set; }
        public SqlDbParameterDirection Direction { get; set; }
        public int Size { get; set; }
        public object Value { get; set; }

        public SqlDbParameter() { }
        public SqlDbParameter(string ParameterName, SqlDataType DataType, SqlDbParameterDirection ParameterDirection, object ParameterValue)
        {
            this.Name = ParameterName;
            this.DataType = DataType;
            this.Direction = ParameterDirection;
            this.Value = ParameterValue;
        }
        public SqlDbParameter(string ParameterName, SqlDataType DataType, SqlDbParameterDirection ParameterDirection)
        {
            this.Name = ParameterName;
            this.DataType = DataType;
            this.Direction = ParameterDirection;
        }
        public SqlDbParameter(string ParameterName, SqlDataType DataType, int Size, SqlDbParameterDirection ParameterDirection)
        {
            this.Name = ParameterName;
            this.DataType = DataType;
            this.Direction = ParameterDirection;
            this.Size = Size;
        }
        public SqlDbParameter(string ParameterName, SqlDataType DataType, SqlDbParameterDirection ParameterDirection, int Size, object ParameterValue)
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

        public DataSet GetDataMultipleResultSet(string SQLTextOrSPName, SqlCommandType CommandType, List<SqlDbParameter> InAndOutParameters = null)
        {
            DataSet dsRetVal = new DataSet();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlParameter sqlPrm = null;

            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString);
                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = (System.Data.CommandType)CommandType;
                cmd.CommandText = SQLTextOrSPName;

                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    foreach (SqlDbParameter prm in InAndOutParameters)
                    {
                        if (prm.Size == 0)
                            sqlPrm = new SqlParameter(string.Format("@{0}", prm.Name), (SqlDbType)prm.DataType);
                        else
                            sqlPrm = new SqlParameter(string.Format("@{0}", prm.Name), (SqlDbType)prm.DataType, prm.Size);

                        sqlPrm.Direction = (ParameterDirection)prm.Direction;
                        sqlPrm.Value = prm.Value;

                        cmd.Parameters.Add(sqlPrm);
                    }
                }

                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                // Return the data result set, AND Fill the output Parameters
                adapter.Fill(dsRetVal);

                // Return Output values if exists.
                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    foreach (SqlDbParameter prm in InAndOutParameters.Where(prm => prm.Direction == SqlDbParameterDirection.Output))
                    {
                        prm.Value = cmd.Parameters[string.Format("@{0}", prm.Name)].Value;
                    }
                }

                return dsRetVal;
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

        public DataTable GetDataResultSet(string SQLTextOrSPName, SqlCommandType CommandType, List<SqlDbParameter> InAndOutParameters = null)
        {
            DataTable dtRetVal = new DataTable();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlParameter sqlPrm = null;

            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString);
                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = (System.Data.CommandType)CommandType;
                cmd.CommandText = SQLTextOrSPName;

                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    foreach (SqlDbParameter prm in InAndOutParameters)
                    {
                        if (prm.Size == 0)
                            sqlPrm = new SqlParameter(string.Format("@{0}", prm.Name), (SqlDbType)prm.DataType);
                        else
                            sqlPrm = new SqlParameter(string.Format("@{0}", prm.Name), (SqlDbType)prm.DataType, prm.Size);

                        sqlPrm.Direction = (ParameterDirection)prm.Direction;
                        sqlPrm.Value = prm.Value;

                        cmd.Parameters.Add(sqlPrm);
                    }
                }

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                // Return the data result set.
                dtRetVal.Load(reader);

                // Return Output values if exists.
                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    foreach (SqlDbParameter prm in InAndOutParameters.Where(prm => prm.Direction == SqlDbParameterDirection.Output))
                    {
                        prm.Value = cmd.Parameters[string.Format("@{0}", prm.Name)].Value;
                    }
                }

                return dtRetVal;
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

        public object GetScalarValueByFunction(string FunctionName, SqlCommandType ReturnValueType, int ReturnValueSize, List<SqlDbParameter> InAndOutParameters = null)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlParameter sqlPrm = null;

            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString);
                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = FunctionName;

                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    foreach (SqlDbParameter prm in InAndOutParameters)
                    {
                        if (prm.Size == 0)
                            sqlPrm = new SqlParameter(string.Format("@{0}", prm.Name), (SqlDbType)prm.DataType);
                        else
                            sqlPrm = new SqlParameter(string.Format("@{0}", prm.Name), (SqlDbType)prm.DataType, prm.Size);

                        sqlPrm.Direction = (ParameterDirection)prm.Direction;
                        sqlPrm.Value = prm.Value;

                        cmd.Parameters.Add(sqlPrm);
                    }
                }

                con.Open();

                object retVal = cmd.ExecuteScalar();
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

        public int ExecuteCUDOperation(string SQLTextOrSPName, SqlCommandType CommandType, List<SqlDbParameter> InAndOutParameters = null)
        {
            int retVal = 0;
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlParameter sqlPrm = null;

            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString);
                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = (System.Data.CommandType)CommandType;
                cmd.CommandText = SQLTextOrSPName;
                //cmd.BindByName = true;

                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    foreach (SqlDbParameter prm in InAndOutParameters)
                    {
                        if (prm.Size == 0)
                            sqlPrm = new SqlParameter(string.Format("@{0}", prm.Name), (SqlDbType)prm.DataType);
                        else
                            sqlPrm = new SqlParameter(string.Format("@{0}", prm.Name), (SqlDbType)prm.DataType, prm.Size);

                        sqlPrm.Direction = (ParameterDirection)prm.Direction;
                        sqlPrm.Value = prm.Value;

                        cmd.Parameters.Add(sqlPrm);
                    }
                }

                con.Open();

                retVal = cmd.ExecuteNonQuery();

                // Return Output values if exists.
                if (InAndOutParameters != null && InAndOutParameters.Count > 0)
                {
                    foreach (SqlDbParameter prm in InAndOutParameters.Where(prm => prm.Direction == SqlDbParameterDirection.Output))
                    {
                        prm.Value = cmd.Parameters[string.Format("@{0}", prm.Name)].Value;
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
    }
}

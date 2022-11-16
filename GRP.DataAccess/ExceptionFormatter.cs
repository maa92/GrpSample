using System;
using System.Linq;
using System.Text;

namespace GrpSample.DataAccess
{
    static class ExceptionFormatter
    {
        public static string GetExceptionMessageForWeb(Exception ex)
        {
            string retVal = retVal = ex.Message;
            if (ex is Oracle.DataAccess.Client.OracleException)
            {
                string[] exceptionVals = ex.Message.Split(new string[] { "\n" }, StringSplitOptions.None);
                if (exceptionVals.Length > 0)
                {
                    retVal = exceptionVals[0];
                    //StringBuilder sb = new StringBuilder(exceptionVals[0]);
                    //sb.Append("<br/>");
                    //sb.Append(exceptionVals[1].Replace("at \"SRCAERP.", string.Empty).Replace("\", line", ","));
                    //retVal = sb.ToString();
                    //sb = null;
                }
                /*
                 ORA-20006: القسمه على صفر
                 ORA-06512: at "SRCAERP.HR_FRM_VAC_EMP_CHECK", line 20
                 ORA-06512: at line 1
                */
            }
            return retVal;
        }
    }
}

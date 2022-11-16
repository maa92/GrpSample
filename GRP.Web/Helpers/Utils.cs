using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace GrpSample.Web.Helpers
{
    public class Utils
    {
        public static string GetIPAddress()
        {
            return (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                   HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]).Split(':')[0].Trim();
        }

        public static string GetCompName()//string IP)
        {
            try
            {
                IPAddress myIP = IPAddress.Parse(GetIPAddress());
                IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
                string[] compName = GetIPHost.HostName.ToString().Split('.');
                if (compName != null && compName.Length > 0)
                    return compName[0];
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Compute(string s, string t)
        {
            int n = string.IsNullOrWhiteSpace(s) ? 0 : s.Length;
            int m = string.IsNullOrWhiteSpace(t) ? 0 : t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
                return m;

            if (m == 0)
                return n;

            for (int i = 0; i <= n; d[i, 0] = i++)
            { }

            for (int j = 0; j <= m; d[0, j] = j++)
            { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        #region error logging
        public static void LogErrorInFile(string MethodName, string ErrorMsg, string StackTrace, Dictionary<string, object> d)
        {
            try
            {
                string path = @"C:\Temp\GRPErrorLog\" + DateTime.Today.ToString("dd-MM-yyyy") + ".txt";
                // This text is added only once to the file.
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine("Method Name  : " + MethodName);
                        sw.WriteLine("Date         : " + DateTime.Now);
                        sw.WriteLine("ErrorMsg     : " + ErrorMsg);
                        sw.WriteLine("Stack Trace  : " + StackTrace);

                        foreach (var item in d)
                        {
                            sw.WriteLine(item.Key + ":" + item.Value + "  ,  ");
                        }
                        sw.WriteLine("========================================================================================");
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine("Method Name  : " + MethodName);
                        sw.WriteLine("Date         : " + DateTime.Now);
                        sw.WriteLine("ErrorMSG     : " + ErrorMsg);
                        sw.WriteLine("Stack Trace  : " + StackTrace);

                        foreach (var item in d)
                        {
                            sw.WriteLine(item.Key + ":" + item.Value + "  ,  ");
                        }
                        sw.WriteLine("========================================================================================");
                    }
                }

            }
            catch (Exception ex)
            {

            }

        }

        public static void Log(string MethodName, string ErrorMsg, string StackTrace, Dictionary<string, object> d)
        {
            try
            {
                string path = @"C:\Temp\GRPErrorLog\" + DateTime.Today.ToString("dd-MM-yyyy") + ".txt";

                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine("Method Name  : " + MethodName);
                        sw.WriteLine("Date         : " + DateTime.Now);
                        sw.WriteLine("ErrorMsg     : " + ErrorMsg);
                        sw.WriteLine("Stack Trace  : " + StackTrace);

                        foreach (var item in d)
                        {
                            sw.WriteLine(item.Key + ":" + item.Value + "  ,  ");
                        }
                        sw.WriteLine("========================================================================================");
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine("Method Name  : " + MethodName);
                        sw.WriteLine("Date         : " + DateTime.Now);
                        sw.WriteLine("ErrorMSG     : " + ErrorMsg);
                        sw.WriteLine("Stack Trace  : " + StackTrace);

                        foreach (var item in d)
                        {
                            sw.WriteLine(item.Key + ":" + item.Value + "  ,  ");
                        }
                        sw.WriteLine("========================================================================================");
                    }
                }

            }
            catch (Exception ex)
            {

            }

        }
        #endregion
    }
}
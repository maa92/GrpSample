using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.DataAccess
{
    public static class Utils
    {
        public static string EncodeString(string InputString)
        {
            string encStr = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(InputString));

            encStr = encStr.Replace("=", string.Empty);
            encStr = encStr.Replace("+", "-");
            encStr = encStr.Replace("/", "_");

            return new string(encStr.Reverse().ToArray());
        }

        public static string DecodeString(string EncodedString)
        {
            string decStr = new string(EncodedString.Reverse().ToArray());

            decStr = decStr.Replace("-", "+");
            decStr = decStr.Replace("_", "/");
            switch (decStr.Length % 4)
            {
                case 0: break;                  // No padding
                case 2: decStr += "=="; break;  // Two chars padding
                case 3: decStr += "="; break;   // One char padding
                default: throw new System.Exception("Invalid encoded string!");
            }

            return UTF8Encoding.UTF8.GetString(Convert.FromBase64String(decStr));
        }

        public static string GetHijriMonthName(string HijriMonthNumber)
        {
            string retVal = string.Empty;
            if (HijriMonthNumber == "01" || HijriMonthNumber == "1")
                retVal = "محرم";
            else if (HijriMonthNumber == "02" || HijriMonthNumber == "2")
                retVal = "صفر";
            else if (HijriMonthNumber == "03" || HijriMonthNumber == "3")
                retVal = "ربيع الأول";
            else if (HijriMonthNumber == "04" || HijriMonthNumber == "4")
                retVal = "ربيع الثاني";
            else if (HijriMonthNumber == "05" || HijriMonthNumber == "5")
                retVal = "جمادى الأولى";
            else if (HijriMonthNumber == "06" || HijriMonthNumber == "6")
                retVal = "جمادى الآخرة";
            else if (HijriMonthNumber == "07" || HijriMonthNumber == "7")
                retVal = "رجب";
            else if (HijriMonthNumber == "08" || HijriMonthNumber == "8")
                retVal = "شعبان";
            else if (HijriMonthNumber == "09" || HijriMonthNumber == "9")
                retVal = "رمضان";
            else if (HijriMonthNumber == "10")
                retVal = "شوال";
            else if (HijriMonthNumber == "11")
                retVal = "ذو القعدة";
            else if (HijriMonthNumber == "12")
                retVal = "ذو الحجة";

            return retVal;
        }

        public static string ConvertHijriDateToGregorian(string HijriDate, char HijriDateSeparator, string GregorianDateFormat)
        {
            System.Globalization.UmAlQuraCalendar uac = new System.Globalization.UmAlQuraCalendar();
            string[] arrDateValues = HijriDate.Split(HijriDateSeparator);
            DateTime baseDateH =
                new DateTime(arrDateValues[0].PadLeft(2, '0').Length == 2 ? Convert.ToInt32(arrDateValues[2]) : Convert.ToInt32(arrDateValues[0]),
                    Convert.ToInt32(arrDateValues[1]),
                    arrDateValues[0].PadLeft(2, '0').Length == 2 ? Convert.ToInt32(arrDateValues[0]) : Convert.ToInt32(arrDateValues[2]), uac);

            return baseDateH.ToString(GregorianDateFormat, new System.Globalization.CultureInfo("en-US"));
        }

        public static string ConvertGregorianDateToHijri(string GregorianDate, char GregorianDateSeparator, string HijriDateFormat)
        {
            string[] arrDateValues = GregorianDate.Split(GregorianDateSeparator);
            DateTime dtmG = new DateTime(arrDateValues[0].PadLeft(2, '0').Length == 2 ? Convert.ToInt32(arrDateValues[2]) : Convert.ToInt32(arrDateValues[0]),
                    Convert.ToInt32(arrDateValues[1]),
                    arrDateValues[0].PadLeft(2, '0').Length == 2 ? Convert.ToInt32(arrDateValues[0]) : Convert.ToInt32(arrDateValues[2]));

            System.Globalization.UmAlQuraCalendar uac = new System.Globalization.UmAlQuraCalendar();

            DateTime baseDateH = new DateTime(uac.GetYear(dtmG), uac.GetMonth(dtmG), uac.GetDayOfMonth(dtmG), uac);

            return baseDateH.ToString(HijriDateFormat, new System.Globalization.CultureInfo("ar-SA"));
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

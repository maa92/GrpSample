using GrpSample.Models.SysCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrpSample.Web.Helpers
{
    static public class DateManager
    {
        //static string strDate;
        public static string ConvertDate(string NumericDate)
        {
            string strDate = NumericDate.Substring(6) + "/" + NumericDate.Substring(4, 2) + "/" + NumericDate.Substring(0, 4) + "هــ";

            return strDate;
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

        public static string ConvertDateGregorianToHijri(DateTime GregorianDate, string HijriDateFormat)
        {
            System.Globalization.UmAlQuraCalendar uac = new System.Globalization.UmAlQuraCalendar();

            DateTime baseDateH = new DateTime(uac.GetYear(GregorianDate), uac.GetMonth(GregorianDate), uac.GetDayOfMonth(GregorianDate), uac);

            return baseDateH.Date.ToString(HijriDateFormat, new System.Globalization.CultureInfo("ar-SA"));
        }

        public static int GetCurrentHijriYear()
        {
            System.Globalization.UmAlQuraCalendar uac = new System.Globalization.UmAlQuraCalendar();

            return uac.GetYear(DateTime.Now);
        }

        public static int GetCurrentHijriMonth()
        {
            System.Globalization.UmAlQuraCalendar uac = new System.Globalization.UmAlQuraCalendar();

            return uac.GetMonth(DateTime.Now);
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

        public static List<HijriMonth> GetHijriMonths()
        {
            List<HijriMonth> retVal = new List<HijriMonth>();

            retVal.Add(new HijriMonth { MonthValue = "1", MonthName = "محرم" });
            retVal.Add(new HijriMonth { MonthValue ="2" , MonthName = "صفر" });
            retVal.Add(new HijriMonth { MonthValue ="3" , MonthName = "ربيع الأول" });
            retVal.Add(new HijriMonth { MonthValue ="4" , MonthName = "ربيع الثاني" });
            retVal.Add(new HijriMonth { MonthValue ="5" , MonthName = "جمادى الأول" });
            retVal.Add(new HijriMonth { MonthValue ="6" , MonthName = "جمادى الآخر" });
            retVal.Add(new HijriMonth { MonthValue ="7" , MonthName = "رجب" });
            retVal.Add(new HijriMonth { MonthValue ="8" , MonthName = "شعبان" });
            retVal.Add(new HijriMonth { MonthValue ="9" , MonthName = "رمضان" });
            retVal.Add(new HijriMonth { MonthValue ="10" , MonthName = "شوال" });
            retVal.Add(new HijriMonth { MonthValue ="11" , MonthName = "ذو القعدة" });
            retVal.Add(new HijriMonth { MonthValue = "12", MonthName = "ذو الحجة" });

            return retVal;
        }

        public static List<GMonth> GetGMonths()
        {
            List<GMonth> retVal = new List<GMonth>();

            retVal.Add(new GMonth { MonthValue = "1", MonthName = "يناير" });
            retVal.Add(new GMonth { MonthValue = "2", MonthName = "فبراير" });
            retVal.Add(new GMonth { MonthValue = "3", MonthName = "مارس" });
            retVal.Add(new GMonth { MonthValue = "4", MonthName = "ابريل" });
            retVal.Add(new GMonth { MonthValue = "5", MonthName = "مايو" });
            retVal.Add(new GMonth { MonthValue = "6", MonthName = "يونيو" });
            retVal.Add(new GMonth { MonthValue = "7", MonthName = "يوليو" });
            retVal.Add(new GMonth { MonthValue = "8", MonthName = "اغسطس" });
            retVal.Add(new GMonth { MonthValue = "9", MonthName = "سبتمبر" });
            retVal.Add(new GMonth { MonthValue = "10", MonthName = "اكتوبر" });
            retVal.Add(new GMonth { MonthValue = "11", MonthName = "نوفمبر" });
            retVal.Add(new GMonth { MonthValue = "12", MonthName = "ديسمبر" });

            return retVal;
        }
    }
}

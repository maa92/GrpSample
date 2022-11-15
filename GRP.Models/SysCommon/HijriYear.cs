using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Models.SysCommon
{
    public class HijriYear
    {
        public Int16 year { get; set; }
        public DateTime jDate { get; set; }
        public Int16 mon01 { get; set; }
        public Int16 mon02 { get; set; }
        public Int16 mon03 { get; set; }
        public Int16 mon04 { get; set; }
        public Int16 mon05 { get; set; }
        public Int16 mon06 { get; set; }
        public Int16 mon07 { get; set; }
        public Int16 mon08 { get; set; }
        public Int16 mon09 { get; set; }
        public Int16 mon10 { get; set; }
        public Int16 mon11 { get; set; }
        public Int16 mon12 { get; set; }
        public Int32 totalHDays
        {
            get
            {
                return mon01 + mon02 + mon03 + mon04 + mon05 + mon06 + mon07 + mon08 + mon09 + mon10 + mon11 + mon12;
            }
        }
    }
}

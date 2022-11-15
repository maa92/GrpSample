using GRP.Models.SysCommon;
using System;
using System.Collections.Generic;

namespace GRP.Models.System
{
    public class SysSettings
    {
        public String system_link { get; set; }
        public DateTime ovr_st_time { get; set; }
        public String report_server { get; set; }
        public String forms_path { get; set; }
        public String current_hDate { get; set; }
        public List<HijriYear> hijriYears { get; set; }
    }
}

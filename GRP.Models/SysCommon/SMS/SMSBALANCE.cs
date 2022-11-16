using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.SysCommon.SMS
{
    public class SMSBALANCE
    {
        public class Balance
        { 
            public string success { get; set; }
            public string message { get; set; }
            public string errorCode { get; set; }
            public Data data { get; set; }
         }
        public class Data
        {
            public string Balance { get; set; }
            public string CurrencyCode { get; set; }
            public string SharedBalance { get; set; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Models.SysCommon
{
    public class SendSMS
    {
        public class SMSResult
        {
            public string success { get; set; }
            public string message { get; set; }
            public string errorCode { get; set; }
            public Data data { get; set; }
        }
        public class Data
        {
            public List<Message> Messages { get; set; }
            public int NumberOfUnits { get; set; }
            public string Cost { get; set; }
            public string Balance { get; set; }
            public string TimeCreated { get; set; }
            public string CurrencyCode { get; set; }
        }
        public class Message
        {
            public string MessageID { get; set; }
            public string Recipient { get; set; }
            public string Status { get; set; }
        }

    }
}

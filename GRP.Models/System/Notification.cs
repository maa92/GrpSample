using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.System
{
    public class Notification
    {
        /// <summary> Notification Id </summary>
        public int Id { get; set; }
        /// <summary> Notification Group Name </summary>
        public string GroupName { get; set; }
        /// <summary> Notification Group Display Text </summary>
        public string GroupText { get; set; }
        /// /// <summary> Notification Type </summary>
        public string Type { get; set; }
        /// <summary> Notification Display Text </summary>
        public string Text { get; set; }
        /// <summary> Notification Parameters (Url, Request Id, etc...) </summary>
        public Dictionary<string,string> Params { get; set; }

        public Notification()
        {
            Id = new Random().Next();
            Params = new Dictionary<string, string>();
        }
    }

    public class NotificationDb
    {
        public Int32 ES { get; set; }
        public Int32 SL { get; set; }
        public Int16 YR { get; set; }
        public Int32 EC { get; set; }
        public String EN { get; set; }
        public String RQT { get; set; }
        public String NTF { get; set; }
        public String NTN { get; set; }
        public String EAF { get; set; }
    }
}

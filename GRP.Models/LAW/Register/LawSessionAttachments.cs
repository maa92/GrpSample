using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpSample.Models.LAW.Register
{
    public class LawSessionAttachments
    {
        public string CaseYear { get; set; }
        public Int32 CaseSerial { get; set; }
        public Int32 SessionId { get; set; }
        public int[] FileInInt { get; set; }  //for sending file bytes from javascript and convert it to byte[] in controller 
        public byte[] AttachFile { get; set; }
        public int? AttachType { get; set; }  //نوع المرفق
        public SessionAttachment SessionAttach { get; set; }
    }

    public class SessionAttachment
    {
        public string DF { get; set; }
        public string ATTACH_ID { get; set; }
        public string ATTACH_DATE { get; set; }
        public string ATTACH_FILE_NAME { get; set; }
        public string ATTACH_FILE_TYPE { get; set; }
        public string ATTACH_NOTES { get; set; }
    }
}

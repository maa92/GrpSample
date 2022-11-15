using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Models.LAW.Register
{
    public class SessionRegFormOpen   //For SessionReg.cshtml onOpen
    {
        public List<SessionStatus> sStatus { get; set; }
        public List<AttachType> aType { get; set; }
    }

    public class SessionStatus
    {
        public int CODE_ID { get; set; }
        public string CODE_NAME { get; set; }
    }

    public class AttachType
    {
        public int CODE_ID { get; set; }
        public string CODE_NAME { get; set; }
    }
}

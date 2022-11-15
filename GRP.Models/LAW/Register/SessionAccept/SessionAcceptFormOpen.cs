using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Models.LAW.Register.SessionAccept
{
    public class SessionAcceptFormOpen
    {
        public string formUniqueId { get; set; }
        public SessionRegForAccept[] sessionLst { get; set; }
    }
}

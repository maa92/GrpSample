using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Models.System
{
    public class BaseHomePage
    {
        public string EnvironmentType { get; set; }
        public string UserFullName { get; set; }
        public string UserDeptName { get; set; }
        public bool UserCanChangeLogin { get; set; } 
    }
}

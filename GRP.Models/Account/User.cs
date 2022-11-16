using GrpSample.Models.System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GrpSample.Models.Account
{
    public class User
    {
        public string userId { get; set; }
        
        public string userFullName { get; set; }
        
        public string userDeptName { get; set; }

        public string userType { get; set; }

        public string userIsSuperAdmin { get; set; }

        public string userLoginFlag { get; set; }

        public string loginSerial { get; set; }

        public string loginMessage { get; set; }

        public List<SysForm> userSys { get; set; }

    }
}

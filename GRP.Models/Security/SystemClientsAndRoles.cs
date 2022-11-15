using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Models.Security
{
    public class SystemClientsAndRoles
    {
        public List<Client> Clients { get; set; }
        public List<Role> Roles { get; set; }
    }

    public class Client
    {
        public string id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
    }

    public class Role
    {
        public string id { get; set; }
        public string name { get; set; }
        public string tokenValidity { get; set; }
        public string clients { get; set; }
    }
}


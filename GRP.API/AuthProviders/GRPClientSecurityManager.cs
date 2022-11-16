using System.Collections.Generic;
//using Microsoft.Owin;
using System.Runtime.Caching;
using Newtonsoft.Json;
using System.IO;
using System.Configuration;
using GrpSample.Models.Security;

namespace GrpSample.API.AuthProviders
{
    public class GRPClientSecurityManager
    {
        //IOwinContext _context;
        SystemClientsAndRoles _listOfClientsAndRoles;

        public GRPClientSecurityManager()
        {
            _listOfClientsAndRoles = GetRolesConfigValues();
        }

        public static GRPClientSecurityManager Create()//IOwinContext context)
        {
            return new GRPClientSecurityManager();//context); //new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }

        public bool IsClientInRole(string ClientName, string RoleName)
        {
            bool retVal = false;

            if (_listOfClientsAndRoles != null)
            {
                string[] arrRoles = RoleName.Split(',');
                foreach (string role in arrRoles)
                {
                    retVal = _listOfClientsAndRoles.Roles.Find(r => r.name == role).clients.Contains(ClientName);
                    if (retVal)
                        break;
                }
            }

            return retVal;
        }

        public bool IsClientAllowed(string ClientName, string ClientPassword)
        {
            bool retVal = false;

            if (_listOfClientsAndRoles != null)
            {
                retVal = _listOfClientsAndRoles.Clients.Exists(c => c.name == ClientName && c.password == ClientPassword);
            }

            return retVal;
        }

        public string GetClientTokenExpirationTime(string ClientName)
        {
            string retVal = "h_12";

            if (_listOfClientsAndRoles != null)
            {
                retVal = _listOfClientsAndRoles.Roles.Find(r => r.clients.Contains(ClientName)).tokenValidity;
            }

            return retVal;
        }

        private SystemClientsAndRoles GetRolesConfigValues()
        {
            SystemClientsAndRoles retVal = null;
            string filePath = ConfigurationManager.AppSettings["rolesConfigFileName"];

            ObjectCache cache = MemoryCache.Default;
            retVal = cache["sysRoles"] as SystemClientsAndRoles;

            if (retVal == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy();

                List<string> filePaths = new List<string>();
                filePaths.Add(filePath);
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(filePaths));

                retVal = JsonConvert.DeserializeObject<SystemClientsAndRoles>(File.ReadAllText(filePath));

                cache.Set("sysRoles", retVal, policy);
            }

            return retVal;
        }
    }
}
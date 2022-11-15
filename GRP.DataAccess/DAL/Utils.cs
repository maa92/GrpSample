using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.DataAccess.DAL
{
    public class BaseDataHandler
    {
        public string ConnectionStringName { get; set; }
        public BaseDataHandler(string EnvironmentFlag)
        {
            ConnectionStringName = ConfigurationManager.AppSettings[EnvironmentFlag];
        }
    }
}

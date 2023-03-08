using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BedAppManage
{
    public static class DBConfig
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["DB_Server"].ConnectionString;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracker
{
    public static class DatabaseConfig
    {
        public static string ConnectionString { get; } = "server=localhost;database=tracker;uid=;pwd=;";
    }

}

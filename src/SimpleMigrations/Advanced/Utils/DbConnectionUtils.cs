using System.Configuration;

namespace SimpleMigrations.Advanced.Utils
{
    public static class DbConnectionUtils
    {        
        public static string GetNamedConnectionString(string name)
        {
            var connectionConfig = ConfigurationManager.ConnectionStrings[name];
            return connectionConfig.ConnectionString;
        }
    }
}

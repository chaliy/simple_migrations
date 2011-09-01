using System.Configuration;

namespace SimpleMigrations.Database
{
    public static class Db
    {
        public static DatabaseConnection CreateByNamedConnection(string name)
        {
            var connectionConfig = ConfigurationManager.ConnectionStrings["Inventory"];            
            return new DatabaseConnection(connectionConfig.ConnectionString);
        }
    }
}

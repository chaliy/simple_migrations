using System.Configuration;
using JetBrains.Annotations;

namespace SimpleMigrations.Database
{
    public static class Db
    {
        public static DatabaseConnection CreateWithNamedConnection([NotNull]string name)
        {
            var connectionConfig = ConfigurationManager.ConnectionStrings[name];
            var connectionString = connectionConfig.ConnectionString;
            return CreateWithConnectionString(connectionString);
        }

        public static DatabaseConnection CreateWithConnectionString([NotNull]string connectionString)
        {
            return new DatabaseConnection(connectionString);
        }
    }
}

using System;
using System.Data.Common;
using System.Data.SqlClient;
using JetBrains.Annotations;
using Simple.Data.SqlServer;
using SimpleMigrations.Advanced.Utils;

namespace SimpleMigrations.Advanced.Registry
{
    public class SqlServerRegistryProvider : IRegistryProvider
    {
        private readonly string _connectionString;
        private dynamic _db;

        public SqlServerRegistryProvider([NotNull]string connectionString)
        {
            InternalContracts.Require(connectionString, "connectionString");
            _connectionString = connectionString;

            // This create requires to force Visual Studio to copy Simple.Data.SqlServer.dll 
            // to projects that references SimpleMigrations, this problem occurs because
            // Simple.Data uses Simple.Data.SqlServer.dll implicitly, and VS fail to see this ;)            
            new SqlConnectionProvider(); 
        }

        public bool IsMigrationApplied(string migrationId)
        {
            EnsureDb();            
            return _db.__Migrations.ExistsByMigrationID(migrationId);            
        }

        public void RegisterMigrationApplied(string migrationId)
        {
            EnsureDb();            
            _db.__Migrations.Insert(MigrationID: migrationId, LastExecuted: DateTime.Now);
        }

        private void EnsureDb()
        {
            if (_db == null)
            {
                ExecuteScript(
                    @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__Migrations]') AND type in (N'U'))
CREATE TABLE [dbo].[__Migrations](
	[MigrationID] [nvarchar](450) NOT NULL PRIMARY KEY,
	[LastExecuted] [datetime2](7) NOT NULL )");
                _db = Simple.Data.Database.OpenConnection(_connectionString);
            }
        }

        private void ExecuteScript(string sql)
        {
            using (var con = CreateOpenConnection())
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        private DbConnection CreateOpenConnection()
        {
            var factory = SqlClientFactory.Instance;

            var con = factory.CreateConnection();
            con.ConnectionString = _connectionString;
            con.Open();

            return con;
        }
    }
}

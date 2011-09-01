using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reactive.Subjects;

namespace SimpleMigrations
{
    public class Migrator
    {
        private readonly Subject<string> _infoSubject = new Subject<string>();        
        private readonly string _connectionString;

        // lifetime
        private bool _initialized;

        public Migrator(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IObservable<string> Info
        {
            get { return _infoSubject; }
        }

        public void ExecuteMigration(string migrationId, params string[] migrations)
        {
            EnsureInit();

            var db = Simple.Data.Database.OpenConnection(_connectionString);
            var migration = db.__Migrations.FindByMigrationID(migrationId);

            if (migration == null)
            {
                _infoSubject.OnNext("Executing migration: " + migrationId);

                foreach (var script in migrations)
                {
                    ExecuteScript(script);
                }

                db.__Migrations.Insert(MigrationID: migrationId, LastExecuted: DateTime.Now);
            }
        }


        private void EnsureInit()
        {
            if (!_initialized)
            {
                ExecuteScript(
                    @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__Migrations]') AND type in (N'U'))
	CREATE TABLE [dbo].[__Migrations](
		[MigrationID] [nvarchar](450) NOT NULL PRIMARY KEY,
		[LastExecuted] [datetime2](7) NOT NULL )");

                _initialized = true;
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

        public static Migrator WithNamedConnection(string connectionName)
        {
            var connectionConfig = ConfigurationManager.ConnectionStrings["Inventory"];
            return new Migrator(connectionConfig.ConnectionString);
        }
    }
}

using System;
using System.Collections.Generic;
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
        private readonly List<MigrationDef> _migrations = new List<MigrationDef>();

        public Migrator(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IObservable<string> Info
        {
            get { return _infoSubject; }
        }        

        public void Define(string migrationId, Action action)
        {
            _migrations.Add(new MigrationDef(migrationId, action));
        }

        public void Run()
        {
            EnsureInit();

            var db = Simple.Data.Database.OpenConnection(_connectionString);

            foreach (var migration in _migrations)
            {
                var migrationId = migration.Id;
                var migrationRec = db.__Migrations.FindByMigrationID(migrationId);

                if (migrationRec == null)
                {
                    _infoSubject.OnNext("Executing migration: " + migrationId);

                    migration.Action();
                    
                    db.__Migrations.Insert(MigrationID: migrationId, LastExecuted: DateTime.Now);
                }    
            }            
        }        

        private void EnsureInit()
        {            
            ExecuteScript(
                @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__Migrations]') AND type in (N'U'))
CREATE TABLE [dbo].[__Migrations](
	[MigrationID] [nvarchar](450) NOT NULL PRIMARY KEY,
	[LastExecuted] [datetime2](7) NOT NULL )");
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

        public static Migrator CreateByNamedConnection(string connectionName)
        {
            var connectionConfig = ConfigurationManager.ConnectionStrings["Inventory"];
            return new Migrator(connectionConfig.ConnectionString);
        }

        private class MigrationDef
        {
            private readonly string _id;
            private readonly Action _action;

            public MigrationDef(string id, Action action)
            {
                _id = id;
                _action = action;
            }

            public string Id
            {
                get { return _id; }
            }

            public Action Action
            {
                get { return _action; }
            }
        }
    }
}

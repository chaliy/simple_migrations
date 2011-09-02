using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using Simple.Data;
using Simple.Data.Ado;
using Simple.Data.Ado.Schema;
using Simple.Data.SqlServer;

namespace SimpleMigrations.Database
{
    public class DatabaseConnection
    {
        private readonly string _connectionString;        

        public DatabaseConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AlterTable(string existingTable, Action<AlterTableSpec> builder)
        {
            var spec = new AlterTableSpec(existingTable);
            builder(spec);

            ExecuteScript(spec.ToString());
        }

        public void CreateTable(string table, string primaryKey, Action<CreateTableSpec> builder)
        {
            var spec = new CreateTableSpec(table, primaryKey);
            builder(spec);

            ExecuteScript(spec.ToString());
        }
        
        public void ModifyData(Action<dynamic> action)
        {
            // This is hack to invalidate cached database schemas
            // More details - http://groups.google.com/group/simpledata/browse_thread/thread/9ecef3e939dcd62d
            var instances = (ConcurrentDictionary<string, DatabaseSchema>)typeof(DatabaseSchema)
                .GetField("Instances", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            instances.Clear();

            var connectionProvider = new SqlConnectionProvider(_connectionString);
            var adapterCtor = typeof(AdoAdapter).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, new[] { typeof(IConnectionProvider) }, null);
            var adapter = (Adapter)adapterCtor.Invoke(new [] {connectionProvider});

            var dbCtor = typeof(Simple.Data.Database).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, new[] { typeof(Adapter) }, null);
            
            var db = (dynamic)dbCtor.Invoke(new [] {adapter});
            action(db);
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
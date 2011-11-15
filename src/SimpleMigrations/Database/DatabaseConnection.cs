using System;
using System.Data.Common;
using System.Data.SqlClient;

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
            var db = Simple.Data.Database.OpenConnection(_connectionString);            
            db.GetAdapter().Reset();            
            action(db);
        }

        public void ExecuteScript(string sql)
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
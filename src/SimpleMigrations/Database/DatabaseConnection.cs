using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace SimpleMigrations.Database
{
    public class DatabaseConnection
    {
        private readonly string _connectionString;
        private dynamic _tables;

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
        
        public dynamic Tables
        {
            get
            {
                if (_tables == null)
                {
                    _tables = Simple.Data.Database.OpenConnection(_connectionString);
                }

                return _tables;
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
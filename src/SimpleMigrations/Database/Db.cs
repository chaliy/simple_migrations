using System;

namespace SimpleMigrations.Database
{
    public static class Db
    {
        public static string AlterTable(string existingTable, Action<AlterTableSpec> builder)
        {
            var spec = new AlterTableSpec(existingTable);
            builder(spec);

            return spec.ToString();
        }

        public static string CreateTable(string table, string primaryKey, Action<CreateTableSpec> builder)
        {
            var spec = new CreateTableSpec(table, primaryKey);
            builder(spec);

            return spec.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMigrations.Database
{
    public class CreateTableSpec : ICanAddColumns
    {
        private readonly string _tableName;
        private readonly string _primmaryKey;
        private readonly List<ColumnSpec> _columns = new List<ColumnSpec>();

        public CreateTableSpec(string tableName, string primmaryKey)
        {
            _tableName = tableName;
            _primmaryKey = primmaryKey;
        }

        public void AddColumn(ColumnSpec columnSpec)
        {
            _columns.Add(columnSpec);
        }

        public override string ToString()
        {
            var script = new StringBuilder();

            script.AppendLine("CREATE TABLE " + _tableName + " (");
            script.AppendLine(_primmaryKey + " UNIQUEIDENTIFIER NOT NULL, ");

            var columns = String.Join(", ", _columns
                                                .Select(c => c.Name + " " + c.Type + " NULL"));
            
            script.AppendLine(columns);

            script.AppendLine(" )");

            script.AppendLine();

            script.AppendLine(@"ALTER TABLE " + _tableName + @"
	ADD CONSTRAINT [" + _primmaryKey +@"_PK]
	PRIMARY KEY (" + _primmaryKey + @")
");

            foreach (var column in _columns.Where(x => x.Reference != null))
            {
                script.AppendLine(@"ALTER TABLE " + _tableName + @"
	ADD CONSTRAINT [" + _tableName.Replace(".", "_").Replace("[", "").Replace("]", "") + @"_" + column.Name + @"_FK] 
	FOREIGN KEY (" + column.Name + @")
	REFERENCES " + column.Reference.Table + @" (" + column.Reference.Column + @")
");
            }

            return script.ToString();
        }        
    }
}
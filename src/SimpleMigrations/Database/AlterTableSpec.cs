using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMigrations.Database
{
    public class AlterTableSpec : ICanAddColumns
    {
        private readonly string _tableName;
        private readonly List<ColumnSpec> _columns = new List<ColumnSpec>();

        public AlterTableSpec(string tableName)
        {
            _tableName = tableName;
        }

        public void AddColumn(ColumnSpec columnSpec)
        {
            _columns.Add(columnSpec);
        }

        public override string ToString()
        {
            var script = new StringBuilder();

            script.AppendLine("ALTER TABLE " + _tableName + " ADD ");

            var columns = String.Join(", ", _columns
                                                .Select(c => c.Name + " " + c.Type + " NULL"));
            
            script.AppendLine(columns);
            script.AppendLine();

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
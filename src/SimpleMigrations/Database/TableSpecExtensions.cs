namespace SimpleMigrations.Database
{
    public static class TableSpecExtensions
    {        
        public static T BooleanColumn<T>(this T @this, string columnName)
            where T : ICanAddColumns
        {
            @this.AddColumn(new ColumnSpec
            {
                Name = columnName,
                Type = "BIT"
            });
            return @this;
        }

        public static T StringColumn<T>(this T @this, string columnName, int length = 450)
            where T : ICanAddColumns
        {
            @this.AddColumn(new ColumnSpec
            {
                Name = columnName,
                Type = "nvarchar(" + length + ")"
            });
            return @this;
        }
        
        public static T ReferenceColumn<T>(this T @this, string columnName, string referenceTable, string referenceColumn)
            where T : ICanAddColumns
        {
            @this.AddColumn(new ColumnSpec
            {
                Name = columnName,
                Type = "UNIQUEIDENTIFIER",
                Reference = new ReferenceSpec
                                {
                                    Table = referenceTable,
                                    Column = referenceColumn
                                }
            });
            return @this;
        }
    }
}

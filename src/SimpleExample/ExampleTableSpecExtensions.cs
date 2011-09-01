using SimpleMigrations.Database;

namespace SimpleExample
{
    public static class ExampleTableSpecExtensions
    {        
        public static T AuditColumns<T>(this T @this)
            where T : ICanAddColumns
        {
            @this.AddColumn(new ColumnSpec
            {
                Name = "Version",
                Type = "UNIQUEIDENTIFIER"
            });
            @this.AddColumn(new ColumnSpec
            {
                Name = "LastUpdatedBy",
                Type = "UNIQUEIDENTIFIER"
            });
            @this.AddColumn(new ColumnSpec
            {
                Name = "LastUpdatedDate",
                Type = "DATETIME"
            });
            @this.AddColumn(new ColumnSpec
            {
                Name = "CreatedBy",
                Type = "UNIQUEIDENTIFIER"
            });
            @this.AddColumn(new ColumnSpec
            {
                Name = "CreatedDate",
                Type = "DATETIME"
            });

            return @this;
        }

        public static T MultiTenancyColumn<T>(this T @this)
            where T : ICanAddColumns
        {
            @this.AddColumn(new ColumnSpec
            {
                Name = "TenantID",
                Type = "UNIQUEIDENTIFIER"
            });
            return @this;
        }        
    }
}

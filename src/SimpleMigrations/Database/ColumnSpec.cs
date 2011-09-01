namespace SimpleMigrations.Database
{
    public class ColumnSpec
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public ReferenceSpec Reference { get; set; } 
    }
}
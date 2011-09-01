namespace SimpleMigrations.Database
{
    public interface ICanAddColumns
    {
        void AddColumn(ColumnSpec columnSpec);
    }
}
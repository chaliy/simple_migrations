namespace SimpleMigrations.Advanced.Registry
{
    public interface IRegistryProvider
    {
        bool IsMigrationApplied(string migrationId);
        void RegisterMigrationApplied(string migrationId);
    }
}

using System;
using System.Collections.Generic;                
using System.Reactive.Subjects;
using JetBrains.Annotations;
using SimpleMigrations.Advanced.Registry;
using SimpleMigrations.Advanced.Utils;

namespace SimpleMigrations
{
    public class Migrator
    {
        private readonly Subject<string> _infoSubject = new Subject<string>();
        private readonly IRegistryProvider _registryProvider;
        private readonly List<MigrationDef> _migrations = new List<MigrationDef>();

        public Migrator(IRegistryProvider registryProvider)
        {
            _registryProvider = registryProvider;
        }

        public IObservable<string> Info
        {
            get { return _infoSubject; }
        }        

        public void Define(string migrationId, Action action)
        {
            _migrations.Add(new MigrationDef(migrationId, action));
        }

        public void Run()
        {            
            foreach (var migration in _migrations)
            {
                var migrationId = migration.Id;

                if (!_registryProvider.IsMigrationApplied(migrationId))
                {
                    _infoSubject.OnNext("Executing migration: " + migrationId);

                    migration.Action();
                    
                    _registryProvider.RegisterMigrationApplied(migrationId);
                }    
            }
        }

        public static Migrator CreateWithNamedConnection([NotNull]string name)
        {
            var connectionString = DbConnectionUtils.GetNamedConnectionString(name);
            return CreateWithConnectionString(connectionString);
        }

        public static Migrator CreateWithConnectionString([NotNull]string connectionString)
        {            
            var provider = new SqlServerRegistryProvider(connectionString);
            return new Migrator(provider);
        }

        private class MigrationDef
        {
            private readonly string _id;
            private readonly Action _action;

            public MigrationDef(string id, Action action)
            {
                _id = id;
                _action = action;
            }

            public string Id
            {
                get { return _id; }
            }

            public Action Action
            {
                get { return _action; }
            }
        }
    }
}

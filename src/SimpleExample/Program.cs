using System;
using SimpleMigrations;
using SimpleMigrations.Database;

namespace SimpleExample
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                var migrator = new Migrator("");
                using (migrator.Info.Subscribe(Console.WriteLine))
                {
                    // Add new migration at the end of this block

                    // 2011-08-22
                    migrator.ExecuteMigration("Create Purchasing.Invoice",
                                            Db.CreateTable("Purchasing.Invoice", "InvoiceID",
                                                t =>
                                                {
                                                    t.StringColumn("InvoiceDate");
                                                    t.AuditColumns();
                                                    t.MultiTenancyColumn();
                                                })
                    );

                    // 2011-08-23
                    migrator.ExecuteMigration("Add InvoiceTotal to Purchasing.Invoice",
                                            Db.AlterTable("Purchasing.Invoice", t =>
                                            {
                                                t.StringColumn("InvoiceTotal");
                                            })
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during app migration. " + ex.Message, ex);
            }

        }
    }
}

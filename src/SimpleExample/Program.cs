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
                var db = Db.CreateByNamedConnection("Something");
                var migrator = Migrator.CreateByNamedConnection("Something");
                using (migrator.Info.Subscribe(Console.WriteLine))
                {
                    // Add new migration at the end of this block

                    // 2011-08-22
                    migrator.Define("Create Invoice", () =>
                                            db.CreateTable("Invoice", "InvoiceID",
                                                t =>
                                                {
                                                    t.StringColumn("InvoiceDate");
                                                    t.AuditColumns();
                                                    t.MultiTenancyColumn();
                                                })
                    );

                    // 2011-08-23
                    migrator.Define("Add InvoiceTotal to Invoice", () =>
                    {
                        // Add column InvoiceTotal
                        db.AlterTable("Invoice", t =>
                            {
                                t.StringColumn("InvoiceTotal");
                            });

                        // And update this total with default value in the same migration
                        db.Tables.Invoice.Update(InvoiceTotal: "12.00");
                    });

                    // Run migrations
                    migrator.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during app migration. " + ex.Message, ex);
            }

        }
    }
}

Simple Migrations
=================

IMPORTANT : For now I am assuming that I am the only user of this lib, so contracts are changing without any notice or backward compatibility. If you are intersting in using this lib, let me know.

Yet another naive attemt to create migration tool. The only difference it meant to run inline with your main code. Another possible intersting feature is that migration code could be in terms of your main code.

Features
========

1. Run migrations(?)

Example
=======

Simple example:

	var migrator = new Migrator("CONNECTION STRING");
	
	migrator.ExecuteMigration("Create Purchasing.Invoice",
							Db.CreateTable("Purchasing.Invoice", "InvoiceID",
								t =>
								{
									t.StringColumn("InvoiceDate");									
								})

Complete

	try
	{
		var migrator = new Migrator("CONNECTION STRING");
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

Installation
============
	
[TBD]
	
If you need other distribution, pls contact me.
	
License
=======

Licensed under the MIT
namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class afterconcurrencymodelupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boundaries", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Enquiries", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Organisations", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Invoices", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.InvoiceReminders", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.InvoiceReminderTypes", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Quotes", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.SearchTypes", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.LrcInfoes", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LrcInfoes", "RowVersion");
            DropColumn("dbo.SearchTypes", "RowVersion");
            DropColumn("dbo.Quotes", "RowVersion");
            DropColumn("dbo.InvoiceReminderTypes", "RowVersion");
            DropColumn("dbo.InvoiceReminders", "RowVersion");
            DropColumn("dbo.Invoices", "RowVersion");
            DropColumn("dbo.Organisations", "RowVersion");
            DropColumn("dbo.Enquiries", "RowVersion");
            DropColumn("dbo.Boundaries", "RowVersion");
        }
    }
}

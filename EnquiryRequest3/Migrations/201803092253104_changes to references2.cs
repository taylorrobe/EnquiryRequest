namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changestoreferences2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InvoiceReminders", "Invoice_InvoiceId", "dbo.Invoices");
            DropIndex("dbo.InvoiceReminders", new[] { "Invoice_InvoiceId" });
            RenameColumn(table: "dbo.InvoiceReminders", name: "Invoice_InvoiceId", newName: "InvoiceId");
            AlterColumn("dbo.InvoiceReminders", "InvoiceId", c => c.Int(nullable: false));
            CreateIndex("dbo.InvoiceReminders", "InvoiceId");
            AddForeignKey("dbo.InvoiceReminders", "InvoiceId", "dbo.Invoices", "InvoiceId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvoiceReminders", "InvoiceId", "dbo.Invoices");
            DropIndex("dbo.InvoiceReminders", new[] { "InvoiceId" });
            AlterColumn("dbo.InvoiceReminders", "InvoiceId", c => c.Int());
            RenameColumn(table: "dbo.InvoiceReminders", name: "InvoiceId", newName: "Invoice_InvoiceId");
            CreateIndex("dbo.InvoiceReminders", "Invoice_InvoiceId");
            AddForeignKey("dbo.InvoiceReminders", "Invoice_InvoiceId", "dbo.Invoices", "InvoiceId");
        }
    }
}

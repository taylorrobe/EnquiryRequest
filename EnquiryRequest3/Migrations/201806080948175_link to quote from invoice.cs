namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class linktoquotefrominvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "QuoteId", c => c.Int(nullable: false));
            CreateIndex("dbo.Invoices", "QuoteId");
            AddForeignKey("dbo.Invoices", "QuoteId", "dbo.Quotes", "QuoteId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invoices", "QuoteId", "dbo.Quotes");
            DropIndex("dbo.Invoices", new[] { "QuoteId" });
            DropColumn("dbo.Invoices", "QuoteId");
        }
    }
}

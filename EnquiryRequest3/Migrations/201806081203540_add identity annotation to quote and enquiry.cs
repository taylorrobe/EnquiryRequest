namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addidentityannotationtoquoteandenquiry : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Invoices", "QuoteId", "dbo.Quotes");
            DropPrimaryKey("dbo.Quotes");
            AlterColumn("dbo.Quotes", "QuoteId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Quotes", "QuoteId");
            AddForeignKey("dbo.Invoices", "QuoteId", "dbo.Quotes", "QuoteId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invoices", "QuoteId", "dbo.Quotes");
            DropPrimaryKey("dbo.Quotes");
            AlterColumn("dbo.Quotes", "QuoteId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Quotes", "QuoteId");
            AddForeignKey("dbo.Invoices", "QuoteId", "dbo.Quotes", "QuoteId", cascadeDelete: true);
        }
    }
}

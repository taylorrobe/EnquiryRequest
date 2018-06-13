namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changetoquoteacceptdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Quotes", "AcceptedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Quotes", "AcceptedDate", c => c.DateTime(nullable: false));
        }
    }
}

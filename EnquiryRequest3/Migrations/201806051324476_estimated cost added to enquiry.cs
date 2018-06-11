namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class estimatedcostaddedtoenquiry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enquiries", "EstimatedCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enquiries", "EstimatedCost");
        }
    }
}

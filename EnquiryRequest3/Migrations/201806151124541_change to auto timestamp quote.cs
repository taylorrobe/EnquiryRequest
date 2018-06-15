namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changetoautotimestampquote : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Enquiries", "EnquiryDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Enquiries", "EnquiryDate", c => c.DateTime(nullable: false));
        }
    }
}

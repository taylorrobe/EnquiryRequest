namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeenquirycode : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Enquiries", "Code", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Enquiries", "Code", c => c.String(nullable: false, maxLength: 100));
        }
    }
}

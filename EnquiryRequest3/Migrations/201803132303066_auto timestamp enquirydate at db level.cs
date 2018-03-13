namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class autotimestampenquirydateatdblevel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Quotes", "EnquiryId", "dbo.Enquiries");
            DropPrimaryKey("dbo.Enquiries");
            AlterColumn("dbo.Enquiries", "EnquiryId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Enquiries", "EnquiryDate", c => c.DateTime(nullable: false));
            AddPrimaryKey("dbo.Enquiries", "EnquiryId");
            AddForeignKey("dbo.Quotes", "EnquiryId", "dbo.Enquiries", "EnquiryId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Quotes", "EnquiryId", "dbo.Enquiries");
            DropPrimaryKey("dbo.Enquiries");
            AlterColumn("dbo.Enquiries", "EnquiryDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Enquiries", "EnquiryId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Enquiries", "EnquiryId");
            AddForeignKey("dbo.Quotes", "EnquiryId", "dbo.Enquiries", "EnquiryId", cascadeDelete: true);
        }
    }
}

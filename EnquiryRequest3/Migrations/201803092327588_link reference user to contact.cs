namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class linkreferenceusertocontact : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Enquiries", "ContactId", "dbo.Contacts");
            DropPrimaryKey("dbo.Contacts");
            AlterColumn("dbo.Contacts", "ContactId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Contacts", "ContactId");
            CreateIndex("dbo.Contacts", "ContactId");
            AddForeignKey("dbo.Contacts", "ContactId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Enquiries", "ContactId", "dbo.Contacts", "ContactId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Enquiries", "ContactId", "dbo.Contacts");
            DropForeignKey("dbo.Contacts", "ContactId", "dbo.AspNetUsers");
            DropIndex("dbo.Contacts", new[] { "ContactId" });
            DropPrimaryKey("dbo.Contacts");
            AlterColumn("dbo.Contacts", "ContactId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Contacts", "ContactId");
            AddForeignKey("dbo.Enquiries", "ContactId", "dbo.Contacts", "ContactId", cascadeDelete: true);
        }
    }
}

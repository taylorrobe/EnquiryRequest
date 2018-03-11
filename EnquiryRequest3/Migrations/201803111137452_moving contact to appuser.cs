namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class movingcontacttoappuser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Forename", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.AspNetUsers", "Surname", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.AspNetUsers", "Address1", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.AspNetUsers", "Address2", c => c.String(maxLength: 100));
            AddColumn("dbo.AspNetUsers", "Address3", c => c.String(maxLength: 100));
            AddColumn("dbo.AspNetUsers", "PostCode", c => c.String(nullable: false, maxLength: 10));
            AddColumn("dbo.AspNetUsers", "OrganisationId", c => c.Int());
            AddColumn("dbo.AspNetUsers", "DefaultInvoicingEmail", c => c.String(maxLength: 100));
            AlterColumn("dbo.Contacts", "Email", c => c.String());
            AlterColumn("dbo.Contacts", "PhoneNumber", c => c.String());
            CreateIndex("dbo.AspNetUsers", "OrganisationId");
            AddForeignKey("dbo.AspNetUsers", "OrganisationId", "dbo.Organisations", "OrganisationId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.AspNetUsers", new[] { "OrganisationId" });
            AlterColumn("dbo.Contacts", "PhoneNumber", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Contacts", "Email", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.AspNetUsers", "DefaultInvoicingEmail");
            DropColumn("dbo.AspNetUsers", "OrganisationId");
            DropColumn("dbo.AspNetUsers", "PostCode");
            DropColumn("dbo.AspNetUsers", "Address3");
            DropColumn("dbo.AspNetUsers", "Address2");
            DropColumn("dbo.AspNetUsers", "Address1");
            DropColumn("dbo.AspNetUsers", "Surname");
            DropColumn("dbo.AspNetUsers", "Forename");
        }
    }
}

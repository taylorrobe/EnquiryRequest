namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deletedcontact : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Contacts", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Contacts", "ContactId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Enquiries", "ContactId", "dbo.Contacts");
            DropIndex("dbo.Contacts", new[] { "ContactId" });
            DropIndex("dbo.Contacts", new[] { "OrganisationId" });
            DropIndex("dbo.Enquiries", new[] { "ContactId" });
            AddColumn("dbo.Enquiries", "ApplicationUserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Enquiries", "ApplicationUserId");
            AddForeignKey("dbo.Enquiries", "ApplicationUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            DropColumn("dbo.Enquiries", "ContactId");
            DropTable("dbo.Contacts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        ContactId = c.Int(nullable: false),
                        Forename = c.String(nullable: false, maxLength: 100),
                        Surname = c.String(nullable: false, maxLength: 100),
                        Address1 = c.String(nullable: false, maxLength: 100),
                        Address2 = c.String(maxLength: 100),
                        Address3 = c.String(maxLength: 100),
                        PostCode = c.String(nullable: false, maxLength: 10),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        OrganisationId = c.Int(),
                        DefaultInvoicingEmail = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ContactId);
            
            AddColumn("dbo.Enquiries", "ContactId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Enquiries", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Enquiries", new[] { "ApplicationUserId" });
            DropColumn("dbo.Enquiries", "ApplicationUserId");
            CreateIndex("dbo.Enquiries", "ContactId");
            CreateIndex("dbo.Contacts", "OrganisationId");
            CreateIndex("dbo.Contacts", "ContactId");
            AddForeignKey("dbo.Enquiries", "ContactId", "dbo.Contacts", "ContactId", cascadeDelete: true);
            AddForeignKey("dbo.Contacts", "ContactId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Contacts", "OrganisationId", "dbo.Organisations", "OrganisationId");
        }
    }
}

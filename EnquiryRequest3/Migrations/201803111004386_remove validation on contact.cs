namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removevalidationoncontact : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contacts", "Forename", c => c.String());
            AlterColumn("dbo.Contacts", "Surname", c => c.String());
            AlterColumn("dbo.Contacts", "Address1", c => c.String());
            AlterColumn("dbo.Contacts", "Address2", c => c.String());
            AlterColumn("dbo.Contacts", "Address3", c => c.String());
            AlterColumn("dbo.Contacts", "PostCode", c => c.String());
            AlterColumn("dbo.Contacts", "Email", c => c.String());
            AlterColumn("dbo.Contacts", "PhoneNumber", c => c.String());
            AlterColumn("dbo.Contacts", "DefaultInvoicingEmail", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Contacts", "DefaultInvoicingEmail", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Contacts", "PhoneNumber", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Contacts", "Email", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Contacts", "PostCode", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.Contacts", "Address3", c => c.String(maxLength: 100));
            AlterColumn("dbo.Contacts", "Address2", c => c.String(maxLength: 100));
            AlterColumn("dbo.Contacts", "Address1", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Contacts", "Surname", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Contacts", "Forename", c => c.String(nullable: false, maxLength: 100));
        }
    }
}

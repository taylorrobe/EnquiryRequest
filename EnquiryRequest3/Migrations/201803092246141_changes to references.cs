namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changestoreferences : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Enquiries", name: "Contact_ContactId", newName: "ContactId");
            RenameColumn(table: "dbo.Enquiries", name: "SearchType_SearchTypeId", newName: "SearchTypeId");
            RenameIndex(table: "dbo.Enquiries", name: "IX_Contact_ContactId", newName: "IX_ContactId");
            RenameIndex(table: "dbo.Enquiries", name: "IX_SearchType_SearchTypeId", newName: "IX_SearchTypeId");
            DropColumn("dbo.Invoices", "PaymentMethodId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoices", "PaymentMethodId", c => c.Int());
            RenameIndex(table: "dbo.Enquiries", name: "IX_SearchTypeId", newName: "IX_SearchType_SearchTypeId");
            RenameIndex(table: "dbo.Enquiries", name: "IX_ContactId", newName: "IX_Contact_ContactId");
            RenameColumn(table: "dbo.Enquiries", name: "SearchTypeId", newName: "SearchType_SearchTypeId");
            RenameColumn(table: "dbo.Enquiries", name: "ContactId", newName: "Contact_ContactId");
        }
    }
}

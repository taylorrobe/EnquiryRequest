namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Boundaries",
                c => new
                    {
                        BoundaryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Area = c.Geometry(nullable: false),
                    })
                .PrimaryKey(t => t.BoundaryId);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        ContactId = c.Int(nullable: false, identity: true),
                        Forename = c.String(nullable: false, maxLength: 100),
                        Surname = c.String(nullable: false, maxLength: 100),
                        Address1 = c.String(nullable: false, maxLength: 100),
                        Address2 = c.String(maxLength: 100),
                        Address3 = c.String(maxLength: 100),
                        PostCode = c.String(nullable: false, maxLength: 10),
                        Email = c.String(nullable: false, maxLength: 100),
                        PhoneNumber = c.String(nullable: false, maxLength: 20),
                        OrganisationId = c.Int(),
                        DefaultInvoicingEmail = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.ContactId)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.Organisations",
                c => new
                    {
                        OrganisationId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.OrganisationId);
            
            CreateTable(
                "dbo.Enquiries",
                c => new
                    {
                        EnquiryId = c.Int(nullable: false),
                        Code = c.String(nullable: false, maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 100),
                        InvoiceEmail = c.String(nullable: false, maxLength: 100),
                        SearchArea = c.Geometry(nullable: false),
                        NoOfYears = c.Int(nullable: false),
                        JobNumber = c.String(maxLength: 100),
                        Agency = c.String(maxLength: 100),
                        AgencyContact = c.String(maxLength: 100),
                        DataUsedFor = c.String(nullable: false, maxLength: 100),
                        Citations = c.Boolean(nullable: false),
                        GisKml = c.Boolean(nullable: false),
                        Express = c.Boolean(nullable: false),
                        EnquiryDate = c.DateTime(nullable: false),
                        Comment = c.String(),
                        AddedToRersDate = c.DateTime(),
                        DataCleanedDate = c.DateTime(),
                        ReporCompleteDate = c.DateTime(),
                        DocumentsCleanedDate = c.DateTime(),
                        EnquiryDeliveredDate = c.DateTime(),
                        AdminComment = c.String(),
                        InvoiceId = c.Int(),
                        Contact_ContactId = c.Int(nullable: false),
                        SearchType_SearchTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EnquiryId)
                .ForeignKey("dbo.Contacts", t => t.Contact_ContactId, cascadeDelete: true)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId)
                .ForeignKey("dbo.SearchTypes", t => t.SearchType_SearchTypeId, cascadeDelete: true)
                .Index(t => t.InvoiceId)
                .Index(t => t.Contact_ContactId)
                .Index(t => t.SearchType_SearchTypeId);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        InvoiceId = c.Int(nullable: false),
                        Code = c.String(nullable: false, maxLength: 100),
                        Amount = c.Double(nullable: false),
                        PONumber = c.String(maxLength: 100),
                        InvoiceDate = c.DateTime(nullable: false),
                        PaidDate = c.DateTime(nullable: false),
                        PaymentMethodId = c.Int(),
                        PaymentMethod = c.Int(nullable: false),
                        ChequeNumber = c.String(maxLength: 100),
                        InSlipNumber = c.String(maxLength: 100),
                        RemittanceReference = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.InvoiceId);
            
            CreateTable(
                "dbo.InvoiceReminders",
                c => new
                    {
                        InvoiceReminderId = c.Int(nullable: false),
                        InvoiceReminderDate = c.DateTime(nullable: false),
                        InvoiceReminderTypeId = c.Int(nullable: false),
                        Invoice_InvoiceId = c.Int(),
                    })
                .PrimaryKey(t => t.InvoiceReminderId)
                .ForeignKey("dbo.InvoiceReminderTypes", t => t.InvoiceReminderTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Invoices", t => t.Invoice_InvoiceId)
                .Index(t => t.InvoiceReminderTypeId)
                .Index(t => t.Invoice_InvoiceId);
            
            CreateTable(
                "dbo.InvoiceReminderTypes",
                c => new
                    {
                        InvoiceReminderTypeId = c.Int(nullable: false, identity: true),
                        ReminderSubject = c.String(nullable: false, maxLength: 100),
                        ReminderBody = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.InvoiceReminderTypeId);
            
            CreateTable(
                "dbo.Quotes",
                c => new
                    {
                        QuoteId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, storeType: "money"),
                        QuotedDate = c.DateTime(nullable: false),
                        AcceptedDate = c.DateTime(nullable: false),
                        EnquiryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QuoteId)
                .ForeignKey("dbo.Enquiries", t => t.EnquiryId, cascadeDelete: true)
                .Index(t => t.EnquiryId);
            
            CreateTable(
                "dbo.SearchTypes",
                c => new
                    {
                        SearchTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.SearchTypeId);
            
            CreateTable(
                "dbo.LrcInfoes",
                c => new
                    {
                        LrcInfoId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        Area = c.Geometry(nullable: false),
                        CompanyNumber = c.String(maxLength: 100),
                        CharityNumber = c.String(maxLength: 100),
                        Address1 = c.String(nullable: false, maxLength: 100),
                        Address2 = c.String(maxLength: 100),
                        Address3 = c.String(maxLength: 100),
                        PostCode = c.String(nullable: false, maxLength: 10),
                        Phone = c.String(maxLength: 20),
                        Email = c.String(nullable: false, maxLength: 100),
                        Website = c.String(maxLength: 100),
                        PaymentTerms = c.String(nullable: false),
                        BankAccountName = c.String(nullable: false, maxLength: 100),
                        BankAccountSortCode = c.String(nullable: false),
                        BankAccountNumber = c.String(nullable: false),
                        InformationRequested = c.String(),
                        Declaration = c.String(),
                    })
                .PrimaryKey(t => t.LrcInfoId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Enquiries", "SearchType_SearchTypeId", "dbo.SearchTypes");
            DropForeignKey("dbo.Quotes", "EnquiryId", "dbo.Enquiries");
            DropForeignKey("dbo.Enquiries", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.InvoiceReminders", "Invoice_InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.InvoiceReminders", "InvoiceReminderTypeId", "dbo.InvoiceReminderTypes");
            DropForeignKey("dbo.Enquiries", "Contact_ContactId", "dbo.Contacts");
            DropForeignKey("dbo.Contacts", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Quotes", new[] { "EnquiryId" });
            DropIndex("dbo.InvoiceReminders", new[] { "Invoice_InvoiceId" });
            DropIndex("dbo.InvoiceReminders", new[] { "InvoiceReminderTypeId" });
            DropIndex("dbo.Enquiries", new[] { "SearchType_SearchTypeId" });
            DropIndex("dbo.Enquiries", new[] { "Contact_ContactId" });
            DropIndex("dbo.Enquiries", new[] { "InvoiceId" });
            DropIndex("dbo.Contacts", new[] { "OrganisationId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.LrcInfoes");
            DropTable("dbo.SearchTypes");
            DropTable("dbo.Quotes");
            DropTable("dbo.InvoiceReminderTypes");
            DropTable("dbo.InvoiceReminders");
            DropTable("dbo.Invoices");
            DropTable("dbo.Enquiries");
            DropTable("dbo.Organisations");
            DropTable("dbo.Contacts");
            DropTable("dbo.Boundaries");
        }
    }
}

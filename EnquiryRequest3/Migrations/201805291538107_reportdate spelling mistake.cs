namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reportdatespellingmistake : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enquiries", "ReportCompleteDate", c => c.DateTime());
            DropColumn("dbo.Enquiries", "ReporCompleteDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Enquiries", "ReporCompleteDate", c => c.DateTime());
            DropColumn("dbo.Enquiries", "ReportCompleteDate");
        }
    }
}

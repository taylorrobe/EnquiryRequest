namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatingemail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UnConfirmedEmail", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "UnConfirmedEmail");
        }
    }
}

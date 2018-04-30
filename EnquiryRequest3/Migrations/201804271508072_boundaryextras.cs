namespace EnquiryRequest3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class boundaryextras : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boundaries", "displayOnMap", c => c.Boolean(nullable: false));
            AddColumn("dbo.Boundaries", "isCoverageArea", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Boundaries", "isCoverageArea");
            DropColumn("dbo.Boundaries", "displayOnMap");
        }
    }
}

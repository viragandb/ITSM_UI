namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240425_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IncidentRequests", "BusinessImpact", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.IncidentRequests", "BusinessImpact");
        }
    }
}

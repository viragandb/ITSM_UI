namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251205_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAccessRequests", "PlannedSchedule", c => c.Int(nullable: false));
            DropColumn("dbo.RemoteAccessRequests", "DurationType");
            DropColumn("dbo.RemoteAccessRequests", "LocationType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RemoteAccessRequests", "LocationType", c => c.Int(nullable: false));
            AddColumn("dbo.RemoteAccessRequests", "DurationType", c => c.Int(nullable: false));
            DropColumn("dbo.RemoteAccessRequests", "PlannedSchedule");
        }
    }
}

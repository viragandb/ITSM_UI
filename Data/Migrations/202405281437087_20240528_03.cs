namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240528_03 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DeviceManagementRequests", "TeamId", "dbo.Teams");
            DropIndex("dbo.DeviceManagementRequests", new[] { "TeamId" });
            DropColumn("dbo.DeviceManagementRequests", "TeamId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DeviceManagementRequests", "TeamId", c => c.Long(nullable: false));
            CreateIndex("dbo.DeviceManagementRequests", "TeamId");
            AddForeignKey("dbo.DeviceManagementRequests", "TeamId", "dbo.Teams", "TeamId", cascadeDelete: true);
        }
    }
}

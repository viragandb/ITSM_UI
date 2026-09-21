namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240528_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "DeviceManagementType", c => c.Int(nullable: false));
            AddColumn("dbo.DeviceManagementRequests", "DeviceManagementType", c => c.Int(nullable: false));
            DropColumn("dbo.Assets", "Type");
            DropColumn("dbo.DeviceManagementRequests", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DeviceManagementRequests", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.Assets", "Type", c => c.Int(nullable: false));
            DropColumn("dbo.DeviceManagementRequests", "DeviceManagementType");
            DropColumn("dbo.Assets", "DeviceManagementType");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240528_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.DeviceManagementRequests", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeviceManagementRequests", "Type");
            DropColumn("dbo.Assets", "Type");
        }
    }
}

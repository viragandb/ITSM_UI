namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0725_02 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeviceAccesses",
                c => new
                    {
                        DeviceAccessId = c.Long(nullable: false, identity: true),
                        AccessRequestId = c.Long(nullable: false),
                        AssetId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeviceAccessId)
                .ForeignKey("dbo.AccessRequests", t => t.AccessRequestId)
                .ForeignKey("dbo.Assets", t => t.AssetId, cascadeDelete: true)
                .Index(t => t.AccessRequestId)
                .Index(t => t.AssetId);
            
            CreateTable(
                "dbo.DeviceAccessItems",
                c => new
                    {
                        DeviceAccessItemId = c.Long(nullable: false, identity: true),
                        DeviceAccessId = c.Long(nullable: false),
                        DeviceAccessItemTypeId = c.Long(nullable: false),
                        EndUserAcceptance = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeviceAccessItemId)
                .ForeignKey("dbo.DeviceAccesses", t => t.DeviceAccessId, cascadeDelete: true)
                .ForeignKey("dbo.DeviceAccessItemTypes", t => t.DeviceAccessItemTypeId, cascadeDelete: true)
                .Index(t => t.DeviceAccessId)
                .Index(t => t.DeviceAccessItemTypeId);
            
            CreateTable(
                "dbo.DeviceAccessItemTypes",
                c => new
                    {
                        DeviceAccessItemTypeId = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeviceAccessItemTypeId);
            
            AddColumn("dbo.AccessRequests", "DeviceAccessId", c => c.Long());
            CreateIndex("dbo.AccessRequests", "DeviceAccessId");
            AddForeignKey("dbo.AccessRequests", "DeviceAccessId", "dbo.DeviceAccesses", "DeviceAccessId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccessRequests", "DeviceAccessId", "dbo.DeviceAccesses");
            DropForeignKey("dbo.DeviceAccessItems", "DeviceAccessItemTypeId", "dbo.DeviceAccessItemTypes");
            DropForeignKey("dbo.DeviceAccessItems", "DeviceAccessId", "dbo.DeviceAccesses");
            DropForeignKey("dbo.DeviceAccesses", "AssetId", "dbo.Assets");
            DropForeignKey("dbo.DeviceAccesses", "AccessRequestId", "dbo.AccessRequests");
            DropIndex("dbo.DeviceAccessItems", new[] { "DeviceAccessItemTypeId" });
            DropIndex("dbo.DeviceAccessItems", new[] { "DeviceAccessId" });
            DropIndex("dbo.DeviceAccesses", new[] { "AssetId" });
            DropIndex("dbo.DeviceAccesses", new[] { "AccessRequestId" });
            DropIndex("dbo.AccessRequests", new[] { "DeviceAccessId" });
            DropColumn("dbo.AccessRequests", "DeviceAccessId");
            DropTable("dbo.DeviceAccessItemTypes");
            DropTable("dbo.DeviceAccessItems");
            DropTable("dbo.DeviceAccesses");
        }
    }
}

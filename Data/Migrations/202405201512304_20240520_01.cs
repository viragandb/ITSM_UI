namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240520_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeviceManagementRequests",
                c => new
                    {
                        DeviceManagementRequestId = c.Long(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Subject = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        RequestedFor = c.String(nullable: false, maxLength: 20, unicode: false),
                        RequestedBy = c.String(nullable: false, maxLength: 20, unicode: false),
                        ApprovalBy = c.String(nullable: false, maxLength: 20, unicode: false),
                        RequestedDate = c.DateTime(nullable: false),
                        DurationType = c.Int(nullable: false),
                        ExpireDate = c.DateTime(nullable: false),
                        AssetId = c.Long(),
                        TeamId = c.Long(nullable: false),
                        Resolve = c.Double(nullable: false),
                        SpentTime = c.Double(nullable: false),
                        SpentTimeStatus = c.Double(nullable: false),
                        SpentTimeSync = c.DateTime(nullable: false),
                        IsTimeViolated = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeviceManagementRequestId)
                .ForeignKey("dbo.Users", t => t.ApprovalBy)
                .ForeignKey("dbo.Assets", t => t.AssetId)
                .ForeignKey("dbo.Users", t => t.RequestedBy)
                .ForeignKey("dbo.Users", t => t.RequestedFor)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.RequestedFor)
                .Index(t => t.RequestedBy)
                .Index(t => t.ApprovalBy)
                .Index(t => t.AssetId)
                .Index(t => t.TeamId);
            
            CreateTable(
                "dbo.DeviceManagementRequestLogs",
                c => new
                    {
                        DeviceManagementRequestLogId = c.Long(nullable: false, identity: true),
                        DeviceManagementRequestId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Comment = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeviceManagementRequestLogId)
                .ForeignKey("dbo.DeviceManagementRequests", t => t.DeviceManagementRequestId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.DeviceManagementRequestId)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.DeviceManagementRequestUpdates",
                c => new
                    {
                        DeviceManagementRequestUpdateId = c.Long(nullable: false, identity: true),
                        DeviceManagementRequestId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        SpentTime = c.Double(nullable: false),
                        Comment = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeviceManagementRequestUpdateId)
                .ForeignKey("dbo.DeviceManagementRequests", t => t.DeviceManagementRequestId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.DeviceManagementRequestId)
                .Index(t => t.UpdatedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DeviceManagementRequests", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.DeviceManagementRequests", "RequestedFor", "dbo.Users");
            DropForeignKey("dbo.DeviceManagementRequests", "RequestedBy", "dbo.Users");
            DropForeignKey("dbo.DeviceManagementRequestUpdates", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.DeviceManagementRequestUpdates", "DeviceManagementRequestId", "dbo.DeviceManagementRequests");
            DropForeignKey("dbo.DeviceManagementRequestLogs", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.DeviceManagementRequestLogs", "DeviceManagementRequestId", "dbo.DeviceManagementRequests");
            DropForeignKey("dbo.DeviceManagementRequests", "AssetId", "dbo.Assets");
            DropForeignKey("dbo.DeviceManagementRequests", "ApprovalBy", "dbo.Users");
            DropIndex("dbo.DeviceManagementRequestUpdates", new[] { "UpdatedBy" });
            DropIndex("dbo.DeviceManagementRequestUpdates", new[] { "DeviceManagementRequestId" });
            DropIndex("dbo.DeviceManagementRequestLogs", new[] { "UpdatedBy" });
            DropIndex("dbo.DeviceManagementRequestLogs", new[] { "DeviceManagementRequestId" });
            DropIndex("dbo.DeviceManagementRequests", new[] { "TeamId" });
            DropIndex("dbo.DeviceManagementRequests", new[] { "AssetId" });
            DropIndex("dbo.DeviceManagementRequests", new[] { "ApprovalBy" });
            DropIndex("dbo.DeviceManagementRequests", new[] { "RequestedBy" });
            DropIndex("dbo.DeviceManagementRequests", new[] { "RequestedFor" });
            DropTable("dbo.DeviceManagementRequestUpdates");
            DropTable("dbo.DeviceManagementRequestLogs");
            DropTable("dbo.DeviceManagementRequests");
        }
    }
}

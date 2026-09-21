namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240424_02 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IncidentRequestAssets",
                c => new
                    {
                        IncidentRequestAssetId = c.Long(nullable: false, identity: true),
                        IncidentRequestId = c.Long(nullable: false),
                        AssetId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IncidentRequestAssetId)
                .ForeignKey("dbo.Assets", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("dbo.IncidentRequests", t => t.IncidentRequestId, cascadeDelete: true)
                .Index(t => t.IncidentRequestId)
                .Index(t => t.AssetId);
            
            CreateTable(
                "dbo.IncidentRequests",
                c => new
                    {
                        IncidentRequestId = c.Long(nullable: false, identity: true),
                        Subject = c.String(nullable: false),
                        Description = c.String(),
                        Level01TeamId = c.Long(nullable: false),
                        PendingTeamId = c.Long(nullable: false),
                        RequestedBy = c.String(nullable: false, maxLength: 20, unicode: false),
                        RequestedDate = c.DateTime(nullable: false),
                        OccurredDate = c.DateTime(nullable: false),
                        Impact = c.Int(nullable: false),
                        Resolve = c.Double(nullable: false),
                        Status = c.Int(nullable: false),
                        SpentTime = c.Double(nullable: false),
                        SpentTimeStatus = c.Double(nullable: false),
                        SpentTimeSync = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 20, unicode: false),
                        RootCause = c.String(),
                        LessonsLearnt = c.String(),
                        CorrectiveAction = c.String(),
                        PreventiveAction = c.String(),
                        IsTimeViolated = c.Boolean(nullable: false),
                        VendorId = c.Long(),
                        VendorRefNo = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IncidentRequestId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Teams", t => t.Level01TeamId)
                .ForeignKey("dbo.Teams", t => t.PendingTeamId)
                .ForeignKey("dbo.Users", t => t.RequestedBy, cascadeDelete: true)
                .ForeignKey("dbo.Vendors", t => t.VendorId)
                .Index(t => t.Level01TeamId)
                .Index(t => t.PendingTeamId)
                .Index(t => t.RequestedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.VendorId);
            
            CreateTable(
                "dbo.IncidentRequestDocs",
                c => new
                    {
                        IncidentRequestDocId = c.Long(nullable: false, identity: true),
                        IncidentRequestId = c.Long(nullable: false),
                        DocumentName = c.String(nullable: false),
                        DocType = c.Int(nullable: false),
                        FileName = c.String(nullable: false),
                        FileUrl = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IncidentRequestDocId)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .ForeignKey("dbo.IncidentRequests", t => t.IncidentRequestId, cascadeDelete: true)
                .Index(t => t.IncidentRequestId)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.IncidentRequestLogs",
                c => new
                    {
                        IncidentRequestLogId = c.Long(nullable: false, identity: true),
                        IncidentRequestId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Comment = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IncidentRequestLogId)
                .ForeignKey("dbo.IncidentRequests", t => t.IncidentRequestId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.IncidentRequestId)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.IncidentRequestUpdates",
                c => new
                    {
                        IncidentRequestUpdateId = c.Long(nullable: false, identity: true),
                        IncidentRequestId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        SpentTime = c.Double(nullable: false),
                        Comment = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IncidentRequestUpdateId)
                .ForeignKey("dbo.IncidentRequests", t => t.IncidentRequestId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.IncidentRequestId)
                .Index(t => t.UpdatedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IncidentRequests", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.IncidentRequests", "RequestedBy", "dbo.Users");
            DropForeignKey("dbo.IncidentRequests", "PendingTeamId", "dbo.Teams");
            DropForeignKey("dbo.IncidentRequests", "Level01TeamId", "dbo.Teams");
            DropForeignKey("dbo.IncidentRequestUpdates", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.IncidentRequestUpdates", "IncidentRequestId", "dbo.IncidentRequests");
            DropForeignKey("dbo.IncidentRequestLogs", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.IncidentRequestLogs", "IncidentRequestId", "dbo.IncidentRequests");
            DropForeignKey("dbo.IncidentRequestDocs", "IncidentRequestId", "dbo.IncidentRequests");
            DropForeignKey("dbo.IncidentRequestDocs", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.IncidentRequests", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.IncidentRequestAssets", "IncidentRequestId", "dbo.IncidentRequests");
            DropForeignKey("dbo.IncidentRequestAssets", "AssetId", "dbo.Assets");
            DropIndex("dbo.IncidentRequestUpdates", new[] { "UpdatedBy" });
            DropIndex("dbo.IncidentRequestUpdates", new[] { "IncidentRequestId" });
            DropIndex("dbo.IncidentRequestLogs", new[] { "UpdatedBy" });
            DropIndex("dbo.IncidentRequestLogs", new[] { "IncidentRequestId" });
            DropIndex("dbo.IncidentRequestDocs", new[] { "UpdatedBy" });
            DropIndex("dbo.IncidentRequestDocs", new[] { "IncidentRequestId" });
            DropIndex("dbo.IncidentRequests", new[] { "VendorId" });
            DropIndex("dbo.IncidentRequests", new[] { "CreatedBy" });
            DropIndex("dbo.IncidentRequests", new[] { "RequestedBy" });
            DropIndex("dbo.IncidentRequests", new[] { "PendingTeamId" });
            DropIndex("dbo.IncidentRequests", new[] { "Level01TeamId" });
            DropIndex("dbo.IncidentRequestAssets", new[] { "AssetId" });
            DropIndex("dbo.IncidentRequestAssets", new[] { "IncidentRequestId" });
            DropTable("dbo.IncidentRequestUpdates");
            DropTable("dbo.IncidentRequestLogs");
            DropTable("dbo.IncidentRequestDocs");
            DropTable("dbo.IncidentRequests");
            DropTable("dbo.IncidentRequestAssets");
        }
    }
}

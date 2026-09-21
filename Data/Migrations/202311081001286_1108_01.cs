namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1108_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChangeAreas",
                c => new
                    {
                        ChangeAreaId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Icon = c.String(),
                        Color = c.String(),
                        TeamId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeAreaId)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.TeamId);
            
            CreateTable(
                "dbo.ChangeTypes",
                c => new
                    {
                        ChangeTypeId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        ChangeAreaId = c.Long(nullable: false),
                        Sla = c.Double(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeTypeId)
                .ForeignKey("dbo.ChangeAreas", t => t.ChangeAreaId, cascadeDelete: true)
                .Index(t => t.ChangeAreaId);
            
            CreateTable(
                "dbo.ChangeImplementDatas",
                c => new
                    {
                        ChangeImplementDataId = c.Long(nullable: false, identity: true),
                        ChangeRequestId = c.Long(nullable: false),
                        ChangeCategory = c.Int(nullable: false),
                        ChangeAreaId = c.Long(nullable: false),
                        PlannedChange = c.String(nullable: false),
                        RiskIdentification = c.String(nullable: false),
                        RollbackPlan = c.String(nullable: false),
                        RollbackTestResults = c.String(),
                        RiskAssessment = c.String(),
                        IsDowntimeRequired = c.Boolean(nullable: false),
                        Downtime = c.String(),
                        DowntimeImpactedAreas = c.String(),
                        DowntimePlannedDate = c.DateTime(nullable: false),
                        DowntimeTimePeriod = c.String(),
                        SendDowntimeAlert = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeImplementDataId)
                .ForeignKey("dbo.ChangeAreas", t => t.ChangeAreaId, cascadeDelete: true)
                .ForeignKey("dbo.ChangeRequests", t => t.ChangeRequestId)
                .Index(t => t.ChangeRequestId)
                .Index(t => t.ChangeAreaId);
            
            CreateTable(
                "dbo.ChangeRequests",
                c => new
                    {
                        ChangeRequestId = c.Long(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Subject = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Justification = c.String(nullable: false),
                        RequestedBy = c.String(nullable: false, maxLength: 20, unicode: false),
                        ApprovalBy = c.String(nullable: false, maxLength: 20, unicode: false),
                        RequestedDate = c.DateTime(nullable: false),
                        DurationType = c.Int(nullable: false),
                        ExpireDate = c.DateTime(nullable: false),
                        TeamId = c.Long(),
                        ImplementedBy = c.String(maxLength: 20, unicode: false),
                        Resolve = c.Double(nullable: false),
                        SpentTime = c.Double(nullable: false),
                        SpentTimeStatus = c.Double(nullable: false),
                        SpentTimeSync = c.DateTime(nullable: false),
                        ChangeImplementDataId = c.Long(),
                        IsTimeViolated = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeRequestId)
                .ForeignKey("dbo.Users", t => t.ApprovalBy)
                .ForeignKey("dbo.ChangeImplementDatas", t => t.ChangeImplementDataId)
                .ForeignKey("dbo.Users", t => t.ImplementedBy)
                .ForeignKey("dbo.Users", t => t.RequestedBy, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId)
                .Index(t => t.RequestedBy)
                .Index(t => t.ApprovalBy)
                .Index(t => t.TeamId)
                .Index(t => t.ImplementedBy)
                .Index(t => t.ChangeImplementDataId);
            
            CreateTable(
                "dbo.ChangeRequestLogs",
                c => new
                    {
                        ChangeRequestLogId = c.Long(nullable: false, identity: true),
                        ChangeRequestId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Comment = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeRequestLogId)
                .ForeignKey("dbo.ChangeRequests", t => t.ChangeRequestId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.ChangeRequestId)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.ChangeRequestUpdates",
                c => new
                    {
                        ChangeRequestUpdateId = c.Long(nullable: false, identity: true),
                        ChangeRequestId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        SpentTime = c.Double(nullable: false),
                        Comment = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeRequestUpdateId)
                .ForeignKey("dbo.ChangeRequests", t => t.ChangeRequestId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.ChangeRequestId)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.ChangeRequestTasks",
                c => new
                    {
                        ChangeRequestTaskId = c.Long(nullable: false, identity: true),
                        ChangeImplementDataId = c.Long(nullable: false),
                        ChangeTypeId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeRequestTaskId)
                .ForeignKey("dbo.ChangeImplementDatas", t => t.ChangeImplementDataId, cascadeDelete: true)
                .ForeignKey("dbo.ChangeTypes", t => t.ChangeTypeId)
                .Index(t => t.ChangeImplementDataId)
                .Index(t => t.ChangeTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChangeRequestTasks", "ChangeTypeId", "dbo.ChangeTypes");
            DropForeignKey("dbo.ChangeRequestTasks", "ChangeImplementDataId", "dbo.ChangeImplementDatas");
            DropForeignKey("dbo.ChangeImplementDatas", "ChangeRequestId", "dbo.ChangeRequests");
            DropForeignKey("dbo.ChangeRequests", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.ChangeRequests", "RequestedBy", "dbo.Users");
            DropForeignKey("dbo.ChangeRequests", "ImplementedBy", "dbo.Users");
            DropForeignKey("dbo.ChangeRequestUpdates", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.ChangeRequestUpdates", "ChangeRequestId", "dbo.ChangeRequests");
            DropForeignKey("dbo.ChangeRequestLogs", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.ChangeRequestLogs", "ChangeRequestId", "dbo.ChangeRequests");
            DropForeignKey("dbo.ChangeRequests", "ChangeImplementDataId", "dbo.ChangeImplementDatas");
            DropForeignKey("dbo.ChangeRequests", "ApprovalBy", "dbo.Users");
            DropForeignKey("dbo.ChangeImplementDatas", "ChangeAreaId", "dbo.ChangeAreas");
            DropForeignKey("dbo.ChangeAreas", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.ChangeTypes", "ChangeAreaId", "dbo.ChangeAreas");
            DropIndex("dbo.ChangeRequestTasks", new[] { "ChangeTypeId" });
            DropIndex("dbo.ChangeRequestTasks", new[] { "ChangeImplementDataId" });
            DropIndex("dbo.ChangeRequestUpdates", new[] { "UpdatedBy" });
            DropIndex("dbo.ChangeRequestUpdates", new[] { "ChangeRequestId" });
            DropIndex("dbo.ChangeRequestLogs", new[] { "UpdatedBy" });
            DropIndex("dbo.ChangeRequestLogs", new[] { "ChangeRequestId" });
            DropIndex("dbo.ChangeRequests", new[] { "ChangeImplementDataId" });
            DropIndex("dbo.ChangeRequests", new[] { "ImplementedBy" });
            DropIndex("dbo.ChangeRequests", new[] { "TeamId" });
            DropIndex("dbo.ChangeRequests", new[] { "ApprovalBy" });
            DropIndex("dbo.ChangeRequests", new[] { "RequestedBy" });
            DropIndex("dbo.ChangeImplementDatas", new[] { "ChangeAreaId" });
            DropIndex("dbo.ChangeImplementDatas", new[] { "ChangeRequestId" });
            DropIndex("dbo.ChangeTypes", new[] { "ChangeAreaId" });
            DropIndex("dbo.ChangeAreas", new[] { "TeamId" });
            DropTable("dbo.ChangeRequestTasks");
            DropTable("dbo.ChangeRequestUpdates");
            DropTable("dbo.ChangeRequestLogs");
            DropTable("dbo.ChangeRequests");
            DropTable("dbo.ChangeImplementDatas");
            DropTable("dbo.ChangeTypes");
            DropTable("dbo.ChangeAreas");
        }
    }
}

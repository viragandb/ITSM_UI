namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251104_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RemoteAccessRequestLogs",
                c => new
                    {
                        RemoteAccessRequestLogId = c.Long(nullable: false, identity: true),
                        RemoteAccessRequestId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Comment = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RemoteAccessRequestLogId)
                .ForeignKey("dbo.RemoteAccessRequests", t => t.RemoteAccessRequestId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.RemoteAccessRequestId)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.RemoteAccessRequests",
                c => new
                    {
                        RemoteAccessRequestId = c.Long(nullable: false, identity: true),
                        DepartmentId = c.Long(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 20, unicode: false),
                        RequestedBy = c.String(nullable: false, maxLength: 20, unicode: false),
                        RequestedDate = c.DateTime(nullable: false),
                        ApprovalBy = c.String(nullable: false, maxLength: 20, unicode: false),
                        BusinessJustification = c.String(nullable: false),
                        BusinessImpact = c.String(nullable: false),
                        DurationType = c.Int(nullable: false),
                        AssetTypeId = c.Long(nullable: false),
                        AssetId = c.Long(nullable: false),
                        RemoteWorkLocation = c.String(),
                        RemoteAccessType = c.Int(nullable: false),
                        RequestFrom = c.DateTime(nullable: false),
                        RequestTo = c.DateTime(nullable: false),
                        LocationType = c.Int(nullable: false),
                        AdditionalJustification = c.String(),
                        Status = c.Int(nullable: false),
                        TeamId = c.Long(),
                        Resolve = c.Double(nullable: false),
                        SpentTime = c.Double(nullable: false),
                        SpentTimeStatus = c.Double(nullable: false),
                        SpentTimeSync = c.DateTime(nullable: false),
                        IsTimeViolated = c.Boolean(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RemoteAccessRequestId)
                .ForeignKey("dbo.Users", t => t.ApprovalBy)
                .ForeignKey("dbo.Assets", t => t.AssetId)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.Users", t => t.RequestedBy)
                .ForeignKey("dbo.Teams", t => t.TeamId)
                .Index(t => t.DepartmentId)
                .Index(t => t.CreatedBy)
                .Index(t => t.RequestedBy)
                .Index(t => t.ApprovalBy)
                .Index(t => t.AssetTypeId)
                .Index(t => t.AssetId)
                .Index(t => t.TeamId);
            
            CreateTable(
                "dbo.RemoteAccessRequestUpdates",
                c => new
                    {
                        RemoteAccessRequestUpdateId = c.Long(nullable: false, identity: true),
                        RemoteAccessRequestId = c.Long(nullable: false),
                        TeamId = c.Long(),
                        Comment = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        SpentTime = c.Double(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RemoteAccessRequestUpdateId)
                .ForeignKey("dbo.RemoteAccessRequests", t => t.RemoteAccessRequestId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.RemoteAccessRequestId)
                .Index(t => t.TeamId)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.RemoteAgreements",
                c => new
                    {
                        RemoteAgreementId = c.Int(nullable: false, identity: true),
                        RemoteAccessRequestId = c.Long(nullable: false),
                        PlannedSchedule = c.Int(nullable: false),
                        IsNoiseFree = c.Boolean(nullable: false),
                        WillTravelOverseas = c.Boolean(nullable: false),
                        DestinationCountry = c.String(),
                        DurationOfTravel = c.String(),
                        ReasonForTtravel = c.String(),
                        ConnectivityConform = c.Boolean(nullable: false),
                        AdherenceToInfoSecConfidentiality = c.Boolean(nullable: false),
                        Notifysupervisorofchanges = c.Boolean(nullable: false),
                        Preserveconfidentiality = c.Boolean(nullable: false),
                        DaysofWeek = c.String(),
                        PerformanceMonitoring = c.String(),
                        Date = c.DateTime(nullable: false),
                        EmployeeName = c.String(),
                        EmployeeAddress = c.String(),
                        Duration = c.String(),
                        WorkLocation = c.String(),
                        WorkHours = c.String(),
                        SignedbyEmployee = c.String(),
                        SignedbyEmployer = c.String(),
                        DateofSigning = c.DateTime(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RemoteAgreementId)
                .ForeignKey("dbo.RemoteAccessRequests", t => t.RemoteAccessRequestId, cascadeDelete: true)
                .Index(t => t.RemoteAccessRequestId);
            
            CreateTable(
                "dbo.RevokeRequests",
                c => new
                    {
                        RevokeRequestId = c.Long(nullable: false, identity: true),
                        RemoteAccessRequestId = c.Long(nullable: false),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        RevokedDate = c.DateTime(nullable: false),
                        Supervisor = c.String(),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RevokeRequestId)
                .ForeignKey("dbo.RemoteAccessRequests", t => t.RemoteAccessRequestId, cascadeDelete: true)
                .Index(t => t.RemoteAccessRequestId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RevokeRequests", "RemoteAccessRequestId", "dbo.RemoteAccessRequests");
            DropForeignKey("dbo.RemoteAgreements", "RemoteAccessRequestId", "dbo.RemoteAccessRequests");
            DropForeignKey("dbo.RemoteAccessRequestLogs", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.RemoteAccessRequests", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.RemoteAccessRequests", "RequestedBy", "dbo.Users");
            DropForeignKey("dbo.RemoteAccessRequestUpdates", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.RemoteAccessRequestUpdates", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.RemoteAccessRequestUpdates", "RemoteAccessRequestId", "dbo.RemoteAccessRequests");
            DropForeignKey("dbo.RemoteAccessRequestLogs", "RemoteAccessRequestId", "dbo.RemoteAccessRequests");
            DropForeignKey("dbo.RemoteAccessRequests", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.RemoteAccessRequests", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.RemoteAccessRequests", "AssetTypeId", "dbo.AssetTypes");
            DropForeignKey("dbo.RemoteAccessRequests", "AssetId", "dbo.Assets");
            DropForeignKey("dbo.RemoteAccessRequests", "ApprovalBy", "dbo.Users");
            DropIndex("dbo.RevokeRequests", new[] { "RemoteAccessRequestId" });
            DropIndex("dbo.RemoteAgreements", new[] { "RemoteAccessRequestId" });
            DropIndex("dbo.RemoteAccessRequestUpdates", new[] { "UpdatedBy" });
            DropIndex("dbo.RemoteAccessRequestUpdates", new[] { "TeamId" });
            DropIndex("dbo.RemoteAccessRequestUpdates", new[] { "RemoteAccessRequestId" });
            DropIndex("dbo.RemoteAccessRequests", new[] { "TeamId" });
            DropIndex("dbo.RemoteAccessRequests", new[] { "AssetId" });
            DropIndex("dbo.RemoteAccessRequests", new[] { "AssetTypeId" });
            DropIndex("dbo.RemoteAccessRequests", new[] { "ApprovalBy" });
            DropIndex("dbo.RemoteAccessRequests", new[] { "RequestedBy" });
            DropIndex("dbo.RemoteAccessRequests", new[] { "CreatedBy" });
            DropIndex("dbo.RemoteAccessRequests", new[] { "DepartmentId" });
            DropIndex("dbo.RemoteAccessRequestLogs", new[] { "UpdatedBy" });
            DropIndex("dbo.RemoteAccessRequestLogs", new[] { "RemoteAccessRequestId" });
            DropTable("dbo.RevokeRequests");
            DropTable("dbo.RemoteAgreements");
            DropTable("dbo.RemoteAccessRequestUpdates");
            DropTable("dbo.RemoteAccessRequests");
            DropTable("dbo.RemoteAccessRequestLogs");
        }
    }
}

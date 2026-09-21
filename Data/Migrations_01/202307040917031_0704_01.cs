namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0704_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccessRequests",
                c => new
                    {
                        AccessRequestId = c.Long(nullable: false, identity: true),
                        AccessRequestTypeId = c.Long(nullable: false),
                        WorkflowId = c.Long(nullable: false),
                        NextWorkflowlevel = c.Int(nullable: false),
                        TeamId = c.Long(),
                        Subject = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Justification = c.String(nullable: false),
                        BranchId = c.Long(nullable: false),
                        DepartmentId = c.Long(nullable: false),
                        CreatedBy = c.String(maxLength: 20, unicode: false),
                        RequestedBy = c.String(nullable: false, maxLength: 20, unicode: false),
                        RequestedDate = c.DateTime(nullable: false),
                        DurationType = c.Int(nullable: false),
                        ExpireDate = c.DateTime(nullable: false),
                        AccessType = c.Int(nullable: false),
                        LegalId = c.String(),
                        Organization = c.String(),
                        RespondTime = c.Double(nullable: false),
                        SpentTime = c.Double(nullable: false),
                        SpentTimeStatus = c.Double(nullable: false),
                        SpentTimeSync = c.DateTime(nullable: false),
                        SystemAccessId = c.Long(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                        SystemAccess_SystemAccessId = c.Long(),
                    })
                .PrimaryKey(t => t.AccessRequestId)
                .ForeignKey("dbo.AccessRequestTypes", t => t.AccessRequestTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.RequestedBy, cascadeDelete: true)
                .ForeignKey("dbo.SystemAccesses", t => t.SystemAccess_SystemAccessId)
                .ForeignKey("dbo.Teams", t => t.TeamId)
                .ForeignKey("dbo.Workflows", t => t.WorkflowId, cascadeDelete: true)
                .Index(t => t.AccessRequestTypeId)
                .Index(t => t.WorkflowId)
                .Index(t => t.TeamId)
                .Index(t => t.BranchId)
                .Index(t => t.DepartmentId)
                .Index(t => t.CreatedBy)
                .Index(t => t.RequestedBy)
                .Index(t => t.SystemAccess_SystemAccessId);
            
            CreateTable(
                "dbo.AccessRequestTypes",
                c => new
                    {
                        AccessRequestTypeId = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        WorkflowId = c.Long(nullable: false),
                        ITOnly = c.Boolean(nullable: false),
                        External = c.Boolean(nullable: false),
                        Internal = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AccessRequestTypeId)
                .ForeignKey("dbo.Workflows", t => t.WorkflowId)
                .Index(t => t.WorkflowId);
            
            CreateTable(
                "dbo.Workflows",
                c => new
                    {
                        WorkflowId = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.WorkflowId);
            
            CreateTable(
                "dbo.SystemAccesses",
                c => new
                    {
                        SystemAccessId = c.Long(nullable: false, identity: true),
                        AccessRequestId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SystemAccessId)
                .ForeignKey("dbo.AccessRequests", t => t.AccessRequestId)
                .Index(t => t.AccessRequestId);
            
            CreateTable(
                "dbo.SystemAccessItems",
                c => new
                    {
                        SystemAccessItemId = c.Long(nullable: false, identity: true),
                        SystemAccessId = c.Long(nullable: false),
                        SystemEnvironmentId = c.Long(nullable: false),
                        Description = c.String(nullable: false),
                        Justification = c.String(nullable: false),
                        PrivilegeLevel = c.String(),
                        MonitoredBy = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SystemAccessItemId)
                .ForeignKey("dbo.SystemAccesses", t => t.SystemAccessId, cascadeDelete: true)
                .ForeignKey("dbo.SystemEnvironments", t => t.SystemEnvironmentId, cascadeDelete: true)
                .Index(t => t.SystemAccessId)
                .Index(t => t.SystemEnvironmentId);
            
            CreateTable(
                "dbo.SystemEnvironments",
                c => new
                    {
                        SystemEnvironmentId = c.Long(nullable: false, identity: true),
                        EnvironmentType = c.Int(nullable: false),
                        AssetTypeId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SystemEnvironmentId)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId, cascadeDelete: true)
                .Index(t => t.AssetTypeId);
            
            CreateTable(
                "dbo.WorkflowLevels",
                c => new
                    {
                        WorkflowLevelId = c.Long(nullable: false, identity: true),
                        WorkflowId = c.Long(nullable: false),
                        TeamId = c.Long(nullable: false),
                        LevelNo = c.Int(nullable: false),
                        WorkflowLevelType = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.WorkflowLevelId)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .ForeignKey("dbo.Workflows", t => t.WorkflowId, cascadeDelete: true)
                .Index(t => t.WorkflowId)
                .Index(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkflowLevels", "WorkflowId", "dbo.Workflows");
            DropForeignKey("dbo.WorkflowLevels", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.AccessRequests", "WorkflowId", "dbo.Workflows");
            DropForeignKey("dbo.AccessRequests", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.AccessRequests", "SystemAccess_SystemAccessId", "dbo.SystemAccesses");
            DropForeignKey("dbo.SystemAccessItems", "SystemEnvironmentId", "dbo.SystemEnvironments");
            DropForeignKey("dbo.SystemEnvironments", "AssetTypeId", "dbo.AssetTypes");
            DropForeignKey("dbo.SystemAccessItems", "SystemAccessId", "dbo.SystemAccesses");
            DropForeignKey("dbo.SystemAccesses", "AccessRequestId", "dbo.AccessRequests");
            DropForeignKey("dbo.AccessRequests", "RequestedBy", "dbo.Users");
            DropForeignKey("dbo.AccessRequests", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.AccessRequests", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.AccessRequests", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.AccessRequests", "AccessRequestTypeId", "dbo.AccessRequestTypes");
            DropForeignKey("dbo.AccessRequestTypes", "WorkflowId", "dbo.Workflows");
            DropIndex("dbo.WorkflowLevels", new[] { "TeamId" });
            DropIndex("dbo.WorkflowLevels", new[] { "WorkflowId" });
            DropIndex("dbo.SystemEnvironments", new[] { "AssetTypeId" });
            DropIndex("dbo.SystemAccessItems", new[] { "SystemEnvironmentId" });
            DropIndex("dbo.SystemAccessItems", new[] { "SystemAccessId" });
            DropIndex("dbo.SystemAccesses", new[] { "AccessRequestId" });
            DropIndex("dbo.AccessRequestTypes", new[] { "WorkflowId" });
            DropIndex("dbo.AccessRequests", new[] { "SystemAccess_SystemAccessId" });
            DropIndex("dbo.AccessRequests", new[] { "RequestedBy" });
            DropIndex("dbo.AccessRequests", new[] { "CreatedBy" });
            DropIndex("dbo.AccessRequests", new[] { "DepartmentId" });
            DropIndex("dbo.AccessRequests", new[] { "BranchId" });
            DropIndex("dbo.AccessRequests", new[] { "TeamId" });
            DropIndex("dbo.AccessRequests", new[] { "WorkflowId" });
            DropIndex("dbo.AccessRequests", new[] { "AccessRequestTypeId" });
            DropTable("dbo.WorkflowLevels");
            DropTable("dbo.SystemEnvironments");
            DropTable("dbo.SystemAccessItems");
            DropTable("dbo.SystemAccesses");
            DropTable("dbo.Workflows");
            DropTable("dbo.AccessRequestTypes");
            DropTable("dbo.AccessRequests");
        }
    }
}

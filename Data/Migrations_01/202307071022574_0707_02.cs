namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0707_02 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccessRequestLogs",
                c => new
                    {
                        AccessRequestLogId = c.Long(nullable: false, identity: true),
                        AccessRequestId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Comment = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AccessRequestLogId)
                .ForeignKey("dbo.AccessRequests", t => t.AccessRequestId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.AccessRequestId)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.AccessRequestUpdates",
                c => new
                    {
                        AccessRequestUpdateId = c.Long(nullable: false, identity: true),
                        AccessRequestId = c.Long(nullable: false),
                        TeamId = c.Long(),
                        Status = c.Int(nullable: false),
                        SpentTime = c.Double(nullable: false),
                        Comment = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AccessRequestUpdateId)
                .ForeignKey("dbo.AccessRequests", t => t.AccessRequestId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.AccessRequestId)
                .Index(t => t.TeamId)
                .Index(t => t.UpdatedBy);
            
            AddColumn("dbo.AccessRequests", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.AccessRequests", "Resolve", c => c.Double(nullable: false));
            AddColumn("dbo.AccessRequests", "IsTimeViolated", c => c.Boolean(nullable: false));
            AddColumn("dbo.Workflows", "WorkflowType", c => c.Int(nullable: false));
            DropColumn("dbo.AccessRequests", "RespondTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccessRequests", "RespondTime", c => c.Double(nullable: false));
            DropForeignKey("dbo.AccessRequestLogs", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.AccessRequestUpdates", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.AccessRequestUpdates", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.AccessRequestUpdates", "AccessRequestId", "dbo.AccessRequests");
            DropForeignKey("dbo.AccessRequestLogs", "AccessRequestId", "dbo.AccessRequests");
            DropIndex("dbo.AccessRequestUpdates", new[] { "UpdatedBy" });
            DropIndex("dbo.AccessRequestUpdates", new[] { "TeamId" });
            DropIndex("dbo.AccessRequestUpdates", new[] { "AccessRequestId" });
            DropIndex("dbo.AccessRequestLogs", new[] { "UpdatedBy" });
            DropIndex("dbo.AccessRequestLogs", new[] { "AccessRequestId" });
            DropColumn("dbo.Workflows", "WorkflowType");
            DropColumn("dbo.AccessRequests", "IsTimeViolated");
            DropColumn("dbo.AccessRequests", "Resolve");
            DropColumn("dbo.AccessRequests", "Status");
            DropTable("dbo.AccessRequestUpdates");
            DropTable("dbo.AccessRequestLogs");
        }
    }
}

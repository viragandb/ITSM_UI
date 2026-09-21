namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251203_01 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RemoteAccessRequests", "AssetTypeId", "dbo.AssetTypes");
            DropIndex("dbo.RemoteAccessRequests", new[] { "AssetTypeId" });
            CreateTable(
                "dbo.RemoteAccessRequiredSystems",
                c => new
                    {
                        RemoteAccessRequiredSystemsId = c.Long(nullable: false, identity: true),
                        RemoteAccessRequestId = c.Long(nullable: false),
                        AssetTypeId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        RemoteAccessRequest_RemoteAccessRequestId = c.Long(),
                    })
                .PrimaryKey(t => t.RemoteAccessRequiredSystemsId)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId)
                .ForeignKey("dbo.RemoteAccessRequests", t => t.RemoteAccessRequestId)
                .ForeignKey("dbo.RemoteAccessRequests", t => t.RemoteAccessRequest_RemoteAccessRequestId)
                .Index(t => t.RemoteAccessRequestId)
                .Index(t => t.AssetTypeId)
                .Index(t => t.RemoteAccessRequest_RemoteAccessRequestId);
            
            AddColumn("dbo.RemoteAccessRequestLogs", "Description", c => c.String(nullable: false));
            AddColumn("dbo.RemoteAccessRequests", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.RemoteAccessRequests", "EndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.RemoteAccessRequests", "BelowAMJustification", c => c.String());
            AddColumn("dbo.RemoteAccessRequests", "ToleranceLimitExceededJustification", c => c.String());
            AddColumn("dbo.RemoteAccessRequests", "IsToleranceLimitExceeded", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAccessRequestUpdates", "Description", c => c.String(nullable: false));
            DropColumn("dbo.RemoteAccessRequestLogs", "Comment");
            DropColumn("dbo.RemoteAccessRequests", "AssetTypeId");
            DropColumn("dbo.RemoteAccessRequests", "RequestFrom");
            DropColumn("dbo.RemoteAccessRequests", "RequestTo");
            DropColumn("dbo.RemoteAccessRequests", "AdditionalJustification");
            DropColumn("dbo.RemoteAccessRequests", "IsActive");
            DropColumn("dbo.RemoteAccessRequestUpdates", "Comment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RemoteAccessRequestUpdates", "Comment", c => c.String(nullable: false));
            AddColumn("dbo.RemoteAccessRequests", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAccessRequests", "AdditionalJustification", c => c.String());
            AddColumn("dbo.RemoteAccessRequests", "RequestTo", c => c.DateTime(nullable: false));
            AddColumn("dbo.RemoteAccessRequests", "RequestFrom", c => c.DateTime(nullable: false));
            AddColumn("dbo.RemoteAccessRequests", "AssetTypeId", c => c.Long(nullable: false));
            AddColumn("dbo.RemoteAccessRequestLogs", "Comment", c => c.String(nullable: false));
            DropForeignKey("dbo.RemoteAccessRequiredSystems", "RemoteAccessRequest_RemoteAccessRequestId", "dbo.RemoteAccessRequests");
            DropForeignKey("dbo.RemoteAccessRequiredSystems", "RemoteAccessRequestId", "dbo.RemoteAccessRequests");
            DropForeignKey("dbo.RemoteAccessRequiredSystems", "AssetTypeId", "dbo.AssetTypes");
            DropIndex("dbo.RemoteAccessRequiredSystems", new[] { "RemoteAccessRequest_RemoteAccessRequestId" });
            DropIndex("dbo.RemoteAccessRequiredSystems", new[] { "AssetTypeId" });
            DropIndex("dbo.RemoteAccessRequiredSystems", new[] { "RemoteAccessRequestId" });
            DropColumn("dbo.RemoteAccessRequestUpdates", "Description");
            DropColumn("dbo.RemoteAccessRequests", "IsToleranceLimitExceeded");
            DropColumn("dbo.RemoteAccessRequests", "ToleranceLimitExceededJustification");
            DropColumn("dbo.RemoteAccessRequests", "BelowAMJustification");
            DropColumn("dbo.RemoteAccessRequests", "EndDate");
            DropColumn("dbo.RemoteAccessRequests", "StartDate");
            DropColumn("dbo.RemoteAccessRequestLogs", "Description");
            DropTable("dbo.RemoteAccessRequiredSystems");
            CreateIndex("dbo.RemoteAccessRequests", "AssetTypeId");
            AddForeignKey("dbo.RemoteAccessRequests", "AssetTypeId", "dbo.AssetTypes", "AssetTypeId");
        }
    }
}

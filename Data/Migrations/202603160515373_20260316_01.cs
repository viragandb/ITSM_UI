namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260316_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DisposalItems",
                c => new
                    {
                        DisposalItemId = c.Long(nullable: false, identity: true),
                        DisposalRequestId = c.Long(nullable: false),
                        AssetId = c.Long(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DisposalItemId)
                .ForeignKey("dbo.Assets", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("dbo.DisposalRequests", t => t.DisposalRequestId, cascadeDelete: true)
                .Index(t => t.DisposalRequestId)
                .Index(t => t.AssetId);
            
            CreateTable(
                "dbo.DisposalRequests",
                c => new
                    {
                        DisposalRequestId = c.Long(nullable: false, identity: true),
                        Title = c.String(),
                        Status = c.Int(nullable: false),
                        RequestCreatedDate = c.DateTime(nullable: false),
                        BuyerName = c.String(),
                        SpendsSmartFilePath = c.String(),
                        ProcurementFilePath = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DisposalRequestId);
            
            CreateTable(
                "dbo.DisposalRequestLogs",
                c => new
                    {
                        DisposalRequestLogId = c.Long(nullable: false, identity: true),
                        DisposalRequestId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Description = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DisposalRequestLogId)
                .ForeignKey("dbo.DisposalRequests", t => t.DisposalRequestId, cascadeDelete: true)
                .Index(t => t.DisposalRequestId);
            
            CreateTable(
                "dbo.DisposalRequestUpdates",
                c => new
                    {
                        DisposalRequestUpdateId = c.Long(nullable: false, identity: true),
                        DisposalRequestId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Comment = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DisposalRequestUpdateId)
                .ForeignKey("dbo.DisposalRequests", t => t.DisposalRequestId, cascadeDelete: true)
                .Index(t => t.DisposalRequestId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DisposalRequestUpdates", "DisposalRequestId", "dbo.DisposalRequests");
            DropForeignKey("dbo.DisposalRequestLogs", "DisposalRequestId", "dbo.DisposalRequests");
            DropForeignKey("dbo.DisposalItems", "DisposalRequestId", "dbo.DisposalRequests");
            DropForeignKey("dbo.DisposalItems", "AssetId", "dbo.Assets");
            DropIndex("dbo.DisposalRequestUpdates", new[] { "DisposalRequestId" });
            DropIndex("dbo.DisposalRequestLogs", new[] { "DisposalRequestId" });
            DropIndex("dbo.DisposalItems", new[] { "AssetId" });
            DropIndex("dbo.DisposalItems", new[] { "DisposalRequestId" });
            DropTable("dbo.DisposalRequestUpdates");
            DropTable("dbo.DisposalRequestLogs");
            DropTable("dbo.DisposalRequests");
            DropTable("dbo.DisposalItems");
        }
    }
}

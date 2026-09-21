namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260515_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetLinkLogs",
                c => new
                    {
                        AssetLinkLogId = c.Long(nullable: false, identity: true),
                        ParentAssetId = c.Long(nullable: false),
                        ChildAssetId = c.Long(nullable: false),
                        ActionType = c.Int(nullable: false),
                        Description = c.String(),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        ChildAsset_AssetId = c.Long(),
                        ParentAsset_AssetId = c.Long(),
                    })
                .PrimaryKey(t => t.AssetLinkLogId)
                .ForeignKey("dbo.Assets", t => t.ChildAsset_AssetId)
                .ForeignKey("dbo.Assets", t => t.ParentAsset_AssetId)
                .Index(t => t.ChildAsset_AssetId)
                .Index(t => t.ParentAsset_AssetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetLinkLogs", "ParentAsset_AssetId", "dbo.Assets");
            DropForeignKey("dbo.AssetLinkLogs", "ChildAsset_AssetId", "dbo.Assets");
            DropIndex("dbo.AssetLinkLogs", new[] { "ParentAsset_AssetId" });
            DropIndex("dbo.AssetLinkLogs", new[] { "ChildAsset_AssetId" });
            DropTable("dbo.AssetLinkLogs");
        }
    }
}

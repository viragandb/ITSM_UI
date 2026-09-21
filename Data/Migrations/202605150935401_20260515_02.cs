namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260515_02 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AssetLinkLogs", new[] { "ChildAsset_AssetId" });
            DropIndex("dbo.AssetLinkLogs", new[] { "ParentAsset_AssetId" });
            DropColumn("dbo.AssetLinkLogs", "ChildAssetId");
            DropColumn("dbo.AssetLinkLogs", "ParentAssetId");
            RenameColumn(table: "dbo.AssetLinkLogs", name: "ChildAsset_AssetId", newName: "ChildAssetId");
            RenameColumn(table: "dbo.AssetLinkLogs", name: "ParentAsset_AssetId", newName: "ParentAssetId");
            AlterColumn("dbo.AssetLinkLogs", "ChildAssetId", c => c.Long(nullable: false));
            AlterColumn("dbo.AssetLinkLogs", "ParentAssetId", c => c.Long(nullable: false));
            CreateIndex("dbo.AssetLinkLogs", "ParentAssetId");
            CreateIndex("dbo.AssetLinkLogs", "ChildAssetId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AssetLinkLogs", new[] { "ChildAssetId" });
            DropIndex("dbo.AssetLinkLogs", new[] { "ParentAssetId" });
            AlterColumn("dbo.AssetLinkLogs", "ParentAssetId", c => c.Long());
            AlterColumn("dbo.AssetLinkLogs", "ChildAssetId", c => c.Long());
            RenameColumn(table: "dbo.AssetLinkLogs", name: "ParentAssetId", newName: "ParentAsset_AssetId");
            RenameColumn(table: "dbo.AssetLinkLogs", name: "ChildAssetId", newName: "ChildAsset_AssetId");
            AddColumn("dbo.AssetLinkLogs", "ParentAssetId", c => c.Long(nullable: false));
            AddColumn("dbo.AssetLinkLogs", "ChildAssetId", c => c.Long(nullable: false));
            CreateIndex("dbo.AssetLinkLogs", "ParentAsset_AssetId");
            CreateIndex("dbo.AssetLinkLogs", "ChildAsset_AssetId");
        }
    }
}

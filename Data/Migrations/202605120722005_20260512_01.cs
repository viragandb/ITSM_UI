namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260512_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetLinks",
                c => new
                    {
                        AssetLinkId = c.Long(nullable: false, identity: true),
                        ParentAssetId = c.Long(nullable: false),
                        ChildAssetId = c.Long(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AssetLinkId)
                .ForeignKey("dbo.Assets", t => t.ChildAssetId)
                .ForeignKey("dbo.Assets", t => t.ParentAssetId)
                .Index(t => t.ParentAssetId)
                .Index(t => t.ChildAssetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetLinks", "ParentAssetId", "dbo.Assets");
            DropForeignKey("dbo.AssetLinks", "ChildAssetId", "dbo.Assets");
            DropIndex("dbo.AssetLinks", new[] { "ChildAssetId" });
            DropIndex("dbo.AssetLinks", new[] { "ParentAssetId" });
            DropTable("dbo.AssetLinks");
        }
    }
}

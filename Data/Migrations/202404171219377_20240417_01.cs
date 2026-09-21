namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240417_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChangeRequestAssets",
                c => new
                    {
                        ChangeRequestAssetId = c.Long(nullable: false, identity: true),
                        ChangeRequestId = c.Long(nullable: false),
                        AssetId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeRequestAssetId)
                .ForeignKey("dbo.Assets", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("dbo.ChangeRequests", t => t.ChangeRequestId, cascadeDelete: true)
                .Index(t => t.ChangeRequestId)
                .Index(t => t.AssetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChangeRequestAssets", "ChangeRequestId", "dbo.ChangeRequests");
            DropForeignKey("dbo.ChangeRequestAssets", "AssetId", "dbo.Assets");
            DropIndex("dbo.ChangeRequestAssets", new[] { "AssetId" });
            DropIndex("dbo.ChangeRequestAssets", new[] { "ChangeRequestId" });
            DropTable("dbo.ChangeRequestAssets");
        }
    }
}

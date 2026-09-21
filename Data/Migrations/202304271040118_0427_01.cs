namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0427_01 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ItemAssets", "AssetCategory");
            DropColumn("dbo.ItemAssets", "AssetCode");
            DropColumn("dbo.ItemAssets", "AssetName");
            DropColumn("dbo.ItemAssets", "SerialNo");
            DropColumn("dbo.ItemAssets", "ModelName");
            DropColumn("dbo.ItemAssets", "Description");
            DropColumn("dbo.ItemAssets", "Location");
            DropColumn("dbo.ItemAssets", "Company");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemAssets", "Company", c => c.String());
            AddColumn("dbo.ItemAssets", "Location", c => c.String());
            AddColumn("dbo.ItemAssets", "Description", c => c.String());
            AddColumn("dbo.ItemAssets", "ModelName", c => c.String());
            AddColumn("dbo.ItemAssets", "SerialNo", c => c.String());
            AddColumn("dbo.ItemAssets", "AssetName", c => c.String());
            AddColumn("dbo.ItemAssets", "AssetCode", c => c.String());
            AddColumn("dbo.ItemAssets", "AssetCategory", c => c.Int(nullable: false));
        }
    }
}

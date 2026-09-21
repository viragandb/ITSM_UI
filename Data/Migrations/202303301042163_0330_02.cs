namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0330_02 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TempAssets",
                c => new
                    {
                        TempAssetId = c.Long(nullable: false, identity: true),
                        EmpNo = c.String(),
                        AssetCategoryId = c.Long(nullable: false),
                        AssetCategoryName = c.String(),
                        AssetTypeId = c.Long(nullable: false),
                        AssetTypeName = c.String(),
                        AssetMakeId = c.Long(nullable: false),
                        AssetMakeName = c.String(),
                        ModelName = c.String(),
                        AssetNo = c.String(nullable: false),
                        Barcode = c.String(),
                        SerialNo = c.String(),
                        PurchasePrice = c.Double(nullable: false),
                        WarrantyPeriod = c.Int(nullable: false),
                        ToBeReturned = c.Int(nullable: false),
                        Description = c.String(),
                        IsCritical = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TempAssetId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TempAssets");
        }
    }
}

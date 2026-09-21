namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0519_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VendorSLAs",
                c => new
                    {
                        VendorSLAId = c.Long(nullable: false, identity: true),
                        VendorId = c.Long(nullable: false),
                        AssetTypeId = c.Long(nullable: false),
                        SLA = c.Double(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.VendorSLAId)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Vendors", t => t.VendorId, cascadeDelete: true)
                .Index(t => t.VendorId)
                .Index(t => t.AssetTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorSLAs", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.VendorSLAs", "AssetTypeId", "dbo.AssetTypes");
            DropIndex("dbo.VendorSLAs", new[] { "AssetTypeId" });
            DropIndex("dbo.VendorSLAs", new[] { "VendorId" });
            DropTable("dbo.VendorSLAs");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0126_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetMakes",
                c => new
                    {
                        AssetMakeId = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AssetMakeId);
            
            AddColumn("dbo.Assets", "AssetTypeId", c => c.Long(nullable: false));
            AddColumn("dbo.Assets", "VendorId", c => c.Long(nullable: false));
            AddColumn("dbo.Assets", "AssetMakeId", c => c.Long(nullable: false));
            AddColumn("dbo.Assets", "AssetNo", c => c.String());
            AddColumn("dbo.Assets", "AssetCode", c => c.String());
            AddColumn("dbo.Assets", "BranchId", c => c.Long(nullable: false));
            AddColumn("dbo.Assets", "DepartmentId", c => c.Long(nullable: false));
            AddColumn("dbo.AssetTypes", "IsAccessory", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Assets", "AssetName", c => c.String());
            CreateIndex("dbo.Assets", "AssetTypeId");
            CreateIndex("dbo.Assets", "VendorId");
            CreateIndex("dbo.Assets", "AssetMakeId");
            CreateIndex("dbo.Assets", "BranchId");
            CreateIndex("dbo.Assets", "DepartmentId");
            AddForeignKey("dbo.Assets", "AssetMakeId", "dbo.AssetMakes", "AssetMakeId", cascadeDelete: true);
            AddForeignKey("dbo.Assets", "AssetTypeId", "dbo.AssetTypes", "AssetTypeId");
            AddForeignKey("dbo.Assets", "BranchId", "dbo.Branches", "BranchId", cascadeDelete: true);
            AddForeignKey("dbo.Assets", "DepartmentId", "dbo.Departments", "DepartmentId", cascadeDelete: true);
            AddForeignKey("dbo.Assets", "VendorId", "dbo.Vendors", "VendorId", cascadeDelete: true);
            DropColumn("dbo.Assets", "ItemId");
            DropColumn("dbo.Assets", "TicketId");
            DropColumn("dbo.Assets", "AssetTypeName");
            DropColumn("dbo.Assets", "Location");
            DropColumn("dbo.Assets", "Company");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assets", "Company", c => c.String());
            AddColumn("dbo.Assets", "Location", c => c.String());
            AddColumn("dbo.Assets", "AssetTypeName", c => c.String());
            AddColumn("dbo.Assets", "TicketId", c => c.Long(nullable: false));
            AddColumn("dbo.Assets", "ItemId", c => c.Long(nullable: false));
            DropForeignKey("dbo.Assets", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.Assets", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Assets", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Assets", "AssetTypeId", "dbo.AssetTypes");
            DropForeignKey("dbo.Assets", "AssetMakeId", "dbo.AssetMakes");
            DropIndex("dbo.Assets", new[] { "DepartmentId" });
            DropIndex("dbo.Assets", new[] { "BranchId" });
            DropIndex("dbo.Assets", new[] { "AssetMakeId" });
            DropIndex("dbo.Assets", new[] { "VendorId" });
            DropIndex("dbo.Assets", new[] { "AssetTypeId" });
            AlterColumn("dbo.Assets", "AssetName", c => c.String(nullable: false));
            DropColumn("dbo.AssetTypes", "IsAccessory");
            DropColumn("dbo.Assets", "DepartmentId");
            DropColumn("dbo.Assets", "BranchId");
            DropColumn("dbo.Assets", "AssetCode");
            DropColumn("dbo.Assets", "AssetNo");
            DropColumn("dbo.Assets", "AssetMakeId");
            DropColumn("dbo.Assets", "VendorId");
            DropColumn("dbo.Assets", "AssetTypeId");
            DropTable("dbo.AssetMakes");
        }
    }
}

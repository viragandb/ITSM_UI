namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0404_01 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AssetTransferRequests", "Branch_BranchId", "dbo.Branches");
            DropForeignKey("dbo.AssetTransferRequests", "Department_DepartmentId", "dbo.Departments");
            DropIndex("dbo.AssetTransferRequests", new[] { "Branch_BranchId" });
            DropIndex("dbo.AssetTransferRequests", new[] { "Department_DepartmentId" });
            DropColumn("dbo.AssetTransferRequests", "BranchId");
            DropColumn("dbo.AssetTransferRequests", "DepartmentId");
            DropColumn("dbo.AssetTransferRequests", "Branch_BranchId");
            DropColumn("dbo.AssetTransferRequests", "Department_DepartmentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AssetTransferRequests", "Department_DepartmentId", c => c.Long());
            AddColumn("dbo.AssetTransferRequests", "Branch_BranchId", c => c.Long());
            AddColumn("dbo.AssetTransferRequests", "DepartmentId", c => c.Long(nullable: false));
            AddColumn("dbo.AssetTransferRequests", "BranchId", c => c.Long(nullable: false));
            CreateIndex("dbo.AssetTransferRequests", "Department_DepartmentId");
            CreateIndex("dbo.AssetTransferRequests", "Branch_BranchId");
            AddForeignKey("dbo.AssetTransferRequests", "Department_DepartmentId", "dbo.Departments", "DepartmentId");
            AddForeignKey("dbo.AssetTransferRequests", "Branch_BranchId", "dbo.Branches", "BranchId");
        }
    }
}

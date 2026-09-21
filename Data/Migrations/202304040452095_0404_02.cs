namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0404_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssetTransferRequests", "BranchId", c => c.Long(nullable: false));
            AddColumn("dbo.AssetTransferRequests", "DepartmentId", c => c.Long(nullable: false));
            CreateIndex("dbo.AssetTransferRequests", "BranchId");
            CreateIndex("dbo.AssetTransferRequests", "DepartmentId");
            AddForeignKey("dbo.AssetTransferRequests", "BranchId", "dbo.Branches", "BranchId");
            AddForeignKey("dbo.AssetTransferRequests", "DepartmentId", "dbo.Departments", "DepartmentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetTransferRequests", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.AssetTransferRequests", "BranchId", "dbo.Branches");
            DropIndex("dbo.AssetTransferRequests", new[] { "DepartmentId" });
            DropIndex("dbo.AssetTransferRequests", new[] { "BranchId" });
            DropColumn("dbo.AssetTransferRequests", "DepartmentId");
            DropColumn("dbo.AssetTransferRequests", "BranchId");
        }
    }
}

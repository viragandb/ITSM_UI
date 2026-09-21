namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0331_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetTransferRequests",
                c => new
                    {
                        AssetTransferRequestId = c.Long(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        TransactionType = c.Int(nullable: false),
                        InitiatedBy = c.String(maxLength: 20, unicode: false),
                        InitiatorComment = c.String(),
                        InitiatedDate = c.DateTime(nullable: false),
                        CompletedBy = c.String(maxLength: 20, unicode: false),
                        CompletedComment = c.String(),
                        CompletedDate = c.DateTime(nullable: false),
                        BranchIdFrom = c.Long(nullable: false),
                        DepartmentIdFrom = c.Long(nullable: false),
                        BranchId = c.Long(nullable: false),
                        DepartmentId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        Branch_BranchId = c.Long(),
                        Department_DepartmentId = c.Long(),
                    })
                .PrimaryKey(t => t.AssetTransferRequestId)
                .ForeignKey("dbo.Branches", t => t.Branch_BranchId)
                .ForeignKey("dbo.Branches", t => t.BranchIdFrom, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CompletedBy)
                .ForeignKey("dbo.Departments", t => t.Department_DepartmentId)
                .ForeignKey("dbo.Departments", t => t.DepartmentIdFrom, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.InitiatedBy)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.InitiatedBy)
                .Index(t => t.CompletedBy)
                .Index(t => t.BranchIdFrom)
                .Index(t => t.DepartmentIdFrom)
                .Index(t => t.UpdatedBy)
                .Index(t => t.Branch_BranchId)
                .Index(t => t.Department_DepartmentId);
            
            CreateTable(
                "dbo.TransferItems",
                c => new
                    {
                        TransferItemId = c.Long(nullable: false, identity: true),
                        AssetTransferRequestId = c.Long(nullable: false),
                        AssetId = c.Long(nullable: false),
                        AssignedTo = c.String(),
                        AssetTransferRequest_AssetTransferRequestId = c.Long(),
                    })
                .PrimaryKey(t => t.TransferItemId)
                .ForeignKey("dbo.Assets", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("dbo.AssetTransferRequests", t => t.AssetTransferRequestId)
                .ForeignKey("dbo.AssetTransferRequests", t => t.AssetTransferRequest_AssetTransferRequestId)
                .Index(t => t.AssetTransferRequestId)
                .Index(t => t.AssetId)
                .Index(t => t.AssetTransferRequest_AssetTransferRequestId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetTransferRequests", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.TransferItems", "AssetTransferRequest_AssetTransferRequestId", "dbo.AssetTransferRequests");
            DropForeignKey("dbo.TransferItems", "AssetTransferRequestId", "dbo.AssetTransferRequests");
            DropForeignKey("dbo.TransferItems", "AssetId", "dbo.Assets");
            DropForeignKey("dbo.AssetTransferRequests", "InitiatedBy", "dbo.Users");
            DropForeignKey("dbo.AssetTransferRequests", "DepartmentIdFrom", "dbo.Departments");
            DropForeignKey("dbo.AssetTransferRequests", "Department_DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.AssetTransferRequests", "CompletedBy", "dbo.Users");
            DropForeignKey("dbo.AssetTransferRequests", "BranchIdFrom", "dbo.Branches");
            DropForeignKey("dbo.AssetTransferRequests", "Branch_BranchId", "dbo.Branches");
            DropIndex("dbo.TransferItems", new[] { "AssetTransferRequest_AssetTransferRequestId" });
            DropIndex("dbo.TransferItems", new[] { "AssetId" });
            DropIndex("dbo.TransferItems", new[] { "AssetTransferRequestId" });
            DropIndex("dbo.AssetTransferRequests", new[] { "Department_DepartmentId" });
            DropIndex("dbo.AssetTransferRequests", new[] { "Branch_BranchId" });
            DropIndex("dbo.AssetTransferRequests", new[] { "UpdatedBy" });
            DropIndex("dbo.AssetTransferRequests", new[] { "DepartmentIdFrom" });
            DropIndex("dbo.AssetTransferRequests", new[] { "BranchIdFrom" });
            DropIndex("dbo.AssetTransferRequests", new[] { "CompletedBy" });
            DropIndex("dbo.AssetTransferRequests", new[] { "InitiatedBy" });
            DropTable("dbo.TransferItems");
            DropTable("dbo.AssetTransferRequests");
        }
    }
}

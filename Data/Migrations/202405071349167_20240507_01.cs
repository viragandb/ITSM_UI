namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240507_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetVerificationItems",
                c => new
                    {
                        AssetVerificationItemId = c.Long(nullable: false, identity: true),
                        AssetVerificationRequestId = c.Long(nullable: false),
                        AssetId = c.Long(nullable: false),
                        IsAvailable = c.Boolean(nullable: false),
                        Comment = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AssetVerificationItemId)
                .ForeignKey("dbo.Assets", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("dbo.AssetVerificationRequests", t => t.AssetVerificationRequestId, cascadeDelete: true)
                .Index(t => t.AssetVerificationRequestId)
                .Index(t => t.AssetId);
            
            CreateTable(
                "dbo.AssetVerificationRequests",
                c => new
                    {
                        AssetVerificationRequestId = c.Long(nullable: false, identity: true),
                        BranchId = c.Long(nullable: false),
                        DepartmentId = c.Long(nullable: false),
                        VerificationDate = c.DateTime(nullable: false),
                        AssetCategory = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        RequestedBy = c.String(nullable: false, maxLength: 20, unicode: false),
                        RequestedDate = c.DateTime(nullable: false),
                        VerifiedBy = c.String(maxLength: 20, unicode: false),
                        VerifiedDate = c.DateTime(nullable: false),
                        Comment = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AssetVerificationRequestId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.Users", t => t.RequestedBy, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.VerifiedBy)
                .Index(t => t.BranchId)
                .Index(t => t.DepartmentId)
                .Index(t => t.RequestedBy)
                .Index(t => t.VerifiedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetVerificationRequests", "VerifiedBy", "dbo.Users");
            DropForeignKey("dbo.AssetVerificationRequests", "RequestedBy", "dbo.Users");
            DropForeignKey("dbo.AssetVerificationRequests", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.AssetVerificationRequests", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.AssetVerificationItems", "AssetVerificationRequestId", "dbo.AssetVerificationRequests");
            DropForeignKey("dbo.AssetVerificationItems", "AssetId", "dbo.Assets");
            DropIndex("dbo.AssetVerificationRequests", new[] { "VerifiedBy" });
            DropIndex("dbo.AssetVerificationRequests", new[] { "RequestedBy" });
            DropIndex("dbo.AssetVerificationRequests", new[] { "DepartmentId" });
            DropIndex("dbo.AssetVerificationRequests", new[] { "BranchId" });
            DropIndex("dbo.AssetVerificationItems", new[] { "AssetId" });
            DropIndex("dbo.AssetVerificationItems", new[] { "AssetVerificationRequestId" });
            DropTable("dbo.AssetVerificationRequests");
            DropTable("dbo.AssetVerificationItems");
        }
    }
}

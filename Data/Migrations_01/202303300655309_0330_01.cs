namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0330_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetLogs",
                c => new
                    {
                        AssetLogId = c.Long(nullable: false, identity: true),
                        AssetId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Comment = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AssetLogId)
                .ForeignKey("dbo.Assets", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.AssetId)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.AssetTransactionLogs",
                c => new
                    {
                        AssetTransactionLogId = c.Long(nullable: false, identity: true),
                        AssetId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        TransactionType = c.Int(nullable: false),
                        Comment = c.String(nullable: false),
                        AssignedTo = c.String(maxLength: 20, unicode: false),
                        BranchId = c.Long(nullable: false),
                        DepartmentId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AssetTransactionLogId)
                .ForeignKey("dbo.Assets", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.AssignedTo)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.AssetId)
                .Index(t => t.AssignedTo)
                .Index(t => t.BranchId)
                .Index(t => t.DepartmentId)
                .Index(t => t.UpdatedBy);
            
            AddColumn("dbo.Assets", "Code", c => c.String());
            AddColumn("dbo.Assets", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Assets", "AssignedTo", c => c.String(maxLength: 20, unicode: false));
            AddColumn("dbo.GRNotes", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.GRNotes", "UpdatedBy", c => c.String());
            AddColumn("dbo.GRNotes", "UpdatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Assets", "ToBeReturnedDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Assets", "AssignedTo");
            AddForeignKey("dbo.Assets", "AssignedTo", "dbo.Users", "EmpNo");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetLogs", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.Assets", "AssignedTo", "dbo.Users");
            DropForeignKey("dbo.AssetTransactionLogs", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.AssetTransactionLogs", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.AssetTransactionLogs", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.AssetTransactionLogs", "AssignedTo", "dbo.Users");
            DropForeignKey("dbo.AssetTransactionLogs", "AssetId", "dbo.Assets");
            DropForeignKey("dbo.AssetLogs", "AssetId", "dbo.Assets");
            DropIndex("dbo.AssetTransactionLogs", new[] { "UpdatedBy" });
            DropIndex("dbo.AssetTransactionLogs", new[] { "DepartmentId" });
            DropIndex("dbo.AssetTransactionLogs", new[] { "BranchId" });
            DropIndex("dbo.AssetTransactionLogs", new[] { "AssignedTo" });
            DropIndex("dbo.AssetTransactionLogs", new[] { "AssetId" });
            DropIndex("dbo.Assets", new[] { "AssignedTo" });
            DropIndex("dbo.AssetLogs", new[] { "UpdatedBy" });
            DropIndex("dbo.AssetLogs", new[] { "AssetId" });
            AlterColumn("dbo.Assets", "ToBeReturnedDate", c => c.DateTime());
            DropColumn("dbo.GRNotes", "UpdatedDate");
            DropColumn("dbo.GRNotes", "UpdatedBy");
            DropColumn("dbo.GRNotes", "IsDeleted");
            DropColumn("dbo.Assets", "AssignedTo");
            DropColumn("dbo.Assets", "Status");
            DropColumn("dbo.Assets", "Code");
            DropTable("dbo.AssetTransactionLogs");
            DropTable("dbo.AssetLogs");
        }
    }
}

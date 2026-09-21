namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0403_02 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserBranches",
                c => new
                    {
                        UserBranchId = c.Long(nullable: false, identity: true),
                        EmpNo = c.String(nullable: false, maxLength: 20, unicode: false),
                        BranchId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserBranchId)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.EmpNo, cascadeDelete: true)
                .Index(t => t.EmpNo)
                .Index(t => t.BranchId);
            
            CreateTable(
                "dbo.UserDepartments",
                c => new
                    {
                        UserDepartmentId = c.Long(nullable: false, identity: true),
                        EmpNo = c.String(nullable: false, maxLength: 20, unicode: false),
                        DepartmentId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserDepartmentId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.EmpNo, cascadeDelete: true)
                .Index(t => t.EmpNo)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserDepartments", "EmpNo", "dbo.Users");
            DropForeignKey("dbo.UserDepartments", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.UserBranches", "EmpNo", "dbo.Users");
            DropForeignKey("dbo.UserBranches", "BranchId", "dbo.Branches");
            DropIndex("dbo.UserDepartments", new[] { "DepartmentId" });
            DropIndex("dbo.UserDepartments", new[] { "EmpNo" });
            DropIndex("dbo.UserBranches", new[] { "BranchId" });
            DropIndex("dbo.UserBranches", new[] { "EmpNo" });
            DropTable("dbo.UserDepartments");
            DropTable("dbo.UserBranches");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0404_03 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BranchDepartments",
                c => new
                    {
                        BranchDepartmentId = c.Long(nullable: false, identity: true),
                        BranchId = c.Long(nullable: false),
                        DepartmentId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BranchDepartmentId)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.BranchId)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BranchDepartments", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.BranchDepartments", "BranchId", "dbo.Branches");
            DropIndex("dbo.BranchDepartments", new[] { "DepartmentId" });
            DropIndex("dbo.BranchDepartments", new[] { "BranchId" });
            DropTable("dbo.BranchDepartments");
        }
    }
}

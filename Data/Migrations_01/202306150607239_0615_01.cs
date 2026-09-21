namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0615_01 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Users", "DepartmentId");
            AddForeignKey("dbo.Users", "DepartmentId", "dbo.Departments", "DepartmentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.Users", new[] { "DepartmentId" });
        }
    }
}

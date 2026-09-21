namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0622_03 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "DepartmentId", c => c.Long(nullable: true));
            CreateIndex("dbo.Tickets", "DepartmentId");
            AddForeignKey("dbo.Tickets", "DepartmentId", "dbo.Departments", "DepartmentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.Tickets", new[] { "DepartmentId" });
            DropColumn("dbo.Tickets", "DepartmentId");
        }
    }
}

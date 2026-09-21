namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0720_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessApplications", "AccessPrivilegeCategoryId", c => c.Long(nullable: false));
            CreateIndex("dbo.AccessApplications", "AccessPrivilegeCategoryId");
            AddForeignKey("dbo.AccessApplications", "AccessPrivilegeCategoryId", "dbo.AccessPrivilegeCategories", "AccessPrivilegeCategoryId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccessApplications", "AccessPrivilegeCategoryId", "dbo.AccessPrivilegeCategories");
            DropIndex("dbo.AccessApplications", new[] { "AccessPrivilegeCategoryId" });
            DropColumn("dbo.AccessApplications", "AccessPrivilegeCategoryId");
        }
    }
}

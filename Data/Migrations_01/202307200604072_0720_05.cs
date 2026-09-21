namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0720_05 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AccessApplications", "AccessPrivilegeCategory_AccessPrivilegeCategoryId", "dbo.AccessPrivilegeCategories");
            DropForeignKey("dbo.AccessApplications", "AccessPrivilegeCategoryId", "dbo.AccessPrivilegeCategories");
            DropIndex("dbo.AccessApplications", new[] { "AccessPrivilegeCategoryId" });
            DropIndex("dbo.AccessApplications", new[] { "AccessPrivilegeCategory_AccessPrivilegeCategoryId" });
            DropColumn("dbo.AccessApplications", "AccessPrivilegeCategoryId");
            DropColumn("dbo.AccessApplications", "AccessPrivilegeCategory_AccessPrivilegeCategoryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccessApplications", "AccessPrivilegeCategory_AccessPrivilegeCategoryId", c => c.Long());
            AddColumn("dbo.AccessApplications", "AccessPrivilegeCategoryId", c => c.Long(nullable: false));
            CreateIndex("dbo.AccessApplications", "AccessPrivilegeCategory_AccessPrivilegeCategoryId");
            CreateIndex("dbo.AccessApplications", "AccessPrivilegeCategoryId");
            AddForeignKey("dbo.AccessApplications", "AccessPrivilegeCategoryId", "dbo.AccessPrivilegeCategories", "AccessPrivilegeCategoryId");
            AddForeignKey("dbo.AccessApplications", "AccessPrivilegeCategory_AccessPrivilegeCategoryId", "dbo.AccessPrivilegeCategories", "AccessPrivilegeCategoryId");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0720_06 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessApplications", "AccessPrivilegeCategoryId", c => c.Long(nullable: false));
            AddColumn("dbo.AccessApplications", "AccessPrivilegeCategory_AccessPrivilegeCategoryId", c => c.Long());
            CreateIndex("dbo.AccessApplications", "AccessPrivilegeCategoryId");
            CreateIndex("dbo.AccessApplications", "AccessPrivilegeCategory_AccessPrivilegeCategoryId");
            AddForeignKey("dbo.AccessApplications", "AccessPrivilegeCategory_AccessPrivilegeCategoryId", "dbo.AccessPrivilegeCategories", "AccessPrivilegeCategoryId");
            AddForeignKey("dbo.AccessApplications", "AccessPrivilegeCategoryId", "dbo.AccessPrivilegeCategories", "AccessPrivilegeCategoryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccessApplications", "AccessPrivilegeCategoryId", "dbo.AccessPrivilegeCategories");
            DropForeignKey("dbo.AccessApplications", "AccessPrivilegeCategory_AccessPrivilegeCategoryId", "dbo.AccessPrivilegeCategories");
            DropIndex("dbo.AccessApplications", new[] { "AccessPrivilegeCategory_AccessPrivilegeCategoryId" });
            DropIndex("dbo.AccessApplications", new[] { "AccessPrivilegeCategoryId" });
            DropColumn("dbo.AccessApplications", "AccessPrivilegeCategory_AccessPrivilegeCategoryId");
            DropColumn("dbo.AccessApplications", "AccessPrivilegeCategoryId");
        }
    }
}

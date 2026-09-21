namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0718_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccessApplications",
                c => new
                    {
                        AccessApplicationId = c.Long(nullable: false, identity: true),
                        AssetTypeId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AccessApplicationId)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId, cascadeDelete: true)
                .Index(t => t.AssetTypeId);
            
            CreateTable(
                "dbo.AccessLevels",
                c => new
                    {
                        AccessLevelId = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AccessLevelId);
            
            CreateTable(
                "dbo.AccessPrivilegeCategories",
                c => new
                    {
                        AccessPrivilegeCategoryId = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AccessPrivilegeCategoryId);
            
            CreateTable(
                "dbo.UserPrivilegeAccesses",
                c => new
                    {
                        UserPrivilegeAccessId = c.Long(nullable: false, identity: true),
                        AccessRequestId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserPrivilegeAccessId)
                .ForeignKey("dbo.AccessRequests", t => t.AccessRequestId)
                .Index(t => t.AccessRequestId);
            
            CreateTable(
                "dbo.UserPrivilegeAccessItems",
                c => new
                    {
                        UserPrivilegeAccessItemId = c.Long(nullable: false, identity: true),
                        UserPrivilegeAccessId = c.Long(nullable: false),
                        AccessPrivilegeCategoryId = c.Long(nullable: false),
                        AccessApplicationId = c.Long(nullable: false),
                        AccessLevelId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserPrivilegeAccessItemId)
                .ForeignKey("dbo.AccessApplications", t => t.AccessApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.AccessLevels", t => t.AccessLevelId, cascadeDelete: true)
                .ForeignKey("dbo.AccessPrivilegeCategories", t => t.AccessPrivilegeCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.UserPrivilegeAccesses", t => t.UserPrivilegeAccessId, cascadeDelete: true)
                .Index(t => t.UserPrivilegeAccessId)
                .Index(t => t.AccessPrivilegeCategoryId)
                .Index(t => t.AccessApplicationId)
                .Index(t => t.AccessLevelId);
            
            AddColumn("dbo.AccessRequests", "UserPrivilegeAccessId", c => c.Long());
            CreateIndex("dbo.AccessRequests", "UserPrivilegeAccessId");
            AddForeignKey("dbo.AccessRequests", "UserPrivilegeAccessId", "dbo.UserPrivilegeAccesses", "UserPrivilegeAccessId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccessRequests", "UserPrivilegeAccessId", "dbo.UserPrivilegeAccesses");
            DropForeignKey("dbo.UserPrivilegeAccessItems", "UserPrivilegeAccessId", "dbo.UserPrivilegeAccesses");
            DropForeignKey("dbo.UserPrivilegeAccessItems", "AccessPrivilegeCategoryId", "dbo.AccessPrivilegeCategories");
            DropForeignKey("dbo.UserPrivilegeAccessItems", "AccessLevelId", "dbo.AccessLevels");
            DropForeignKey("dbo.UserPrivilegeAccessItems", "AccessApplicationId", "dbo.AccessApplications");
            DropForeignKey("dbo.UserPrivilegeAccesses", "AccessRequestId", "dbo.AccessRequests");
            DropForeignKey("dbo.AccessApplications", "AssetTypeId", "dbo.AssetTypes");
            DropIndex("dbo.UserPrivilegeAccessItems", new[] { "AccessLevelId" });
            DropIndex("dbo.UserPrivilegeAccessItems", new[] { "AccessApplicationId" });
            DropIndex("dbo.UserPrivilegeAccessItems", new[] { "AccessPrivilegeCategoryId" });
            DropIndex("dbo.UserPrivilegeAccessItems", new[] { "UserPrivilegeAccessId" });
            DropIndex("dbo.UserPrivilegeAccesses", new[] { "AccessRequestId" });
            DropIndex("dbo.AccessRequests", new[] { "UserPrivilegeAccessId" });
            DropIndex("dbo.AccessApplications", new[] { "AssetTypeId" });
            DropColumn("dbo.AccessRequests", "UserPrivilegeAccessId");
            DropTable("dbo.UserPrivilegeAccessItems");
            DropTable("dbo.UserPrivilegeAccesses");
            DropTable("dbo.AccessPrivilegeCategories");
            DropTable("dbo.AccessLevels");
            DropTable("dbo.AccessApplications");
        }
    }
}

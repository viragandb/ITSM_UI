namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1114_02 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ChangeRequests", new[] { "TeamId" });
            AddColumn("dbo.ChangeRequests", "ChangeRequestCategoryId", c => c.Long(nullable: false));
            AlterColumn("dbo.ChangeRequests", "TeamId", c => c.Long(nullable: false));
            CreateIndex("dbo.ChangeRequests", "ChangeRequestCategoryId");
            CreateIndex("dbo.ChangeRequests", "TeamId");
            AddForeignKey("dbo.ChangeRequests", "ChangeRequestCategoryId", "dbo.ChangeRequestCategories", "ChangeRequestCategoryId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChangeRequests", "ChangeRequestCategoryId", "dbo.ChangeRequestCategories");
            DropIndex("dbo.ChangeRequests", new[] { "TeamId" });
            DropIndex("dbo.ChangeRequests", new[] { "ChangeRequestCategoryId" });
            AlterColumn("dbo.ChangeRequests", "TeamId", c => c.Long());
            DropColumn("dbo.ChangeRequests", "ChangeRequestCategoryId");
            CreateIndex("dbo.ChangeRequests", "TeamId");
        }
    }
}

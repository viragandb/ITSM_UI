namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1114_01 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChangeAreas", "TeamId", "dbo.Teams");
            DropIndex("dbo.ChangeAreas", new[] { "TeamId" });
            CreateTable(
                "dbo.ChangeRequestCategories",
                c => new
                    {
                        ChangeRequestCategoryId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Icon = c.String(),
                        Color = c.String(),
                        TeamId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeRequestCategoryId)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.TeamId);
            
            AddColumn("dbo.ChangeAreas", "ChangeRequestCategoryId", c => c.Long(nullable: false));
            CreateIndex("dbo.ChangeAreas", "ChangeRequestCategoryId");
            AddForeignKey("dbo.ChangeAreas", "ChangeRequestCategoryId", "dbo.ChangeRequestCategories", "ChangeRequestCategoryId", cascadeDelete: true);
            DropColumn("dbo.ChangeAreas", "TeamId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChangeAreas", "TeamId", c => c.Long(nullable: false));
            DropForeignKey("dbo.ChangeRequestCategories", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.ChangeAreas", "ChangeRequestCategoryId", "dbo.ChangeRequestCategories");
            DropIndex("dbo.ChangeRequestCategories", new[] { "TeamId" });
            DropIndex("dbo.ChangeAreas", new[] { "ChangeRequestCategoryId" });
            DropColumn("dbo.ChangeAreas", "ChangeRequestCategoryId");
            DropTable("dbo.ChangeRequestCategories");
            CreateIndex("dbo.ChangeAreas", "TeamId");
            AddForeignKey("dbo.ChangeAreas", "TeamId", "dbo.Teams", "TeamId", cascadeDelete: true);
        }
    }
}

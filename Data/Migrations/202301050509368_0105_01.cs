namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0105_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        RegionId = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RegionId);
            
            AddColumn("dbo.Branches", "Email", c => c.String());
            AddColumn("dbo.Branches", "EscalationEmail", c => c.String());
            AddColumn("dbo.Branches", "RegionId", c => c.Long(nullable: false));
            CreateIndex("dbo.Branches", "RegionId");
            AddForeignKey("dbo.Branches", "RegionId", "dbo.Regions", "RegionId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Branches", "RegionId", "dbo.Regions");
            DropIndex("dbo.Branches", new[] { "RegionId" });
            DropColumn("dbo.Branches", "RegionId");
            DropColumn("dbo.Branches", "EscalationEmail");
            DropColumn("dbo.Branches", "Email");
            DropTable("dbo.Regions");
        }
    }
}

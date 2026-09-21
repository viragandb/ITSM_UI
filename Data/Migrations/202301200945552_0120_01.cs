namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0120_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeliverySLAs",
                c => new
                    {
                        DeliverySLAId = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        SLA = c.Double(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeliverySLAId);
            
            AddColumn("dbo.Branches", "DeliverySLAId", c => c.Long());
            CreateIndex("dbo.Branches", "DeliverySLAId");
            AddForeignKey("dbo.Branches", "DeliverySLAId", "dbo.DeliverySLAs", "DeliverySLAId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Branches", "DeliverySLAId", "dbo.DeliverySLAs");
            DropIndex("dbo.Branches", new[] { "DeliverySLAId" });
            DropColumn("dbo.Branches", "DeliverySLAId");
            DropTable("dbo.DeliverySLAs");
        }
    }
}

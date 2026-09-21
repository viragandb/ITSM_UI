namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0120_02 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Branches", "DeliverySLAId", "dbo.DeliverySLAs");
            DropIndex("dbo.Branches", new[] { "DeliverySLAId" });
            AlterColumn("dbo.Branches", "DeliverySLAId", c => c.Long(nullable: false));
            CreateIndex("dbo.Branches", "DeliverySLAId");
            AddForeignKey("dbo.Branches", "DeliverySLAId", "dbo.DeliverySLAs", "DeliverySLAId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Branches", "DeliverySLAId", "dbo.DeliverySLAs");
            DropIndex("dbo.Branches", new[] { "DeliverySLAId" });
            AlterColumn("dbo.Branches", "DeliverySLAId", c => c.Long());
            CreateIndex("dbo.Branches", "DeliverySLAId");
            AddForeignKey("dbo.Branches", "DeliverySLAId", "dbo.DeliverySLAs", "DeliverySLAId");
        }
    }
}

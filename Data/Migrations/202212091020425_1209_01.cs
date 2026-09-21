namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1209_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "Level01TeamId", c => c.Long(nullable: false));
            AddColumn("dbo.Tickets", "FlowType", c => c.Int(nullable: false));
            CreateIndex("dbo.Tickets", "Level01TeamId");
            AddForeignKey("dbo.Tickets", "Level01TeamId", "dbo.Teams", "TeamId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "Level01TeamId", "dbo.Teams");
            DropIndex("dbo.Tickets", new[] { "Level01TeamId" });
            DropColumn("dbo.Tickets", "FlowType");
            DropColumn("dbo.Tickets", "Level01TeamId");
        }
    }
}

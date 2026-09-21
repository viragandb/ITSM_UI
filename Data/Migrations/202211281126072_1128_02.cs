namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1128_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "RespondTime", c => c.Double(nullable: false));
            AddColumn("dbo.Tickets", "SpentTimeStatus", c => c.Double(nullable: false));
            AddColumn("dbo.Tickets", "SpentTimeSync", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "SpentTimeSync");
            DropColumn("dbo.Tickets", "SpentTimeStatus");
            DropColumn("dbo.Tickets", "RespondTime");
        }
    }
}

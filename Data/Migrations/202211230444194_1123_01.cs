namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1123_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RequestTypePriorities", "Respond", c => c.Double(nullable: false));
            DropColumn("dbo.TicketPriorities", "Respond");
            DropColumn("dbo.TicketPriorities", "Resolve");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TicketPriorities", "Resolve", c => c.Double(nullable: false));
            AddColumn("dbo.TicketPriorities", "Respond", c => c.Double(nullable: false));
            DropColumn("dbo.RequestTypePriorities", "Respond");
        }
    }
}

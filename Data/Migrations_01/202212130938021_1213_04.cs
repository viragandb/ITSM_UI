namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1213_04 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketTasks", "IsMyTeam", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketTasks", "IsMyTeam");
        }
    }
}

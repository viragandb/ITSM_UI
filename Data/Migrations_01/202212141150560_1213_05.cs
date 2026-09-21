namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1213_05 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TicketTasks", "Ticket_TicketId", "dbo.Tickets");
            DropIndex("dbo.TicketTasks", new[] { "Ticket_TicketId" });
            DropColumn("dbo.TicketTasks", "Ticket_TicketId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TicketTasks", "Ticket_TicketId", c => c.Long());
            CreateIndex("dbo.TicketTasks", "Ticket_TicketId");
            AddForeignKey("dbo.TicketTasks", "Ticket_TicketId", "dbo.Tickets", "TicketId");
        }
    }
}

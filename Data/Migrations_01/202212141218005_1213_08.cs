namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1213_08 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TicketTasks", "Ticket_TicketId", "dbo.Tickets");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TicketTasks", "Ticket_TicketId", c => c.Long());
            CreateIndex("dbo.TicketTasks", "Ticket_TicketId");
            AddForeignKey("dbo.TicketTasks", "Ticket_TicketId", "dbo.Tickets", "TicketId");
        }
    }
}

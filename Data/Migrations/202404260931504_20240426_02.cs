namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240426_02 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.IncidentRequestTickets", new[] { "Ticket_TicketId" });
            RenameColumn(table: "dbo.IncidentRequestTickets", name: "Ticket_TicketId", newName: "TicketId");
            AlterColumn("dbo.IncidentRequestTickets", "TicketId", c => c.Long(nullable: false));
            CreateIndex("dbo.IncidentRequestTickets", "TicketId");
            DropColumn("dbo.IncidentRequestTickets", "TicketIc");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IncidentRequestTickets", "TicketIc", c => c.Long(nullable: false));
            DropIndex("dbo.IncidentRequestTickets", new[] { "TicketId" });
            AlterColumn("dbo.IncidentRequestTickets", "TicketId", c => c.Long());
            RenameColumn(table: "dbo.IncidentRequestTickets", name: "TicketId", newName: "Ticket_TicketId");
            CreateIndex("dbo.IncidentRequestTickets", "Ticket_TicketId");
        }
    }
}

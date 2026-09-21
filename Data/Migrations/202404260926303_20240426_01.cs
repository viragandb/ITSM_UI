namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240426_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IncidentRequestTickets",
                c => new
                    {
                        IncidentRequestTicketId = c.Long(nullable: false, identity: true),
                        IncidentRequestId = c.Long(nullable: false),
                        TicketIc = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                        Ticket_TicketId = c.Long(),
                    })
                .PrimaryKey(t => t.IncidentRequestTicketId)
                .ForeignKey("dbo.IncidentRequests", t => t.IncidentRequestId, cascadeDelete: true)
                .ForeignKey("dbo.Tickets", t => t.Ticket_TicketId)
                .Index(t => t.IncidentRequestId)
                .Index(t => t.Ticket_TicketId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IncidentRequestTickets", "Ticket_TicketId", "dbo.Tickets");
            DropForeignKey("dbo.IncidentRequestTickets", "IncidentRequestId", "dbo.IncidentRequests");
            DropIndex("dbo.IncidentRequestTickets", new[] { "Ticket_TicketId" });
            DropIndex("dbo.IncidentRequestTickets", new[] { "IncidentRequestId" });
            DropTable("dbo.IncidentRequestTickets");
        }
    }
}

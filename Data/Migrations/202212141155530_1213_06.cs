namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1213_06 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TicketTasks", "AllocatedTeamId", "dbo.Teams");
            DropIndex("dbo.TicketTasks", new[] { "TicketId" });
            RenameColumn(table: "dbo.TicketTasks", name: "TicketId", newName: "Ticket_TicketId");
            AlterColumn("dbo.TicketTasks", "Ticket_TicketId", c => c.Long());
            CreateIndex("dbo.TicketTasks", "Ticket_TicketId");
            AddForeignKey("dbo.TicketTasks", "AllocatedTeamId", "dbo.Teams", "TeamId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketTasks", "AllocatedTeamId", "dbo.Teams");
            DropIndex("dbo.TicketTasks", new[] { "Ticket_TicketId" });
            AlterColumn("dbo.TicketTasks", "Ticket_TicketId", c => c.Long(nullable: false));
            RenameColumn(table: "dbo.TicketTasks", name: "Ticket_TicketId", newName: "TicketId");
            CreateIndex("dbo.TicketTasks", "TicketId");
            AddForeignKey("dbo.TicketTasks", "AllocatedTeamId", "dbo.Teams", "TeamId", cascadeDelete: true);
        }
    }
}

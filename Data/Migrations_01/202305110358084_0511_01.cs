namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0511_01 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ItemAssets", name: "Ticket_TicketId", newName: "TicketId");
            RenameIndex(table: "dbo.ItemAssets", name: "IX_Ticket_TicketId", newName: "IX_TicketId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.ItemAssets", name: "IX_TicketId", newName: "IX_Ticket_TicketId");
            RenameColumn(table: "dbo.ItemAssets", name: "TicketId", newName: "Ticket_TicketId");
        }
    }
}

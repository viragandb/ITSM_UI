namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1213_03 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketTasks",
                c => new
                    {
                        TicketTaskId = c.Long(nullable: false, identity: true),
                        TicketId = c.Long(nullable: false),
                        Subject = c.String(),
                        Description = c.String(),
                        Comment = c.String(),
                        AllocatedTeamId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        SpentTime = c.Double(nullable: false),
                        CreatedBy = c.String(maxLength: 20, unicode: false),
                        AssignedBy = c.String(maxLength: 20, unicode: false),
                        AssignedTo = c.String(maxLength: 20, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                        Ticket_TicketId = c.Long(),
                    })
                .PrimaryKey(t => t.TicketTaskId)
                .ForeignKey("dbo.Teams", t => t.AllocatedTeamId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.AssignedBy)
                .ForeignKey("dbo.Users", t => t.AssignedTo)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Tickets", t => t.TicketId)
                .ForeignKey("dbo.Tickets", t => t.Ticket_TicketId)
                .Index(t => t.TicketId)
                .Index(t => t.AllocatedTeamId)
                .Index(t => t.CreatedBy)
                .Index(t => t.AssignedBy)
                .Index(t => t.AssignedTo)
                .Index(t => t.Ticket_TicketId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketTasks", "Ticket_TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketTasks", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketTasks", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.TicketTasks", "AssignedTo", "dbo.Users");
            DropForeignKey("dbo.TicketTasks", "AssignedBy", "dbo.Users");
            DropForeignKey("dbo.TicketTasks", "AllocatedTeamId", "dbo.Teams");
            DropIndex("dbo.TicketTasks", new[] { "Ticket_TicketId" });
            DropIndex("dbo.TicketTasks", new[] { "AssignedTo" });
            DropIndex("dbo.TicketTasks", new[] { "AssignedBy" });
            DropIndex("dbo.TicketTasks", new[] { "CreatedBy" });
            DropIndex("dbo.TicketTasks", new[] { "AllocatedTeamId" });
            DropIndex("dbo.TicketTasks", new[] { "TicketId" });
            DropTable("dbo.TicketTasks");
        }
    }
}

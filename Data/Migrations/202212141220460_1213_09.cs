namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1213_09 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketTasks", "TicketId", c => c.Long(nullable: false));
            AddColumn("dbo.TicketTasks", "Subject", c => c.String());
            AddColumn("dbo.TicketTasks", "Description", c => c.String());
            AddColumn("dbo.TicketTasks", "Comment", c => c.String());
            AddColumn("dbo.TicketTasks", "AllocatedTeamId", c => c.Long(nullable: false));
            AddColumn("dbo.TicketTasks", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.TicketTasks", "SpentTime", c => c.Double(nullable: false));
            AddColumn("dbo.TicketTasks", "IsMyTeam", c => c.Boolean(nullable: false));
            AddColumn("dbo.TicketTasks", "CreatedBy", c => c.String(maxLength: 20, unicode: false));
            AddColumn("dbo.TicketTasks", "AssignedBy", c => c.String(maxLength: 20, unicode: false));
            AddColumn("dbo.TicketTasks", "AssignedTo", c => c.String(maxLength: 20, unicode: false));
            AddColumn("dbo.TicketTasks", "CreatedDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.TicketTasks", "TicketId");
            CreateIndex("dbo.TicketTasks", "AllocatedTeamId");
            CreateIndex("dbo.TicketTasks", "CreatedBy");
            CreateIndex("dbo.TicketTasks", "AssignedBy");
            CreateIndex("dbo.TicketTasks", "AssignedTo");
            AddForeignKey("dbo.TicketTasks", "AllocatedTeamId", "dbo.Teams", "TeamId");
            AddForeignKey("dbo.TicketTasks", "AssignedBy", "dbo.Users", "EmpNo");
            AddForeignKey("dbo.TicketTasks", "AssignedTo", "dbo.Users", "EmpNo");
            AddForeignKey("dbo.TicketTasks", "CreatedBy", "dbo.Users", "EmpNo");
            AddForeignKey("dbo.TicketTasks", "TicketId", "dbo.Tickets", "TicketId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketTasks", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketTasks", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.TicketTasks", "AssignedTo", "dbo.Users");
            DropForeignKey("dbo.TicketTasks", "AssignedBy", "dbo.Users");
            DropForeignKey("dbo.TicketTasks", "AllocatedTeamId", "dbo.Teams");
            DropIndex("dbo.TicketTasks", new[] { "AssignedTo" });
            DropIndex("dbo.TicketTasks", new[] { "AssignedBy" });
            DropIndex("dbo.TicketTasks", new[] { "CreatedBy" });
            DropIndex("dbo.TicketTasks", new[] { "AllocatedTeamId" });
            DropIndex("dbo.TicketTasks", new[] { "TicketId" });
            DropColumn("dbo.TicketTasks", "CreatedDate");
            DropColumn("dbo.TicketTasks", "AssignedTo");
            DropColumn("dbo.TicketTasks", "AssignedBy");
            DropColumn("dbo.TicketTasks", "CreatedBy");
            DropColumn("dbo.TicketTasks", "IsMyTeam");
            DropColumn("dbo.TicketTasks", "SpentTime");
            DropColumn("dbo.TicketTasks", "Status");
            DropColumn("dbo.TicketTasks", "AllocatedTeamId");
            DropColumn("dbo.TicketTasks", "Comment");
            DropColumn("dbo.TicketTasks", "Description");
            DropColumn("dbo.TicketTasks", "Subject");
            DropColumn("dbo.TicketTasks", "TicketId");
        }
    }
}

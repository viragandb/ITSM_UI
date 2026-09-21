namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1213_07 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TicketTasks", "AllocatedTeamId", "dbo.Teams");
            DropForeignKey("dbo.TicketTasks", "AssignedBy", "dbo.Users");
            DropForeignKey("dbo.TicketTasks", "AssignedTo", "dbo.Users");
            DropForeignKey("dbo.TicketTasks", "CreatedBy", "dbo.Users");
            DropIndex("dbo.TicketTasks", new[] { "AllocatedTeamId" });
            DropIndex("dbo.TicketTasks", new[] { "CreatedBy" });
            DropIndex("dbo.TicketTasks", new[] { "AssignedBy" });
            DropIndex("dbo.TicketTasks", new[] { "AssignedTo" });
            DropColumn("dbo.TicketTasks", "Subject");
            DropColumn("dbo.TicketTasks", "Description");
            DropColumn("dbo.TicketTasks", "Comment");
            DropColumn("dbo.TicketTasks", "AllocatedTeamId");
            DropColumn("dbo.TicketTasks", "Status");
            DropColumn("dbo.TicketTasks", "SpentTime");
            DropColumn("dbo.TicketTasks", "IsMyTeam");
            DropColumn("dbo.TicketTasks", "CreatedBy");
            DropColumn("dbo.TicketTasks", "AssignedBy");
            DropColumn("dbo.TicketTasks", "AssignedTo");
            DropColumn("dbo.TicketTasks", "CreatedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TicketTasks", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.TicketTasks", "AssignedTo", c => c.String(maxLength: 20, unicode: false));
            AddColumn("dbo.TicketTasks", "AssignedBy", c => c.String(maxLength: 20, unicode: false));
            AddColumn("dbo.TicketTasks", "CreatedBy", c => c.String(maxLength: 20, unicode: false));
            AddColumn("dbo.TicketTasks", "IsMyTeam", c => c.Boolean(nullable: false));
            AddColumn("dbo.TicketTasks", "SpentTime", c => c.Double(nullable: false));
            AddColumn("dbo.TicketTasks", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.TicketTasks", "AllocatedTeamId", c => c.Long(nullable: false));
            AddColumn("dbo.TicketTasks", "Comment", c => c.String());
            AddColumn("dbo.TicketTasks", "Description", c => c.String());
            AddColumn("dbo.TicketTasks", "Subject", c => c.String());
            CreateIndex("dbo.TicketTasks", "AssignedTo");
            CreateIndex("dbo.TicketTasks", "AssignedBy");
            CreateIndex("dbo.TicketTasks", "CreatedBy");
            CreateIndex("dbo.TicketTasks", "AllocatedTeamId");
            AddForeignKey("dbo.TicketTasks", "CreatedBy", "dbo.Users", "EmpNo");
            AddForeignKey("dbo.TicketTasks", "AssignedTo", "dbo.Users", "EmpNo");
            AddForeignKey("dbo.TicketTasks", "AssignedBy", "dbo.Users", "EmpNo");
            AddForeignKey("dbo.TicketTasks", "AllocatedTeamId", "dbo.Teams", "TeamId");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260710_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAccessRequests", "ClarificationRequestedByTeamId", c => c.Long());
            AddColumn("dbo.RemoteAccessRequests", "ClarificationAssignedToTeamId", c => c.Long());
            AddColumn("dbo.RemoteAccessRequestUpdates", "ClarificationRequestedByTeamId", c => c.Long());
            AddColumn("dbo.RemoteAccessRequestUpdates", "ClarificationAssignedToTeamId", c => c.Long());
            CreateIndex("dbo.RemoteAccessRequestUpdates", "ClarificationRequestedByTeamId");
            CreateIndex("dbo.RemoteAccessRequestUpdates", "ClarificationAssignedToTeamId");
            CreateIndex("dbo.RemoteAccessRequests", "ClarificationRequestedByTeamId");
            CreateIndex("dbo.RemoteAccessRequests", "ClarificationAssignedToTeamId");
            AddForeignKey("dbo.RemoteAccessRequestUpdates", "ClarificationAssignedToTeamId", "dbo.Teams", "TeamId");
            AddForeignKey("dbo.RemoteAccessRequestUpdates", "ClarificationRequestedByTeamId", "dbo.Teams", "TeamId");
            AddForeignKey("dbo.RemoteAccessRequests", "ClarificationAssignedToTeamId", "dbo.Teams", "TeamId");
            AddForeignKey("dbo.RemoteAccessRequests", "ClarificationRequestedByTeamId", "dbo.Teams", "TeamId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RemoteAccessRequests", "ClarificationRequestedByTeamId", "dbo.Teams");
            DropForeignKey("dbo.RemoteAccessRequests", "ClarificationAssignedToTeamId", "dbo.Teams");
            DropForeignKey("dbo.RemoteAccessRequestUpdates", "ClarificationRequestedByTeamId", "dbo.Teams");
            DropForeignKey("dbo.RemoteAccessRequestUpdates", "ClarificationAssignedToTeamId", "dbo.Teams");
            DropIndex("dbo.RemoteAccessRequests", new[] { "ClarificationAssignedToTeamId" });
            DropIndex("dbo.RemoteAccessRequests", new[] { "ClarificationRequestedByTeamId" });
            DropIndex("dbo.RemoteAccessRequestUpdates", new[] { "ClarificationAssignedToTeamId" });
            DropIndex("dbo.RemoteAccessRequestUpdates", new[] { "ClarificationRequestedByTeamId" });
            DropColumn("dbo.RemoteAccessRequestUpdates", "ClarificationAssignedToTeamId");
            DropColumn("dbo.RemoteAccessRequestUpdates", "ClarificationRequestedByTeamId");
            DropColumn("dbo.RemoteAccessRequests", "ClarificationAssignedToTeamId");
            DropColumn("dbo.RemoteAccessRequests", "ClarificationRequestedByTeamId");
        }
    }
}

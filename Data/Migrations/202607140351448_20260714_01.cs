namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260714_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAccessRequestUpdates", "ClarificationType", c => c.Int());
            AddColumn("dbo.RemoteAccessRequests", "ClarificationType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RemoteAccessRequests", "ClarificationType");
            DropColumn("dbo.RemoteAccessRequestUpdates", "ClarificationType");
        }
    }
}

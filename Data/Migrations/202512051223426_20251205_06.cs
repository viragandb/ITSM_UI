namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251205_06 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAccessRequestLogs", "Comment", c => c.String(nullable: false));
            AddColumn("dbo.RemoteAccessRequestUpdates", "Comment", c => c.String(nullable: false));
            DropColumn("dbo.RemoteAccessRequestLogs", "Description");
            DropColumn("dbo.RemoteAccessRequestUpdates", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RemoteAccessRequestUpdates", "Description", c => c.String(nullable: false));
            AddColumn("dbo.RemoteAccessRequestLogs", "Description", c => c.String(nullable: false));
            DropColumn("dbo.RemoteAccessRequestUpdates", "Comment");
            DropColumn("dbo.RemoteAccessRequestLogs", "Comment");
        }
    }
}

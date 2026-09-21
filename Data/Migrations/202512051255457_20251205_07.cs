namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251205_07 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAccessRequestLogs", "Description", c => c.String(nullable: false));
            DropColumn("dbo.RemoteAccessRequestLogs", "Comment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RemoteAccessRequestLogs", "Comment", c => c.String(nullable: false));
            DropColumn("dbo.RemoteAccessRequestLogs", "Description");
        }
    }
}

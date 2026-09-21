namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251211_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAccessRequests", "IsClarificationNeeded", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RemoteAccessRequests", "IsClarificationNeeded");
        }
    }
}

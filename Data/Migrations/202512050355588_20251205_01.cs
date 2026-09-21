namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251205_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAccessRequests", "HRClarification", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RemoteAccessRequests", "HRClarification");
        }
    }
}

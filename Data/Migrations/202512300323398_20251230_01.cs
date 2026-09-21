namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251230_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAccessRequests", "ReasonForHRClarification", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RemoteAccessRequests", "ReasonForHRClarification");
        }
    }
}

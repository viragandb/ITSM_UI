namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260121_01 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RemoteAccessRequests", new[] { "ApprovedSupervisor" });
            DropIndex("dbo.RemoteAccessRequests", new[] { "ApprovedAVPOrVP" });
            AlterColumn("dbo.RemoteAccessRequests", "ApprovedSupervisor", c => c.String(maxLength: 20, unicode: false));
            AlterColumn("dbo.RemoteAccessRequests", "ApprovedAVPOrVP", c => c.String(maxLength: 20, unicode: false));
            CreateIndex("dbo.RemoteAccessRequests", "ApprovedSupervisor");
            CreateIndex("dbo.RemoteAccessRequests", "ApprovedAVPOrVP");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RemoteAccessRequests", new[] { "ApprovedAVPOrVP" });
            DropIndex("dbo.RemoteAccessRequests", new[] { "ApprovedSupervisor" });
            AlterColumn("dbo.RemoteAccessRequests", "ApprovedAVPOrVP", c => c.String(nullable: false, maxLength: 20, unicode: false));
            AlterColumn("dbo.RemoteAccessRequests", "ApprovedSupervisor", c => c.String(nullable: false, maxLength: 20, unicode: false));
            CreateIndex("dbo.RemoteAccessRequests", "ApprovedAVPOrVP");
            CreateIndex("dbo.RemoteAccessRequests", "ApprovedSupervisor");
        }
    }
}

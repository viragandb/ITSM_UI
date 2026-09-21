namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260120_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAccessRequests", "ApprovedSupervisor", c => c.String(nullable: true, maxLength: 20, unicode: false));
            AddColumn("dbo.RemoteAccessRequests", "ApprovedAVPOrVP", c => c.String(nullable: true, maxLength: 20, unicode: false));
            CreateIndex("dbo.RemoteAccessRequests", "ApprovedSupervisor");
            CreateIndex("dbo.RemoteAccessRequests", "ApprovedAVPOrVP");
            AddForeignKey("dbo.RemoteAccessRequests", "ApprovedAVPOrVP", "dbo.Users", "EmpNo");
            AddForeignKey("dbo.RemoteAccessRequests", "ApprovedSupervisor", "dbo.Users", "EmpNo");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RemoteAccessRequests", "ApprovedSupervisor", "dbo.Users");
            DropForeignKey("dbo.RemoteAccessRequests", "ApprovedAVPOrVP", "dbo.Users");
            DropIndex("dbo.RemoteAccessRequests", new[] { "ApprovedAVPOrVP" });
            DropIndex("dbo.RemoteAccessRequests", new[] { "ApprovedSupervisor" });
            DropColumn("dbo.RemoteAccessRequests", "ApprovedAVPOrVP");
            DropColumn("dbo.RemoteAccessRequests", "ApprovedSupervisor");
        }
    }
}

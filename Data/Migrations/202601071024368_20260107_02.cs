namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260107_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAccessRequests", "IsAddedToGroup", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.RemoteAccessRequests", "IsWifiAccessAllow", c => c.Boolean(nullable: false, defaultValue:false));
            AddColumn("dbo.RemoteAccessRequests", "IsCertificateProvided", c => c.Boolean(nullable: false, defaultValue:false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RemoteAccessRequests", "IsCertificateProvided");
            DropColumn("dbo.RemoteAccessRequests", "IsWifiAccessAllow");
            DropColumn("dbo.RemoteAccessRequests", "IsAddedToGroup");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251205_04 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAgreements", "OverseasConnectivityConfirm", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsInternetConnStable", c => c.Boolean(nullable: false));
            DropColumn("dbo.RemoteAgreements", "ConnectivityConform");
            DropColumn("dbo.RemoteAgreements", "IsInternetConn");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RemoteAgreements", "IsInternetConn", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "ConnectivityConform", c => c.Boolean(nullable: false));
            DropColumn("dbo.RemoteAgreements", "IsInternetConnStable");
            DropColumn("dbo.RemoteAgreements", "OverseasConnectivityConfirm");
        }
    }
}

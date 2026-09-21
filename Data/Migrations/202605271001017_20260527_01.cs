namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260527_01 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RemoteAgreements", "RemoteAccessRequestId", "dbo.RemoteAccessRequests");
            AddForeignKey("dbo.RemoteAgreements", "RemoteAccessRequestId", "dbo.RemoteAccessRequests", "RemoteAccessRequestId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RemoteAgreements", "RemoteAccessRequestId", "dbo.RemoteAccessRequests");
            AddForeignKey("dbo.RemoteAgreements", "RemoteAccessRequestId", "dbo.RemoteAccessRequests", "RemoteAccessRequestId", cascadeDelete: true);
        }
    }
}

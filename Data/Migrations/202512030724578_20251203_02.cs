namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251203_02 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RemoteAccessRequiredSystems", "RemoteAccessRequest_RemoteAccessRequestId", "dbo.RemoteAccessRequests");
            DropIndex("dbo.RemoteAccessRequiredSystems", new[] { "RemoteAccessRequest_RemoteAccessRequestId" });
            DropColumn("dbo.RemoteAccessRequiredSystems", "RemoteAccessRequest_RemoteAccessRequestId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RemoteAccessRequiredSystems", "RemoteAccessRequest_RemoteAccessRequestId", c => c.Long());
            CreateIndex("dbo.RemoteAccessRequiredSystems", "RemoteAccessRequest_RemoteAccessRequestId");
            AddForeignKey("dbo.RemoteAccessRequiredSystems", "RemoteAccessRequest_RemoteAccessRequestId", "dbo.RemoteAccessRequests", "RemoteAccessRequestId");
        }
    }
}

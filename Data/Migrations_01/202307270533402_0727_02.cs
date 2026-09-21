namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0727_02 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserAccesses", "AccessRequestId", "dbo.AccessRequests");
            DropForeignKey("dbo.UserPrivilegeAccesses", "AccessRequestId", "dbo.AccessRequests");
            AddForeignKey("dbo.UserAccesses", "AccessRequestId", "dbo.AccessRequests", "AccessRequestId", cascadeDelete: true);
            AddForeignKey("dbo.UserPrivilegeAccesses", "AccessRequestId", "dbo.AccessRequests", "AccessRequestId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPrivilegeAccesses", "AccessRequestId", "dbo.AccessRequests");
            DropForeignKey("dbo.UserAccesses", "AccessRequestId", "dbo.AccessRequests");
            AddForeignKey("dbo.UserPrivilegeAccesses", "AccessRequestId", "dbo.AccessRequests", "AccessRequestId");
            AddForeignKey("dbo.UserAccesses", "AccessRequestId", "dbo.AccessRequests", "AccessRequestId");
        }
    }
}

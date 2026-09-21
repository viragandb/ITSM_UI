namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0727_04 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AccessRequests", "SystemAccessId", "dbo.SystemAccesses");
            DropForeignKey("dbo.SystemAccesses", "AccessRequestId", "dbo.AccessRequests");
            AddForeignKey("dbo.AccessRequests", "SystemAccessId", "dbo.SystemAccesses", "SystemAccessId", cascadeDelete: true);
            AddForeignKey("dbo.SystemAccesses", "AccessRequestId", "dbo.AccessRequests", "AccessRequestId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SystemAccesses", "AccessRequestId", "dbo.AccessRequests");
            DropForeignKey("dbo.AccessRequests", "SystemAccessId", "dbo.SystemAccesses");
            AddForeignKey("dbo.SystemAccesses", "AccessRequestId", "dbo.AccessRequests", "AccessRequestId", cascadeDelete: true);
            AddForeignKey("dbo.AccessRequests", "SystemAccessId", "dbo.SystemAccesses", "SystemAccessId");
        }
    }
}

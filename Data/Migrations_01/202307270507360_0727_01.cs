namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0727_01 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SystemAccesses", "AccessRequestId", "dbo.AccessRequests");
            AddForeignKey("dbo.SystemAccesses", "AccessRequestId", "dbo.AccessRequests", "AccessRequestId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SystemAccesses", "AccessRequestId", "dbo.AccessRequests");
            AddForeignKey("dbo.SystemAccesses", "AccessRequestId", "dbo.AccessRequests", "AccessRequestId");
        }

    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0717_07 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SystemAccesses", "AccessRequestId", "dbo.AccessRequests");
            DropIndex("dbo.SystemAccesses", new[] { "AccessRequestId" });
            DropColumn("dbo.SystemAccesses", "AccessRequestId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SystemAccesses", "AccessRequestId", c => c.Long(nullable: false));
            CreateIndex("dbo.SystemAccesses", "AccessRequestId");
            AddForeignKey("dbo.SystemAccesses", "AccessRequestId", "dbo.AccessRequests", "AccessRequestId");
        }
    }
}

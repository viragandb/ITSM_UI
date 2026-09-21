namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0717_03 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AccessRequests", "SystemAccess_SystemAccessId", "dbo.SystemAccesses");
            DropIndex("dbo.AccessRequests", new[] { "SystemAccess_SystemAccessId" });
            DropColumn("dbo.AccessRequests", "SystemAccessId");
            DropColumn("dbo.AccessRequests", "SystemAccess_SystemAccessId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccessRequests", "SystemAccess_SystemAccessId", c => c.Long());
            AddColumn("dbo.AccessRequests", "SystemAccessId", c => c.Long(nullable: false));
            CreateIndex("dbo.AccessRequests", "SystemAccess_SystemAccessId");
            AddForeignKey("dbo.AccessRequests", "SystemAccess_SystemAccessId", "dbo.SystemAccesses", "SystemAccessId");
        }
    }
}

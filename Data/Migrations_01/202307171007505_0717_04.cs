namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0717_04 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessRequests", "SystemAccessId", c => c.Long(nullable: false));
            AddColumn("dbo.AccessRequests", "SystemAccess_SystemAccessId", c => c.Long());
            CreateIndex("dbo.AccessRequests", "SystemAccess_SystemAccessId");
            AddForeignKey("dbo.AccessRequests", "SystemAccess_SystemAccessId", "dbo.SystemAccesses", "SystemAccessId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccessRequests", "SystemAccess_SystemAccessId", "dbo.SystemAccesses");
            DropIndex("dbo.AccessRequests", new[] { "SystemAccess_SystemAccessId" });
            DropColumn("dbo.AccessRequests", "SystemAccess_SystemAccessId");
            DropColumn("dbo.AccessRequests", "SystemAccessId");
        }
    }
}

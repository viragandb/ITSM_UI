namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0717_06 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessRequests", "SystemAccessId", c => c.Long());
            CreateIndex("dbo.AccessRequests", "SystemAccessId");
            AddForeignKey("dbo.AccessRequests", "SystemAccessId", "dbo.SystemAccesses", "SystemAccessId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccessRequests", "SystemAccessId", "dbo.SystemAccesses");
            DropIndex("dbo.AccessRequests", new[] { "SystemAccessId" });
            DropColumn("dbo.AccessRequests", "SystemAccessId");
        }
    }
}

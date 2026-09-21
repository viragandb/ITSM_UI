namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0727_03 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AccessRequests", "PhysicalAccessId", "dbo.PhysicalAccesses");
            AddForeignKey("dbo.AccessRequests", "PhysicalAccessId", "dbo.PhysicalAccesses", "PhysicalAccessId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccessRequests", "PhysicalAccessId", "dbo.PhysicalAccesses");
            AddForeignKey("dbo.AccessRequests", "PhysicalAccessId", "dbo.PhysicalAccesses", "PhysicalAccessId");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251222_02 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RemoteAccessRequests", new[] { "BranchId" });
            AlterColumn("dbo.RemoteAccessRequests", "BranchId", c => c.Long(nullable: false));
            CreateIndex("dbo.RemoteAccessRequests", "BranchId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RemoteAccessRequests", new[] { "BranchId" });
            AlterColumn("dbo.RemoteAccessRequests", "BranchId", c => c.Long());
            CreateIndex("dbo.RemoteAccessRequests", "BranchId");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251222_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAccessRequests", "BranchId", c => c.Long());
            CreateIndex("dbo.RemoteAccessRequests", "BranchId");
            AddForeignKey("dbo.RemoteAccessRequests", "BranchId", "dbo.Branches", "BranchId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RemoteAccessRequests", "BranchId", "dbo.Branches");
            DropIndex("dbo.RemoteAccessRequests", new[] { "BranchId" });
            DropColumn("dbo.RemoteAccessRequests", "BranchId");
        }
    }
}

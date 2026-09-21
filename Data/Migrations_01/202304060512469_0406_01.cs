namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0406_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssetTransferRequests", "AssignedTo", c => c.String(maxLength: 20, unicode: false));
            CreateIndex("dbo.AssetTransferRequests", "AssignedTo");
            AddForeignKey("dbo.AssetTransferRequests", "AssignedTo", "dbo.Users", "EmpNo");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetTransferRequests", "AssignedTo", "dbo.Users");
            DropIndex("dbo.AssetTransferRequests", new[] { "AssignedTo" });
            DropColumn("dbo.AssetTransferRequests", "AssignedTo");
        }
    }
}

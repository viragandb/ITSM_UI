namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0418_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssetTransferRequests", "VendorId", c => c.Long());
            CreateIndex("dbo.AssetTransferRequests", "VendorId");
            AddForeignKey("dbo.AssetTransferRequests", "VendorId", "dbo.Vendors", "VendorId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetTransferRequests", "VendorId", "dbo.Vendors");
            DropIndex("dbo.AssetTransferRequests", new[] { "VendorId" });
            DropColumn("dbo.AssetTransferRequests", "VendorId");
        }
    }
}

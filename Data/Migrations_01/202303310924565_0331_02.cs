namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0331_02 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AssetTransferRequests", "UpdatedBy", "dbo.Users");
            DropIndex("dbo.AssetTransferRequests", new[] { "UpdatedBy" });
            AddColumn("dbo.AssetTransferRequests", "Comment", c => c.String());
            AddColumn("dbo.AssetTransferRequests", "AuthorizedBy", c => c.String(maxLength: 20, unicode: false));
            AddColumn("dbo.AssetTransferRequests", "AuthorizedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AssetTransferRequests", "UpdatedBy", c => c.String());
            CreateIndex("dbo.AssetTransferRequests", "AuthorizedBy");
            AddForeignKey("dbo.AssetTransferRequests", "AuthorizedBy", "dbo.Users", "EmpNo");
            DropColumn("dbo.AssetTransferRequests", "InitiatorComment");
            DropColumn("dbo.AssetTransferRequests", "CompletedComment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AssetTransferRequests", "CompletedComment", c => c.String());
            AddColumn("dbo.AssetTransferRequests", "InitiatorComment", c => c.String());
            DropForeignKey("dbo.AssetTransferRequests", "AuthorizedBy", "dbo.Users");
            DropIndex("dbo.AssetTransferRequests", new[] { "AuthorizedBy" });
            AlterColumn("dbo.AssetTransferRequests", "UpdatedBy", c => c.String(maxLength: 20, unicode: false));
            DropColumn("dbo.AssetTransferRequests", "AuthorizedDate");
            DropColumn("dbo.AssetTransferRequests", "AuthorizedBy");
            DropColumn("dbo.AssetTransferRequests", "Comment");
            CreateIndex("dbo.AssetTransferRequests", "UpdatedBy");
            AddForeignKey("dbo.AssetTransferRequests", "UpdatedBy", "dbo.Users", "EmpNo");
        }
    }
}

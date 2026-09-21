namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0404_05 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssetTransferRequests", "CommentAuthorize", c => c.String());
            AddColumn("dbo.AssetTransferRequests", "CommentComplete", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssetTransferRequests", "CommentComplete");
            DropColumn("dbo.AssetTransferRequests", "CommentAuthorize");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0411_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "AssignedTypeEnum", c => c.Int(nullable: false));
            AddColumn("dbo.AssetTransferRequests", "AssignedType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssetTransferRequests", "AssignedType");
            DropColumn("dbo.Assets", "AssignedTypeEnum");
        }
    }
}

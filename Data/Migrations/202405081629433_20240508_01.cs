namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240508_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "VerifiedBy", c => c.String());
            AddColumn("dbo.Assets", "VerifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Assets", "AssetVerificationRequestId", c => c.Long());
            CreateIndex("dbo.Assets", "AssetVerificationRequestId");
            AddForeignKey("dbo.Assets", "AssetVerificationRequestId", "dbo.AssetVerificationRequests", "AssetVerificationRequestId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assets", "AssetVerificationRequestId", "dbo.AssetVerificationRequests");
            DropIndex("dbo.Assets", new[] { "AssetVerificationRequestId" });
            DropColumn("dbo.Assets", "AssetVerificationRequestId");
            DropColumn("dbo.Assets", "VerifiedDate");
            DropColumn("dbo.Assets", "VerifiedBy");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1009_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RemoteAccesses",
                c => new
                    {
                        RemoteAccessId = c.Long(nullable: false, identity: true),
                        AccessRequestId = c.Long(nullable: false),
                        RemoteAccessType = c.Int(nullable: false),
                        AssetId = c.Long(nullable: false),
                        Impact = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RemoteAccessId)
                .ForeignKey("dbo.AccessRequests", t => t.AccessRequestId)
                .ForeignKey("dbo.Assets", t => t.AssetId, cascadeDelete: true)
                .Index(t => t.AccessRequestId)
                .Index(t => t.AssetId);
            
            CreateTable(
                "dbo.RemoteAccessItems",
                c => new
                    {
                        RemoteAccessItemId = c.Long(nullable: false, identity: true),
                        AssetTypeId = c.Long(nullable: false),
                        Description = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                        RemoteAccess_RemoteAccessId = c.Long(),
                    })
                .PrimaryKey(t => t.RemoteAccessItemId)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId, cascadeDelete: true)
                .ForeignKey("dbo.RemoteAccesses", t => t.RemoteAccess_RemoteAccessId)
                .Index(t => t.AssetTypeId)
                .Index(t => t.RemoteAccess_RemoteAccessId);
            
            AddColumn("dbo.AccessRequests", "RemoteAccessId", c => c.Long());
            CreateIndex("dbo.AccessRequests", "RemoteAccessId");
            AddForeignKey("dbo.AccessRequests", "RemoteAccessId", "dbo.RemoteAccesses", "RemoteAccessId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccessRequests", "RemoteAccessId", "dbo.RemoteAccesses");
            DropForeignKey("dbo.RemoteAccessItems", "RemoteAccess_RemoteAccessId", "dbo.RemoteAccesses");
            DropForeignKey("dbo.RemoteAccessItems", "AssetTypeId", "dbo.AssetTypes");
            DropForeignKey("dbo.RemoteAccesses", "AssetId", "dbo.Assets");
            DropForeignKey("dbo.RemoteAccesses", "AccessRequestId", "dbo.AccessRequests");
            DropIndex("dbo.RemoteAccessItems", new[] { "RemoteAccess_RemoteAccessId" });
            DropIndex("dbo.RemoteAccessItems", new[] { "AssetTypeId" });
            DropIndex("dbo.RemoteAccesses", new[] { "AssetId" });
            DropIndex("dbo.RemoteAccesses", new[] { "AccessRequestId" });
            DropIndex("dbo.AccessRequests", new[] { "RemoteAccessId" });
            DropColumn("dbo.AccessRequests", "RemoteAccessId");
            DropTable("dbo.RemoteAccessItems");
            DropTable("dbo.RemoteAccesses");
        }
    }
}

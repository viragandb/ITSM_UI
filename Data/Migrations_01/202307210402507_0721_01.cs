namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0721_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PhysicalAccesses",
                c => new
                    {
                        PhysicalAccessId = c.Long(nullable: false, identity: true),
                        AccessRequestId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PhysicalAccessId)
                .ForeignKey("dbo.AccessRequests", t => t.AccessRequestId)
                .Index(t => t.AccessRequestId);
            
            CreateTable(
                "dbo.PhysicalAccessItems",
                c => new
                    {
                        PhysicalAccessItemId = c.Long(nullable: false, identity: true),
                        PhysicalAccessId = c.Long(nullable: false),
                        PhysicalAreaId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PhysicalAccessItemId)
                .ForeignKey("dbo.PhysicalAccesses", t => t.PhysicalAccessId, cascadeDelete: true)
                .ForeignKey("dbo.PhysicalAreas", t => t.PhysicalAreaId, cascadeDelete: true)
                .Index(t => t.PhysicalAccessId)
                .Index(t => t.PhysicalAreaId);
            
            CreateTable(
                "dbo.PhysicalAreas",
                c => new
                    {
                        PhysicalAreaId = c.Long(nullable: false, identity: true),
                        BranchId = c.Long(nullable: false),
                        Name = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PhysicalAreaId)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .Index(t => t.BranchId);
            
            AddColumn("dbo.AccessRequests", "PhysicalAccessId", c => c.Long());
            CreateIndex("dbo.AccessRequests", "PhysicalAccessId");
            AddForeignKey("dbo.AccessRequests", "PhysicalAccessId", "dbo.PhysicalAccesses", "PhysicalAccessId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccessRequests", "PhysicalAccessId", "dbo.PhysicalAccesses");
            DropForeignKey("dbo.PhysicalAccessItems", "PhysicalAreaId", "dbo.PhysicalAreas");
            DropForeignKey("dbo.PhysicalAreas", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.PhysicalAccessItems", "PhysicalAccessId", "dbo.PhysicalAccesses");
            DropForeignKey("dbo.PhysicalAccesses", "AccessRequestId", "dbo.AccessRequests");
            DropIndex("dbo.PhysicalAreas", new[] { "BranchId" });
            DropIndex("dbo.PhysicalAccessItems", new[] { "PhysicalAreaId" });
            DropIndex("dbo.PhysicalAccessItems", new[] { "PhysicalAccessId" });
            DropIndex("dbo.PhysicalAccesses", new[] { "AccessRequestId" });
            DropIndex("dbo.AccessRequests", new[] { "PhysicalAccessId" });
            DropColumn("dbo.AccessRequests", "PhysicalAccessId");
            DropTable("dbo.PhysicalAreas");
            DropTable("dbo.PhysicalAccessItems");
            DropTable("dbo.PhysicalAccesses");
        }
    }
}

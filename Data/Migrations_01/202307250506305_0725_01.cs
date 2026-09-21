namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0725_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserAccesses",
                c => new
                    {
                        UserAccessId = c.Long(nullable: false, identity: true),
                        AccessRequestId = c.Long(nullable: false),
                        LegalIdDoc = c.String(),
                        NonEmpAcknowledgementDoc = c.String(),
                        UserAccessType = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserAccessId)
                .ForeignKey("dbo.AccessRequests", t => t.AccessRequestId)
                .Index(t => t.AccessRequestId);
            
            CreateTable(
                "dbo.UserAccessItems",
                c => new
                    {
                        UserAccessItemId = c.Long(nullable: false, identity: true),
                        UserAccessId = c.Long(nullable: false),
                        UserAccessItemTypeId = c.Long(nullable: false),
                        Description = c.String(nullable: false),
                        Justification = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserAccessItemId)
                .ForeignKey("dbo.UserAccesses", t => t.UserAccessId, cascadeDelete: true)
                .ForeignKey("dbo.UserAccessItemTypes", t => t.UserAccessItemTypeId, cascadeDelete: true)
                .Index(t => t.UserAccessId)
                .Index(t => t.UserAccessItemTypeId);
            
            CreateTable(
                "dbo.UserAccessItemTypes",
                c => new
                    {
                        UserAccessItemTypeId = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        AdditionalInfo = c.String(),
                        External = c.Boolean(nullable: false),
                        Internal = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserAccessItemTypeId);
            
            AddColumn("dbo.AccessRequests", "UserAccessId", c => c.Long());
            CreateIndex("dbo.AccessRequests", "UserAccessId");
            AddForeignKey("dbo.AccessRequests", "UserAccessId", "dbo.UserAccesses", "UserAccessId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccessRequests", "UserAccessId", "dbo.UserAccesses");
            DropForeignKey("dbo.UserAccessItems", "UserAccessItemTypeId", "dbo.UserAccessItemTypes");
            DropForeignKey("dbo.UserAccessItems", "UserAccessId", "dbo.UserAccesses");
            DropForeignKey("dbo.UserAccesses", "AccessRequestId", "dbo.AccessRequests");
            DropIndex("dbo.UserAccessItems", new[] { "UserAccessItemTypeId" });
            DropIndex("dbo.UserAccessItems", new[] { "UserAccessId" });
            DropIndex("dbo.UserAccesses", new[] { "AccessRequestId" });
            DropIndex("dbo.AccessRequests", new[] { "UserAccessId" });
            DropColumn("dbo.AccessRequests", "UserAccessId");
            DropTable("dbo.UserAccessItemTypes");
            DropTable("dbo.UserAccessItems");
            DropTable("dbo.UserAccesses");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1115_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChangeRequestDocs",
                c => new
                    {
                        ChangeRequestDocId = c.Long(nullable: false, identity: true),
                        ChangeRequestId = c.Long(nullable: false),
                        DocumentName = c.String(nullable: false),
                        DocType = c.Int(nullable: false),
                        FileName = c.String(nullable: false),
                        FileUrl = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeRequestDocId)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .ForeignKey("dbo.ChangeRequests", t => t.ChangeRequestId, cascadeDelete: true)
                .Index(t => t.ChangeRequestId)
                .Index(t => t.UpdatedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChangeRequestDocs", "ChangeRequestId", "dbo.ChangeRequests");
            DropForeignKey("dbo.ChangeRequestDocs", "UpdatedBy", "dbo.Users");
            DropIndex("dbo.ChangeRequestDocs", new[] { "UpdatedBy" });
            DropIndex("dbo.ChangeRequestDocs", new[] { "ChangeRequestId" });
            DropTable("dbo.ChangeRequestDocs");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260319_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DisposalRequestDocs",
                c => new
                    {
                        DisposalRequestDocId = c.Long(nullable: false, identity: true),
                        DisposalRequestId = c.Long(nullable: false),
                        DocumentName = c.String(nullable: false),
                        DocType = c.Int(nullable: false),
                        FileName = c.String(nullable: false),
                        FileUrl = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DisposalRequestDocId)
                .ForeignKey("dbo.DisposalRequests", t => t.DisposalRequestId, cascadeDelete: true)
                .Index(t => t.DisposalRequestId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DisposalRequestDocs", "DisposalRequestId", "dbo.DisposalRequests");
            DropIndex("dbo.DisposalRequestDocs", new[] { "DisposalRequestId" });
            DropTable("dbo.DisposalRequestDocs");
        }
    }
}

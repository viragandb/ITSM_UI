namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260130_01 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RevokeRequests", "RemoteAccessRequestId", "dbo.RemoteAccessRequests");
            DropIndex("dbo.RevokeRequests", new[] { "RemoteAccessRequestId" });
            DropTable("dbo.RevokeRequests");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RevokeRequests",
                c => new
                    {
                        RevokeRequestId = c.Long(nullable: false, identity: true),
                        RemoteAccessRequestId = c.Long(nullable: false),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        RevokedDate = c.DateTime(nullable: false),
                        Supervisor = c.String(),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RevokeRequestId);
            
            CreateIndex("dbo.RevokeRequests", "RemoteAccessRequestId");
            AddForeignKey("dbo.RevokeRequests", "RemoteAccessRequestId", "dbo.RemoteAccessRequests", "RemoteAccessRequestId", cascadeDelete: true);
        }
    }
}

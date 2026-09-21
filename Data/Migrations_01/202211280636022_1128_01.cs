namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1128_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketDocs",
                c => new
                    {
                        TicketDocId = c.Long(nullable: false, identity: true),
                        TicketId = c.Long(nullable: false),
                        DocumentName = c.String(nullable: false),
                        DocType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        FileName = c.String(nullable: false),
                        FileUrl = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TicketDocId)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .Index(t => t.TicketId)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.TicketLogs",
                c => new
                    {
                        TicketLogId = c.Long(nullable: false, identity: true),
                        TicketId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Comment = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TicketLogId)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.TicketId)
                .Index(t => t.UpdatedBy);
            
            AddColumn("dbo.Tickets", "PendingTeamId", c => c.Long(nullable: false));
            AddColumn("dbo.Tickets", "AllocatedType", c => c.Int(nullable: false));
            AddColumn("dbo.TicketUpdates", "TeamId", c => c.Long(nullable: false));
            AddColumn("dbo.TicketUpdates", "AllocatedType", c => c.Int(nullable: false));
            CreateIndex("dbo.Tickets", "PendingTeamId");
            CreateIndex("dbo.TicketUpdates", "TeamId");
            AddForeignKey("dbo.Tickets", "PendingTeamId", "dbo.Teams", "TeamId");
            AddForeignKey("dbo.TicketUpdates", "TeamId", "dbo.Teams", "TeamId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketUpdates", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.TicketLogs", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.TicketLogs", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketDocs", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketDocs", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.Tickets", "PendingTeamId", "dbo.Teams");
            DropIndex("dbo.TicketUpdates", new[] { "TeamId" });
            DropIndex("dbo.TicketLogs", new[] { "UpdatedBy" });
            DropIndex("dbo.TicketLogs", new[] { "TicketId" });
            DropIndex("dbo.TicketDocs", new[] { "UpdatedBy" });
            DropIndex("dbo.TicketDocs", new[] { "TicketId" });
            DropIndex("dbo.Tickets", new[] { "PendingTeamId" });
            DropColumn("dbo.TicketUpdates", "AllocatedType");
            DropColumn("dbo.TicketUpdates", "TeamId");
            DropColumn("dbo.Tickets", "AllocatedType");
            DropColumn("dbo.Tickets", "PendingTeamId");
            DropTable("dbo.TicketLogs");
            DropTable("dbo.TicketDocs");
        }
    }
}

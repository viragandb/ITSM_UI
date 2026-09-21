namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0324_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GRNotes",
                c => new
                    {
                        GRNoteId = c.Long(nullable: false, identity: true),
                        GRNNumber = c.String(nullable: false),
                        PONumber = c.String(nullable: false),
                        InvoiceNumber = c.String(),
                        PurchaseDate = c.DateTime(nullable: false),
                        ReceivedDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        VendorId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.GRNoteId)
                .ForeignKey("dbo.Vendors", t => t.VendorId)
                .Index(t => t.VendorId);
            
            AddColumn("dbo.Assets", "GRNoteId", c => c.Long(nullable: false));
            AddColumn("dbo.Assets", "StockInDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Assets", "GRNoteId");
            AddForeignKey("dbo.Assets", "GRNoteId", "dbo.GRNotes", "GRNoteId", cascadeDelete: true);
            DropColumn("dbo.Assets", "PurchaseDate");
            DropColumn("dbo.Assets", "ReceivedDate");
            DropColumn("dbo.Assets", "PONumber");
            DropColumn("dbo.Assets", "InvoiceNumber");
            DropColumn("dbo.Assets", "GRNNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assets", "GRNNumber", c => c.String());
            AddColumn("dbo.Assets", "InvoiceNumber", c => c.String());
            AddColumn("dbo.Assets", "PONumber", c => c.String());
            AddColumn("dbo.Assets", "ReceivedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Assets", "PurchaseDate", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.Assets", "GRNoteId", "dbo.GRNotes");
            DropForeignKey("dbo.GRNotes", "VendorId", "dbo.Vendors");
            DropIndex("dbo.GRNotes", new[] { "VendorId" });
            DropIndex("dbo.Assets", new[] { "GRNoteId" });
            DropColumn("dbo.Assets", "StockInDate");
            DropColumn("dbo.Assets", "GRNoteId");
            DropTable("dbo.GRNotes");
        }
    }
}

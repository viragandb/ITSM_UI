namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0322_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "Barcode", c => c.String(nullable: false));
            AddColumn("dbo.Assets", "PurchasePrice", c => c.Double(nullable: false));
            AddColumn("dbo.Assets", "PurchaseDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Assets", "ReceivedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Assets", "PONumber", c => c.String());
            AddColumn("dbo.Assets", "InvoiceNumber", c => c.String());
            AddColumn("dbo.Assets", "GRNNumber", c => c.String());
            AddColumn("dbo.Assets", "WarrantyPeriod", c => c.Double(nullable: false));
            AddColumn("dbo.Assets", "WarrantyExpire", c => c.DateTime(nullable: false));
            AddColumn("dbo.Assets", "MaintenanceFrequency", c => c.Int(nullable: false));
            AddColumn("dbo.Assets", "NextMaintenanceDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Assets", "AllocatedType", c => c.Int(nullable: false));
            AddColumn("dbo.Assets", "ToBeReturnedDate", c => c.DateTime());
            AddColumn("dbo.Assets", "Priority", c => c.Int(nullable: false));
            AddColumn("dbo.Assets", "ResponsibleTeamId", c => c.Long(nullable: false));
            AlterColumn("dbo.Assets", "AssetNo", c => c.String(nullable: false));
            CreateIndex("dbo.Assets", "ResponsibleTeamId");
            AddForeignKey("dbo.Assets", "ResponsibleTeamId", "dbo.Teams", "TeamId");
            DropColumn("dbo.Assets", "AssetCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assets", "AssetCode", c => c.String());
            DropForeignKey("dbo.Assets", "ResponsibleTeamId", "dbo.Teams");
            DropIndex("dbo.Assets", new[] { "ResponsibleTeamId" });
            AlterColumn("dbo.Assets", "AssetNo", c => c.String());
            DropColumn("dbo.Assets", "ResponsibleTeamId");
            DropColumn("dbo.Assets", "Priority");
            DropColumn("dbo.Assets", "ToBeReturnedDate");
            DropColumn("dbo.Assets", "AllocatedType");
            DropColumn("dbo.Assets", "NextMaintenanceDate");
            DropColumn("dbo.Assets", "MaintenanceFrequency");
            DropColumn("dbo.Assets", "WarrantyExpire");
            DropColumn("dbo.Assets", "WarrantyPeriod");
            DropColumn("dbo.Assets", "GRNNumber");
            DropColumn("dbo.Assets", "InvoiceNumber");
            DropColumn("dbo.Assets", "PONumber");
            DropColumn("dbo.Assets", "ReceivedDate");
            DropColumn("dbo.Assets", "PurchaseDate");
            DropColumn("dbo.Assets", "PurchasePrice");
            DropColumn("dbo.Assets", "Barcode");
        }
    }
}

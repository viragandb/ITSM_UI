namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1125_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "BranchId", c => c.Long(nullable: false));
            AddColumn("dbo.Tickets", "OccurredDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Tickets", "BranchId");
            AddForeignKey("dbo.Tickets", "BranchId", "dbo.Branches", "BranchId");
            DropColumn("dbo.Tickets", "CompanyId");
            DropColumn("dbo.Tickets", "Company");
            DropColumn("dbo.Tickets", "DepartmentId");
            DropColumn("dbo.Tickets", "LocationId");
            DropColumn("dbo.Tickets", "LocationName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickets", "LocationName", c => c.String());
            AddColumn("dbo.Tickets", "LocationId", c => c.Long(nullable: false));
            AddColumn("dbo.Tickets", "DepartmentId", c => c.Long(nullable: false));
            AddColumn("dbo.Tickets", "Company", c => c.String());
            AddColumn("dbo.Tickets", "CompanyId", c => c.Long(nullable: false));
            DropForeignKey("dbo.Tickets", "BranchId", "dbo.Branches");
            DropIndex("dbo.Tickets", new[] { "BranchId" });
            DropColumn("dbo.Tickets", "OccurredDate");
            DropColumn("dbo.Tickets", "BranchId");
        }
    }
}

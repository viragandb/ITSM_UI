namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1212_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "SLAIT", c => c.Double(nullable: false));
            AddColumn("dbo.Tickets", "SLAVendor", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "SLAVendor");
            DropColumn("dbo.Tickets", "SLAIT");
        }
    }
}

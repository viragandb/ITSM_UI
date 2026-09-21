namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1123_02 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RequestTypes", "Sla", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RequestTypes", "Sla", c => c.String());
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260107_01 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RemoteAgreements", "PerformanceMonitoring");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RemoteAgreements", "PerformanceMonitoring", c => c.String());
        }
    }
}

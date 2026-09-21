namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251212_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAgreements", "DaysOfWeek", c => c.Long());
            AddColumn("dbo.RemoteAgreements", "PerformanceMonitoring", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RemoteAgreements", "PerformanceMonitoring");
            DropColumn("dbo.RemoteAgreements", "DaysOfWeek");
        }
    }
}

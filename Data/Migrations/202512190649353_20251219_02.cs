namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251219_02 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RemoteAgreements", "NoticePeriod", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RemoteAgreements", "NoticePeriod", c => c.Long());
        }
    }
}

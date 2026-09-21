namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251219_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAgreements", "NoticePeriod", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RemoteAgreements", "NoticePeriod");
        }
    }
}

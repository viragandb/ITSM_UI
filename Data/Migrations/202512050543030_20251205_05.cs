namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251205_05 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RemoteAgreements", "WorkLocationAddress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RemoteAgreements", "WorkLocationAddress", c => c.String());
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260319_02 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DisposalRequests", "SpendsSmartFilePath");
            DropColumn("dbo.DisposalRequests", "ProcurementFilePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DisposalRequests", "ProcurementFilePath", c => c.String());
            AddColumn("dbo.DisposalRequests", "SpendsSmartFilePath", c => c.String());
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260320_01 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DisposalRequestLogs", "UpdatedBy", c => c.String(maxLength: 20, unicode: false));
            AlterColumn("dbo.DisposalRequestUpdates", "UpdatedBy", c => c.String(maxLength: 20, unicode: false));
            CreateIndex("dbo.DisposalRequestLogs", "UpdatedBy");
            CreateIndex("dbo.DisposalRequestUpdates", "UpdatedBy");
            AddForeignKey("dbo.DisposalRequestLogs", "UpdatedBy", "dbo.Users", "EmpNo");
            AddForeignKey("dbo.DisposalRequestUpdates", "UpdatedBy", "dbo.Users", "EmpNo");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DisposalRequestUpdates", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.DisposalRequestLogs", "UpdatedBy", "dbo.Users");
            DropIndex("dbo.DisposalRequestUpdates", new[] { "UpdatedBy" });
            DropIndex("dbo.DisposalRequestLogs", new[] { "UpdatedBy" });
            AlterColumn("dbo.DisposalRequestUpdates", "UpdatedBy", c => c.String());
            AlterColumn("dbo.DisposalRequestLogs", "UpdatedBy", c => c.String());
        }
    }
}

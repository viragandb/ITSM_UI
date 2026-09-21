namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251209_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAccessRequests", "ApprovalStage", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RemoteAccessRequests", "ApprovalStage");
        }
    }
}

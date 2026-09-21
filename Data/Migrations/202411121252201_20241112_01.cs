namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20241112_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessRequests", "IsApprovalTerm", c => c.Boolean(nullable: false));
            AddColumn("dbo.AccessRequests", "ApprovalAttachment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccessRequests", "ApprovalAttachment");
            DropColumn("dbo.AccessRequests", "IsApprovalTerm");
        }
    }
}

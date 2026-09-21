namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0707_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessRequests", "ApprovalBy", c => c.String(nullable: false, maxLength: 20, unicode: false));
            CreateIndex("dbo.AccessRequests", "ApprovalBy");
            AddForeignKey("dbo.AccessRequests", "ApprovalBy", "dbo.Users", "EmpNo");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccessRequests", "ApprovalBy", "dbo.Users");
            DropIndex("dbo.AccessRequests", new[] { "ApprovalBy" });
            DropColumn("dbo.AccessRequests", "ApprovalBy");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240418_04 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChangeRequests", "ChangeApprovedBy", "dbo.Users");
            DropIndex("dbo.ChangeRequests", new[] { "ChangeApprovedBy" });
            DropColumn("dbo.ChangeRequests", "ChangeApprovedBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChangeRequests", "ChangeApprovedBy", c => c.String(nullable: false, maxLength: 20, unicode: false));
            CreateIndex("dbo.ChangeRequests", "ChangeApprovedBy");
            AddForeignKey("dbo.ChangeRequests", "ChangeApprovedBy", "dbo.Users", "EmpNo");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260120_01 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.RemoteAccessRequests", name: "ApprovalBy", newName: "FinalApprovalBy");
            RenameIndex(table: "dbo.RemoteAccessRequests", name: "IX_ApprovalBy", newName: "IX_FinalApprovalBy");

            DropForeignKey("dbo.RemoteAccessRequests", "FK_dbo.RemoteAccessRequests_dbo.Users_ApprovalBy");
            AddForeignKey("dbo.RemoteAccessRequests", "FinalApprovalBy", "dbo.Users", "EmpNo", name: "FK_dbo.RemoteAccessRequests_dbo.Users_FinalApprovalBy");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RemoteAccessRequests", "FK_dbo.RemoteAccessRequests_dbo.Users_FinalApprovalBy");
            AddForeignKey("dbo.RemoteAccessRequests", "ApprovalBy", "dbo.Users", "EmpNo", name: "FK_dbo.RemoteAccessRequests_dbo.Users_ApprovalBy");

            RenameIndex(table: "dbo.RemoteAccessRequests", name: "IX_FinalApprovalBy", newName: "IX_ApprovalBy");
            RenameColumn(table: "dbo.RemoteAccessRequests", name: "FinalApprovalBy", newName: "ApprovalBy");
        }
    }
}

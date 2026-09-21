namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251219_03 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAgreements", "SignedByEmployerId", c => c.String(nullable: false, maxLength: 20, unicode: false));
            CreateIndex("dbo.RemoteAgreements", "SignedByEmployerId");
            AddForeignKey("dbo.RemoteAgreements", "SignedByEmployerId", "dbo.Users", "EmpNo");
            DropColumn("dbo.RemoteAgreements", "SignedByEmployer");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RemoteAgreements", "SignedByEmployer", c => c.String());
            DropForeignKey("dbo.RemoteAgreements", "SignedByEmployerId", "dbo.Users");
            DropIndex("dbo.RemoteAgreements", new[] { "SignedByEmployerId" });
            DropColumn("dbo.RemoteAgreements", "SignedByEmployerId");
        }
    }
}

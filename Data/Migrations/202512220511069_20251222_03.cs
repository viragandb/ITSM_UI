namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251222_03 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RemoteAgreements", new[] { "SignedByEmployerId" });
            AlterColumn("dbo.RemoteAgreements", "SignedByEmployerId", c => c.String(maxLength: 20, unicode: false));
            CreateIndex("dbo.RemoteAgreements", "SignedByEmployerId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RemoteAgreements", new[] { "SignedByEmployerId" });
            AlterColumn("dbo.RemoteAgreements", "SignedByEmployerId", c => c.String(nullable: false, maxLength: 20, unicode: false));
            CreateIndex("dbo.RemoteAgreements", "SignedByEmployerId");
        }
    }
}

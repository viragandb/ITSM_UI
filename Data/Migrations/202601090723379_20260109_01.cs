namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260109_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAccessRequests", "IsRequesterAVPOrVP", c => c.Boolean(nullable: false, defaultValue:false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RemoteAccessRequests", "IsRequesterAVPOrVP");
        }
    }
}

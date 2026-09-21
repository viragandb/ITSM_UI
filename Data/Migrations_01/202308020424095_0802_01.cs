namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0802_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessRequestTypes", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccessRequestTypes", "IsActive");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1123_04 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RequestTypes", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RequestTypes", "IsActive");
        }
    }
}

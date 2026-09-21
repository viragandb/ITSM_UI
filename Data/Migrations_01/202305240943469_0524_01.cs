namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0524_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "IsReopened", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "IsReopened");
        }
    }
}

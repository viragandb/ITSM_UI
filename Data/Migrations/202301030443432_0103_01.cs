namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0103_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "IsReviewed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "IsReviewed");
        }
    }
}

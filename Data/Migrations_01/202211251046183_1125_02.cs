namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1125_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "Priority", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "Priority");
        }
    }
}

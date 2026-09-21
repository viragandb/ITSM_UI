namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1213_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "RootCause", c => c.String());
            DropColumn("dbo.Tickets", "RouteCause");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickets", "RouteCause", c => c.String());
            DropColumn("dbo.Tickets", "RootCause");
        }
    }
}

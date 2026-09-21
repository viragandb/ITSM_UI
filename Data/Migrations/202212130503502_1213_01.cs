namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1213_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "RouteCause", c => c.String());
            AddColumn("dbo.Tickets", "LessonsLearnt", c => c.String());
            AddColumn("dbo.Tickets", "CorrectiveAction", c => c.String());
            AddColumn("dbo.Tickets", "PreventiveAction", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "PreventiveAction");
            DropColumn("dbo.Tickets", "CorrectiveAction");
            DropColumn("dbo.Tickets", "LessonsLearnt");
            DropColumn("dbo.Tickets", "RouteCause");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1013_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessRequestTypes", "ApproverInstructions", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccessRequestTypes", "ApproverInstructions");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1010_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessRequestTypes", "RequestorInstructions", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccessRequestTypes", "RequestorInstructions");
        }
    }
}

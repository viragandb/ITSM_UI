namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260123_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "ToleranceRate", c => c.Long(nullable: false, defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Departments", "ToleranceRate");
        }
    }
}

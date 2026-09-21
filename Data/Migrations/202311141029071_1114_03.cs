namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1114_03 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ChangeRequestCategories", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ChangeRequestCategories", "Name", c => c.String());
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1026_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "DAOCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Departments", "DAOCode");
        }
    }
}

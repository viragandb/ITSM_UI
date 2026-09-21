namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0526_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Branches", "DAOCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Branches", "DAOCode");
        }
    }
}

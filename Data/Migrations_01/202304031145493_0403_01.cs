namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0403_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "DepartmentId", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "DepartmentId");
        }
    }
}

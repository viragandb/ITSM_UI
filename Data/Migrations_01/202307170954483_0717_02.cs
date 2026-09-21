namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0717_02 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AccessRequests", "SystemAccessId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AccessRequests", "SystemAccessId", c => c.Long());
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0717_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessRequestTypes", "Icon", c => c.String());
            AddColumn("dbo.AccessRequestTypes", "Color", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccessRequestTypes", "Color");
            DropColumn("dbo.AccessRequestTypes", "Icon");
        }
    }
}

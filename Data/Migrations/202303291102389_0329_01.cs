namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0329_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemDocs", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemDocs", "Title");
        }
    }
}

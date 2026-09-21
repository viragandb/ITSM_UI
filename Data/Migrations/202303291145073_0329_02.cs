namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0329_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemDocs", "DocumentName", c => c.String());
            DropColumn("dbo.ItemDocs", "Title");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemDocs", "Title", c => c.String());
            DropColumn("dbo.ItemDocs", "DocumentName");
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240416_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChangeImplementDatas", "ChangePriority", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChangeImplementDatas", "ChangePriority");
        }
    }
}

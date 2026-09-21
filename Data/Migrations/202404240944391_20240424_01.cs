namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20240424_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChangeImplementDatas", "DowntimeTimePurpose", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChangeImplementDatas", "DowntimeTimePurpose");
        }
    }
}

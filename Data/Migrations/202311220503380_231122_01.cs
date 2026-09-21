namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _231122_01 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ChangeImplementDatas", "DowntimePlannedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ChangeImplementDatas", "DowntimePlannedDate", c => c.DateTime(nullable: false));
        }
    }
}

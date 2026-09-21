namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _231128_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DowntimeAlertEmails",
                c => new
                    {
                        DowntimeAlertEmailId = c.Long(nullable: false, identity: true),
                        ChangeImplementDataId = c.Long(nullable: false),
                        Email = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.DowntimeAlertEmailId)
                .ForeignKey("dbo.ChangeImplementDatas", t => t.ChangeImplementDataId, cascadeDelete: true)
                .Index(t => t.ChangeImplementDataId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DowntimeAlertEmails", "ChangeImplementDataId", "dbo.ChangeImplementDatas");
            DropIndex("dbo.DowntimeAlertEmails", new[] { "ChangeImplementDataId" });
            DropTable("dbo.DowntimeAlertEmails");
        }
    }
}

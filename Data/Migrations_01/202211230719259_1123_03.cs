namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1123_03 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RequestTypes", "RequestTypePriorityId", "dbo.RequestTypePriorities");
            DropIndex("dbo.RequestTypes", new[] { "RequestTypePriorityId" });
            CreateIndex("dbo.RequestTypePriorities", "RequestTypeId");
            AddForeignKey("dbo.RequestTypePriorities", "RequestTypeId", "dbo.RequestTypes", "RequestTypeId", cascadeDelete: true);
            DropColumn("dbo.RequestTypes", "RequestTypePriorityId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RequestTypes", "RequestTypePriorityId", c => c.Long());
            DropForeignKey("dbo.RequestTypePriorities", "RequestTypeId", "dbo.RequestTypes");
            DropIndex("dbo.RequestTypePriorities", new[] { "RequestTypeId" });
            CreateIndex("dbo.RequestTypes", "RequestTypePriorityId");
            AddForeignKey("dbo.RequestTypes", "RequestTypePriorityId", "dbo.RequestTypePriorities", "RequestTypePriorityId");
        }
    }
}

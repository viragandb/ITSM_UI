namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1122_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RequestTypePriorities",
                c => new
                    {
                        RequestTypePriorityId = c.Long(nullable: false, identity: true),
                        RequestTypeId = c.Long(nullable: false),
                        Priority = c.Int(nullable: false),
                        Resolve = c.Double(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RequestTypePriorityId);
            
            AddColumn("dbo.RequestTypes", "RequestTypePriorityId", c => c.Long());
            CreateIndex("dbo.RequestTypes", "RequestTypePriorityId");
            AddForeignKey("dbo.RequestTypes", "RequestTypePriorityId", "dbo.RequestTypePriorities", "RequestTypePriorityId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequestTypes", "RequestTypePriorityId", "dbo.RequestTypePriorities");
            DropIndex("dbo.RequestTypes", new[] { "RequestTypePriorityId" });
            DropColumn("dbo.RequestTypes", "RequestTypePriorityId");
            DropTable("dbo.RequestTypePriorities");
        }
    }
}

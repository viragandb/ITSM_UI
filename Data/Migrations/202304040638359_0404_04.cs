namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0404_04 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TransferItems", "AssignedTo", c => c.String(maxLength: 20, unicode: false));
            CreateIndex("dbo.TransferItems", "AssignedTo");
            AddForeignKey("dbo.TransferItems", "AssignedTo", "dbo.Users", "EmpNo");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransferItems", "AssignedTo", "dbo.Users");
            DropIndex("dbo.TransferItems", new[] { "AssignedTo" });
            AlterColumn("dbo.TransferItems", "AssignedTo", c => c.String());
        }
    }
}

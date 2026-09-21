namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0411_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "AssignedType", c => c.Int(nullable: false));
            DropColumn("dbo.Assets", "AssignedTypeEnum");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assets", "AssignedTypeEnum", c => c.Int(nullable: false));
            DropColumn("dbo.Assets", "AssignedType");
        }
    }
}

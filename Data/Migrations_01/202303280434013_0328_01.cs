namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0328_01 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Assets", "Barcode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Assets", "Barcode", c => c.String(nullable: false));
        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1227_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TempDocs",
                c => new
                    {
                        TempDocId = c.Long(nullable: false, identity: true),
                        EmpNo = c.String(),
                        DocumentName = c.String(nullable: false),
                        FileName = c.String(nullable: false),
                        FileUrl = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TempDocId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TempDocs");
        }
    }
}

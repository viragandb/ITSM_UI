namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20260224_01 : DbMigration
    {
        public override void Up()
        {
            // Renames the column while preserving all IDs and data
            RenameColumn(table: "dbo.RemoteAccessRequiredSystems",
                         name: "RemoteAccessRequiredSystemsId",
                         newName: "RemoteAccessRequiredSystemId");
        }
        
        public override void Down()
        {
            // Reverts the name change if you roll back
            RenameColumn(table: "dbo.RemoteAccessRequiredSystems",
                         name: "RemoteAccessRequiredSystemId",
                         newName: "RemoteAccessRequiredSystemsId");
        }
    }
}

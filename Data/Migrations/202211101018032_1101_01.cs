namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1101_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assets",
                c => new
                    {
                        AssetId = c.Long(nullable: false, identity: true),
                        AssetName = c.String(nullable: false),
                        AssetCategory = c.Int(nullable: false),
                        ItemId = c.Long(nullable: false),
                        TicketId = c.Long(nullable: false),
                        AssetTypeName = c.String(),
                        SerialNo = c.String(),
                        ModelName = c.String(),
                        Description = c.String(),
                        Location = c.String(),
                        Company = c.String(),
                        IsCritical = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AssetId);
            
            CreateTable(
                "dbo.DowntimeLogs",
                c => new
                    {
                        DowntimeLogId = c.Long(nullable: false, identity: true),
                        DowntimeType = c.Int(nullable: false),
                        AssetId = c.Long(nullable: false),
                        TicketId = c.Long(nullable: false),
                        Comment = c.String(),
                        DowntimeHrs = c.Double(nullable: false),
                        ApplicableDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DowntimeLogId)
                .ForeignKey("dbo.Assets", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .Index(t => t.AssetId)
                .Index(t => t.TicketId);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        TicketId = c.Long(nullable: false, identity: true),
                        Code = c.String(),
                        Subject = c.String(nullable: false),
                        TicketMedium = c.Int(nullable: false),
                        Description = c.String(),
                        CompanyId = c.Long(nullable: false),
                        Company = c.String(),
                        DepartmentId = c.Long(nullable: false),
                        DepartmentName = c.String(),
                        LocationId = c.Long(nullable: false),
                        LocationName = c.String(),
                        ContactEmail = c.String(),
                        ContactNo = c.String(),
                        RequestedBy = c.String(nullable: false, maxLength: 20, unicode: false),
                        RequestedDate = c.DateTime(nullable: false),
                        Impact = c.Int(nullable: false),
                        Urgency = c.Int(nullable: false),
                        TicketPriorityId = c.Long(),
                        RequestTypeId = c.Long(nullable: false),
                        Respond = c.Double(nullable: false),
                        Resolve = c.Double(nullable: false),
                        Status = c.Int(nullable: false),
                        SpentTime = c.Double(nullable: false),
                        CreatedBy = c.String(maxLength: 20, unicode: false),
                        AssignedBy = c.String(maxLength: 20, unicode: false),
                        AssignedTo = c.String(maxLength: 20, unicode: false),
                        IsTimeViolated = c.Boolean(nullable: false),
                        Rate = c.Int(nullable: false),
                        VendorId = c.Long(),
                        VendorRefNo = c.String(),
                        ReasonForSLA = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TicketId)
                .ForeignKey("dbo.Users", t => t.AssignedBy)
                .ForeignKey("dbo.Users", t => t.AssignedTo)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.RequestedBy, cascadeDelete: true)
                .ForeignKey("dbo.RequestTypes", t => t.RequestTypeId, cascadeDelete: true)
                .ForeignKey("dbo.TicketPriorities", t => t.TicketPriorityId)
                .ForeignKey("dbo.Vendors", t => t.VendorId)
                .Index(t => t.RequestedBy)
                .Index(t => t.TicketPriorityId)
                .Index(t => t.RequestTypeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.AssignedBy)
                .Index(t => t.AssignedTo)
                .Index(t => t.VendorId);
            
            CreateTable(
                "dbo.ItemAssets",
                c => new
                    {
                        ItemAssetId = c.Long(nullable: false, identity: true),
                        ItemType = c.Int(nullable: false),
                        ItemId = c.Long(nullable: false),
                        AssetId = c.Long(nullable: false),
                        AssetCategory = c.Int(nullable: false),
                        AssetCode = c.String(),
                        AssetName = c.String(),
                        SerialNo = c.String(),
                        ModelName = c.String(),
                        Description = c.String(),
                        Location = c.String(),
                        Company = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                        Ticket_TicketId = c.Long(),
                    })
                .PrimaryKey(t => t.ItemAssetId)
                .ForeignKey("dbo.Assets", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("dbo.Tickets", t => t.Ticket_TicketId)
                .Index(t => t.AssetId)
                .Index(t => t.Ticket_TicketId);
            
            CreateTable(
                "dbo.RequestTypes",
                c => new
                    {
                        RequestTypeId = c.Long(nullable: false, identity: true),
                        RequestTypeName = c.String(),
                        TicketType = c.Int(nullable: false),
                        TeamId = c.Long(nullable: false),
                        CategoryId = c.Long(nullable: false),
                        SubCategoryId = c.Long(nullable: false),
                        AssetTypeId = c.Long(nullable: false),
                        Sla = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RequestTypeId)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.SubCategories", t => t.SubCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.TeamId)
                .Index(t => t.CategoryId)
                .Index(t => t.SubCategoryId)
                .Index(t => t.AssetTypeId);
            
            CreateTable(
                "dbo.AssetTypes",
                c => new
                    {
                        AssetTypeId = c.Long(nullable: false, identity: true),
                        AssetTypeName = c.String(nullable: false),
                        AssetCategory = c.Int(nullable: false),
                        Icon = c.String(),
                        Color = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AssetTypeId);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        ItemId = c.Long(nullable: false, identity: true),
                        ItemName = c.String(nullable: false),
                        ItemMake = c.String(),
                        ItemBarCode = c.String(),
                        ItemModelNo = c.String(),
                        ItemSerialNo = c.String(nullable: false),
                        ItemFixedAssetNo = c.String(nullable: false),
                        ItemCost = c.String(),
                        ItemPurchaseDate = c.DateTime(),
                        ItemExpiryDate = c.DateTime(),
                        ItemInputDate = c.DateTime(),
                        CompanyId = c.Long(nullable: false),
                        LocationId = c.Long(nullable: false),
                        DepartmentId = c.Long(nullable: false),
                        VendorId = c.Long(nullable: false),
                        AssetTypeId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ItemId)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.Vendors", t => t.VendorId, cascadeDelete: true)
                .Index(t => t.DepartmentId)
                .Index(t => t.VendorId)
                .Index(t => t.AssetTypeId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Long(nullable: false, identity: true),
                        DepartmentName = c.String(nullable: false),
                        Email = c.String(),
                        EscalationEmail = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Vendors",
                c => new
                    {
                        VendorId = c.Long(nullable: false, identity: true),
                        VendorName = c.String(nullable: false),
                        ContactNo = c.String(),
                        Email = c.String(),
                        Address = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.VendorId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Long(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false),
                        Icon = c.String(),
                        Color = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.SubCategories",
                c => new
                    {
                        SubCategoryId = c.Long(nullable: false, identity: true),
                        SubCategoryName = c.String(nullable: false),
                        Icon = c.String(),
                        Color = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SubCategoryId);
            
            CreateTable(
                "dbo.TicketPriorities",
                c => new
                    {
                        TicketPriorityId = c.Long(nullable: false, identity: true),
                        TicketPriorityName = c.String(nullable: false),
                        Priority = c.Int(nullable: false),
                        Impact = c.Int(nullable: false),
                        Urgency = c.Int(nullable: false),
                        Respond = c.Double(nullable: false),
                        Resolve = c.Double(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TicketPriorityId);
            
            CreateTable(
                "dbo.TicketUpdates",
                c => new
                    {
                        TicketUpdateId = c.Long(nullable: false, identity: true),
                        TicketId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        SpentTime = c.Double(nullable: false),
                        Comment = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TicketUpdateId)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.TicketId)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.ChangeProblems",
                c => new
                    {
                        ChangeProblemId = c.Long(nullable: false, identity: true),
                        ChangeId = c.Long(nullable: false),
                        ProblemId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeProblemId)
                .ForeignKey("dbo.Changes", t => t.ChangeId, cascadeDelete: true)
                .ForeignKey("dbo.Problems", t => t.ProblemId)
                .Index(t => t.ChangeId)
                .Index(t => t.ProblemId);
            
            CreateTable(
                "dbo.Changes",
                c => new
                    {
                        ChangeId = c.Long(nullable: false, identity: true),
                        Subject = c.String(nullable: false),
                        Description = c.String(),
                        Priority = c.Int(nullable: false),
                        ChangeType = c.Int(nullable: false),
                        Risk = c.Int(nullable: false),
                        ChangeRequestTypeId = c.Long(nullable: false),
                        AssetTypeId = c.Long(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        RequestedBy = c.String(maxLength: 20, unicode: false),
                        RequestedDate = c.DateTime(nullable: false),
                        Reason = c.String(),
                        Impact = c.String(),
                        RollOutPlan = c.String(),
                        BackOutPlan = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeId)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId, cascadeDelete: true)
                .ForeignKey("dbo.ChangeRequestTypes", t => t.ChangeRequestTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.RequestedBy)
                .Index(t => t.ChangeRequestTypeId)
                .Index(t => t.AssetTypeId)
                .Index(t => t.RequestedBy);
            
            CreateTable(
                "dbo.ChangeRequestTypes",
                c => new
                    {
                        ChangeRequestTypeId = c.Long(nullable: false, identity: true),
                        ChangeRequestTypeName = c.String(nullable: false),
                        Icon = c.String(),
                        Color = c.String(),
                        IsKPIApplicable = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeRequestTypeId);
            
            CreateTable(
                "dbo.Problems",
                c => new
                    {
                        ProblemId = c.Long(nullable: false, identity: true),
                        Subject = c.String(nullable: false),
                        Description = c.String(),
                        Priority = c.Int(nullable: false),
                        RequestTypeId = c.Long(nullable: false),
                        RootCause = c.String(),
                        Impact = c.String(),
                        Symptoms = c.String(),
                        Solution = c.String(),
                        Status = c.Int(nullable: false),
                        RequestedBy = c.String(maxLength: 20, unicode: false),
                        RequestedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProblemId)
                .ForeignKey("dbo.Users", t => t.RequestedBy)
                .ForeignKey("dbo.RequestTypes", t => t.RequestTypeId)
                .Index(t => t.RequestTypeId)
                .Index(t => t.RequestedBy);
            
            CreateTable(
                "dbo.ChangeTickets",
                c => new
                    {
                        ChangeTicketId = c.Long(nullable: false, identity: true),
                        ChangeId = c.Long(nullable: false),
                        TicketId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChangeTicketId)
                .ForeignKey("dbo.Changes", t => t.ChangeId, cascadeDelete: true)
                .ForeignKey("dbo.Tickets", t => t.TicketId)
                .Index(t => t.ChangeId)
                .Index(t => t.TicketId);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        EventId = c.Long(nullable: false, identity: true),
                        Subject = c.String(nullable: false),
                        Description = c.String(),
                        ActionTaken = c.String(),
                        EventTypeId = c.Long(nullable: false),
                        EventDate = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        RequestedBy = c.String(maxLength: 20, unicode: false),
                        RequestedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EventId)
                .ForeignKey("dbo.EventTypes", t => t.EventTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.RequestedBy)
                .Index(t => t.EventTypeId)
                .Index(t => t.RequestedBy);
            
            CreateTable(
                "dbo.EventTypes",
                c => new
                    {
                        EventTypeId = c.Long(nullable: false, identity: true),
                        EventTypeName = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EventTypeId);
            
            CreateTable(
                "dbo.ItemDocs",
                c => new
                    {
                        ItemDocId = c.Long(nullable: false, identity: true),
                        ItemType = c.Int(nullable: false),
                        ItemId = c.Long(nullable: false),
                        FileName = c.String(nullable: false),
                        FileUrl = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ItemDocId);
            
            CreateTable(
                "dbo.ItemLogs",
                c => new
                    {
                        ItemLogId = c.Long(nullable: false, identity: true),
                        ItemType = c.Int(nullable: false),
                        ItemId = c.Long(nullable: false),
                        Comment = c.String(),
                        Status = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(maxLength: 20, unicode: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ItemLogId)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.KedbItems",
                c => new
                    {
                        KedbItemId = c.Long(nullable: false, identity: true),
                        Subject = c.String(),
                        RootCause = c.String(),
                        Solution = c.String(),
                        Status = c.Int(nullable: false),
                        RequestTypeId = c.Long(nullable: false),
                        TicketId = c.Long(),
                        ProblemId = c.Long(),
                        RequestedBy = c.String(maxLength: 20, unicode: false),
                        RequestedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.KedbItemId)
                .ForeignKey("dbo.Problems", t => t.ProblemId)
                .ForeignKey("dbo.Users", t => t.RequestedBy)
                .ForeignKey("dbo.RequestTypes", t => t.RequestTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Tickets", t => t.TicketId)
                .Index(t => t.RequestTypeId)
                .Index(t => t.TicketId)
                .Index(t => t.ProblemId)
                .Index(t => t.RequestedBy);
            
            CreateTable(
                "dbo.Policies",
                c => new
                    {
                        PolicyId = c.Int(nullable: false, identity: true),
                        PolicyName = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PolicyId);
            
            CreateTable(
                "dbo.ProblemTickets",
                c => new
                    {
                        ProblemTicketId = c.Long(nullable: false, identity: true),
                        ProblemId = c.Long(nullable: false),
                        TicketId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProblemTicketId)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .Index(t => t.TicketId);
            
            CreateTable(
                "dbo.ReleaseChanges",
                c => new
                    {
                        ReleaseChangeId = c.Long(nullable: false, identity: true),
                        ReleaseId = c.Long(nullable: false),
                        ChangeId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReleaseChangeId)
                .ForeignKey("dbo.Changes", t => t.ChangeId, cascadeDelete: true)
                .Index(t => t.ChangeId);
            
            CreateTable(
                "dbo.Releases",
                c => new
                    {
                        ReleaseId = c.Long(nullable: false, identity: true),
                        Subject = c.String(nullable: false),
                        Description = c.String(),
                        Priority = c.Int(nullable: false),
                        ReleaseType = c.Int(nullable: false),
                        ChangeRequestTypeId = c.Long(nullable: false),
                        AssetTypeId = c.Long(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        RequestedBy = c.String(maxLength: 20, unicode: false),
                        RequestedDate = c.DateTime(nullable: false),
                        BuildPlan = c.String(),
                        TestPlan = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReleaseId)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId, cascadeDelete: true)
                .ForeignKey("dbo.ChangeRequestTypes", t => t.ChangeRequestTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.RequestedBy)
                .Index(t => t.ChangeRequestTypeId)
                .Index(t => t.AssetTypeId)
                .Index(t => t.RequestedBy);
            
            CreateTable(
                "dbo.TicketRates",
                c => new
                    {
                        TicketRateId = c.Long(nullable: false, identity: true),
                        Rate = c.Int(nullable: false),
                        RateValue = c.Double(nullable: false),
                        TicketId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TicketRateId)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .Index(t => t.TicketId);
            
            CreateTable(
                "dbo.TicketStatus",
                c => new
                    {
                        TicketStatusId = c.Long(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        TeamId = c.Long(nullable: false),
                        TimeCapture = c.Boolean(nullable: false),
                        AllowUpdate = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TicketStatusId)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketStatus", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.TicketRates", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.Releases", "RequestedBy", "dbo.Users");
            DropForeignKey("dbo.Releases", "ChangeRequestTypeId", "dbo.ChangeRequestTypes");
            DropForeignKey("dbo.Releases", "AssetTypeId", "dbo.AssetTypes");
            DropForeignKey("dbo.ReleaseChanges", "ChangeId", "dbo.Changes");
            DropForeignKey("dbo.ProblemTickets", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.KedbItems", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.KedbItems", "RequestTypeId", "dbo.RequestTypes");
            DropForeignKey("dbo.KedbItems", "RequestedBy", "dbo.Users");
            DropForeignKey("dbo.KedbItems", "ProblemId", "dbo.Problems");
            DropForeignKey("dbo.ItemLogs", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.Events", "RequestedBy", "dbo.Users");
            DropForeignKey("dbo.Events", "EventTypeId", "dbo.EventTypes");
            DropForeignKey("dbo.ChangeTickets", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.ChangeTickets", "ChangeId", "dbo.Changes");
            DropForeignKey("dbo.ChangeProblems", "ProblemId", "dbo.Problems");
            DropForeignKey("dbo.Problems", "RequestTypeId", "dbo.RequestTypes");
            DropForeignKey("dbo.Problems", "RequestedBy", "dbo.Users");
            DropForeignKey("dbo.ChangeProblems", "ChangeId", "dbo.Changes");
            DropForeignKey("dbo.Changes", "RequestedBy", "dbo.Users");
            DropForeignKey("dbo.Changes", "ChangeRequestTypeId", "dbo.ChangeRequestTypes");
            DropForeignKey("dbo.Changes", "AssetTypeId", "dbo.AssetTypes");
            DropForeignKey("dbo.DowntimeLogs", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.Tickets", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.TicketUpdates", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.TicketUpdates", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.Tickets", "TicketPriorityId", "dbo.TicketPriorities");
            DropForeignKey("dbo.Tickets", "RequestTypeId", "dbo.RequestTypes");
            DropForeignKey("dbo.RequestTypes", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.RequestTypes", "SubCategoryId", "dbo.SubCategories");
            DropForeignKey("dbo.RequestTypes", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.RequestTypes", "AssetTypeId", "dbo.AssetTypes");
            DropForeignKey("dbo.Items", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.Items", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Items", "AssetTypeId", "dbo.AssetTypes");
            DropForeignKey("dbo.Tickets", "RequestedBy", "dbo.Users");
            DropForeignKey("dbo.ItemAssets", "Ticket_TicketId", "dbo.Tickets");
            DropForeignKey("dbo.ItemAssets", "AssetId", "dbo.Assets");
            DropForeignKey("dbo.Tickets", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Tickets", "AssignedTo", "dbo.Users");
            DropForeignKey("dbo.Tickets", "AssignedBy", "dbo.Users");
            DropForeignKey("dbo.DowntimeLogs", "AssetId", "dbo.Assets");
            DropIndex("dbo.TicketStatus", new[] { "TeamId" });
            DropIndex("dbo.TicketRates", new[] { "TicketId" });
            DropIndex("dbo.Releases", new[] { "RequestedBy" });
            DropIndex("dbo.Releases", new[] { "AssetTypeId" });
            DropIndex("dbo.Releases", new[] { "ChangeRequestTypeId" });
            DropIndex("dbo.ReleaseChanges", new[] { "ChangeId" });
            DropIndex("dbo.ProblemTickets", new[] { "TicketId" });
            DropIndex("dbo.KedbItems", new[] { "RequestedBy" });
            DropIndex("dbo.KedbItems", new[] { "ProblemId" });
            DropIndex("dbo.KedbItems", new[] { "TicketId" });
            DropIndex("dbo.KedbItems", new[] { "RequestTypeId" });
            DropIndex("dbo.ItemLogs", new[] { "UpdatedBy" });
            DropIndex("dbo.Events", new[] { "RequestedBy" });
            DropIndex("dbo.Events", new[] { "EventTypeId" });
            DropIndex("dbo.ChangeTickets", new[] { "TicketId" });
            DropIndex("dbo.ChangeTickets", new[] { "ChangeId" });
            DropIndex("dbo.Problems", new[] { "RequestedBy" });
            DropIndex("dbo.Problems", new[] { "RequestTypeId" });
            DropIndex("dbo.Changes", new[] { "RequestedBy" });
            DropIndex("dbo.Changes", new[] { "AssetTypeId" });
            DropIndex("dbo.Changes", new[] { "ChangeRequestTypeId" });
            DropIndex("dbo.ChangeProblems", new[] { "ProblemId" });
            DropIndex("dbo.ChangeProblems", new[] { "ChangeId" });
            DropIndex("dbo.TicketUpdates", new[] { "UpdatedBy" });
            DropIndex("dbo.TicketUpdates", new[] { "TicketId" });
            DropIndex("dbo.Items", new[] { "AssetTypeId" });
            DropIndex("dbo.Items", new[] { "VendorId" });
            DropIndex("dbo.Items", new[] { "DepartmentId" });
            DropIndex("dbo.RequestTypes", new[] { "AssetTypeId" });
            DropIndex("dbo.RequestTypes", new[] { "SubCategoryId" });
            DropIndex("dbo.RequestTypes", new[] { "CategoryId" });
            DropIndex("dbo.RequestTypes", new[] { "TeamId" });
            DropIndex("dbo.ItemAssets", new[] { "Ticket_TicketId" });
            DropIndex("dbo.ItemAssets", new[] { "AssetId" });
            DropIndex("dbo.Tickets", new[] { "VendorId" });
            DropIndex("dbo.Tickets", new[] { "AssignedTo" });
            DropIndex("dbo.Tickets", new[] { "AssignedBy" });
            DropIndex("dbo.Tickets", new[] { "CreatedBy" });
            DropIndex("dbo.Tickets", new[] { "RequestTypeId" });
            DropIndex("dbo.Tickets", new[] { "TicketPriorityId" });
            DropIndex("dbo.Tickets", new[] { "RequestedBy" });
            DropIndex("dbo.DowntimeLogs", new[] { "TicketId" });
            DropIndex("dbo.DowntimeLogs", new[] { "AssetId" });
            DropTable("dbo.TicketStatus");
            DropTable("dbo.TicketRates");
            DropTable("dbo.Releases");
            DropTable("dbo.ReleaseChanges");
            DropTable("dbo.ProblemTickets");
            DropTable("dbo.Policies");
            DropTable("dbo.KedbItems");
            DropTable("dbo.ItemLogs");
            DropTable("dbo.ItemDocs");
            DropTable("dbo.EventTypes");
            DropTable("dbo.Events");
            DropTable("dbo.ChangeTickets");
            DropTable("dbo.Problems");
            DropTable("dbo.ChangeRequestTypes");
            DropTable("dbo.Changes");
            DropTable("dbo.ChangeProblems");
            DropTable("dbo.TicketUpdates");
            DropTable("dbo.TicketPriorities");
            DropTable("dbo.SubCategories");
            DropTable("dbo.Categories");
            DropTable("dbo.Vendors");
            DropTable("dbo.Departments");
            DropTable("dbo.Items");
            DropTable("dbo.AssetTypes");
            DropTable("dbo.RequestTypes");
            DropTable("dbo.ItemAssets");
            DropTable("dbo.Tickets");
            DropTable("dbo.DowntimeLogs");
            DropTable("dbo.Assets");
        }
    }
}

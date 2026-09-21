using Domain;
using Domain.AR;
using Domain.CM;
using Domain.DM;
using Domain.DR;
using Domain.IM;
using Domain.RA;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext()
           : base("DBCon")
        {

            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }
        public DbSet<AccessControl> AccessControls { get; set; }
        public DbSet<UserTeam> UserTeams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Serial> Serials { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemFunction> MenuItemFunctions { get; set; }
        public DbSet<Team> Teams { get; set; }


        #region Notification
        public DbSet<NotificationLog> NotificationLogs { get; set; }
        public DbSet<NotificationCategory> NotificationCategories { get; set; }

        #endregion

        public DbSet<Department> Departments { get; set; }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<ChangeRequestType> ChangeRequestTypes { get; set; }
        public DbSet<ItemAsset> ItemAssets { get; set; }
        public DbSet<ItemDoc> ItemDocs { get; set; }
        public DbSet<ItemLog> ItemLogs { get; set; }
        public DbSet<KedbItem> KedbItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<RequestType> RequestTypes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketUpdate> TicketUpdates { get; set; }
        public DbSet<TicketPriority> TicketPriorities { get; set; }
        public DbSet<TicketStatus> TicketStatuses { get; set; }
        public DbSet<TicketRate> TicketRates { get; set; }
        public DbSet<TicketTask> TicketTasks { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<ProblemTicket> ProblemTickets { get; set; }
        public DbSet<Change> Changes { get; set; }
        public DbSet<ChangeProblem> ChangeProblems { get; set; }
        public DbSet<ChangeTicket> ChangeTickets { get; set; }
        public DbSet<Release> Releases { get; set; }
        public DbSet<ReleaseChange> ReleaseChanges { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<VendorSLA> VendorSLAs { get; set; }

        public DbSet<Item> Items { get; set; }
        public DbSet<TempDoc> TempDocs { get; set; }

        public DbSet<SystemEnvironment> SystemEnvironments { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowLevel> WorkflowLevels { get; set; }


        #region Asset

        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<AssetMake> AssetMakes { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<TempAsset> TempAssets { get; set; }
        public DbSet<AssetLog> AssetLog { get; set; }
        public DbSet<AssetTransactionLog> AssetTransactionLogs { get; set; }
        public DbSet<GRNote> GRNotes { get; set; }
        public DbSet<AssetTransferRequest> AssetTransferRequests { get; set; }
        public DbSet<TransferItem> TransferItems { get; set; }
        public DbSet<AssetVerificationRequest> AssetVerificationRequests { get; set; }
        public DbSet<AssetVerificationItem> AssetVerificationItems { get; set; }
        public DbSet<DisposalItem> DisposalItems { get; set; }
        public DbSet<DisposalRequest> DisposalRequests { get; set; }
        public DbSet<DisposalRequestLog> DisposalRequestLogs { get; set; }
        public DbSet<DisposalRequestUpdate> DisposalRequestUpdates { get; set; }
        public DbSet<DisposalRequestDoc> DisposalRequestDocs { get; set; }
        public DbSet<AssetLink> AssetLinks { get; set; }
        public DbSet<AssetLinkLog> AssetLinkLogs { get; set; }



        #endregion

        #region Access Request

        public DbSet<AccessRequest> AccessRequests { get; set; }
        public DbSet<AccessRequestType> AccessRequestTypes { get; set; }
        public DbSet<SystemAccess> SystemAccesses { get; set; }
        public DbSet<SystemAccessItem> SystemAccessItems { get; set; }
        public DbSet<AccessRequestLog> AccessRequestLogs { get; set; }
        public DbSet<AccessRequestUpdate> AccessRequestUpdates { get; set; }

        public DbSet<AccessPrivilegeCategory> AccessPrivilegeCategories { get; set; }
        public DbSet<AccessApplication> AccessApplications { get; set; }
        public DbSet<AccessLevel> AccessLevels { get; set; }
        public DbSet<UserPrivilegeAccess> UserPrivilegeAccesses { get; set; }
        public DbSet<UserPrivilegeAccessItem> UserPrivilegeAccessItems { get; set; }

        public DbSet<UserAccess> UserAccesses { get; set; }
        public DbSet<UserAccessItem> UserAccessItems { get; set; }
        public DbSet<UserAccessItemType> UserAccessItemTypes { get; set; }

        public DbSet<DeviceAccess> DeviceAccesses { get; set; }
        public DbSet<DeviceAccessItem> DeviceAccessItems { get; set; }
        public DbSet<DeviceAccessItemType> DeviceAccessItemTypes { get; set; }

        public DbSet<RemoteAccess> RemoteAccesses { get; set; }
        public DbSet<RemoteAccessItem> RemoteAccessItems { get; set; }


        #endregion

        #region Change Request

        public DbSet<ChangeArea> ChangeAreas { get; set; }
        public DbSet<ChangeRequestCategory> ChangeRequestCategories { get; set; }
        public DbSet<ChangeImplementData> ChangeImplementDatas { get; set; }
        public DbSet<ChangeRequest> ChangeRequests { get; set; }
        public DbSet<ChangeRequestDoc> ChangeRequestDocs { get; set; }
        public DbSet<ChangeRequestLog> ChangeRequestLogs { get; set; }
        public DbSet<ChangeRequestTask> ChangeRequestTasks { get; set; }
        public DbSet<ChangeRequestUpdate> ChangeRequestUpdates { get; set; }
        public DbSet<ChangeType> ChangeTypes { get; set; }
        public DbSet<DowntimeAlertEmail> DowntimeAlertEmails { get; set; }


        #endregion

        #region Incident

        public DbSet<IncidentRequest> IncidentRequests { get; set; }
        public DbSet<IncidentRequestUpdate> IncidentRequestUpdates { get; set; }
        public DbSet<IncidentRequestLog> IncidentRequestLogs { get; set; }
        public DbSet<IncidentRequestDoc> IncidentRequestDocs { get; set; }
        public DbSet<IncidentRequestAsset> IncidentRequestAssets { get; set; }
        public DbSet<IncidentRequestTicket> IncidentRequestTickets { get; set; }


        #endregion

        #region DeviceManagementRequest

        public DbSet<DeviceManagementRequest> DeviceManagementRequest { get; set; }
        public DbSet<DeviceManagementRequestUpdate> DeviceManagementRequestUpdates { get; set; }
        public DbSet<DeviceManagementRequestLog> DeviceManagementRequestLogs { get; set; }


        #endregion

        #region Remote Access Request

        public DbSet<RemoteAccessRequest> RemoteAccessRequests { get; set; }
        public DbSet<RemoteAccessRequestUpdate> RemoteAccessRequestUpdates { get; set; }
        public DbSet<RemoteAccessRequestLog> RemoteAccessRequestLogs { get; set; }

        public DbSet<RemoteAgreement> RemoteAgreements { get; set; }

        public DbSet<RemoteAccessRequiredSystem> RemoteAccessRequiredSystems { get; set; }


        #endregion




        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Problem>()
                        .HasRequired(r => r.RequestType)
                        .WithMany()
                        .HasForeignKey(m => m.RequestTypeId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChangeTicket>()
                        .HasRequired(r => r.Ticket)
                        .WithMany()
                        .HasForeignKey(m => m.TicketId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChangeProblem>()
                     .HasRequired(r => r.Problem)
                     .WithMany()
                     .HasForeignKey(m => m.ProblemId)
                     .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ticket>()
             .HasRequired(r => r.Branch)
             .WithMany()
             .HasForeignKey(m => m.BranchId)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ticket>()
            .HasRequired(r => r.Department)
            .WithMany()
            .HasForeignKey(m => m.DepartmentId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ticket>()
             .HasRequired(r => r.PendingTeam)
             .WithMany()
             .HasForeignKey(m => m.PendingTeamId)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ticket>()
             .HasRequired(r => r.Level01Team)
             .WithMany()
             .HasForeignKey(m => m.Level01TeamId)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<TicketUpdate>()
             .HasRequired(r => r.Team)
             .WithMany()
             .HasForeignKey(m => m.TeamId)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<TicketTask>()
             .HasRequired(r => r.AllocatedTeam)
             .WithMany()
             .HasForeignKey(m => m.AllocatedTeamId)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<Asset>()
            .HasRequired(r => r.ResponsibleTeam)
            .WithMany()
            .HasForeignKey(m => m.ResponsibleTeamId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Asset>()
            .HasRequired(r => r.AssetType)
            .WithMany()
            .HasForeignKey(m => m.AssetTypeId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<GRNote>()
           .HasRequired(r => r.Vendor)
           .WithMany()
           .HasForeignKey(m => m.VendorId)
           .WillCascadeOnDelete(false);

            modelBuilder.Entity<AssetTransactionLog>()
           .HasRequired(r => r.Branch)
           .WithMany()
           .HasForeignKey(m => m.BranchId)
           .WillCascadeOnDelete(false);

            modelBuilder.Entity<AssetTransactionLog>()
            .HasRequired(r => r.Department)
            .WithMany()
            .HasForeignKey(m => m.DepartmentId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<AssetTransferRequest>()
           .HasRequired(r => r.Branch)
           .WithMany()
           .HasForeignKey(m => m.BranchId)
           .WillCascadeOnDelete(false);

            modelBuilder.Entity<AssetTransferRequest>()
            .HasRequired(r => r.Department)
            .WithMany()
            .HasForeignKey(m => m.DepartmentId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransferItem>()
            .HasRequired(r => r.AssetTransferRequest)
            .WithMany()
            .HasForeignKey(m => m.AssetTransferRequestId)
            .WillCascadeOnDelete(false);


            modelBuilder.Entity<AccessRequestType>()
            .HasRequired(r => r.Workflow)
            .WithMany()
            .HasForeignKey(m => m.WorkflowId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<AccessRequest>()
           .HasRequired(r => r.ApprovalByUser)
           .WithMany()
           .HasForeignKey(m => m.ApprovalBy)
           .WillCascadeOnDelete(false);

            //modelBuilder.Entity<AccessRequest>()
            //.HasRequired(r => r.SystemAccess)
            //.WithOptional(x => x.AccessRequest)
            //.WillCascadeOnDelete(true);

            modelBuilder.Entity<SystemAccess>()
            .HasRequired(r => r.AccessRequest)
            .WithMany()
            .HasForeignKey(m => m.AccessRequestId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<AccessRequest>()
            .HasOptional(r => r.SystemAccess)
            .WithMany()
            .HasForeignKey(m => m.SystemAccessId)
            .WillCascadeOnDelete(true);



            modelBuilder.Entity<UserPrivilegeAccess>()
           .HasRequired(r => r.AccessRequest)
           .WithMany()
           .HasForeignKey(m => m.AccessRequestId)
           .WillCascadeOnDelete(true);



            modelBuilder.Entity<AccessApplication>()
           .HasRequired(r => r.AccessPrivilegeCategory)
           .WithMany()
           .HasForeignKey(m => m.AccessPrivilegeCategoryId)
           .WillCascadeOnDelete(false);



            modelBuilder.Entity<PhysicalAccess>()
           .HasRequired(r => r.AccessRequest)
           .WithMany()
           .HasForeignKey(m => m.AccessRequestId)
           .WillCascadeOnDelete(false);

            modelBuilder.Entity<AccessRequest>()
          .HasOptional(r => r.PhysicalAccess)
          .WithMany()
          .HasForeignKey(m => m.PhysicalAccessId)
          .WillCascadeOnDelete(true);



            modelBuilder.Entity<UserAccess>()
          .HasRequired(r => r.AccessRequest)
          .WithMany()
          .HasForeignKey(m => m.AccessRequestId)
          .WillCascadeOnDelete(true);


            modelBuilder.Entity<DeviceAccess>()
           .HasRequired(r => r.AccessRequest)
           .WithMany()
           .HasForeignKey(m => m.AccessRequestId)
           .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAccess>()
             .HasRequired(r => r.AccessRequest)
             .WithMany()
             .HasForeignKey(m => m.AccessRequestId)
             .WillCascadeOnDelete(false);
            //modelBuilder.Entity<AccessRequest>()
            //.HasOptional(r => r.DeviceAccess)
            //.WithMany()
            //.HasForeignKey(m => m.DeviceAccessId)
            //.WillCascadeOnDelete(true);

            modelBuilder.Entity<ChangeImplementData>()
             .HasRequired(r => r.ChangeRequest)
             .WithMany()
             .HasForeignKey(m => m.ChangeRequestId)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChangeRequest>()
              .HasRequired(r => r.ApprovalByUser)
              .WithMany()
              .HasForeignKey(m => m.ApprovalBy)
              .WillCascadeOnDelete(false);

            //modelBuilder.Entity<ChangeRequest>()
            //  .HasRequired(r => r.ChangeApprovedByUser)
            //  .WithMany()
            //  .HasForeignKey(m => m.ChangeApprovedBy)
            //  .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChangeRequestTask>()
             .HasRequired(r => r.ChangeType)
             .WithMany()
             .HasForeignKey(m => m.ChangeTypeId)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChangeRequest>()
           .HasRequired(r => r.Team)
           .WithMany()
           .HasForeignKey(m => m.TeamId)
           .WillCascadeOnDelete(false);

            modelBuilder.Entity<IncidentRequest>()
         .HasRequired(r => r.Level01Team)
         .WithMany()
         .HasForeignKey(m => m.Level01TeamId)
         .WillCascadeOnDelete(false);

            modelBuilder.Entity<IncidentRequest>()
         .HasRequired(r => r.PendingTeam)
         .WithMany()
         .HasForeignKey(m => m.PendingTeamId)
         .WillCascadeOnDelete(false);

            modelBuilder.Entity<IncidentRequestTicket>()
            .HasRequired(r => r.Ticket)
            .WithMany()
            .HasForeignKey(m => m.TicketId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<AssetVerificationRequest>()
            .HasRequired(r => r.Branch)
            .WithMany()
            .HasForeignKey(m => m.BranchId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<AssetVerificationRequest>()
            .HasRequired(r => r.Department)
            .WithMany()
            .HasForeignKey(m => m.DepartmentId)
            .WillCascadeOnDelete(false);


            modelBuilder.Entity<DeviceManagementRequest>()
             .HasRequired(r => r.RequestedByUser)
             .WithMany()
             .HasForeignKey(m => m.RequestedBy)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<DeviceManagementRequest>()
               .HasRequired(r => r.RequestedForUser)
               .WithMany()
               .HasForeignKey(m => m.RequestedFor)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<DeviceManagementRequest>()
               .HasRequired(r => r.ApprovalByUser)
               .WithMany()
               .HasForeignKey(m => m.ApprovalBy)
               .WillCascadeOnDelete(false);


            modelBuilder.Entity<RemoteAccessRequest>()
             .HasRequired(r => r.CreatedUser)
             .WithMany()
             .HasForeignKey(m => m.CreatedBy)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAccessRequest>()
            .HasRequired(r => r.Department)
            .WithMany()
            .HasForeignKey(m => m.DepartmentId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAccessRequest>()
           .HasRequired(r => r.Branch)
           .WithMany()
           .HasForeignKey(m => m.BranchId)
           .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAccessRequest>()
            .HasOptional(r => r.Team)
            .WithMany()
            .HasForeignKey(r => r.TeamId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAccessRequest>()
            .HasRequired(r => r.RequestedUser)
            .WithMany()
            .HasForeignKey(m => m.RequestedBy)
            .WillCascadeOnDelete(false);


            modelBuilder.Entity<RemoteAccessRequest>()
            .HasRequired(r => r.FinalApprovalByUser)
            .WithMany()
            .HasForeignKey(m => m.FinalApprovalBy)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAccessRequest>()
          .HasOptional(r => r.ApprovedSupervisorUser)
          .WithMany()
          .HasForeignKey(m => m.ApprovedSupervisor)
          .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAccessRequest>()
          .HasOptional(r => r.ApprovedAVPOrVPUser)
          .WithMany()
          .HasForeignKey(m => m.ApprovedAVPOrVP)
          .WillCascadeOnDelete(false);


            modelBuilder.Entity<RemoteAccessRequest>()
            .HasRequired(r => r.Asset)
            .WithMany()
            .HasForeignKey(m => m.AssetId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAccessRequest>()
                .HasOptional(r => r.ClarificationRequestedByTeam)
                .WithMany()
                .HasForeignKey(r => r.ClarificationRequestedByTeamId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAccessRequest>()
                .HasOptional(r => r.ClarificationAssignedToTeam)
                .WithMany()
                .HasForeignKey(r => r.ClarificationAssignedToTeamId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAccessRequestUpdate>()
                .HasOptional(r => r.ClarificationRequestedByTeam)
                .WithMany(r => r.ClarificationRequestedByTeamUpdates)
                .HasForeignKey(r => r.ClarificationRequestedByTeamId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAccessRequestUpdate>()
                .HasOptional(r => r.ClarificationAssignedToTeam)
                .WithMany(r => r.ClarificationAssignedToTeamUpdates)
                .HasForeignKey(r => r.ClarificationAssignedToTeamId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAccessRequestUpdate>()
                .HasOptional(r => r.Team)
                .WithMany()
                .HasForeignKey(r => r.TeamId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAgreement>()
            .HasOptional(r => r.SignedByEmployer)
            .WithMany()
            .HasForeignKey(m => m.SignedByEmployerId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAgreement>()
                .HasRequired(r => r.RemoteAccessRequest)
                .WithMany(r => r.RemoteAgreements)
                .HasForeignKey(ar => ar.RemoteAccessRequestId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RemoteAccessRequiredSystem>()
                .HasRequired(r => r.RemoteAccessRequest)
                .WithMany(r => r.RemoteAccessRequiredSystems)
                .HasForeignKey(r => r.RemoteAccessRequestId)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<RemoteAccessRequiredSystem>()
                .HasRequired(r => r.AssetType)
                .WithMany(r => r.RemoteAccessRequiredSystems)
                .HasForeignKey(r => r.AssetTypeId)
                .WillCascadeOnDelete(false);

            // get all records where the asset is one parent linked to many children, this give all child of a parent
            modelBuilder.Entity<AssetLink>()
                 .HasRequired(al => al.ParentAsset)
                 .WithMany(a => a.ChildLinks)
                 .HasForeignKey(al => al.ParentAssetId)
                 .WillCascadeOnDelete(false);

            // get all records where one child belong to one or many parents, this give all parent of child
            modelBuilder.Entity<AssetLink>()
                .HasRequired(al => al.ChildAsset)
                .WithMany(a => a.ParentLinks)
                .HasForeignKey(al => al.ChildAssetId)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<AssetLinkLog>()
                .HasRequired(a => a.ParentAsset)
                .WithMany()
                .HasForeignKey(a => a.ParentAssetId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AssetLinkLog>()
                .HasRequired(a => a.ChildAsset)
                .WithMany()
                .HasForeignKey(a => a.ChildAssetId)
                .WillCascadeOnDelete(false);
        }
    }
}

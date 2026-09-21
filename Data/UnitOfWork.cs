using Domain;
using Domain.AR;
using Domain.CM;
using Domain.DM;
using Domain.DR;
using Domain.IM;
using Domain.RA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UnitOfWork : IDisposable
    {
        private readonly AppDBContext _context = new AppDBContext();

        #region Access Request

        #region AccessRequest
        private GenericRepository<AccessRequest> _accessRequestRepository;
        public GenericRepository<AccessRequest> AccessRequestRepository
        {
            get
            {
                if (this._accessRequestRepository == null)
                {
                    this._accessRequestRepository = new GenericRepository<AccessRequest>(_context);
                }
                return _accessRequestRepository;
            }
        }
        #endregion

        #region AccessRequestType
        private GenericRepository<AccessRequestType> _accessRequestTypeRepository;
        public GenericRepository<AccessRequestType> AccessRequestTypeRepository
        {
            get
            {
                if (this._accessRequestTypeRepository == null)
                {
                    this._accessRequestTypeRepository = new GenericRepository<AccessRequestType>(_context);
                }
                return _accessRequestTypeRepository;
            }
        }
        #endregion

        #region AccessRequestUpdate
        private GenericRepository<AccessRequestUpdate> _accessRequestUpdateRepository;
        public GenericRepository<AccessRequestUpdate> AccessRequestUpdateRepository
        {
            get
            {
                if (this._accessRequestUpdateRepository == null)
                {
                    this._accessRequestUpdateRepository = new GenericRepository<AccessRequestUpdate>(_context);
                }
                return _accessRequestUpdateRepository;
            }
        }
        #endregion

        #region AccessRequestLog
        private GenericRepository<AccessRequestLog> _accessRequestLogRepository;
        public GenericRepository<AccessRequestLog> AccessRequestLogRepository
        {
            get
            {
                if (this._accessRequestLogRepository == null)
                {
                    this._accessRequestLogRepository = new GenericRepository<AccessRequestLog>(_context);
                }
                return _accessRequestLogRepository;
            }
        }
        #endregion

        #region SystemAccess
        private GenericRepository<SystemAccess> _systemAccessRepository;
        public GenericRepository<SystemAccess> SystemAccessRepository
        {
            get
            {
                if (this._systemAccessRepository == null)
                {
                    this._systemAccessRepository = new GenericRepository<SystemAccess>(_context);
                }
                return _systemAccessRepository;
            }
        }
        #endregion

        #region SystemAccessItem
        private GenericRepository<SystemAccessItem> _systemAccessItemRepository;
        public GenericRepository<SystemAccessItem> SystemAccessItemRepository
        {
            get
            {
                if (this._systemAccessItemRepository == null)
                {
                    this._systemAccessItemRepository = new GenericRepository<SystemAccessItem>(_context);
                }
                return _systemAccessItemRepository;
            }
        }
        #endregion

        #region AccessPrivilegeCategory
        private GenericRepository<AccessPrivilegeCategory> _accessPrivilegeCategoryRepository;
        public GenericRepository<AccessPrivilegeCategory> AccessPrivilegeCategoryRepository
        {
            get
            {
                if (this._accessPrivilegeCategoryRepository == null)
                {
                    this._accessPrivilegeCategoryRepository = new GenericRepository<AccessPrivilegeCategory>(_context);
                }
                return _accessPrivilegeCategoryRepository;
            }
        }
        #endregion

        #region AccessApplication
        private GenericRepository<AccessApplication> _accessApplicationRepository;
        public GenericRepository<AccessApplication> AccessApplicationRepository
        {
            get
            {
                if (this._accessApplicationRepository == null)
                {
                    this._accessApplicationRepository = new GenericRepository<AccessApplication>(_context);
                }
                return _accessApplicationRepository;
            }
        }
        #endregion

        #region AccessLevel
        private GenericRepository<AccessLevel> _accessLevelRepository;
        public GenericRepository<AccessLevel> AccessLevelRepository
        {
            get
            {
                if (this._accessLevelRepository == null)
                {
                    this._accessLevelRepository = new GenericRepository<AccessLevel>(_context);
                }
                return _accessLevelRepository;
            }
        }
        #endregion

        #region UserPrivilegeAccess
        private GenericRepository<UserPrivilegeAccess> _userPrivilegeAccessRepository;
        public GenericRepository<UserPrivilegeAccess> UserPrivilegeAccessRepository
        {
            get
            {
                if (this._userPrivilegeAccessRepository == null)
                {
                    this._userPrivilegeAccessRepository = new GenericRepository<UserPrivilegeAccess>(_context);
                }
                return _userPrivilegeAccessRepository;
            }
        }
        #endregion

        #region UserPrivilegeAccessItem
        private GenericRepository<UserPrivilegeAccessItem> _userPrivilegeAccessItemRepository;
        public GenericRepository<UserPrivilegeAccessItem> UserPrivilegeAccessItemRepository
        {
            get
            {
                if (this._userPrivilegeAccessItemRepository == null)
                {
                    this._userPrivilegeAccessItemRepository = new GenericRepository<UserPrivilegeAccessItem>(_context);
                }
                return _userPrivilegeAccessItemRepository;
            }
        }
        #endregion

        #region PhysicalArea
        private GenericRepository<PhysicalArea> _physicalAreaRepository;
        public GenericRepository<PhysicalArea> PhysicalAreaRepository
        {
            get
            {
                if (this._physicalAreaRepository == null)
                {
                    this._physicalAreaRepository = new GenericRepository<PhysicalArea>(_context);
                }
                return _physicalAreaRepository;
            }
        }
        #endregion

        #region PhysicalAccess
        private GenericRepository<PhysicalAccess> _physicalAccessRepository;
        public GenericRepository<PhysicalAccess> PhysicalAccessRepository
        {
            get
            {
                if (this._physicalAccessRepository == null)
                {
                    this._physicalAccessRepository = new GenericRepository<PhysicalAccess>(_context);
                }
                return _physicalAccessRepository;
            }
        }
        #endregion

        #region PhysicalAccessItem
        private GenericRepository<PhysicalAccessItem> _physicalAccessItemRepository;
        public GenericRepository<PhysicalAccessItem> PhysicalAccessItemRepository
        {
            get
            {
                if (this._physicalAccessItemRepository == null)
                {
                    this._physicalAccessItemRepository = new GenericRepository<PhysicalAccessItem>(_context);
                }
                return _physicalAccessItemRepository;
            }
        }
        #endregion

        #region UserAccess
        private GenericRepository<UserAccess> _userAccessRepository;
        public GenericRepository<UserAccess> UserAccessRepository
        {
            get
            {
                if (this._userAccessRepository == null)
                {
                    this._userAccessRepository = new GenericRepository<UserAccess>(_context);
                }
                return _userAccessRepository;
            }
        }
        #endregion

        #region UserAccessItem
        private GenericRepository<UserAccessItem> _userAccessItemRepository;
        public GenericRepository<UserAccessItem> UserAccessItemRepository
        {
            get
            {
                if (this._userAccessItemRepository == null)
                {
                    this._userAccessItemRepository = new GenericRepository<UserAccessItem>(_context);
                }
                return _userAccessItemRepository;
            }
        }
        #endregion

        #region UserAccessItemType
        private GenericRepository<UserAccessItemType> _userAccessItemTypeRepository;
        public GenericRepository<UserAccessItemType> UserAccessItemTypeRepository
        {
            get
            {
                if (this._userAccessItemTypeRepository == null)
                {
                    this._userAccessItemTypeRepository = new GenericRepository<UserAccessItemType>(_context);
                }
                return _userAccessItemTypeRepository;
            }
        }
        #endregion

        #region DeviceAccess
        private GenericRepository<DeviceAccess> _deviceAccessRepository;
        public GenericRepository<DeviceAccess> DeviceAccessRepository
        {
            get
            {
                if (this._deviceAccessRepository == null)
                {
                    this._deviceAccessRepository = new GenericRepository<DeviceAccess>(_context);
                }
                return _deviceAccessRepository;
            }
        }
        #endregion

        #region DeviceAccessItem
        private GenericRepository<DeviceAccessItem> _deviceAccessItemRepository;
        public GenericRepository<DeviceAccessItem> DeviceAccessItemRepository
        {
            get
            {
                if (this._deviceAccessItemRepository == null)
                {
                    this._deviceAccessItemRepository = new GenericRepository<DeviceAccessItem>(_context);
                }
                return _deviceAccessItemRepository;
            }
        }
        #endregion

        #region DeviceAccessItemType
        private GenericRepository<DeviceAccessItemType> _deviceAccessItemTypeRepository;
        public GenericRepository<DeviceAccessItemType> DeviceAccessItemTypeRepository
        {
            get
            {
                if (this._deviceAccessItemTypeRepository == null)
                {
                    this._deviceAccessItemTypeRepository = new GenericRepository<DeviceAccessItemType>(_context);
                }
                return _deviceAccessItemTypeRepository;
            }
        }
        #endregion

        #region RemoteAccess
        private GenericRepository<RemoteAccess> _remoteAccessRepository;
        public GenericRepository<RemoteAccess> RemoteAccessRepository
        {
            get
            {
                if (this._remoteAccessRepository == null)
                {
                    this._remoteAccessRepository = new GenericRepository<RemoteAccess>(_context);
                }
                return _remoteAccessRepository;
            }
        }
        #endregion

        #region RemoteAccessItem
        private GenericRepository<RemoteAccessItem> _remoteAccessItemRepository;
        public GenericRepository<RemoteAccessItem> RemoteAccessItemRepository
        {
            get
            {
                if (this._remoteAccessItemRepository == null)
                {
                    this._remoteAccessItemRepository = new GenericRepository<RemoteAccessItem>(_context);
                }
                return _remoteAccessItemRepository;
            }
        }
        #endregion

        #endregion

        #region Change Request

        #region ChangeArea
        private GenericRepository<ChangeArea> _changeAreaRepository;
        public GenericRepository<ChangeArea> ChangeAreaRepository
        {
            get
            {
                if (this._changeAreaRepository == null)
                {
                    this._changeAreaRepository = new GenericRepository<ChangeArea>(_context);
                }
                return _changeAreaRepository;
            }
        }
        #endregion
        #region ChangeRequestCategory
        private GenericRepository<ChangeRequestCategory> _changeRequestCategoryRepository;
        public GenericRepository<ChangeRequestCategory> ChangeRequestCategoryRepository
        {
            get
            {
                if (this._changeRequestCategoryRepository == null)
                {
                    this._changeRequestCategoryRepository = new GenericRepository<ChangeRequestCategory>(_context);
                }
                return _changeRequestCategoryRepository;
            }
        }
        #endregion
        #region ChangeImplementData
        private GenericRepository<ChangeImplementData> _changeImplementDataRepository;
        public GenericRepository<ChangeImplementData> ChangeImplementDataRepository
        {
            get
            {
                if (this._changeImplementDataRepository == null)
                {
                    this._changeImplementDataRepository = new GenericRepository<ChangeImplementData>(_context);
                }
                return _changeImplementDataRepository;
            }
        }
        #endregion
        #region ChangeRequest
        private GenericRepository<ChangeRequest> _changeRequestRepository;
        public GenericRepository<ChangeRequest> ChangeRequestRepository
        {
            get
            {
                if (this._changeRequestRepository == null)
                {
                    this._changeRequestRepository = new GenericRepository<ChangeRequest>(_context);
                }
                return _changeRequestRepository;
            }
        }
        #endregion
        #region ChangeRequestDoc
        private GenericRepository<ChangeRequestDoc> _changeRequestDocRepository;
        public GenericRepository<ChangeRequestDoc> ChangeRequestDocRepository
        {
            get
            {
                if (this._changeRequestDocRepository == null)
                {
                    this._changeRequestDocRepository = new GenericRepository<ChangeRequestDoc>(_context);
                }
                return _changeRequestDocRepository;
            }
        }
        #endregion

        #region ChangeRequestLog
        private GenericRepository<ChangeRequestLog> _changeRequestLogRepository;
        public GenericRepository<ChangeRequestLog> ChangeRequestLogRepository
        {
            get
            {
                if (this._changeRequestLogRepository == null)
                {
                    this._changeRequestLogRepository = new GenericRepository<ChangeRequestLog>(_context);
                }
                return _changeRequestLogRepository;
            }
        }
        #endregion
        #region ChangeRequestTask
        private GenericRepository<ChangeRequestTask> _changeRequestTaskRepository;
        public GenericRepository<ChangeRequestTask> ChangeRequestTaskRepository
        {
            get
            {
                if (this._changeRequestTaskRepository == null)
                {
                    this._changeRequestTaskRepository = new GenericRepository<ChangeRequestTask>(_context);
                }
                return _changeRequestTaskRepository;
            }
        }
        #endregion
        #region ChangeRequestUpdate
        private GenericRepository<ChangeRequestUpdate> _changeRequestUpdateRepository;
        public GenericRepository<ChangeRequestUpdate> ChangeRequestUpdateRepository
        {
            get
            {
                if (this._changeRequestUpdateRepository == null)
                {
                    this._changeRequestUpdateRepository = new GenericRepository<ChangeRequestUpdate>(_context);
                }
                return _changeRequestUpdateRepository;
            }
        }
        #endregion
        #region ChangeType
        private GenericRepository<ChangeType> _changeTypeRepository;
        public GenericRepository<ChangeType> ChangeTypeRepository
        {
            get
            {
                if (this._changeTypeRepository == null)
                {
                    this._changeTypeRepository = new GenericRepository<ChangeType>(_context);
                }
                return _changeTypeRepository;
            }
        }
        #endregion

        #endregion

        #region Incident

        #region IncidentRequest
        private GenericRepository<IncidentRequest> _incidentRequestRepository;
        public GenericRepository<IncidentRequest> IncidentRequestRepository
        {
            get
            {
                if (this._incidentRequestRepository == null)
                {
                    this._incidentRequestRepository = new GenericRepository<IncidentRequest>(_context);
                }
                return _incidentRequestRepository;
            }
        }
        #endregion
        #region IncidentRequestAsset
        private GenericRepository<IncidentRequestAsset> _incidentRequestAssetRepository;
        public GenericRepository<IncidentRequestAsset> IncidentRequestAssetRepository
        {
            get
            {
                if (this._incidentRequestAssetRepository == null)
                {
                    this._incidentRequestAssetRepository = new GenericRepository<IncidentRequestAsset>(_context);
                }
                return _incidentRequestAssetRepository;
            }
        }
        #endregion
        #region IncidentRequestDoc
        private GenericRepository<IncidentRequestDoc> _incidentRequestDocRepository;
        public GenericRepository<IncidentRequestDoc> IncidentRequestDocRepository
        {
            get
            {
                if (this._incidentRequestDocRepository == null)
                {
                    this._incidentRequestDocRepository = new GenericRepository<IncidentRequestDoc>(_context);
                }
                return _incidentRequestDocRepository;
            }
        }
        #endregion

        #region IncidentRequestLog
        private GenericRepository<IncidentRequestLog> _incidentRequestLogRepository;
        public GenericRepository<IncidentRequestLog> IncidentRequestLogRepository
        {
            get
            {
                if (this._incidentRequestLogRepository == null)
                {
                    this._incidentRequestLogRepository = new GenericRepository<IncidentRequestLog>(_context);
                }
                return _incidentRequestLogRepository;
            }
        }
        #endregion
        #region IncidentRequestUpdate
        private GenericRepository<IncidentRequestUpdate> _incidentRequestUpdateRepository;
        public GenericRepository<IncidentRequestUpdate> IncidentRequestUpdateRepository
        {
            get
            {
                if (this._incidentRequestUpdateRepository == null)
                {
                    this._incidentRequestUpdateRepository = new GenericRepository<IncidentRequestUpdate>(_context);
                }
                return _incidentRequestUpdateRepository;
            }
        }
        #endregion
        #region IncidentRequestTicket
        private GenericRepository<IncidentRequestTicket> _incidentRequestTicketRepository;
        public GenericRepository<IncidentRequestTicket> IncidentRequestTicketRepository
        {
            get
            {
                if (this._incidentRequestTicketRepository == null)
                {
                    this._incidentRequestTicketRepository = new GenericRepository<IncidentRequestTicket>(_context);
                }
                return _incidentRequestTicketRepository;
            }
        }
        #endregion

        #endregion

        #region DeviceManagement

        #region DeviceManagementRequest
        private GenericRepository<DeviceManagementRequest> _deviceManagementRequestRepository;
        public GenericRepository<DeviceManagementRequest> DeviceManagementRequestRepository
        {
            get
            {
                if (this._deviceManagementRequestRepository == null)
                {
                    this._deviceManagementRequestRepository = new GenericRepository<DeviceManagementRequest>(_context);
                }
                return _deviceManagementRequestRepository;
            }
        }
        #endregion


        #region DeviceManagementRequestLog
        private GenericRepository<DeviceManagementRequestLog> _deviceManagementRequestLogRepository;
        public GenericRepository<DeviceManagementRequestLog> DeviceManagementRequestLogRepository
        {
            get
            {
                if (this._deviceManagementRequestLogRepository == null)
                {
                    this._deviceManagementRequestLogRepository = new GenericRepository<DeviceManagementRequestLog>(_context);
                }
                return _deviceManagementRequestLogRepository;
            }
        }
        #endregion
        #region DeviceManagementRequestUpdate
        private GenericRepository<DeviceManagementRequestUpdate> _deviceManagementRequestUpdateRepository;
        public GenericRepository<DeviceManagementRequestUpdate> DeviceManagementRequestUpdateRepository
        {
            get
            {
                if (this._deviceManagementRequestUpdateRepository == null)
                {
                    this._deviceManagementRequestUpdateRepository = new GenericRepository<DeviceManagementRequestUpdate>(_context);
                }
                return _deviceManagementRequestUpdateRepository;
            }
        }
        #endregion
   

        #endregion


        #region Asset


        #region AssetType
        private GenericRepository<AssetType> _assetTypeRepository;
        public GenericRepository<AssetType> AssetTypeRepository
        {
            get
            {
                if (this._assetTypeRepository == null)
                {
                    this._assetTypeRepository = new GenericRepository<AssetType>(_context);
                }
                return _assetTypeRepository;
            }
        }
        #endregion

        #region Asset
        private GenericRepository<Asset> _assetRepository;
        public GenericRepository<Asset> AssetRepository
        {
            get
            {
                if (this._assetRepository == null)
                {
                    this._assetRepository = new GenericRepository<Asset>(_context);
                }
                return _assetRepository;
            }
        }
        #endregion

        #region TempAsset
        private GenericRepository<TempAsset> _tempAssetRepository;
        public GenericRepository<TempAsset> TempAssetRepository
        {
            get
            {
                if (this._tempAssetRepository == null)
                {
                    this._tempAssetRepository = new GenericRepository<TempAsset>(_context);
                }
                return _tempAssetRepository;
            }
        }
        #endregion

        #region AssetMake
        private GenericRepository<AssetMake> _assetMakeRepository;
        public GenericRepository<AssetMake> AssetMakeRepository
        {
            get
            {
                if (this._assetMakeRepository == null)
                {
                    this._assetMakeRepository = new GenericRepository<AssetMake>(_context);
                }
                return _assetMakeRepository;
            }
        }
        #endregion

        #region AssetLog
        private GenericRepository<AssetLog> _assetLogRepository;
        public GenericRepository<AssetLog> AssetLogRepository
        {
            get
            {
                if (this._assetLogRepository == null)
                {
                    this._assetLogRepository = new GenericRepository<AssetLog>(_context);
                }
                return _assetLogRepository;
            }
        }
        #endregion

        #region AssetTransactionLog
        private GenericRepository<AssetTransactionLog> _assetTransactionLogRepository;
        public GenericRepository<AssetTransactionLog> AssetTransactionLogRepository
        {
            get
            {
                if (this._assetTransactionLogRepository == null)
                {
                    this._assetTransactionLogRepository = new GenericRepository<AssetTransactionLog>(_context);
                }
                return _assetTransactionLogRepository;
            }
        }
        #endregion

        #region GRNote
        private GenericRepository<GRNote> _gRNoteRepository;
        public GenericRepository<GRNote> GRNoteRepository
        {
            get
            {
                if (this._gRNoteRepository == null)
                {
                    this._gRNoteRepository = new GenericRepository<GRNote>(_context);
                }
                return _gRNoteRepository;
            }
        }
        #endregion

        #region AssetTransferRequest
        private GenericRepository<AssetTransferRequest> _assetTransferRequestRepository;
        public GenericRepository<AssetTransferRequest> AssetTransferRequestRepository
        {
            get
            {
                if (this._assetTransferRequestRepository == null)
                {
                    this._assetTransferRequestRepository = new GenericRepository<AssetTransferRequest>(_context);
                }
                return _assetTransferRequestRepository;
            }
        }
        #endregion
       
        #region TransferItem
        private GenericRepository<TransferItem> _transferItemRepository;
        public GenericRepository<TransferItem> TransferItemRepository
        {
            get
            {
                if (this._transferItemRepository == null)
                {
                    this._transferItemRepository = new GenericRepository<TransferItem>(_context);
                }
                return _transferItemRepository;
            }
        }
        #endregion

        #region AssetVerificationRequest
        private GenericRepository<AssetVerificationRequest> _assetVerificationRequestRepository;
        public GenericRepository<AssetVerificationRequest> AssetVerificationRequestRepository
        {
            get
            {
                if (this._assetVerificationRequestRepository == null)
                {
                    this._assetVerificationRequestRepository = new GenericRepository<AssetVerificationRequest>(_context);
                }
                return _assetVerificationRequestRepository;
            }
        }
        #endregion

        #region AssetVerificationItem
        private GenericRepository<AssetVerificationItem> _assetVerificationItemRepository;
        public GenericRepository<AssetVerificationItem> AssetVerificationItemRepository
        {
            get
            {
                if (this._assetVerificationItemRepository == null)
                {
                    this._assetVerificationItemRepository = new GenericRepository<AssetVerificationItem>(_context);
                }
                return _assetVerificationItemRepository;
            }
        }
        #endregion

        #region Dispose

        #region DisposalRequest
        private GenericRepository<DisposalRequest> _disposalRequestRepository;
        public GenericRepository<DisposalRequest> DisposalRequestRepository
        {
            get
            {
                if (this._disposalRequestRepository == null)
                {
                    this._disposalRequestRepository = new GenericRepository<DisposalRequest>(_context);
                }
                return _disposalRequestRepository;
            }
        }
        #endregion

        #region DisposalRequestLog
        private GenericRepository<DisposalRequestLog> _disposalRequestLogRepository;
        public GenericRepository<DisposalRequestLog> DisposalRequestLogRepository
        {
            get
            {
                if (this._disposalRequestLogRepository == null)
                {
                    this._disposalRequestLogRepository = new GenericRepository<DisposalRequestLog>(_context);
                }
                return _disposalRequestLogRepository;
            }
        }

        #endregion

        #region DisposalRequestUpdate

        private GenericRepository<DisposalRequestUpdate> _disposalRequestUpdateRepository;
        public GenericRepository<DisposalRequestUpdate> DisposalRequestUpdateRepository
        {
            get
            {
                if (this._disposalRequestUpdateRepository == null)
                {
                    this._disposalRequestUpdateRepository = new GenericRepository<DisposalRequestUpdate>(_context);
                }
                return _disposalRequestUpdateRepository;
            }
        }

        #endregion


        #region DisposalItem
        private GenericRepository<DisposalItem> _disposalItemRepository;
        public GenericRepository<DisposalItem> DisposalItemRepository
        {
            get
            {
                if (this._disposalItemRepository == null)
                {
                    this._disposalItemRepository = new GenericRepository<DisposalItem>(_context);
                }
                return _disposalItemRepository;
            }
        }


        #endregion
        
        #region DisposalRequestDoc
        private GenericRepository<DisposalRequestDoc> _disposalDocRepository;
        public GenericRepository<DisposalRequestDoc> DisposalDocRepository
        {
            get
            {
                if (this._disposalDocRepository == null)
                {
                    this._disposalDocRepository = new GenericRepository<DisposalRequestDoc>(_context);
                }
                return _disposalDocRepository;
            }
        }


        #endregion



        #endregion

        #region Asset Link
        private GenericRepository<AssetLink> _assetLinkRepository;

        public GenericRepository<AssetLink> AssetLinkRepository
        {
            get
            {
                if (this._assetLinkRepository == null)
                {
                    this._assetLinkRepository = new GenericRepository<AssetLink>(_context);
                }
                return _assetLinkRepository;
            }
        }

        private GenericRepository<AssetLinkLog> _assetLinkLogRepository;

        public GenericRepository<AssetLinkLog> AssetLinkLogRepository
        {
            get
            {
                if (this._assetLinkLogRepository == null)
                {
                    this._assetLinkLogRepository = new GenericRepository<AssetLinkLog>(_context);
                }
                return _assetLinkLogRepository;
            }
        }



        #endregion

        #endregion


        #region SystemEnvironment
        private GenericRepository<SystemEnvironment> _systemEnvironmentRepository;
        public GenericRepository<SystemEnvironment> SystemEnvironmentRepository
        {
            get
            {
                if (this._systemEnvironmentRepository == null)
                {
                    this._systemEnvironmentRepository = new GenericRepository<SystemEnvironment>(_context);
                }
                return _systemEnvironmentRepository;
            }
        }
        #endregion

        #region Workflow
        private GenericRepository<Workflow> _workflowRepository;
        public GenericRepository<Workflow> WorkflowRepository
        {
            get
            {
                if (this._workflowRepository == null)
                {
                    this._workflowRepository = new GenericRepository<Workflow>(_context);
                }
                return _workflowRepository;
            }
        }
        #endregion

        #region WorkflowLevel
        private GenericRepository<WorkflowLevel> _workflowLevelRepository;
        public GenericRepository<WorkflowLevel> WorkflowLevelRepository
        {
            get
            {
                if (this._workflowLevelRepository == null)
                {
                    this._workflowLevelRepository = new GenericRepository<WorkflowLevel>(_context);
                }
                return _workflowLevelRepository;
            }
        }
        #endregion

        #region AccessControl
        private GenericRepository<AccessControl> _accessControlRepository;
        public GenericRepository<AccessControl> AccessControlRepository
        {
            get
            {
                if (this._accessControlRepository == null)
                {
                    this._accessControlRepository = new GenericRepository<AccessControl>(_context);
                }
                return _accessControlRepository;
            }
        }
        #endregion


        #region Event
        private GenericRepository<Event> _eventRepository;
        public GenericRepository<Event> EventRepository
        {
            get
            {
                if (this._eventRepository == null)
                {
                    this._eventRepository = new GenericRepository<Event>(_context);
                }
                return _eventRepository;
            }
        }
        #endregion

        #region EventType
        private GenericRepository<EventType> _eventTypeRepository;
        public GenericRepository<EventType> EventTypeRepository
        {
            get
            {
                if (this._eventTypeRepository == null)
                {
                    this._eventTypeRepository = new GenericRepository<EventType>(_context);
                }
                return _eventTypeRepository;
            }
        }
        #endregion

        #region DeliverySLA
        private GenericRepository<DeliverySLA> _deliverySLARepository;
        public GenericRepository<DeliverySLA> DeliverySLARepository
        {
            get
            {
                if (this._deliverySLARepository == null)
                {
                    this._deliverySLARepository = new GenericRepository<DeliverySLA>(_context);
                }
                return _deliverySLARepository;
            }
        }
        #endregion

        #region ItemAsset
        private GenericRepository<ItemAsset> _itemAssetRepository;
        public GenericRepository<ItemAsset> ItemAssetRepository
        {
            get
            {
                if (this._itemAssetRepository == null)
                {
                    this._itemAssetRepository = new GenericRepository<ItemAsset>(_context);
                }
                return _itemAssetRepository;
            }
        }
        #endregion

        #region ItemDoc
        private GenericRepository<ItemDoc> _itemDocRepository;
        public GenericRepository<ItemDoc> ItemDocRepository
        {
            get
            {
                if (this._itemDocRepository == null)
                {
                    this._itemDocRepository = new GenericRepository<ItemDoc>(_context);
                }
                return _itemDocRepository;
            }
        }
        #endregion

        #region ItemLog
        private GenericRepository<ItemLog> _itemLogRepository;
        public GenericRepository<ItemLog> ItemLogRepository
        {
            get
            {
                if (this._itemLogRepository == null)
                {
                    this._itemLogRepository = new GenericRepository<ItemLog>(_context);
                }
                return _itemLogRepository;
            }
        }
        #endregion


        #region ChangeRequestType
        private GenericRepository<ChangeRequestType> _changeRequestTypeRepository;
        public GenericRepository<ChangeRequestType> ChangeRequestTypeRepository
        {
            get
            {
                if (this._changeRequestTypeRepository == null)
                {
                    this._changeRequestTypeRepository = new GenericRepository<ChangeRequestType>(_context);
                }
                return _changeRequestTypeRepository;
            }
        }
        #endregion


        #region KedbItem
        private GenericRepository<KedbItem> _kedbItemRepository;
        public GenericRepository<KedbItem> KedbItemRepository
        {
            get
            {
                if (this._kedbItemRepository == null)
                {
                    this._kedbItemRepository = new GenericRepository<KedbItem>(_context);
                }
                return _kedbItemRepository;
            }
        }
        #endregion

        #region Category
        private GenericRepository<Category> _categoryRepository;
        public GenericRepository<Category> CategoryRepository
        {
            get
            {
                if (this._categoryRepository == null)
                {
                    this._categoryRepository = new GenericRepository<Category>(_context);
                }
                return _categoryRepository;
            }
        }
        #endregion

        #region SubCategory
        private GenericRepository<SubCategory> _subCategoryRepository;
        public GenericRepository<SubCategory> SubCategoryRepository
        {
            get
            {
                if (this._subCategoryRepository == null)
                {
                    this._subCategoryRepository = new GenericRepository<SubCategory>(_context);
                }
                return _subCategoryRepository;
            }
        }
        #endregion

        #region Department
        private GenericRepository<Department> _departmentRepository;
        public GenericRepository<Department> DepartmentRepository
        {
            get
            {
                if (this._departmentRepository == null)
                {
                    this._departmentRepository = new GenericRepository<Department>(_context);
                }
                return _departmentRepository;
            }
        }
        #endregion

        #region RequestType
        private GenericRepository<RequestType> _requestTypeRepository;
        public GenericRepository<RequestType> RequestTypeRepository
        {
            get
            {
                if (this._requestTypeRepository == null)
                {
                    this._requestTypeRepository = new GenericRepository<RequestType>(_context);
                }
                return _requestTypeRepository;
            }
        }
        #endregion


        #region Team
        private GenericRepository<Team> _teamRepository;
        public GenericRepository<Team> TeamRepository
        {
            get
            {
                if (this._teamRepository == null)
                {
                    this._teamRepository = new GenericRepository<Team>(_context);
                }
                return _teamRepository;
            }
        }
        #endregion

        #region WorkTask
        private GenericRepository<WorkTask> _workTaskRepository;
        public GenericRepository<WorkTask> WorkTaskRepository
        {
            get
            {
                if (this._workTaskRepository == null)
                {
                    this._workTaskRepository = new GenericRepository<WorkTask>(_context);
                }
                return _workTaskRepository;
            }
        }
        #endregion

        #region TaskProcess
        private GenericRepository<TaskProcess> _taskProcessRepository;
        public GenericRepository<TaskProcess> TaskProcessRepository
        {
            get
            {
                if (this._taskProcessRepository == null)
                {
                    this._taskProcessRepository = new GenericRepository<TaskProcess>(_context);
                }
                return _taskProcessRepository;
            }
        }
        #endregion

        #region Ticket
        private GenericRepository<Ticket> _ticketRepository;
        public GenericRepository<Ticket> TicketRepository
        {
            get
            {
                if (this._ticketRepository == null)
                {
                    this._ticketRepository = new GenericRepository<Ticket>(_context);
                }
                return _ticketRepository;
            }
        }
        #endregion


        #region TicketUpdate
        private GenericRepository<TicketUpdate> _ticketUpdateRepository;
        public GenericRepository<TicketUpdate> TicketUpdateRepository
        {
            get
            {
                if (this._ticketUpdateRepository == null)
                {
                    this._ticketUpdateRepository = new GenericRepository<TicketUpdate>(_context);
                }
                return _ticketUpdateRepository;
            }
        }
        #endregion

        #region TicketPriority
        private GenericRepository<TicketPriority> _ticketPriorityRepository;
        public GenericRepository<TicketPriority> TicketPriorityRepository
        {
            get
            {
                if (this._ticketPriorityRepository == null)
                {
                    this._ticketPriorityRepository = new GenericRepository<TicketPriority>(_context);
                }
                return _ticketPriorityRepository;
            }
        }
        #endregion

        #region TicketStatus
        private GenericRepository<TicketStatus> _ticketStatusRepository;
        public GenericRepository<TicketStatus> TicketStatusRepository
        {
            get
            {
                if (this._ticketStatusRepository == null)
                {
                    this._ticketStatusRepository = new GenericRepository<TicketStatus>(_context);
                }
                return _ticketStatusRepository;
            }
        }
        #endregion

        #region TicketLog
        private GenericRepository<TicketLog> _ticketLogRepository;
        public GenericRepository<TicketLog> TicketLogRepository
        {
            get
            {
                if (this._ticketLogRepository == null)
                {
                    this._ticketLogRepository = new GenericRepository<TicketLog>(_context);
                }
                return _ticketLogRepository;
            }
        }
        #endregion

        #region TicketDoc
        private GenericRepository<TicketDoc> _ticketDocRepository;
        public GenericRepository<TicketDoc> TicketDocRepository
        {
            get
            {
                if (this._ticketDocRepository == null)
                {
                    this._ticketDocRepository = new GenericRepository<TicketDoc>(_context);
                }
                return _ticketDocRepository;
            }
        }
        #endregion


        #region TempDoc
        private GenericRepository<TempDoc> _tempDocRepository;
        public GenericRepository<TempDoc> TempDocRepository
        {
            get
            {
                if (this._tempDocRepository == null)
                {
                    this._tempDocRepository = new GenericRepository<TempDoc>(_context);
                }
                return _tempDocRepository;
            }
        }
        #endregion

        #region TicketRate
        private GenericRepository<TicketRate> _ticketRateRepository;
        public GenericRepository<TicketRate> TicketRateRepository
        {
            get
            {
                if (this._ticketRateRepository == null)
                {
                    this._ticketRateRepository = new GenericRepository<TicketRate>(_context);
                }
                return _ticketRateRepository;
            }
        }
        #endregion


        #region TicketTask
        private GenericRepository<TicketTask> _ticketTaskRepository;
        public GenericRepository<TicketTask> TicketTaskRepository
        {
            get
            {
                if (this._ticketTaskRepository == null)
                {
                    this._ticketTaskRepository = new GenericRepository<TicketTask>(_context);
                }
                return _ticketTaskRepository;
            }
        }
        #endregion


        #region Problem
        private GenericRepository<Problem> _problemRepository;
        public GenericRepository<Problem> ProblemRepository
        {
            get
            {
                if (this._problemRepository == null)
                {
                    this._problemRepository = new GenericRepository<Problem>(_context);
                }
                return _problemRepository;
            }
        }
        #endregion

        #region Policy
        private GenericRepository<Policy> _policyRepository;
        public GenericRepository<Policy> PolicyRepository
        {
            get
            {
                if (this._policyRepository == null)
                {
                    this._policyRepository = new GenericRepository<Policy>(_context);
                }
                return _policyRepository;
            }
        }
        #endregion

        #region ProblemTicket
        private GenericRepository<ProblemTicket> _problemTicketRepository;
        public GenericRepository<ProblemTicket> ProblemTicketRepository
        {
            get
            {
                if (this._problemTicketRepository == null)
                {
                    this._problemTicketRepository = new GenericRepository<ProblemTicket>(_context);
                }
                return _problemTicketRepository;
            }
        }
        #endregion

        #region Change
        private GenericRepository<Change> _changeRepository;
        public GenericRepository<Change> ChangeRepository
        {
            get
            {
                if (this._changeRepository == null)
                {
                    this._changeRepository = new GenericRepository<Change>(_context);
                }
                return _changeRepository;
            }
        }
        #endregion

        #region ChangeTicket
        private GenericRepository<ChangeTicket> _changeTicketRepository;
        public GenericRepository<ChangeTicket> ChangeTicketRepository
        {
            get
            {
                if (this._changeTicketRepository == null)
                {
                    this._changeTicketRepository = new GenericRepository<ChangeTicket>(_context);
                }
                return _changeTicketRepository;
            }
        }
        #endregion

        #region ChangeProblem
        private GenericRepository<ChangeProblem> _changeProblemRepository;
        public GenericRepository<ChangeProblem> ChangeProblemRepository
        {
            get
            {
                if (this._changeProblemRepository == null)
                {
                    this._changeProblemRepository = new GenericRepository<ChangeProblem>(_context);
                }
                return _changeProblemRepository;
            }
        }
        #endregion

        #region Release
        private GenericRepository<Release> _releaseRepository;
        public GenericRepository<Release> ReleaseRepository
        {
            get
            {
                if (this._releaseRepository == null)
                {
                    this._releaseRepository = new GenericRepository<Release>(_context);
                }
                return _releaseRepository;
            }
        }
        #endregion

        #region ReleaseChange
        private GenericRepository<ReleaseChange> _releaseChangeRepository;
        public GenericRepository<ReleaseChange> ReleaseChangeRepository
        {
            get
            {
                if (this._releaseChangeRepository == null)
                {
                    this._releaseChangeRepository = new GenericRepository<ReleaseChange>(_context);
                }
                return _releaseChangeRepository;
            }
        }
        #endregion

        #region UserTeam
        private GenericRepository<UserTeam> _userTeamRepository;
        public GenericRepository<UserTeam> UserTeamRepository
        {
            get
            {
                if (this._userTeamRepository == null)
                {
                    this._userTeamRepository = new GenericRepository<UserTeam>(_context);
                }
                return _userTeamRepository;
            }
        }
        #endregion

        #region UserBranch
        private GenericRepository<UserBranch> _userBranchRepository;
        public GenericRepository<UserBranch> UserBranchRepository
        {
            get
            {
                if (this._userBranchRepository == null)
                {
                    this._userBranchRepository = new GenericRepository<UserBranch>(_context);
                }
                return _userBranchRepository;
            }
        }

        #endregion

        #region UserDepartment
        private GenericRepository<UserDepartment> _userDepartmentRepository;
        public GenericRepository<UserDepartment> UserDepartmentRepository
        {
            get
            {
                if (this._userDepartmentRepository == null)
                {
                    this._userDepartmentRepository = new GenericRepository<UserDepartment>(_context);
                }
                return _userDepartmentRepository;
            }
        }

        #endregion


        #region Menu
        private GenericRepository<Menu> _menuRepository;
        public GenericRepository<Menu> MenuRepository
        {
            get
            {
                if (this._menuRepository == null)
                {
                    this._menuRepository = new GenericRepository<Menu>(_context);
                }
                return _menuRepository;
            }
        }
        #endregion

        #region MenuItem
        private GenericRepository<MenuItem> _menuItemRepository;
        public GenericRepository<MenuItem> MenuItemRepository
        {
            get
            {
                if (this._menuItemRepository == null)
                {
                    this._menuItemRepository = new GenericRepository<MenuItem>(_context);
                }
                return _menuItemRepository;
            }
        }
        #endregion

        #region MenuItemFunction
        private GenericRepository<MenuItemFunction> _menuItemFunctionRepository;
        public GenericRepository<MenuItemFunction> MenuItemFunctionRepository
        {
            get
            {
                if (this._menuItemFunctionRepository == null)
                {
                    this._menuItemFunctionRepository = new GenericRepository<MenuItemFunction>(_context);
                }
                return _menuItemFunctionRepository;
            }
        }
        #endregion

        #region User
        private GenericRepository<User> _userRepository;
        public GenericRepository<User> UserRepository
        {
            get
            {
                if (this._userRepository == null)
                {
                    this._userRepository = new GenericRepository<User>(_context);
                }
                return _userRepository;
            }
        }
        #endregion

        #region UserGroup
        private GenericRepository<UserGroup> _userGroupRepository;
        public GenericRepository<UserGroup> UserGroupRepository
        {
            get
            {
                if (this._userGroupRepository == null)
                {
                    this._userGroupRepository = new GenericRepository<UserGroup>(_context);
                }
                return _userGroupRepository;
            }
        }
        #endregion

        #region Group
        private GenericRepository<Group> _groupRepository;
        public GenericRepository<Group> GroupRepository
        {
            get
            {
                if (this._groupRepository == null)
                {
                    this._groupRepository = new GenericRepository<Group>(_context);
                }
                return _groupRepository;
            }
        }
        #endregion

        #region Branch
        private GenericRepository<Branch> _branchRepository;
        public GenericRepository<Branch> BranchRepository
        {
            get
            {
                if (this._branchRepository == null)
                {
                    this._branchRepository = new GenericRepository<Branch>(_context);
                }
                return _branchRepository;
            }
        }
        #endregion

        #region BranchDepartment
        private GenericRepository<BranchDepartment> _branchDepartmentRepository;
        public GenericRepository<BranchDepartment> BranchDepartmentRepository
        {
            get
            {
                if (this._branchDepartmentRepository == null)
                {
                    this._branchDepartmentRepository = new GenericRepository<BranchDepartment>(_context);
                }
                return _branchDepartmentRepository;
            }
        }
        #endregion


        #region Region
        private GenericRepository<Region> _regionRepository;
        public GenericRepository<Region> RegionRepository
        {
            get
            {
                if (this._regionRepository == null)
                {
                    this._regionRepository = new GenericRepository<Region>(_context);
                }
                return _regionRepository;
            }
        }
        #endregion


        #region NotificationLog
        private GenericRepository<NotificationLog> _notificationLogRepository;
        public GenericRepository<NotificationLog> NotificationLogRepository
        {
            get
            {
                if (this._notificationLogRepository == null)
                {
                    this._notificationLogRepository = new GenericRepository<NotificationLog>(_context);
                }
                return _notificationLogRepository;
            }
        }
        #endregion

        #region Serial
        private GenericRepository<Serial> _serialRepository;
        public GenericRepository<Serial> SerialRepository
        {
            get
            {
                if (this._serialRepository == null)
                {
                    this._serialRepository = new GenericRepository<Serial>(_context);
                }
                return _serialRepository;
            }
        }
        #endregion

        #region Item
        private GenericRepository<Item> _itemRepository;
        public GenericRepository<Item> ItemRepository
        {
            get
            {
                if (this._itemRepository == null)
                {
                    this._itemRepository = new GenericRepository<Item>(_context);
                }
                return _itemRepository;
            }
        }
        #endregion

        #region ScheduledTask
        private GenericRepository<ScheduledTask> _scheduledTaskRepository;
        public GenericRepository<ScheduledTask> ScheduledTaskRepository
        {
            get
            {
                if (this._scheduledTaskRepository == null)
                {
                    this._scheduledTaskRepository = new GenericRepository<ScheduledTask>(_context);
                }
                return _scheduledTaskRepository;
            }
        }
        #endregion

        #region TaskCategory
        private GenericRepository<TaskCategory> _taskCategoryRepository;
        public GenericRepository<TaskCategory> TaskCategoryRepository
        {
            get
            {
                if (this._taskCategoryRepository == null)
                {
                    this._taskCategoryRepository = new GenericRepository<TaskCategory>(_context);
                }
                return _taskCategoryRepository;
            }
        }
        #endregion

        #region TaskUpdate
        private GenericRepository<TaskUpdate> _taskUpdateRepository;
        public GenericRepository<TaskUpdate> TaskUpdateRepository
        {
            get
            {
                if (this._taskUpdateRepository == null)
                {
                    this._taskUpdateRepository = new GenericRepository<TaskUpdate>(_context);
                }
                return _taskUpdateRepository;
            }
        }
        #endregion

        #region Vendor
        private GenericRepository<Vendor> _vendorRepository;
        public GenericRepository<Vendor> VendorRepository
        {
            get
            {
                if (this._vendorRepository == null)
                {
                    this._vendorRepository = new GenericRepository<Vendor>(_context);
                }
                return _vendorRepository;
            }
        }
        #endregion

        #region VendorSLA
        private GenericRepository<VendorSLA> _vendorSLARepository;
        public GenericRepository<VendorSLA> VendorSLARepository
        {
            get
            {
                if (this._vendorSLARepository == null)
                {
                    this._vendorSLARepository = new GenericRepository<VendorSLA>(_context);
                }
                return _vendorSLARepository;
            }
        }
        #endregion

        #region ScheduledDate
        private GenericRepository<ScheduledDate> _scheduledDateRepository;
        public GenericRepository<ScheduledDate> ScheduledDateRepository
        {
            get
            {
                if (this._scheduledDateRepository == null)
                {
                    this._scheduledDateRepository = new GenericRepository<ScheduledDate>(_context);
                }
                return _scheduledDateRepository;
            }
        }
        #endregion

        #region Remote Access 

        #region RemoteAccessRequest
        private GenericRepository<RemoteAccessRequest> _remoteAccessRequestRepository;
        public GenericRepository<RemoteAccessRequest> RemoteAccessRequestRepository
        {
            get
            {
                if (this._remoteAccessRequestRepository == null)
                {
                    this._remoteAccessRequestRepository = new GenericRepository<RemoteAccessRequest>(_context);
                }
                return _remoteAccessRequestRepository;
            }
        }
        #endregion

        #region RemoteAccessRequestLog
        private GenericRepository<RemoteAccessRequestLog> _remoteAccessRequestLogRepository;
        public GenericRepository<RemoteAccessRequestLog> RemoteAccessRequestLogRepository
        {
            get
            {
                if (this._remoteAccessRequestLogRepository == null)
                {
                    this._remoteAccessRequestLogRepository = new GenericRepository<RemoteAccessRequestLog>(_context);
                }
                return _remoteAccessRequestLogRepository;
            }
        }

        #endregion

        #region RemoteAccessRequestUpdate

        private GenericRepository<RemoteAccessRequestUpdate> _remoteAccessRequestUpdateRepository;
        public GenericRepository<RemoteAccessRequestUpdate> RemoteAccessRequestUpdateRepository
        {
            get
            {
                if (this._remoteAccessRequestUpdateRepository == null)
                {
                    this._remoteAccessRequestUpdateRepository = new GenericRepository<RemoteAccessRequestUpdate>(_context);
                }
                return _remoteAccessRequestUpdateRepository;
            }
        }

        #endregion

     
        #region RemoteAgreement
        private GenericRepository<RemoteAgreement> _remoteAgreementRepository;
        public GenericRepository<RemoteAgreement> RemoteAgreementRepository
        {
            get
            {
                if (this._remoteAgreementRepository == null)
                {
                    this._remoteAgreementRepository = new GenericRepository<RemoteAgreement>(_context);
                }
                return _remoteAgreementRepository;
            }
        }


        #endregion

        #region RemoteAccessRequiredSystems
        private GenericRepository<RemoteAccessRequiredSystem> _remoteAccessRequiredSystems;
        public GenericRepository<RemoteAccessRequiredSystem> RemoteAccessRequiredSystems
        {
            get
            {
                if (this._remoteAccessRequiredSystems == null)
                {
                    this._remoteAccessRequiredSystems = new GenericRepository<RemoteAccessRequiredSystem>(_context);
                }
                return _remoteAccessRequiredSystems;
            }
        }


        #endregion

        #endregion

        public void Save()
        {
            _context.SaveChanges();
        }


        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}

using Data;
using Domain;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
    public class AccessRequestService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<AccessRequest> GetAll(string includeProperties)
        {
            return _contextUow.AccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public AccessRequest GetItem(long? id, string includeProperties)
        {
            return _contextUow.AccessRequestRepository.Get(e => e.AccessRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public AccessRequest GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.AccessRequestRepository.GetAsNoTracking(e => e.AccessRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public IEnumerable<AccessRequest> GetItemsApprovals(string userId, string includeProperties)
        {
            return _contextUow.AccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.ApprovalBy == userId
            , includeProperties: includeProperties);
        }

        public IEnumerable<AccessRequest> GetTikcets(long? teamId, long? accessRequestTypeId, long? branchId, long? departmentId
            , long? durationTypeId, long? accessTypeId
            , long? statusId, DateTime startDate, DateTime endDate, string requestId
             , string includeProperties)
        {
            long accessRequestId = 0;
            try
            {
                accessRequestId = Convert.ToInt64(requestId);
            }catch(Exception ex) { }

            return _contextUow.AccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.TeamId == teamId || teamId == 0)
            && (e.AccessRequestTypeId == accessRequestTypeId || accessRequestTypeId == 0)
            && (e.BranchId == branchId || branchId == 0)
            && (e.DepartmentId == departmentId || departmentId == 0)
            && (durationTypeId == 0 || (Int32)e.DurationType == durationTypeId)
            && (accessTypeId == 0 || (Int32)e.AccessType == accessTypeId)
            && (statusId == 0 || (Int32)e.Status == statusId)
            && (e.AccessRequestId == accessRequestId || accessRequestId == 0)

            && ((e.RequestedDate >= startDate && e.RequestedDate <= endDate) || accessRequestId >0)
            , includeProperties: includeProperties);
        }

        public IEnumerable<AccessRequest> GetItemsOngoing(string includeProperties)
        {
            return _contextUow.AccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.Status < AccessRequestStatusEnum.Completed)

            , includeProperties: includeProperties);
        }

        public IEnumerable<AccessRequest> GetItemsMy(string userId, string includeProperties)
        {
            return _contextUow.AccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.CreatedBy == userId
            , includeProperties: includeProperties);
        }

        public IEnumerable<AccessRequest> GetItemsWorkflow(string userId, WorkflowLevelTypeEnum type, string includeProperties)
        {
            return _contextUow.AccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == AccessRequestStatusEnum.Pending
            && e.WorkflowLevelType == type
            && e.Team.UserTeams.Any(u => u.EmpNo == userId)
            , includeProperties: includeProperties);
        }
        public bool Insert(AccessRequest item)
        {
            try
            {

                _contextUow.AccessRequestRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(AccessRequest item)
        {
            try
            {
                _contextUow.AccessRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(AccessRequest item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.AccessRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(AccessRequest item)
        {
            if (_contextUow.AccessRequestRepository.Get(e => e.AccessRequestId == item.AccessRequestId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }

        public bool InsertLog(AccessRequest request, string comment)
        {
            try
            {
                AccessRequestLog log = new AccessRequestLog();
                log.AccessRequestId = request.AccessRequestId;
                log.Comment = comment;
                log.Status = request.Status;
                log.UpdatedBy = request.UpdatedBy;
                log.UpdatedDate = request.UpdatedDate;

                _contextUow.AccessRequestLogRepository.Insert(log);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertUpdateStatus(AccessRequest item, string comment, long? teamId)
        {

            try
            {
                AccessRequestUpdate update = new AccessRequestUpdate();
                update.UpdatedBy = item.UpdatedBy;
                update.UpdatedDate = item.UpdatedDate;
                update.Status = item.Status;
                update.TeamId = teamId;
                update.Comment = comment;
                update.AccessRequestId = item.AccessRequestId;
                update.SpentTime = item.SpentTime;

                _contextUow.AccessRequestUpdateRepository.Insert(update);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void UpdateStatus(AccessRequest item)
        {
            AccessRequest request = GetItem(item.AccessRequestId, "");
            request.Status = item.Status;

            //request.SpentTime += item.SpentTime;
            //if (request.SpentTime > request.Resolve)
            //    request.IsTimeViolated = true;

            request.UpdatedBy = item.UpdatedBy;
            request.UpdatedDate = item.UpdatedDate;

            Update(item);
        }


        public static String GetUserTypeColor(AllocatedTypeEnum type)
        {
            // danger success warning info primary
            var color = "default";
            if (type == AllocatedTypeEnum.Internal)
                color = "info";
            else if (type == AllocatedTypeEnum.External)
                color = "warning";

            return color;
        }

        public static String GetStatusColor(AccessRequestStatusEnum status)
        {
            var color = "default";
            if (status == AccessRequestStatusEnum.Initiated)
                color = "info";
            else if (status == AccessRequestStatusEnum.Rejected || status == AccessRequestStatusEnum.Expired)
                color = "danger";
            else if (status == AccessRequestStatusEnum.Revoked)
                color = "warning";
            else if (status == AccessRequestStatusEnum.Pending || status == AccessRequestStatusEnum.ClarificationRequested || status == AccessRequestStatusEnum.ClarificationProvided || status == AccessRequestStatusEnum.RevokeConfirmationPending || status == AccessRequestStatusEnum.RevokeConfirmedContinue)
                color = "primary";
            else if (status == AccessRequestStatusEnum.Completed)
                color = "success";
            return color;


        }
    }
}

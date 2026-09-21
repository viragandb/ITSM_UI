using Data;
using Domain.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.CM
{
    public class ChangeRequestService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ChangeRequest> GetAll(string includeProperties)
        {
            return _contextUow.ChangeRequestRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public ChangeRequest GetItem(long? id, string includeProperties)
        {
            return _contextUow.ChangeRequestRepository.Get(e => e.ChangeRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public ChangeRequest GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.ChangeRequestRepository.GetAsNoTracking(e => e.ChangeRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public ChangeRequestUpdate GetLastItem(long? requestId, string includeProperties)
        {
            return _contextUow.ChangeRequestUpdateRepository.Get(e => e.ChangeRequestId == requestId, includeProperties: includeProperties).OrderByDescending(o => o.UpdatedDate).FirstOrDefault();
        }

        public IEnumerable<ChangeRequest> GetItemsApprovals(string userId, string includeProperties)
        {
            return _contextUow.ChangeRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.ApprovalBy == userId
            , includeProperties: includeProperties);
        }

        public IEnumerable<ChangeRequest> GetItemsPending(string userId, string includeProperties)
        {
            return _contextUow.ChangeRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == ChangeRequestStatusEnum.Pending
            && e.Team.UserTeams.Any(u => u.EmpNo == userId)

            , includeProperties: includeProperties);
        }
        public IEnumerable<ChangeRequest> GetItemsPendingApproval(string userId, string includeProperties)
        {
            return _contextUow.ChangeRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == ChangeRequestStatusEnum.PendingApproval
            && e.Team.UserTeams.Any(u => u.EmpNo == userId)

            , includeProperties: includeProperties);
        }

        public IEnumerable<ChangeRequest> GetItemsPendingApprovalAVP(string includeProperties)
        {
            return _contextUow.ChangeRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == ChangeRequestStatusEnum.PendingApprovalAVP

            , includeProperties: includeProperties);
        }
        public IEnumerable<ChangeRequest> GetItemsPendingApprovalVP(string includeProperties)
        {
            return _contextUow.ChangeRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == ChangeRequestStatusEnum.PendingApprovalVP

            , includeProperties: includeProperties);
        }

        public IEnumerable<ChangeRequest> GetItemsPendingApprovalCMC( string includeProperties)
        {
            return _contextUow.ChangeRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == ChangeRequestStatusEnum.PendingApprovalCMC
            , includeProperties: includeProperties);
        }
        public IEnumerable<ChangeRequest> GetItemsPendingImplement(string userId, string includeProperties)
        {
            return _contextUow.ChangeRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == ChangeRequestStatusEnum.Approved
            && e.ImplementedBy == userId
            //&& e.Team.UserTeams.Any(u => u.EmpNo == userId)

            , includeProperties: includeProperties);
        }

        public IEnumerable<ChangeRequest> GetItemsPendingReview(string userId, string includeProperties)
        {
            return _contextUow.ChangeRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == ChangeRequestStatusEnum.Completed
            && e.Team.UserTeams.Any(u => u.EmpNo == userId)

            , includeProperties: includeProperties);
        }
        public IEnumerable<ChangeRequest> GetItemsOngoing(string includeProperties)
        {
            return _contextUow.ChangeRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status < ChangeRequestStatusEnum.Closed
            , includeProperties: includeProperties);
        }
        public IEnumerable<ChangeRequest> GetItemsMy(string userId, string includeProperties)
        {
            return _contextUow.ChangeRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.RequestedBy == userId
            , includeProperties: includeProperties);
        }

        public IEnumerable<ChangeRequest> GetItems(long? teamId, long? requestCategoryId, long? durationTypeId, long? changeAreaId, long? changeCategoryId
            , long? statusId, DateTime startDate, DateTime endDate, string requestId
             , string includeProperties)
        {
            long ChangeRequestId = 0;
            try
            {
                ChangeRequestId = Convert.ToInt64(requestId);
            }
            catch (Exception ex) { }

            return _contextUow.ChangeRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.TeamId == teamId || teamId == 0)
            && (e.ChangeRequestCategoryId == requestCategoryId || requestCategoryId == 0)
            && (durationTypeId == 0 || (Int32)e.DurationType == durationTypeId)
            && (e.ChangeImplementData.ChangeAreaId == changeAreaId || changeAreaId == 0)
            && (changeCategoryId == 0 || (Int32)e.ChangeImplementData.ChangeCategory == changeCategoryId)
            && (statusId == 0 || (Int32)e.Status == statusId)
            && (e.ChangeRequestId == ChangeRequestId || ChangeRequestId == 0)

            && ((e.RequestedDate >= startDate && e.RequestedDate <= endDate) || ChangeRequestId > 0)
            , includeProperties: includeProperties);
        }


        public bool Insert(ChangeRequest item)
        {
            try
            {
                _contextUow.ChangeRequestRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ChangeRequest item)
        {
            try
            {
                if (item.SpentTime > item.Resolve)
                    item.IsTimeViolated = true;
                else
                    item.IsTimeViolated = false;
                _contextUow.ChangeRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ChangeRequest item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ChangeRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(ChangeRequest item)
        {
            if (_contextUow.ChangeRequestRepository.Get(e => e.ChangeRequestId == item.ChangeRequestId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }

        public bool InsertLog(ChangeRequest request, string comment)
        {
            try
            {
                ChangeRequestLog log = new ChangeRequestLog();
                log.ChangeRequestId = request.ChangeRequestId;
                log.Comment = comment;
                log.Status = request.Status;
                log.UpdatedBy = request.UpdatedBy;
                log.UpdatedDate = request.UpdatedDate;

                _contextUow.ChangeRequestLogRepository.Insert(log);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertUpdateStatus(ChangeRequest item,double statusTime, string comment)
        {

            try
            {
                ChangeRequestUpdate update = new ChangeRequestUpdate();
                update.UpdatedBy = item.UpdatedBy;
                update.UpdatedDate = item.UpdatedDate;
                update.Status = item.Status;
                update.Comment = comment;
                update.ChangeRequestId = item.ChangeRequestId;
                update.SpentTime = statusTime;

                _contextUow.ChangeRequestUpdateRepository.Insert(update);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void UpdateStatus(ChangeRequest item)
        {
            ChangeRequest request = GetItem(item.ChangeRequestId, "");
            request.Status = item.Status;

            request.SpentTime += item.SpentTime;
            if (request.SpentTime > request.Resolve)
                request.IsTimeViolated = true;

            request.UpdatedBy = item.UpdatedBy;
            request.UpdatedDate = item.UpdatedDate;

            Update(item);
        }

        public bool InsertDoc(ChangeRequestDoc item)
        {
            try
            {
                _contextUow.ChangeRequestDocRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static String GetStatusColor(ChangeRequestStatusEnum status)
        {
            var color = "info";
            if (status == ChangeRequestStatusEnum.Initiated)
                color = "info";
            else if (status == ChangeRequestStatusEnum.Pending)
                color = "primary";
            else if (status == ChangeRequestStatusEnum.PendingApproval || status == ChangeRequestStatusEnum.PendingApprovalAVP 
                || status == ChangeRequestStatusEnum.PendingApprovalVP || status == ChangeRequestStatusEnum.PendingApprovalCMC)
                color = "warning"; 
            else if (status == ChangeRequestStatusEnum.Rejected)
                color = "danger";
         
            else if (status == ChangeRequestStatusEnum.Completed)
                color = "success";
            return color;

        }

        public static String GetImpactColor(ChangeCategoryEnum status)
        {
            var color = "info";
            if (status == ChangeCategoryEnum.Minor)
                color = "info";
            else if (status == ChangeCategoryEnum.Significant)
                color = "primary";
            else if (status == ChangeCategoryEnum.Major )
                color = "warning";
            else if (status == ChangeCategoryEnum.Emergency)
                color = "danger";

            return color;
        }

    }
}

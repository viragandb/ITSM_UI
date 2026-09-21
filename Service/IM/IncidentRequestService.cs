using Data;
using Domain.IM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IM
{
    public class IncidentRequestService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<IncidentRequest> GetAll(string includeProperties)
        {
            return _contextUow.IncidentRequestRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IncidentRequest GetItem(long? id, string includeProperties)
        {
            return _contextUow.IncidentRequestRepository.Get(e => e.IncidentRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public IncidentRequest GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.IncidentRequestRepository.GetAsNoTracking(e => e.IncidentRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public IEnumerable<IncidentRequest> GetItemsPending(string userId, string includeProperties)
        {
            return _contextUow.IncidentRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.Status < IncidentStatusEnum.Completed)
            && e.PendingTeam.UserTeams.Any(u => u.EmpNo == userId)

            , includeProperties: includeProperties);
        }

        public IEnumerable<IncidentRequest> GetItemsReview(string userId, string includeProperties)
        {
            return _contextUow.IncidentRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.Status == IncidentStatusEnum.Completed)
            && e.PendingTeam.UserTeams.Any(u => u.EmpNo == userId)

            , includeProperties: includeProperties);
        }
        public IEnumerable<IncidentRequest> GetItemsClose(string userId, string includeProperties)
        {
            return _contextUow.IncidentRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.Status == IncidentStatusEnum.Reviewed)
            && e.PendingTeam.UserTeams.Any(u => u.EmpNo == userId)

            , includeProperties: includeProperties);
        }
        public IEnumerable<IncidentRequest> GetItemsOngoing(string includeProperties)
        {
            return _contextUow.IncidentRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.Status < IncidentStatusEnum.Completed)

            , includeProperties: includeProperties);
        }
        //public IEnumerable<IncidentRequest> GetItemsApprovals(string userId, string includeProperties)
        //{
        //    return _contextUow.IncidentRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
        //    && e.ApprovalBy == userId
        //    , includeProperties: includeProperties);
        //}


        //public IEnumerable<IncidentRequest> GetItemsPendingApproval(string userId, string includeProperties)
        //{
        //    return _contextUow.IncidentRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
        //    && e.Status == IncidentRequestStatusEnum.PendingApproval
        //    && e.Team.UserTeams.Any(u => u.EmpNo == userId)

        //    , includeProperties: includeProperties);
        //}



        public IEnumerable<IncidentRequest> GetItems(long? teamId, long? statusId, DateTime startDate, DateTime endDate, string includeProperties)
        {
            //long IncidentRequestId = 0;
            //try
            //{
            //    IncidentRequestId = Convert.ToInt64(requestId);
            //}
            //catch (Exception ex) { }

            return _contextUow.IncidentRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.Level01TeamId == teamId || teamId == 0)
            && (statusId == 0 || (Int32)e.Status == statusId)
            && ((e.OccurredDate >= startDate && e.OccurredDate <= endDate))
            , includeProperties: includeProperties);
        }


        public bool Insert(IncidentRequest item)
        {
            try
            {
                _contextUow.IncidentRequestRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(IncidentRequest item)
        {
            try
            {
                if (item.SpentTime > item.Resolve)
                    item.IsTimeViolated = true;
                else
                    item.IsTimeViolated = false;
                _contextUow.IncidentRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(IncidentRequest item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.IncidentRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(IncidentRequest item)
        {
            if (_contextUow.IncidentRequestRepository.Get(e => e.IncidentRequestId == item.IncidentRequestId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }

        public bool InsertLog(IncidentRequest request, string comment)
        {
            try
            {
                IncidentRequestLog log = new IncidentRequestLog();
                log.IncidentRequestId = request.IncidentRequestId;
                log.Comment = comment;
                log.Status = request.Status;
                log.UpdatedBy = request.UpdatedBy;
                log.UpdatedDate = request.UpdatedDate;

                _contextUow.IncidentRequestLogRepository.Insert(log);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertUpdateStatus(IncidentRequest item, double statusTime, string comment)
        {

            try
            {
                IncidentRequestUpdate update = new IncidentRequestUpdate();
                update.UpdatedBy = item.UpdatedBy;
                update.UpdatedDate = item.UpdatedDate;
                update.Status = item.Status;
                update.Comment = comment;
                update.IncidentRequestId = item.IncidentRequestId;
                update.SpentTime = statusTime;

                _contextUow.IncidentRequestUpdateRepository.Insert(update);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void UpdateStatus(IncidentRequest item)
        {
            IncidentRequest request = GetItem(item.IncidentRequestId, "");
            request.Status = item.Status;

            request.SpentTime += item.SpentTime;
            if (request.SpentTime > request.Resolve)
                request.IsTimeViolated = true;

            request.UpdatedBy = item.UpdatedBy;
            request.UpdatedDate = item.UpdatedDate;

            Update(item);
        }

        public bool InsertDoc(IncidentRequestDoc item)
        {
            try
            {
                _contextUow.IncidentRequestDocRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static String GetStatusColor(IncidentStatusEnum status)
        {
            var color = "info";
            if (status == IncidentStatusEnum.Pending)
                color = "info";
            else if (status == IncidentStatusEnum.InProgress)
                color = "primary";
            else if (status == IncidentStatusEnum.Vendor)
                color = "warning";
            else if (status == IncidentStatusEnum.Completed)
                color = "primary";
            else if (status == IncidentStatusEnum.Reviewed)
                color = "success";
            else if (status == IncidentStatusEnum.Closed)
                color = "success";
            else if (status == IncidentStatusEnum.Rejected)
                color = "danger";
            return color;


        }

        public static String GetImpactColor(IncidentImpactEnum status)
        {
            var color = "info";
            if (status == IncidentImpactEnum.VeryLow)
                color = "info";
            else if (status == IncidentImpactEnum.Low)
                color = "primary";
            else if (status == IncidentImpactEnum.Medium)
                color = "success";
            else if (status == IncidentImpactEnum.High)
                color = "warning";
            else if (status == IncidentImpactEnum.VeryHigh)
                color = "danger";
            return color;


        }

    }
}

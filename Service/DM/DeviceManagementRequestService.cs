using Data;
using Domain.DM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DM
{
    public class DeviceManagementRequestService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<DeviceManagementRequest> GetAll(string includeProperties)
        {
            return _contextUow.DeviceManagementRequestRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public DeviceManagementRequest GetItem(long? id, string includeProperties)
        {
            return _contextUow.DeviceManagementRequestRepository.Get(e => e.DeviceManagementRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public DeviceManagementRequest GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.DeviceManagementRequestRepository.GetAsNoTracking(e => e.DeviceManagementRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        //public IEnumerable<DeviceManagementRequest> GetItemsPending(string userId, string includeProperties)
        //{
        //    return _contextUow.DeviceManagementRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
        //    && (e.Status < IncidentStatusEnum.Completed)
        //    && e.PendingTeam.UserTeams.Any(u => u.EmpNo == userId)

        //    , includeProperties: includeProperties);
        //}


        public bool Insert(DeviceManagementRequest item)
        {
            try
            {
                _contextUow.DeviceManagementRequestRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(DeviceManagementRequest item)
        {
            try
            {
                if (item.SpentTime > item.Resolve)
                    item.IsTimeViolated = true;
                else
                    item.IsTimeViolated = false;
                _contextUow.DeviceManagementRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(DeviceManagementRequest item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.DeviceManagementRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(DeviceManagementRequest item)
        {
            if (_contextUow.DeviceManagementRequestRepository.Get(e => e.DeviceManagementRequestId == item.DeviceManagementRequestId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }

        public bool InsertLog(DeviceManagementRequest request, string comment)
        {
            try
            {
                DeviceManagementRequestLog log = new DeviceManagementRequestLog();
                log.DeviceManagementRequestId = request.DeviceManagementRequestId;
                log.Comment = comment;
                log.Status = request.Status;
                log.UpdatedBy = request.UpdatedBy;
                log.UpdatedDate = request.UpdatedDate;

                _contextUow.DeviceManagementRequestLogRepository.Insert(log);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertUpdateStatus(DeviceManagementRequest item, double statusTime, string comment)
        {

            try
            {
                DeviceManagementRequestUpdate update = new DeviceManagementRequestUpdate();
                update.UpdatedBy = item.UpdatedBy;
                update.UpdatedDate = item.UpdatedDate;
                update.Status = item.Status;
                update.Comment = comment;
                update.DeviceManagementRequestId = item.DeviceManagementRequestId;
                update.SpentTime = statusTime;

                _contextUow.DeviceManagementRequestUpdateRepository.Insert(update);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void UpdateStatus(DeviceManagementRequest item)
        {
            DeviceManagementRequest request = GetItem(item.DeviceManagementRequestId, "");
            request.Status = item.Status;

            request.SpentTime += item.SpentTime;
            if (request.SpentTime > request.Resolve)
                request.IsTimeViolated = true;

            request.UpdatedBy = item.UpdatedBy;
            request.UpdatedDate = item.UpdatedDate;

            Update(item);
        }


        public static String GetStatusColor(DeviceManagementStatusEnum status)
        {
            var color = "info";
            if (status == DeviceManagementStatusEnum.Pending)
                color = "info";
            else if (status == DeviceManagementStatusEnum.PendingApprovalIT)
                color = "primary";
            else if (status == DeviceManagementStatusEnum.PendingApprovalAVP)
                color = "primary";
            else if (status == DeviceManagementStatusEnum.PendingApprovalVP)
                color = "primary";
            else if (status == DeviceManagementStatusEnum.Approved)
                color = "success";
            else if (status == DeviceManagementStatusEnum.Completed)
                color = "warning";
            else if (status == DeviceManagementStatusEnum.Rejected)
                color = "danger";
            return color;


        }

     

    }
}

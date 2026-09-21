using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ChangeService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<Change> GetAll(string includeProperties)
        {
            return _contextUow.ChangeRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<Change> GetItems(long? catType, long? assetTypeId, long? changeRequestTypeId, long? statusId, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.ChangeRepository.GetAsNoTracking(e => e.IsDeleted == false
            && ((Int32)e.AssetType.AssetCategory == catType || catType == 0)
            && (e.AssetTypeId == assetTypeId || assetTypeId == 0)
            && (e.ChangeRequestTypeId == changeRequestTypeId || changeRequestTypeId == 0)
            && (statusId == 0 || (Int32)e.Status == statusId)
            && e.RequestedDate >= sDate && e.RequestedDate < eDate
            , includeProperties: includeProperties);
        }

        public IEnumerable<Change> GetItems(long? requestTypeId, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.ChangeRepository.GetAsNoTracking(e => e.IsDeleted == false
            // && (e.RequestTypeId == requestTypeId || requestTypeId == 0)
            && e.RequestedDate >= sDate && e.RequestedDate < eDate
            , includeProperties: includeProperties);
        }

        public IEnumerable<Change> GetItemsToAssign(long? requestTypeId, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.ChangeRepository.GetAsNoTracking(e => e.IsDeleted == false
           && (e.ChangeRequestTypeId == requestTypeId || requestTypeId == 0)
            && e.RequestedDate >= sDate && e.RequestedDate < eDate && e.Status == ChangeStatusEnum.Reviewed
            , includeProperties: includeProperties);
        }

        public IEnumerable<Change> GetItemsForAssign(string userId, string includeProperties)
        {
            return _contextUow.ChangeRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.UpdatedBy == userId && e.Status < ChangeStatusEnum.Reviewed, includeProperties: includeProperties);
        }
        public IEnumerable<Change> GetAllByUpdatedBy(string userId, string includeProperties)
        {
            return _contextUow.ChangeRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.UpdatedBy == userId, includeProperties: includeProperties);
        }

        public IEnumerable<Change> GetAllByStatus(ChangeStatusEnum status, string includeProperties)
        {
            return _contextUow.ChangeRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == status, includeProperties: includeProperties);
        }

        public double GetKPIChageTicketCount(DateTime sDate, DateTime eDate)
        {

            var tCount = 0;
            var q = (from t in _contextUow.ProblemTicketRepository.GetAsNoTracking(e => e.IsDeleted == false)
                     join p in _contextUow.ChangeProblemRepository.GetAsNoTracking(e => e.IsDeleted == false) on t.ProblemId equals p.ProblemId
                     join c in _contextUow.ChangeRepository.GetAsNoTracking(e => e.IsDeleted == false
                     && e.StartDate >= sDate && e.StartDate < eDate && e.ChangeRequestType.IsKPIApplicable == true
                     )
                     on p.ChangeId equals c.ChangeId
                     select t);
            tCount = q.Count();

            var q2 = (from t in _contextUow.ChangeTicketRepository.GetAsNoTracking(e => e.IsDeleted == false)
                      join c in _contextUow.ChangeRepository.GetAsNoTracking(e => e.IsDeleted == false
                      && e.StartDate >= sDate && e.StartDate < eDate && e.ChangeRequestType.IsKPIApplicable == true
                      )
                      on t.ChangeId equals c.ChangeId
                      select t);
            tCount += q2.Count();

            return tCount;


        }
        public IEnumerable<Change> GetChangesWithTickets(DateTime sDate, DateTime eDate, string includeProperties)
        {

            return _contextUow.ChangeRepository.GetAsNoTracking(e => e.IsDeleted == false
           && e.RequestedDate >= sDate && e.RequestedDate < eDate
            && e.ChangeRequestType.IsKPIApplicable == true
           && e.Problems.Any(p=> p.IsDeleted==false)

           , includeProperties: includeProperties);


        }
        public IEnumerable<Change> GetItemsKPI(DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.ChangeRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.RequestedDate >= sDate && e.RequestedDate < eDate
            && e.ChangeRequestType.IsKPIApplicable == true

            , includeProperties: includeProperties);
        }
        public Change GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.ChangeRepository.GetAsNoTracking(e => e.ChangeId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public Change GetItem(long? id, string includeProperties)
        {
            return _contextUow.ChangeRepository.Get(e => e.ChangeId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(Change item)
        {
            try
            {
                _contextUow.ChangeRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Change item)
        {
            try
            {
                _contextUow.ChangeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Change item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ChangeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Change item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.ChangeId == item.ChangeId && e.IsDeleted == false).Any())
            //    return true;

            return false;
        }

        public int GetChangeIdInfra()
        {

            return 1;
        }


        public static String GetStatusColor(ChangeStatusEnum status)
        {
            var color = "default";
            if (status == ChangeStatusEnum.Pending)
                color = "primary";
            else if (status == ChangeStatusEnum.Planned)
                color = "success";
            else if (status == ChangeStatusEnum.Reviewed)
                color = "warning";
            else if (status == ChangeStatusEnum.Approved)
                color = "info";
            return color;
        }

    }
}


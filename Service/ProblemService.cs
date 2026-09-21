using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ProblemService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<Problem> GetAll(string includeProperties)
        {
            return _contextUow.ProblemRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<Problem> GetItems(long? teamId, long? categoryId, long? statusId, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.ProblemRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.RequestType.TeamId == teamId || teamId == 0)
            && (e.RequestType.CategoryId == categoryId || categoryId == 0)
            && (statusId == 0 || (Int32)e.Status == statusId)
            && e.RequestedDate >= sDate && e.RequestedDate < eDate
            , includeProperties: includeProperties);
        }
        public IEnumerable<Problem> GetItems(long? requestTypeId, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.ProblemRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.RequestTypeId == requestTypeId || requestTypeId == 0)
            && e.RequestedDate >= sDate && e.RequestedDate < eDate
            , includeProperties: includeProperties);
        }


        public IEnumerable<Problem> GetItemsByDR(ProblemStatusEnum status, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.ProblemRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == status
            && e.RequestedDate >= sDate && e.RequestedDate < eDate
            , includeProperties: includeProperties);
        }

        public IEnumerable<Problem> GetAllByUpdatedBy(string userId,string includeProperties)
        {
            return _contextUow.ProblemRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.UpdatedBy ==userId, includeProperties: includeProperties);
        }

        public IEnumerable<Problem> GetAllByStatus(ProblemStatusEnum status, string includeProperties)
        {
            return _contextUow.ProblemRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == status, includeProperties: includeProperties);
        }
        public Problem GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.ProblemRepository.GetAsNoTracking(e => e.ProblemId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public Problem GetItem(long? id, string includeProperties)
        {
            return _contextUow.ProblemRepository.Get(e => e.ProblemId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(Problem item)
        {
            try
            {
                _contextUow.ProblemRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Problem item)
        {
            try
            {
                _contextUow.ProblemRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Problem item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ProblemRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Problem item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.ProblemId == item.ProblemId && e.IsDeleted == false).Any())
            //    return true;

            return false;
        }

        public int GetProblemIdInfra()
        {

            return 1;
        }


        public static String GetStatusColor(ProblemStatusEnum status)
        {
            var color = "default";
            if (status == ProblemStatusEnum.Pending)
                color = "primary";
            else if (status == ProblemStatusEnum.Analysed)
                color = "success";
            else if (status == ProblemStatusEnum.Closed)
                color = "warning";
            //else if (status == RequestStatusEnum.Analysed)
            //    color = "warning";
            return color;
        }

    }
}

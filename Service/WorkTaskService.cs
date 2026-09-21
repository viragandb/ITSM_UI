using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class WorkTaskService
    {

        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<WorkTask> GetAll()
        {
            return GetAll("");
        }
        public IEnumerable<WorkTask> GetAll(string includeProperties)
        {
            return _contextUow.WorkTaskRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<WorkTask> GetItems(long? statusId, long? userId,long? teamId, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.WorkTaskRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (statusId == 0 || (Int32)e.Status == statusId)
            && e.TaskDate >= sDate && e.TaskDate  <eDate
            && (e.TaskUserId == userId.ToString() || userId == 0)
            , includeProperties: includeProperties).Where(s => _contextUow.UserTeamRepository.Get(e => e.IsDeleted == false
             && (e.TeamId == teamId || userId == 0) && e.EmpNo == s.TaskUserId, includeProperties: "").Any());
        }
        public IEnumerable<WorkTask> GetItemsMy(long? statusId, string userId,DateTime sDate, DateTime eDate,string includeProperties)
        {
            return _contextUow.WorkTaskRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (statusId == 0 || (Int32)e.Status == statusId)
            && e.TaskUserId == userId
            && e.TaskDate >= sDate && e.TaskDate  <eDate
            , includeProperties: includeProperties);
        }

        public IEnumerable<WorkTask> Pending(long? teamId, string userId,DateTime date, string includeProperties)
        {
            return _contextUow.WorkTaskRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.TaskUserId == userId || userId == "")
            && e.Status == TaskStatusEnum.Pending
            && e.TaskDate >= date
            , includeProperties: includeProperties).Where(s => _contextUow.UserTeamRepository.Get(e => e.IsDeleted == false
             && e.TeamId == teamId && e.EmpNo == s.TaskUserId, includeProperties: "").Any());
        }
        public IEnumerable<WorkTask> Pending( string userId,DateTime date, string includeProperties)
        {
            return _contextUow.WorkTaskRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.TaskUserId == userId)
            && e.TaskDate >= date
            && e.Status == TaskStatusEnum.Pending
            , includeProperties: includeProperties);
        }


        #region KPI

        public double GetTaskSpentTime(string userId, DateTime date, string includeProperties)
        {
            return _contextUow.WorkTaskRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.TaskUserId == userId
            && e.TaskDate == date && e.Status == TaskStatusEnum.Approved
            , includeProperties: includeProperties).Sum(s=> s.SpentTime);
        }

        #endregion


        public WorkTask GetItem(long? id, string includeProperties)
        {
            return _contextUow.WorkTaskRepository.Get(e => e.WorkTaskId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(WorkTask item)
        {
            try
            {
                _contextUow.WorkTaskRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(WorkTask item)
        {
            try
            {
                _contextUow.WorkTaskRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(WorkTask item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.WorkTaskRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(WorkTask item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.WorkTaskId == item.WorkTaskId && e.IsDeleted == false).Any())
            //    return true;

            return false;
        }

        public static String GetStatusColor(TaskStatusEnum status)
        {
            var color = "default";
            if (status == TaskStatusEnum.Pending)
                color = "warning";
            else if (status == TaskStatusEnum.Approved)
                color = "success";
            else if (status == TaskStatusEnum.Rejected)
                color = "danger";
            //else if (status == RequestStatusEnum.Analysed)
            //    color = "warning";
            return color;
        }

    }
}

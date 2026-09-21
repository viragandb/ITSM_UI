using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TaskUpdateService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<TaskUpdate> GetAll()
        {
            return GetAll("");
        }
        public IEnumerable<TaskUpdate> GetAll(string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
        public IEnumerable<TaskUpdate> GetItems(string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<TaskUpdate> GetItemsPendingsByUser(string assignedTo,long categoryId, DateTime sDate, DateTime eDate,bool isChecklist, string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false
           && (e.ScheduledTask.AssignedTo == assignedTo || assignedTo == "")
           && (e.ScheduledTask.TaskCategoryId == categoryId || categoryId == 0)
           && e.Status == ScheduledTaskStatusEnum.Pending
           && e.ScheduledTask.IsChecklist== isChecklist
           && e.TaskDate >= sDate && e.TaskDate <= eDate
            , includeProperties: includeProperties);
        }

        public IEnumerable<TaskUpdate> GetItemsByUser(string assignedTo, long categoryId, DateTime sDate, DateTime eDate, bool isChecklist, string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false
           && (e.ScheduledTask.AssignedTo == assignedTo || assignedTo == "")
           && (e.ScheduledTask.TaskCategoryId == categoryId || categoryId == 0)
           && e.ScheduledTask.IsChecklist == isChecklist
           && e.TaskDate >= sDate && e.TaskDate <= eDate
            , includeProperties: includeProperties);
        }

        public IEnumerable<TaskUpdate> GetItemsTaskId(long? id, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false
           && e.ScheduledTaskId== id
           && e.TaskDate >= sDate && e.TaskDate <= eDate
            , includeProperties: includeProperties);
        }

        public IEnumerable<TaskUpdate> GetItemsPendingsEmail(string assignedTo,  DateTime date, string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false
           && (e.ScheduledTask.AssignedTo == assignedTo || assignedTo == "")
           && e.Status == ScheduledTaskStatusEnum.Pending
           && e.TaskDate <= date
            , includeProperties: includeProperties);
        }

        public IEnumerable<TaskUpdate> GetItemsDueSummaryEmail(string reviewBy, DateTime date, string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false
           && (e.ScheduledTask.ReviewBy == reviewBy || reviewBy == "")
           && ( (e.Status == ScheduledTaskStatusEnum.Pending && e.TaskDate < date) || e.TaskDate==date)
            , includeProperties: includeProperties);
        }

        public IEnumerable<TaskUpdate> GetItemsPendingsByReviewer(string reviewBy, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.ScheduledTask.ReviewBy == reviewBy || reviewBy == "")
            && e.Status == ScheduledTaskStatusEnum.Completed
            && e.TaskDate >= sDate && e.TaskDate <= eDate
            , includeProperties: includeProperties);
        }

        public IEnumerable<TaskUpdate> GetItemsPendingsReviewDayEnd(DateTime date, string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == ScheduledTaskStatusEnum.Completed
            && e.TaskDate < date
            , includeProperties: includeProperties);
        }


        public IEnumerable<TaskUpdate> GetItems(long? teamId, long? categoryId, long? assignedTo, long? reviewBy, long? statusId
            , bool isDateCheck, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false
           && (e.ScheduledTask.TaskCategory.TeamId == teamId)
           && (e.ScheduledTask.TaskCategoryId == categoryId || categoryId == 0)
           && (e.ScheduledTask.AssignedTo == assignedTo.ToString() || assignedTo == 0)
           && (e.ScheduledTask.ReviewBy == reviewBy.ToString() || reviewBy == 0)
           && (statusId == 0 || (Int32)e.Status == statusId)
           && (!isDateCheck || (e.TaskDate >= sDate && e.TaskDate <= eDate))
            , includeProperties: includeProperties);
        }

        public IEnumerable<TaskUpdate> GetItemsKPI(long? categoryId, string assignedTo, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false
           && e.ScheduledTask.TaskCategoryId == categoryId
           && e.ScheduledTask.AssignedTo == assignedTo
           && e.TaskDate >= sDate && e.TaskDate <= eDate
           && e.ScheduledTask.IsDeleted == false
           && e.ScheduledTask.IsKPI == true
            , includeProperties: includeProperties);
        }

        public IEnumerable<TaskUpdate> GetItemsPendingNotDelayed(DateTime date, string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false
           && e.TaskDate <= date
           && e.IsDelayed == false
           && e.Status == ScheduledTaskStatusEnum.Pending
            , includeProperties: includeProperties);
        }

        public TaskUpdate GetItem(long? id, string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.Get(e => e.TaskUpdateId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public TaskUpdate GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.TaskUpdateRepository.GetAsNoTracking(e => e.TaskUpdateId == id, includeProperties: includeProperties).FirstOrDefault();
        }

      

        public bool IsTaskCreated(long scheduledTaskId, DateTime date)
        {
            if (_contextUow.TaskUpdateRepository.Get(e => e.ScheduledTaskId == scheduledTaskId
            && e.IsDeleted == false && e.TaskDate == date).Any())
                return true;

            return false;
        }
        public bool Insert(TaskUpdate item)
        {
            try
            {
                _contextUow.TaskUpdateRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(TaskUpdate item)
        {
            try
            {
                _contextUow.TaskUpdateRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteByTaskId(long taskId, DateTime date)
        {
            try
            {
                var items = _contextUow.TaskUpdateRepository.Get(e => e.ScheduledTaskId == taskId
            && e.Status == ScheduledTaskStatusEnum.Pending
            && e.TaskDate > date
            , includeProperties: "");

                foreach (TaskUpdate i in items)
                {
                    _contextUow.TaskUpdateRepository.Delete(i);
                    _contextUow.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Delete(TaskUpdate item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.TaskUpdateRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(TaskUpdate item)
        {
            //if (_contextUow.UserTaskUpdateRepository.Get(e => e.TaskUpdateId == item.TaskUpdateId && e.IsDeleted == false).Any())
            return true;

            return false;
        }


    }

}

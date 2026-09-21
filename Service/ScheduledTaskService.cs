using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ScheduledTaskService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ScheduledTask> GetAll()
        {
            return GetAll("");
        }
        public IEnumerable<ScheduledTask> GetAll(string includeProperties)
        {
            return _contextUow.ScheduledTaskRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<ScheduledTask> GetAllAsNoTracking(string includeProperties)
        {
            return _contextUow.ScheduledTaskRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<ScheduledTask> GetItems(long? teamId, long? categoryId, long? assignedTo, string includeProperties)
        {
            return _contextUow.ScheduledTaskRepository.Get(e => e.IsDeleted == false
           && (e.TaskCategory.TeamId == teamId)
           && (e.TaskCategoryId == categoryId || categoryId == 0)
           && (e.AssignedTo == assignedTo.ToString() || assignedTo == 0)
            , includeProperties: includeProperties);
        }


        public IEnumerable<ScheduledTask> GetItemsKPI(long? categoryId, string assignedTo, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.ScheduledTaskRepository.Get(e => e.IsDeleted == false
           && (e.TaskCategoryId == categoryId || categoryId == 0)
           && e.AssignedTo == assignedTo
            , includeProperties: includeProperties);
        }

        public IEnumerable<ScheduledTask> GetItemsWeek(DateTime date, string includeProperties)
        {
            return _contextUow.ScheduledTaskRepository.Get(e => e.IsDeleted == false
            && e.RepeatType == RepeatTypeEnum.Week
            && e.StartDate <= date && e.EndDate >= date
           && e.ScheduledDates.Any(d => d.IsDeleted == false && d.TaskDayNo == (int)date.DayOfWeek)
            , includeProperties: includeProperties);
        }

        public IEnumerable<ScheduledTask> GetItemsMonth(DateTime date, string includeProperties)
        {
            return _contextUow.ScheduledTaskRepository.Get(e => e.IsDeleted == false
            && e.RepeatType == RepeatTypeEnum.Month
            && e.StartDate <= date && e.EndDate >= date
           && e.ScheduledDates.Any(d => d.IsDeleted == false && d.TaskDayNo == date.Day)
            , includeProperties: includeProperties);
        }

        public ScheduledTask GetItem(long? id, string includeProperties)
        {
            return _contextUow.ScheduledTaskRepository.Get(e => e.ScheduledTaskId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public ScheduledTask GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.ScheduledTaskRepository.GetAsNoTracking(e => e.ScheduledTaskId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(ScheduledTask item)
        {
            try
            {
                _contextUow.ScheduledTaskRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ScheduledTask item)
        {
            try
            {
                _contextUow.ScheduledTaskRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ScheduledTask item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ScheduledTaskRepository.Delete(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(ScheduledTask item)
        {
            if (_contextUow.TaskUpdateRepository.Get(e => e.ScheduledTaskId == item.ScheduledTaskId && e.IsDeleted == false && e.Status > ScheduledTaskStatusEnum.Pending).Any())
                return true;

            return false;
        }


        public static String GetTaskRepeatTypeColor(RepeatTypeEnum status)
        {
            var color = "default";
            if (status == RepeatTypeEnum.Never)
                color = "success";
            else if (status == RepeatTypeEnum.Week)
                color = "primary";
            else if (status == RepeatTypeEnum.Month)
                color = "info";
            else if (status == RepeatTypeEnum.Year)
                color = "warning";
            return color;
        }

        public static String GetTaskStatusColor(ScheduledTaskStatusEnum status)
        {
            var color = "default";
            if (status == ScheduledTaskStatusEnum.Pending)
                color = "warning";
            else if (status == ScheduledTaskStatusEnum.Completed)
                color = "primary";
            else if (status == ScheduledTaskStatusEnum.Approved)
                color = "success";
            else if (status == ScheduledTaskStatusEnum.Rejected)
                color = "danger";
            return color;
        }

    }


}

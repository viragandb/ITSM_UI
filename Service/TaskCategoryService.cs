using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TaskCategoryService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<TaskCategory> GetAll()
        {
            return GetAll("");
        }
        public IEnumerable<TaskCategory> GetAll(string includeProperties)
        {
            return _contextUow.TaskCategoryRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<TaskCategory> GetItemsByTeamId(long? teamId, string includeProperties)
        {
            return _contextUow.TaskCategoryRepository.Get(e => e.IsDeleted == false
            && e.TeamId == teamId
            , includeProperties: includeProperties);
        }

        public IEnumerable<TaskCategory> GetItemsKPI(long? teamId,string userEmpNo,string includeProperties)
        {
            return _contextUow.TaskCategoryRepository.Get(e => e.IsDeleted == false
           && e.TeamId == teamId
           && e.ScheduledTasks.Any(t=> t.IsDeleted==false && t.IsKPI==true
                                    && t.AssignedTo==userEmpNo
                                    )
           , includeProperties: includeProperties);
        }

        public IEnumerable<TaskCategory> GetItemsByCheckList(long? teamId,DateTime date, string includeProperties)
        {
            return _contextUow.TaskCategoryRepository.Get(e => e.IsDeleted == false
            && e.TeamId==teamId
           && e.ScheduledTasks.Any(t => t.IsDeleted == false && t.IsChecklist == true
                                    && t.TaskUpdates.Any(u => u.TaskDate == date)
                                    )
                                   
           , includeProperties: includeProperties);

          //  return _contextUow.TaskUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false
          //&& (e.ScheduledTask.AssignedTo == assignedTo || assignedTo == "")
          //&& e.Status == ScheduledTaskStatusEnum.Pending
          //&& e.ScheduledTask.IsChecklist == isChecklist
          //&& e.TaskDate >= sDate && e.TaskDate <= eDate

        }
        public TaskCategory GetItem(long? id, string includeProperties)
        {
            return _contextUow.TaskCategoryRepository.Get(e => e.TaskCategoryId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public TaskCategory GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.TaskCategoryRepository.GetAsNoTracking(e => e.TaskCategoryId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(TaskCategory item)
        {
            try
            {
                _contextUow.TaskCategoryRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(TaskCategory item)
        {
            try
            {
                _contextUow.TaskCategoryRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(TaskCategory item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.TaskCategoryRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(TaskCategory item)
        {
            if (_contextUow.ScheduledTaskRepository.Get(e => e.TaskCategoryId == item.TaskCategoryId && e.IsDeleted == false).Any())
                return true;

            return false;
        }


    }


}

using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TicketTaskService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<TicketTask> GetAll(string includeProperties)
        {
            return _contextUow.TicketTaskRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

      
        public TicketTask GetItem(long? id, string includeProperties)
        {
            return _contextUow.TicketTaskRepository.Get(e => e.TicketTaskId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public TicketTask GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.TicketTaskRepository.GetAsNoTracking(e => e.TicketTaskId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public IEnumerable<TicketTask> GetItemsPending(string userId, string includeProperties)
        {
            return _contextUow.TicketTaskRepository.GetAsNoTracking(e => e.IsDeleted == false 
            && e.AllocatedTeam.UserTeams.Any(u => u.EmpNo== userId)
            && e.Status== TaskStatusEnum.Pending
            , includeProperties: includeProperties);
        }

        public IEnumerable<TicketTask> GetItemsAssigned(string userId, string includeProperties)
        {
            return _contextUow.TicketTaskRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.AssignedTo == userId
            && e.Status == TaskStatusEnum.Assigned
            , includeProperties: includeProperties);
        }

        public bool Insert(TicketTask item)
        {
            try
            {
                _contextUow.TicketTaskRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(TicketTask item)
        {
            try
            {
                _contextUow.TicketTaskRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(TicketTask item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.TicketTaskRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Delete(long id)
        {
            try
            {
                _contextUow.TicketTaskRepository.Delete(id);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsTaskNotCompleted(long ticketid)
        {
            return _contextUow.TicketTaskRepository.GetAsNoTracking(e => e.TicketId == ticketid && e.Status < TaskStatusEnum.Completed).Any();
        }
        public bool HasRelationalData(TicketTask item)
        {
            //if (_contextUow.RequestTypeRepository.Get(e => e.TicketTaskId == item.TicketTaskId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

        public static String GetTicketStatusColor(TaskStatusEnum status)
        {
            var color = "default";
            if (status == TaskStatusEnum.Pending )
                color = "primary";
            else if (status == TaskStatusEnum.Completed)
                color = "success";
            else if (status == TaskStatusEnum.Assigned)
                color = "warning";
            else if (status == TaskStatusEnum.Approved)
                color = "info";
            else if (status == TaskStatusEnum.Rejected)
                color = "danger";
            return color;
        }
    }
}

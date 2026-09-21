using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ScheduledDateService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ScheduledDate> GetAll()
        {
            return GetAll("");
        }
        public IEnumerable<ScheduledDate> GetAll(string includeProperties)
        {
            return _contextUow.ScheduledDateRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
        public IEnumerable<ScheduledDate> GetItems(long scheduledTaskId, string includeProperties)
        {
            return _contextUow.ScheduledDateRepository.Get(e => e.IsDeleted == false && e.ScheduledTaskId == scheduledTaskId, includeProperties: includeProperties);
        }

        public ScheduledDate GetItem(long? id, string includeProperties)
        {
            return _contextUow.ScheduledDateRepository.Get(e => e.ScheduledDateId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public ScheduledDate GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.ScheduledDateRepository.GetAsNoTracking(e => e.ScheduledDateId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(ScheduledDate item)
        {
            try
            {
                _contextUow.ScheduledDateRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ScheduledDate item)
        {
            try
            {
                _contextUow.ScheduledDateRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteByTaskId(long taskId)
        {
            try
            {
                var dates = GetItems(taskId, "");
                foreach (ScheduledDate d in dates)
                {
                    _contextUow.ScheduledDateRepository.Delete(d);
                    _contextUow.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ScheduledDate item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ScheduledDateRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(ScheduledDate item)
        {
            //if (_contextUow.UserScheduledDateRepository.Get(e => e.ScheduledDateId == item.ScheduledDateId && e.IsDeleted == false).Any())
            return true;

            return false;
        }


    }


}

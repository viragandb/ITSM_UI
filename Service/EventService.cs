using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class EventService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<Event> GetAll()
        {
            return GetAll("");
        }
        public IEnumerable<Event> GetAll(string includeProperties)
        {
            return _contextUow.EventRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<Event> GetItems(long? eventTypeId, long? statusId, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.EventRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.EventTypeId == eventTypeId || eventTypeId == 0)
            && (statusId == 0 || (Int32)e.Status == statusId)
            && e.EventDate >= sDate && e.EventDate < eDate
            , includeProperties: includeProperties);
        }


        public IEnumerable<Event> GetByStatus(EventStatusEnum status, string includeProperties)
        {
            return _contextUow.EventRepository.Get(e => e.IsDeleted == false
            && e.Status == status
            , includeProperties: includeProperties);
        }


        public Event GetItem(long? id, string includeProperties)
        {
            return _contextUow.EventRepository.Get(e => e.EventId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public Event GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.EventRepository.GetAsNoTracking(e => e.EventId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public IEnumerable<EventType> GetEventTypes(string includeProperties)
        {
            return _contextUow.EventTypeRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<Event> GetAllByStatus(EventStatusEnum status, string includeProperties)
        {
            return _contextUow.EventRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == status, includeProperties: includeProperties);
        }


        public bool Insert(Event item)
        {
            try
            {
                _contextUow.EventRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Event item)
        {
            try
            {
                _contextUow.EventRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Event item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.EventRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Event item)
        {

            return false;
        }

        public static String GetStatusColor(EventStatusEnum status)
        {
            var color = "default";
            if (status == EventStatusEnum.Opened)
                color = "primary";
            else if (status == EventStatusEnum.Closed)
                color = "success";
           
            return color;
        }

       
    }
}

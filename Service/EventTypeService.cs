using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class EventTypeService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<EventType> GetAll(string includeProperties)
        {
            return _contextUow.EventTypeRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
             

        public EventType GetItem(long? id, string includeProperties)
        {
            return _contextUow.EventTypeRepository.Get(e => e.EventTypeId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(EventType item)
        {
            try
            {
                _contextUow.EventTypeRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(EventType item)
        {
            try
            {
                _contextUow.EventTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(EventType item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.EventTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(EventType item)
        {
            if (_contextUow.EventRepository.Get(e => e.EventTypeId == item.EventTypeId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }

    }
}

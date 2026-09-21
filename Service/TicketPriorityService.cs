using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TicketPriorityService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<TicketPriority> GetAll(string includeProperties)
        {
            return _contextUow.TicketPriorityRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<TicketPriority> GetAllByUugencynImpact(TicketUrgencyEnum urgency, TicketImpactEnum impact, string includeProperties)
        {
            var items =  _contextUow.TicketPriorityRepository.Get(e => e.IsDeleted == false && e.Urgency==urgency && e.Impact==impact
            , includeProperties: includeProperties);

            foreach(var item in items)
            {
                item.TicketPriorityName = item.Priority.EnumDisplayName();
            }
            return items;
        }

        public TicketPriority GetItem(long? id, string includeProperties)
        {
            var item = _contextUow.TicketPriorityRepository.Get(e => e.TicketPriorityId == id, includeProperties: includeProperties).FirstOrDefault();
            item.TicketPriorityName = item.Priority.EnumDisplayName();
            return item;
        }
        public TicketPriority GetItemAsNoTracking(long? id, string includeProperties)
        {
            var item =  _contextUow.TicketPriorityRepository.GetAsNoTracking(e => e.TicketPriorityId == id, includeProperties: includeProperties).FirstOrDefault();
            item.TicketPriorityName = item.Priority.EnumDisplayName();
            return item;
        }
        public bool Insert(TicketPriority item)
        {
            try
            {
                _contextUow.TicketPriorityRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(TicketPriority item)
        {
            try
            {
                _contextUow.TicketPriorityRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(TicketPriority item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.TicketPriorityRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(TicketPriority item)
        {
            if (_contextUow.TicketRepository.Get(e => e.TicketPriorityId == item.TicketPriorityId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }
    }
}

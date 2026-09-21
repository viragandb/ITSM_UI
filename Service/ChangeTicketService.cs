using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
     public class ChangeTicketService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ChangeTicket> GetAll(string includeProperties)
        {
            return _contextUow.ChangeTicketRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<ChangeTicket> GetChangeTicketByChangeId(long? id, string includeProperties)
        {
            return _contextUow.ChangeTicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.ChangeId == id, includeProperties: includeProperties);
        }
        public ChangeTicket GetItem(long? id, string includeProperties)
        {
            return _contextUow.ChangeTicketRepository.Get(e => e.ChangeTicketId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public IEnumerable<ChangeTicket> GetChangesWithTickets(DateTime sDate, DateTime eDate, string includeProperties)
        {

            return _contextUow.ChangeTicketRepository.GetAsNoTracking(e => e.IsDeleted == false
           && e.Change.RequestedDate >= sDate && e.Change.RequestedDate < eDate
            && e.Change.ChangeRequestType.IsKPIApplicable == true
           , includeProperties: includeProperties);


        }

        public ChangeTicket GetItem(long? ChangeId, long? ticketId, string includeProperties)
        {
            return _contextUow.ChangeTicketRepository.Get(e => e.ChangeId == ChangeId && e.TicketId == ticketId && e.IsDeleted == false, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(ChangeTicket item)
        {
            try
            {
                _contextUow.ChangeTicketRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ChangeTicket item)
        {
            try
            {
                _contextUow.ChangeTicketRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ChangeTicket item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ChangeTicketRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsAvailable(ChangeTicket item)
        {
            if (_contextUow.ChangeTicketRepository.Get(e => e.ChangeId == item.ChangeId && e.TicketId == item.TicketId && e.IsDeleted == false).Any())
                return true;

            return false;
        }

        public bool HasRelationalData(ChangeTicket item)
        {
            //if (_contextUow.ChangeTicketRepository.Get(e => e.ChangeTicketId == item.ChangeTicketId && e.IsDeleted == false).Any())
            //    return true;

            return false;
        }


    }
}
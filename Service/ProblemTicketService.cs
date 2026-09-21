using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ProblemTicketService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ProblemTicket> GetAll(string includeProperties)
        {
            return _contextUow.ProblemTicketRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<ProblemTicket> GetProblemTicketByProblemId(long? id, string includeProperties)
        {
            return _contextUow.ProblemTicketRepository.GetAsNoTracking(e => e.IsDeleted == false 
            && e.ProblemId==id , includeProperties: includeProperties);
        }
        public ProblemTicket GetItem(long? id, string includeProperties)
        {
            return _contextUow.ProblemTicketRepository.Get(e => e.ProblemTicketId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public ProblemTicket GetItem(long? problemId,long? ticketId, string includeProperties)
        {
            return _contextUow.ProblemTicketRepository.Get(e => e.ProblemId == problemId && e.TicketId==ticketId && e.IsDeleted ==false, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(ProblemTicket item)
        {
            try
            {
                _contextUow.ProblemTicketRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ProblemTicket item)
        {
            try
            {
                _contextUow.ProblemTicketRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ProblemTicket item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ProblemTicketRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsAvailable(ProblemTicket item)
        {
            if (_contextUow.ProblemTicketRepository.Get(e => e.ProblemId == item.ProblemId && e.TicketId == item.TicketId && e.IsDeleted == false).Any())
                return true;

            return false;
        }

        public bool HasRelationalData(ProblemTicket item)
        {
            //if (_contextUow.ProblemTicketRepository.Get(e => e.ProblemTicketId == item.ProblemTicketId && e.IsDeleted == false).Any())
            //    return true;

            return false;
        }

       
    }
}

using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TicketLogService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<TicketLog> GetAll(string includeProperties)
        {
            return _contextUow.TicketLogRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public bool Insert(TicketLog item)
        {
            try
            {
                _contextUow.TicketLogRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(TicketLog item)
        {
            try
            {
                _contextUow.TicketLogRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(TicketLog item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.TicketLogRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Ticket item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.TicketId == item.TicketId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

    }
}

using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TicketUpdateService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<TicketUpdate> GetAll(string includeProperties)
        {
            return _contextUow.TicketUpdateRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public TicketUpdate GetItem(long? id, string includeProperties)
        {
            return _contextUow.TicketUpdateRepository.Get(e => e.TicketUpdateId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public TicketUpdate GetLastItem(long? ticketId, string includeProperties)
        {
            return _contextUow.TicketUpdateRepository.Get(e => e.TicketId == ticketId, includeProperties: includeProperties).OrderByDescending(o => o.UpdatedDate).FirstOrDefault();
        }

        public bool Insert(TicketUpdate item)
        {
            try
            {
                _contextUow.TicketUpdateRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(TicketUpdate item)
        {
            try
            {
                _contextUow.TicketUpdateRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(TicketUpdate item)
        {
            try
            {
                var deleteItem = _contextUow.TicketUpdateRepository.GetById(item.TicketUpdateId);

                if (item != null)
                {
                    // item.IsDeleted = true;
                    _contextUow.TicketUpdateRepository.Delete(deleteItem);
                    _contextUow.Save();

                    return true;
                }
                else
                    return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(TicketUpdate item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.TicketUpdateId == item.TicketUpdateId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }



    }
}
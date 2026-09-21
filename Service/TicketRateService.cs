using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TicketRateService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<TicketRate> GetAll(string includeProperties)
        {
            return _contextUow.TicketRateRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public TicketRate GetItem(long? id, string includeProperties)
        {
            return _contextUow.TicketRateRepository.Get(e => e.TicketRateId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(TicketRate item)
        {
            try
            {
                _contextUow.TicketRateRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(TicketRate item)
        {
            try
            {
                _contextUow.TicketRateRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(TicketRate item)
        {
            try
            {
                var deleteItem = _contextUow.TicketRateRepository.GetById(item.TicketRateId);

                if (item != null)
                {
                    // item.IsDeleted = true;
                    _contextUow.TicketRateRepository.Delete(deleteItem);
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

        public bool HasRelationalData(TicketRate item)
        {
            //if (_contextUow.TicketRepository.Get(e => e.Rate. == item.TicketRateId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }
    }
}

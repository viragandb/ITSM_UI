using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PolicyService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<Policy> GetAll(string includeProperties)
        {
            return _contextUow.PolicyRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public Policy GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.PolicyRepository.GetAsNoTracking(e => e.PolicyId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public Policy GetItem(long? id, string includeProperties)
        {
            return _contextUow.PolicyRepository.Get(e => e.PolicyId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(Policy item)
        {
            try
            {
                _contextUow.PolicyRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Policy item)
        {
            try
            {
                _contextUow.PolicyRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Policy item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.PolicyRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Policy item)
        {
            //if (_contextUow.TicketRepository.Get(e => e.PolicyId == item.PolicyId && e.IsDeleted == false).Any())
            //    return true;

            return false;
        }




    }
}

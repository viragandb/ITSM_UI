using Data;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
    public class PhysicalAccessService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<PhysicalAccess> GetAll(string includeProperties)
        {
            return _contextUow.PhysicalAccessRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public PhysicalAccess GetItem(long? id, string includeProperties)
        {
            return _contextUow.PhysicalAccessRepository.Get(e => e.PhysicalAccessId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public PhysicalAccess GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.PhysicalAccessRepository.GetAsNoTracking(e => e.PhysicalAccessId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(PhysicalAccess item)
        {
            try
            {
                _contextUow.PhysicalAccessRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(PhysicalAccess item)
        {
            try
            {
                _contextUow.PhysicalAccessRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(PhysicalAccess item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.PhysicalAccessRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(PhysicalAccess item)
        {
            if (_contextUow.AccessRequestRepository.Get(e => e.PhysicalAccessId == item.PhysicalAccessId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }


      
    }
}

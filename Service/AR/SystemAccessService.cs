using Data;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
     public class SystemAccessService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<SystemAccess> GetAll(string includeProperties)
        {
            return _contextUow.SystemAccessRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public SystemAccess GetItem(long? id, string includeProperties)
        {
            return _contextUow.SystemAccessRepository.Get(e => e.SystemAccessId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public SystemAccess GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.SystemAccessRepository.GetAsNoTracking(e => e.SystemAccessId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(SystemAccess item)
        {
            try
            {
                _contextUow.SystemAccessRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(SystemAccess item)
        {
            try
            {
                _contextUow.SystemAccessRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(SystemAccess item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.SystemAccessRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(SystemAccess item)
        {
            if (_contextUow.AccessRequestRepository.Get(e => e.SystemAccessId == item.SystemAccessId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }


       
    }
}

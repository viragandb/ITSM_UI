using Data;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
     public class RemoteAccessService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<RemoteAccess> GetAll(string includeProperties)
        {
            return _contextUow.RemoteAccessRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public RemoteAccess GetItem(long? id, string includeProperties)
        {
            return _contextUow.RemoteAccessRepository.Get(e => e.RemoteAccessId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public RemoteAccess GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.RemoteAccessRepository.GetAsNoTracking(e => e.RemoteAccessId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(RemoteAccess item)
        {
            try
            {
                _contextUow.RemoteAccessRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(RemoteAccess item)
        {
            try
            {
                _contextUow.RemoteAccessRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(RemoteAccess item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.RemoteAccessRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(RemoteAccess item)
        {
            if (_contextUow.AccessRequestRepository.Get(e => e.RemoteAccessId == item.RemoteAccessId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }



    }
}

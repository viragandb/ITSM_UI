using Data;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
    public class DeviceAccessService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<DeviceAccess> GetAll(string includeProperties)
        {
            return _contextUow.DeviceAccessRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public DeviceAccess GetItem(long? id, string includeProperties)
        {
            return _contextUow.DeviceAccessRepository.Get(e => e.DeviceAccessId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public DeviceAccess GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.DeviceAccessRepository.GetAsNoTracking(e => e.DeviceAccessId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(DeviceAccess item)
        {
            try
            {
                _contextUow.DeviceAccessRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(DeviceAccess item)
        {
            try
            {
                _contextUow.DeviceAccessRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(DeviceAccess item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.DeviceAccessRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(DeviceAccess item)
        {
            if (_contextUow.AccessRequestRepository.Get(e => e.DeviceAccessId == item.DeviceAccessId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }



    }
}

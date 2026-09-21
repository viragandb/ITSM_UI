using Data;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
     public class UserPrivilegeAccessService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<UserPrivilegeAccess> GetAll(string includeProperties)
        {
            return _contextUow.UserPrivilegeAccessRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public UserPrivilegeAccess GetItem(long? id, string includeProperties)
        {
            return _contextUow.UserPrivilegeAccessRepository.Get(e => e.UserPrivilegeAccessId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public UserPrivilegeAccess GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.UserPrivilegeAccessRepository.GetAsNoTracking(e => e.UserPrivilegeAccessId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(UserPrivilegeAccess item)
        {
            try
            {
                _contextUow.UserPrivilegeAccessRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(UserPrivilegeAccess item)
        {
            try
            {
                _contextUow.UserPrivilegeAccessRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(UserPrivilegeAccess item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.UserPrivilegeAccessRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(UserPrivilegeAccess item)
        {
            if (_contextUow.AccessRequestRepository.Get(e => e.UserPrivilegeAccessId == item.UserPrivilegeAccessId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }


    }
}

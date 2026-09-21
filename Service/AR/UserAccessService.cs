using Data;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
   public class UserAccessService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<UserAccess> GetAll(string includeProperties)
        {
            return _contextUow.UserAccessRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public UserAccess GetItem(long? id, string includeProperties)
        {
            return _contextUow.UserAccessRepository.Get(e => e.UserAccessId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public UserAccess GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.UserAccessRepository.GetAsNoTracking(e => e.UserAccessId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(UserAccess item)
        {
            try
            {
                _contextUow.UserAccessRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(UserAccess item)
        {
            try
            {
                _contextUow.UserAccessRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(UserAccess item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.UserAccessRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(UserAccess item)
        {
            if (_contextUow.AccessRequestRepository.Get(e => e.UserAccessId == item.UserAccessId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }



    }
}

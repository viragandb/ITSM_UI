using Data;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
    public class AccessRequestTypeService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<AccessRequestType> GetAll(string includeProperties)
        {
            return _contextUow.AccessRequestTypeRepository.GetAsNoTracking(e => e.IsDeleted == false , includeProperties: includeProperties);
        }
        public IEnumerable<AccessRequestType> GetAllActive(string includeProperties)
        {
            return _contextUow.AccessRequestTypeRepository.GetAsNoTracking(e => e.IsDeleted == false && e.IsActive == true, includeProperties: includeProperties);
        }
        public IEnumerable<AccessRequestType> GetAllNoIT(string includeProperties)
        {
            return _contextUow.AccessRequestTypeRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.ITOnly== false && e.IsActive == true
            , includeProperties: includeProperties);
        }
        public AccessRequestType GetItem(long? id, string includeProperties)
        {
            return _contextUow.AccessRequestTypeRepository.Get(e => e.AccessRequestTypeId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public AccessRequestType GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.AccessRequestTypeRepository.GetAsNoTracking(e => e.AccessRequestTypeId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(AccessRequestType item)
        {
            try
            {
                item.Color = "info";
                item.Icon = "fa fa-key";
                _contextUow.AccessRequestTypeRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(AccessRequestType item)
        {
            try
            {
                _contextUow.AccessRequestTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(AccessRequestType item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.AccessRequestTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(AccessRequestType item)
        {
            if (_contextUow.AccessRequestRepository.Get(e => e.AccessRequestTypeId == item.AccessRequestTypeId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }
   
    
        //public long GetAccessRequestTypeIdSystemAccess()
        //{
        //    return 1;
        //}

        public static int GetSystemAccessId()
        {

            return 1;
        }
        public static int GetFirewallChangeId()
        {

            return 4;
        }

        public static int GetUserPrivilegeAccessId()
        {

            return 6;
        }

        public static int GetPhysicalAccessId()
        {

            return 7;
        }
        public static int GetUserAccessIdInternal()
        {

            return 8;
        }
        public static int GetUserAccessIdExternal()
        {

            return 13;
        }

        public static int GetDeviceAccessId()
        {

            return 12;
        }

        public static int GetRemoteAccessId()
        {

            return 10;
        }
    }
}

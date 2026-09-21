using Data;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
    public class DeviceAccessItemTypeService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<DeviceAccessItemType> GetAll(string includeProperties)
        {
            return _contextUow.DeviceAccessItemTypeRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public DeviceAccessItemType GetItem(long? id, string includeProperties)
        {
            return _contextUow.DeviceAccessItemTypeRepository.Get(e => e.DeviceAccessItemTypeId == id, includeProperties: includeProperties).FirstOrDefault();
        }


        public bool Insert(DeviceAccessItemType item)
        {
            try
            {
                _contextUow.DeviceAccessItemTypeRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(DeviceAccessItemType item)
        {
            try
            {
                _contextUow.DeviceAccessItemTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(DeviceAccessItemType item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.DeviceAccessItemTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(DeviceAccessItemType item)
        {
            if (_contextUow.DeviceAccessItemRepository.Get(e => e.DeviceAccessItemTypeId == item.DeviceAccessItemTypeId && e.IsDeleted == false).Any())
                return true;

            return false;
        }


    }
}

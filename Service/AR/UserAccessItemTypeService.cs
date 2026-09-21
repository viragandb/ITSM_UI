using Data;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
     public class UserAccessItemTypeService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<UserAccessItemType> GetAll(string includeProperties)
        {
            return _contextUow.UserAccessItemTypeRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public UserAccessItemType GetItem(long? id, string includeProperties)
        {
            return _contextUow.UserAccessItemTypeRepository.Get(e => e.UserAccessItemTypeId == id, includeProperties: includeProperties).FirstOrDefault();
        }


        public bool Insert(UserAccessItemType item)
        {
            try
            {
                _contextUow.UserAccessItemTypeRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(UserAccessItemType item)
        {
            try
            {
                _contextUow.UserAccessItemTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(UserAccessItemType item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.UserAccessItemTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(UserAccessItemType item)
        {
            if (_contextUow.UserAccessItemRepository.Get(e => e.UserAccessItemTypeId == item.UserAccessItemTypeId && e.IsDeleted == false).Any())
                return true;

            return false;
        }


    }
}

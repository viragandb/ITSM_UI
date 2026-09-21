using Data;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
    public class AccessApplicationService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<AccessApplication> GetAll(string includeProperties)
        {
            return _contextUow.AccessApplicationRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<AccessApplication> GetAllByPrivilegeCategoryId(long? privilegeCategoryId, string includeProperties)
        {
            return _contextUow.AccessApplicationRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.AccessPrivilegeCategoryId == privilegeCategoryId
            , includeProperties: includeProperties);
        }

        public AccessApplication GetItem(long? id, string includeProperties)
        {
            return _contextUow.AccessApplicationRepository.Get(e => e.AccessApplicationId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool ItemAvilable( long assetTypeId, string includeProperties)
        {
            return _contextUow.AccessApplicationRepository.Get(e => e.IsDeleted == false 
             && e.AssetTypeId == assetTypeId, includeProperties: includeProperties).Any();
        }
        public bool Insert(AccessApplication item)
        {
            try
            {
                _contextUow.AccessApplicationRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(AccessApplication item)
        {
            try
            {
                _contextUow.AccessApplicationRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(AccessApplication item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.AccessApplicationRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(AccessApplication item)
        {
            if (_contextUow.UserPrivilegeAccessItemRepository.Get(e => e.AccessApplicationId == item.AccessApplicationId && e.IsDeleted == false).Any())
                return true;

            return false;
        }


    }
}

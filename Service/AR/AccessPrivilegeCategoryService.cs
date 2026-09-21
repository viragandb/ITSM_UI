using Data;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
    public class AccessPrivilegeCategoryService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<AccessPrivilegeCategory> GetAll(string includeProperties)
        {
            return _contextUow.AccessPrivilegeCategoryRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public AccessPrivilegeCategory GetItem(long? id, string includeProperties)
        {
            return _contextUow.AccessPrivilegeCategoryRepository.Get(e => e.AccessPrivilegeCategoryId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(AccessPrivilegeCategory item)
        {
            try
            {
                _contextUow.AccessPrivilegeCategoryRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(AccessPrivilegeCategory item)
        {
            try
            {
                _contextUow.AccessPrivilegeCategoryRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(AccessPrivilegeCategory item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.AccessPrivilegeCategoryRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(AccessPrivilegeCategory item)
        {
            if (_contextUow.UserPrivilegeAccessItemRepository.Get(e => e.AccessPrivilegeCategoryId == item.AccessPrivilegeCategoryId && e.IsDeleted == false).Any())
                return true;

            return false;
        }


    }
}

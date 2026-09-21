using Data;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
    public class AccessLevelService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<AccessLevel> GetAll(string includeProperties)
        {
            return _contextUow.AccessLevelRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public AccessLevel GetItem(long? id, string includeProperties)
        {
            return _contextUow.AccessLevelRepository.Get(e => e.AccessLevelId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(AccessLevel item)
        {
            try
            {
                _contextUow.AccessLevelRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(AccessLevel item)
        {
            try
            {
                _contextUow.AccessLevelRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(AccessLevel item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.AccessLevelRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(AccessLevel item)
        {
            if (_contextUow.UserPrivilegeAccessItemRepository.Get(e => e.AccessLevelId == item.AccessLevelId && e.IsDeleted == false).Any())
                return true;

            return false;
        }


    }
}

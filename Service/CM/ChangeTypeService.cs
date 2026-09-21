using Data;
using Domain.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.CM
{
    public class ChangeTypeService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ChangeType> GetAll(string includeProperties)
        {
            return _contextUow.ChangeTypeRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
        public IEnumerable<ChangeType> GetAllByArea(long? changeAreaId, string includeProperties)
        {
            return _contextUow.ChangeTypeRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.ChangeAreaId == changeAreaId
            , includeProperties: includeProperties);
        }

        public ChangeType GetItem(long? id, string includeProperties)
        {
            return _contextUow.ChangeTypeRepository.Get(e => e.ChangeTypeId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public ChangeType GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.ChangeTypeRepository.GetAsNoTracking(e => e.ChangeTypeId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(ChangeType item)
        {
            try
            {
                _contextUow.ChangeTypeRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ChangeType item)
        {
            try
            {
                _contextUow.ChangeTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ChangeType item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ChangeTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(ChangeType item)
        {
            if (_contextUow.ChangeRequestTaskRepository.Get(e => e.ChangeTypeId == item.ChangeTypeId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }



    }
}

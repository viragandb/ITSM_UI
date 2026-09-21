using Data;
using Domain.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.CM
{
     public class ChangeAreaService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ChangeArea> GetAll(string includeProperties)
        {
            return _contextUow.ChangeAreaRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
        public IEnumerable<ChangeArea> GetAllByRequestCategory(long? changeRequestCategoryId, string includeProperties)
        {
            return _contextUow.ChangeAreaRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.ChangeRequestCategoryId == changeRequestCategoryId
            , includeProperties: includeProperties);
        }
        public ChangeArea GetItem(long? id, string includeProperties)
        {
            return _contextUow.ChangeAreaRepository.Get(e => e.ChangeAreaId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public ChangeArea GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.ChangeAreaRepository.GetAsNoTracking(e => e.ChangeAreaId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(ChangeArea item)
        {
            try
            {
                item.Color = "info";
                item.Icon = "fa fa-cogs";
                _contextUow.ChangeAreaRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ChangeArea item)
        {
            try
            {
                _contextUow.ChangeAreaRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ChangeArea item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ChangeAreaRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(ChangeArea item)
        {
            if (_contextUow.ChangeImplementDataRepository.Get(e => e.ChangeAreaId == item.ChangeAreaId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }



    }
}

using Data;
using Domain.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.CM
{
    public class ChangeImplementDataService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ChangeImplementData> GetAll(string includeProperties)
        {
            return _contextUow.ChangeImplementDataRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
        public IEnumerable<ChangeImplementData> GetAllByArea(long? changeAreaId, string includeProperties)
        {
            return _contextUow.ChangeImplementDataRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.ChangeAreaId == changeAreaId
            , includeProperties: includeProperties);
        }

        public ChangeImplementData GetItem(long? id, string includeProperties)
        {
            return _contextUow.ChangeImplementDataRepository.Get(e => e.ChangeImplementDataId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public ChangeImplementData GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.ChangeImplementDataRepository.GetAsNoTracking(e => e.ChangeImplementDataId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(ChangeImplementData item)
        {
            try
            {
                _contextUow.ChangeImplementDataRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ChangeImplementData item)
        {
            try
            {
                _contextUow.ChangeImplementDataRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ChangeImplementData item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ChangeImplementDataRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(ChangeImplementData item)
        {
            if (_contextUow.ChangeRequestTaskRepository.Get(e => e.ChangeImplementDataId == item.ChangeImplementDataId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }



    }
}

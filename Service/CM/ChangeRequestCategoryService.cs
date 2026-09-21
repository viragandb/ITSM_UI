using Data;
using Domain.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.CM
{
    public class ChangeRequestCategoryService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ChangeRequestCategory> GetAll(string includeProperties)
        {
            return _contextUow.ChangeRequestCategoryRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
       
        public ChangeRequestCategory GetItem(long? id, string includeProperties)
        {
            return _contextUow.ChangeRequestCategoryRepository.Get(e => e.ChangeRequestCategoryId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public ChangeRequestCategory GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.ChangeRequestCategoryRepository.GetAsNoTracking(e => e.ChangeRequestCategoryId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(ChangeRequestCategory item)
        {
            try
            {
                item.Color = "info";
                item.Icon = "fa fa-cogs";
                _contextUow.ChangeRequestCategoryRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ChangeRequestCategory item)
        {
            try
            {
                _contextUow.ChangeRequestCategoryRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ChangeRequestCategory item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ChangeRequestCategoryRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(ChangeRequestCategory item)
        {
            if (_contextUow.ChangeRequestRepository.Get(e => e.ChangeRequestCategoryId == item.ChangeRequestCategoryId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }



    }
}

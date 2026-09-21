using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SubCategoryService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<SubCategory> GetAll(string includeProperties)
        {
            return _contextUow.SubCategoryRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<SubCategory> GetAllByTicketType(TicketTypeEnum ticketType, long? categoryId, long? assetTypeId, string includeProperties)
        {
            return _contextUow.SubCategoryRepository.GetAsNoTracking(e => e.IsDeleted == false
             && e.RequestTypes.Any(r => r.TicketType == ticketType && r.CategoryId == categoryId
             && r.AssetTypeId == assetTypeId && r.IsDeleted == false)
            , includeProperties: includeProperties);
        }

        public SubCategory GetItem(long? id, string includeProperties)
        {
            return _contextUow.SubCategoryRepository.Get(e => e.SubCategoryId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public SubCategory GetItemByName(string name, string includeProperties)
        {
            return _contextUow.SubCategoryRepository.GetAsNoTracking(e => e.SubCategoryName.Contains(name), includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(SubCategory item)
        {
            try
            {
                _contextUow.SubCategoryRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(SubCategory item)
        {
            try
            {
                _contextUow.SubCategoryRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(SubCategory item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.SubCategoryRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsUpdated(string name)
        {
            if (_contextUow.SubCategoryRepository.Get(e => e.SubCategoryName == name && e.IsDeleted == false).Any())
                return true;

            return false;
        }

        public bool HasRelationalData(SubCategory item)
        {
            if (_contextUow.RequestTypeRepository.Get(e => e.SubCategoryId == item.SubCategoryId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }

    }
}
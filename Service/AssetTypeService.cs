using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AssetTypeService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<AssetType> GetAll(string includeProperties)
        {
            return _contextUow.AssetTypeRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
        public IEnumerable<AssetType> GetAll(long? assetCatId,string includeProperties)
        {
            return _contextUow.AssetTypeRepository.Get(e => e.IsDeleted == false 
           && (assetCatId == 0 || (Int32)e.AssetCategory == assetCatId), includeProperties: includeProperties);
        }
        public IEnumerable<AssetType> GetAllByTicketType(TicketTypeEnum ticketType,long? categoryId, string includeProperties)
        {
            return _contextUow.AssetTypeRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.RequestTypes.Any(r=> r.TicketType==ticketType && r.CategoryId==categoryId && r.IsDeleted==false)
            , includeProperties: includeProperties);
            
        }

        public IEnumerable<AssetType> GetAllByAssetCategory(AssetCategoryEnum assetCategory, string includeProperties)
        {
            return _contextUow.AssetTypeRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.AssetCategory == assetCategory
            , includeProperties: includeProperties);

        }

        public AssetType GetItem(long? id, string includeProperties)
        {
            return _contextUow.AssetTypeRepository.Get(e => e.AssetTypeId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public AssetType GetItemByName(string name, string includeProperties)
        {
            return _contextUow.AssetTypeRepository.GetAsNoTracking(e => e.AssetTypeName.Contains(name), includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(AssetType item)
        {
            try
            {
                _contextUow.AssetTypeRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(AssetType item)
        {
            try
            {
                _contextUow.AssetTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(AssetType item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.AssetTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(AssetType item)
        {
            if (_contextUow.RequestTypeRepository.Get(e => e.AssetTypeId == item.AssetTypeId && e.IsDeleted == false).Any())
                return true;
            if (_contextUow.AssetRepository.Get(e => e.AssetTypeId == item.AssetTypeId && e.IsDeleted == false).Any())
                return true;
            return false; ;
        }

        public bool IsUpdated(string name)
        {
            if (_contextUow.AssetTypeRepository.Get(e => e.AssetTypeName == name && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }
    }
}

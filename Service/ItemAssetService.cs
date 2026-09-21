using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ItemAssetService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ItemAsset> GetAll(string includeProperties)
        {
            return _contextUow.ItemAssetRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }


        public ItemAsset GetItem(long? id, string includeProperties)
        {
            return _contextUow.ItemAssetRepository.Get(e => e.ItemAssetId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public IEnumerable<ItemAsset> GetAssets(ItemTypeEnum type, long? itemId, string includeProperties)
        {
            return _contextUow.ItemAssetRepository.Get(e => e.IsDeleted == false
            && e.ItemType == type && e.ItemId == itemId && e.Asset.IsDeleted == false
            , includeProperties: includeProperties);
        }

        public IEnumerable<ItemAsset> GetTickets( long? assetId, string includeProperties)
        {
            return _contextUow.ItemAssetRepository.Get(e => e.IsDeleted == false
            && e.ItemType == ItemTypeEnum.Ticket && e.AssetId == assetId
            , includeProperties: includeProperties);
        }


        public IEnumerable<ItemAsset> GetCriticalAssets(ItemTypeEnum type, long? itemId, string includeProperties)
        {
            return _contextUow.ItemAssetRepository.Get(e => e.IsDeleted == false
            && e.ItemType == type && e.ItemId == itemId
            && e.Asset.IsCritical == true
            , includeProperties: includeProperties);
        }

        public bool Insert(ItemAsset item)
        {
            try
            {
                _contextUow.ItemAssetRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ItemAsset item)
        {
            try
            {
                _contextUow.ItemAssetRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ItemAsset item)
        {
            try
            {
                _contextUow.ItemAssetRepository.Delete(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(ItemAsset item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.ItemAssetId == item.ItemAssetId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

        public bool IsAvailable(ItemAsset item)
        {
            if (_contextUow.ItemAssetRepository.Get(e => e.ItemType == item.ItemType
            && e.ItemId == item.ItemId
            && e.AssetId == item.AssetId
            //&& e.AssetCategory== item.AssetCategory
            && e.IsDeleted == false).Any())
                return true;

            return false;
        }


        public string GetAssetCategoryIdHardware()
        {
            //Convert.ToInt32(AssetCategoryEnum.ITAsset)
            return Convert.ToInt32(AssetCategoryEnum.ITAsset).ToString();
        }

        //public string GetAssetCategoryIdDataCenter()
        //{
        //    return Convert.ToInt32(AssetCategoryEnum.DataCenter).ToString();
        //}

        public string GetAssetCategoryIdTelco()
        {
            return Convert.ToInt32(AssetCategoryEnum.Telco).ToString();
        }
    }
}


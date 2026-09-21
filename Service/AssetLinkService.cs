using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Domain;
using Domain.DR;

namespace Service
{
    public class AssetLinkService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();


        public AssetLink GetAssetLinkItem(long ParentAssetId, long ChildAssetId, string includeProperties)
        {
           return  _contextUow.AssetLinkRepository.Get(e => e.IsDeleted == false
            && e.ParentAssetId == ParentAssetId && e.ChildAssetId == ChildAssetId, includeProperties: includeProperties)
                .FirstOrDefault();
        }

        public ICollection<AssetLink> GetAllChildAsset(long ParentAssetId, string includeProperties)
        {
            return _contextUow.AssetLinkRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.ParentAssetId == ParentAssetId, includeProperties:includeProperties).ToList();
        }


        public bool Insert(AssetLink item)
        {
            try
            {

                _contextUow.AssetLinkRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(AssetLink item)
        {
            try
            {
                _contextUow.AssetLinkRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

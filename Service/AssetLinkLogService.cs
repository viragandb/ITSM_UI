using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Domain;
using Domain.RA;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Service
{
    public class AssetLinkLogService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<AssetLinkLog> GetAll(string includeProperties)
        {
            return _contextUow.AssetLinkLogRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }


        public AssetLinkLog GetItem(long? id, string includeProperties)
        {
            return _contextUow.AssetLinkLogRepository.Get(e => e.AssetLinkLogId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(long ParentAssetId, long ChildAssetId, AssetLinkActionEnum Status, string Description, string UpdatedBy, DateTime UpdatedDate, bool IsDeleted)
        {
            try
            {

                AssetLinkLog log = new AssetLinkLog();
                log.ParentAssetId = ParentAssetId;
                log.ChildAssetId = ChildAssetId;
                log.ActionType = Status;
                log.Description = Description;
                log.UpdatedBy = UpdatedBy;
                log.UpdatedDate = UpdatedDate;
                log.IsDeleted = IsDeleted;             

                _contextUow.AssetLinkLogRepository.Insert(log);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(AssetLinkLog item)
        {
            try
            {
                _contextUow.AssetLinkLogRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(AssetLinkLog item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.AssetLinkLogRepository.Update(item);
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

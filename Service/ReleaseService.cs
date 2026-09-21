using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ReleaseService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<Release> GetAll(string includeProperties)
        {
            return _contextUow.ReleaseRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<Release> GetItems(long? catType, long? assetTypeId, long? changeRequestTypeId, long? statusId, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.ReleaseRepository.GetAsNoTracking(e => e.IsDeleted == false
            && ((Int32)e.AssetType.AssetCategory == catType || catType == 0)
            && (e.AssetTypeId == assetTypeId || assetTypeId == 0)
            && (e.ChangeRequestTypeId == changeRequestTypeId || changeRequestTypeId == 0)
            && (statusId == 0 || (Int32)e.Status == statusId)
            && e.RequestedDate >= sDate && e.RequestedDate < eDate
            , includeProperties: includeProperties);
        }




        public IEnumerable<Release> GetItemsForAssign(string userId, string includeProperties)
        {
            return _contextUow.ReleaseRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.UpdatedBy == userId && e.Status < ReleaseStatusEnum.Completed, includeProperties: includeProperties);
        }
        public IEnumerable<Release> GetAllByUpdatedBy(string userId, string includeProperties)
        {
            return _contextUow.ReleaseRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.UpdatedBy == userId, includeProperties: includeProperties);
        }

        public IEnumerable<Release> GetAllByStatus(ReleaseStatusEnum status, string includeProperties)
        {
            return _contextUow.ReleaseRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == status, includeProperties: includeProperties);
        }
        public Release GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.ReleaseRepository.GetAsNoTracking(e => e.ReleaseId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public Release GetItem(long? id, string includeProperties)
        {
            return _contextUow.ReleaseRepository.Get(e => e.ReleaseId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(Release item)
        {
            try
            {
                _contextUow.ReleaseRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Release item)
        {
            try
            {
                _contextUow.ReleaseRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Release item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ReleaseRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Release item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.ReleaseId == item.ReleaseId && e.IsDeleted == false).Any())
            //    return true;

            return false;
        }

        public int GetReleaseIdInfra()
        {

            return 1;
        }


        public static String GetStatusColor(ReleaseStatusEnum status)
        {
            var color = "default";
            if (status == ReleaseStatusEnum.Pending)
                color = "primary";
            else if (status == ReleaseStatusEnum.Planned)
                color = "success";
            else if (status == ReleaseStatusEnum.Completed)
                color = "warning";
            else if (status == ReleaseStatusEnum.Reviewed)
                color = "info";
            else if (status == ReleaseStatusEnum.Rejected)
                color = "danger";
            return color;
        }

    }
}


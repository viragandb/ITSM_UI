using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AssetVerificationRequestService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<AssetVerificationRequest> GetAll(string includeProperties)
        {
            return _contextUow.AssetVerificationRequestRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public AssetVerificationRequest GetItem(long? id, string includeProperties)
        {
            return _contextUow.AssetVerificationRequestRepository.Get(e => e.AssetVerificationRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public AssetVerificationRequest GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.AssetVerificationRequestRepository.GetAsNoTracking(e => e.AssetVerificationRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }


        public IEnumerable<AssetVerificationRequest> GetItemsPending(long? assetCategoryId, long? branchId, long? departmentId,  string includeProperties)
        {
            return _contextUow.AssetVerificationRequestRepository.Get(e => e.IsDeleted == false
            && ((int)e.AssetCategory == assetCategoryId || assetCategoryId == 0)
            && (e.BranchId == branchId || branchId == 0)
            && (e.DepartmentId == departmentId || departmentId == 0)
            && e.Status ==AssetVerificationStatusEnum.Pending
            , includeProperties: includeProperties);
        }

        public IEnumerable<AssetVerificationRequest> GetItems(long? assetCategoryId, long? branchId, long? departmentId, DateTime startDate, DateTime endDate,  string includeProperties)
        {
            return _contextUow.AssetVerificationRequestRepository.Get(e => e.IsDeleted == false
            && ((int)e.AssetCategory == assetCategoryId || assetCategoryId == 0)
            && (e.BranchId == branchId || branchId == 0)
            && (e.DepartmentId == departmentId || departmentId == 0)
            //&& e.Status == AssetVerificationStatusEnum.Pending
              && ((e.VerificationDate >= startDate && e.VerificationDate <= endDate))
            , includeProperties: includeProperties);
        }


        public IEnumerable<AssetVerificationItem> GetItemsDiscrepancy(long? assetCategoryId, long? branchId, long? departmentId, DateTime startDate, DateTime endDate, string includeProperties)
        {
            return _contextUow.AssetVerificationItemRepository.Get(e => e.IsDeleted == false
            && ((int)e.AssetVerificationRequest.AssetCategory == assetCategoryId || assetCategoryId == 0)
            && (e.AssetVerificationRequest.BranchId == branchId || branchId == 0)
            && (e.AssetVerificationRequest.DepartmentId == departmentId || departmentId == 0)
            && e.AssetVerificationRequest.Status == AssetVerificationStatusEnum.Completed
              && ((e.AssetVerificationRequest.VerificationDate >= startDate && e.AssetVerificationRequest.VerificationDate <= endDate))
              && e.IsAvailable == false
            , includeProperties: includeProperties);
        }

        public bool Insert(AssetVerificationRequest item)
        {
            try
            {
                _contextUow.AssetVerificationRequestRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(AssetVerificationRequest item)
        {
            try
            {
                _contextUow.AssetVerificationRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(AssetVerificationRequest item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.AssetVerificationRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(AssetVerificationRequest item)
        {
            //if (_contextUow.AssetVerificationRequestRepository.Get(e => e.AssetVerificationRequestId == item.AssetVerificationRequestId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

        #region AssetVerificationItem

        public AssetVerificationItem GetAssetVerificationItem(long? requestId,string assetNo, string includeProperties)
        {
            return _contextUow.AssetVerificationItemRepository.Get(e => e.AssetVerificationRequestId == requestId 
            && e.Asset.AssetNo== assetNo, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool UpdateAssetVerificationItem(AssetVerificationItem item)
        {
            try
            {
                _contextUow.AssetVerificationItemRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion



        public static String GetStatusColor(AssetVerificationStatusEnum status)
        {
            var color = "info";
            if (status == AssetVerificationStatusEnum.Pending)
                color = "primary";
            else if (status == AssetVerificationStatusEnum.Completed)
                color = "success";
            //else if (status == IncidentStatusEnum.Reviewed)
            //    color = "success";
            //else if (status == IncidentStatusEnum.Closed)
            //    color = "success";
            //else if (status == IncidentStatusEnum.Rejected)
            //    color = "danger";
            return color;


        }

       

    }
}

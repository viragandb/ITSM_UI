using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AssetService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();


        public IEnumerable<Asset> GetAll(string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<Asset> GetAll(long? assetCategoryId, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
            && ((long)e.AssetCategory == assetCategoryId || assetCategoryId == 0), includeProperties: includeProperties);
        }

        public IEnumerable<Asset> GetItems(long? assetCategoryId, long? assetTypeId, long? allocatedTypeId
          , long? branchId, long? departmentId, long? statusId, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
            && ((int)e.AssetCategory == assetCategoryId || assetCategoryId == 0)
            && (e.AssetTypeId == assetTypeId || assetTypeId == 0)
            && ((int)e.Status == statusId || statusId == 0)
            && ((int)e.AllocatedType == allocatedTypeId || allocatedTypeId == 0)
            && (e.BranchId == branchId || branchId == 0)

            && (e.DepartmentId == departmentId || departmentId == 0)
            , includeProperties: includeProperties);
        }

        public IEnumerable<Asset> GetItemsAdv(long? assetCategoryId, long? assetTypeId, long? allocatedTypeId
            , long? branchId, long? departmentId, int[] statusId, DateTime date, bool warrantyExpired, bool maintenanceExpired, bool toBeReturned
             , string assetNo, string serialNo, string barcodeNo, string assignTo
            , string includeProperties)
        {
            //if (statusId != null)
            //{
            var _list = _contextUow.AssetRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (assetNo == "" || e.AssetNo.Contains(assetNo))
            && (serialNo == "" || e.SerialNo.Contains(serialNo))
            && (barcodeNo == "" || e.Barcode.Contains(barcodeNo))
            && (assignTo == "" || e.AssignedTo == assignTo)

             && ((int)e.AssetCategory == assetCategoryId)
             //&& ((int)e.AssetCategory == assetCategoryId || assetCategoryId == 0)
             && (e.AssetTypeId == assetTypeId || assetTypeId == 0)
             //&& ((int)e.Status == statusId || statusId == 0)
             //&& (statusId != null ?  ((int)e.Status == statusId[0] ) : true)
             && ((int)e.AllocatedType == allocatedTypeId || allocatedTypeId == 0)
             && (e.BranchId == branchId || branchId == 0)

             && (e.DepartmentId == departmentId || departmentId == 0)

             && (warrantyExpired ? (e.WarrantyExpire < date) : true)
             && (maintenanceExpired ? (e.NextMaintenanceDate < date) : true)
             && (toBeReturned ? (e.ToBeReturnedDate < date && e.AllocatedType == AllocatedTypeEnum.External) : true)

            , includeProperties: includeProperties);

            IEnumerable<Asset> resList = Enumerable.Empty<Asset>();
            if (statusId != null)
            {
                foreach (int status in statusId)
                {
                    var subList = _list.Where(p => (int)p.Status == status).ToList();

                    resList = resList.Concat(subList);
                }
            }
            else
            {
                resList = _list;

            }
            return resList;
            //var predicate = PredicateBuilder.False<Asset>();
            //foreach (int status in statusId)
            //{
            //    predicate = predicate.Or(e => (int)e.Status == status);
            //}
            ////foreach (string keyword in keywords)
            ////    predicate = predicate.Or(p => p.Description.Contains(keyword));

            //return objectContext.Products.AsExpandable().Where(predicate);
            //return _contextUow.AsEx
            //}
            //else
            //{
            //    return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
            //   && ((int)e.AssetCategory == assetCategoryId || assetCategoryId == 0)
            //   && (e.AssetTypeId == assetTypeId || assetTypeId == 0)
            //   && ((int)e.AllocatedType == allocatedTypeId || allocatedTypeId == 0)
            //   && (e.BranchId == branchId || branchId == 0)

            //   && (e.DepartmentId == departmentId || departmentId == 0)
            //   , includeProperties: includeProperties);
            //}
        }



        public IEnumerable<Asset> GetItemsAvb(long? barnchId, long? deptId, long? assetCategoryId, long? assetTypeId, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
            && e.Status <= AssetStatusEnum.Assigned
            && (e.BranchId == barnchId || barnchId == 0)
            && (e.DepartmentId == deptId || deptId == 0)
            //&& (e.BranchId == barnchId)
            //&& (e.DepartmentId == deptId)
            && ((long)e.AssetCategory == assetCategoryId || assetCategoryId == 0)
            && (e.AssetTypeId == assetTypeId || assetTypeId == 0)
            , includeProperties: includeProperties);
        }
         public IEnumerable<Asset> GetParentAssetsToLink(long? barnchId, long? deptId, long? assetCategoryId, long? assetTypeId, string assetNo, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false  
            && (string.IsNullOrEmpty(assetNo) || e.AssetNo.Contains(assetNo))
            && (e.BranchId == barnchId || barnchId == 0)
            && (e.DepartmentId == deptId || deptId == 0)          
            && ((long)e.AssetCategory == assetCategoryId || assetCategoryId == 0)
            && (e.AssetTypeId == assetTypeId || assetTypeId == 0)
            , includeProperties: includeProperties);
        }

          public IEnumerable<Asset> GetChildAssetsToLink(long? barnchId, long? deptId, long? assetCategoryId, long? assetTypeId, string assetNo, string includeProperties)
        {
            // need to filter out alrady linked child
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false  
            && (string.IsNullOrEmpty(assetNo) || e.AssetNo.Contains(assetNo))
            && (e.BranchId == barnchId || barnchId == 0)
            && (e.DepartmentId == deptId || deptId == 0)          
            && ((long)e.AssetCategory == assetCategoryId || assetCategoryId == 0)
            && (e.AssetTypeId == assetTypeId || assetTypeId == 0)
            && !e.ParentLinks.Any(pl => pl.IsDeleted == false)

            , includeProperties: includeProperties);
        }



        public IEnumerable<Asset> GetItemsAvbNewTicket(long? barnchId, long? assetTypeId, long? deptId, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
            && e.Status <= AssetStatusEnum.Assigned
            && (e.BranchId == barnchId)
            && (e.DepartmentId == deptId || deptId == 0)
            && (e.AssetTypeId == assetTypeId)
            //&& (e.AssignedTo!= userId)
            , includeProperties: includeProperties);
        }

        public IEnumerable<Asset> GetItemsAvbByUser(DeviceManagementTypeEnum type, string userId, long? assetCategoryId, long? assetTypeId, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
            && e.Status <= AssetStatusEnum.Assigned
            && (e.AssignedTo == userId)
            && (e.DeviceManagementType == type)
            && ((long)e.AssetCategory == assetCategoryId || assetCategoryId == 0)
            && (e.AssetTypeId == assetTypeId || assetTypeId == 0)
            , includeProperties: includeProperties);
        }
        public IEnumerable<Asset> GetItemsUserId(long? barnchId, long? assetTypeId, long? deptId, string userId, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
            && e.Status <= AssetStatusEnum.Assigned
            && (e.BranchId == barnchId)
            && (e.DepartmentId == deptId || deptId == 0)
            && (e.AssetTypeId == assetTypeId)
            && (e.AssignedTo == userId)
            , includeProperties: includeProperties);
        }

        public IEnumerable<Asset> GetItemsSearch(string barcode, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
            && e.Status <= AssetStatusEnum.Assigned
            //&& e.Barcode == barcode
            && e.Barcode.Contains(barcode)
            , includeProperties: includeProperties);
        }

        public IEnumerable<Asset> GetMyAssets(string userId, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
            && e.Status == AssetStatusEnum.Assigned
            && e.AssignedTo == userId
                     , includeProperties: includeProperties);
        }

        public IEnumerable<Asset> GetItemsByLocation(long? assetCategoryId, long? assetTypeId
           , long? branchId, long? departmentId, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
            && ((int)e.AssetCategory == assetCategoryId || assetCategoryId == 0)
            && (e.AssetTypeId == assetTypeId || assetTypeId == 0)
            && (e.BranchId == branchId || branchId == 0)
            && (e.DepartmentId == departmentId || departmentId == 0)
            && (e.Status < AssetStatusEnum.ToBeDisposed)
            , includeProperties: includeProperties);
        }

        public IEnumerable<Asset> GetItemsAvbByLocation(long? assetCategoryId, long? assetTypeId
          , long? branchId, long? departmentId, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
            && ((int)e.AssetCategory == assetCategoryId)
            && (e.AssetTypeId == assetTypeId || assetTypeId == 0)
            && (e.BranchId == branchId)
            && (e.DepartmentId == departmentId)
            && e.Status <= AssetStatusEnum.Assigned
            , includeProperties: includeProperties);
        }


        public IEnumerable<Asset> GetCriticalAssets(long? assetCategoryId, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
            && ((long)e.AssetCategory == assetCategoryId || assetCategoryId == 0)
            && e.IsCritical == true
            , includeProperties: includeProperties);
        }

        public IEnumerable<Asset> GetCriticalAssetsByDR(DateTime sDate, DateTime eDate, DowntimeTypeEnum status, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
                               && e.IsCritical == true
                               && e.DowntimeLogs.Any(d => d.IsDeleted == false
                                                && d.ApplicableDate >= sDate && d.ApplicableDate < eDate
                                                && d.DowntimeType == status)
                               , includeProperties: includeProperties);


        }

        public bool IsAssetNoAvb(string assetNo)
        {
            if (_contextUow.AssetRepository.Get(e => e.AssetNo == assetNo).Any())
                return true;

            return false;
        }
        public static String GetStatusColor(AssetStatusEnum status)
        {
            var color = "default";
            if (status == AssetStatusEnum.Opened)
                color = "info";
            else if (status == AssetStatusEnum.Unassigned)
                color = "info";
            else if (status == AssetStatusEnum.Assigned)
                color = "success";
            else if (status == AssetStatusEnum.InTransit)
                color = "warning";
            else if (status == AssetStatusEnum.UnderRepair)
                color = "primary";
            else if (status == AssetStatusEnum.ToBeDisposed)
                color = "warning";
            else if (status == AssetStatusEnum.Disposed)
                color = "danger";
            return color;
        }


        //public Asset GetItemsByCode(long code, string includeProperties)
        //{
        //    return _contextUow.AssetRepository.Get(e => e.IsDeleted == false
        //    && e.ItemId == code, includeProperties: includeProperties).FirstOrDefault();
        //}

        public void InsertLog(Asset item, string comment)
        {
            AssetLogService _logService = new AssetLogService();
            AssetLog log = new AssetLog();
            log.AssetId = item.AssetId;
            log.Comment = comment;
            log.Status = item.Status;
            log.UpdatedBy = item.UpdatedBy;
            log.UpdatedDate = item.UpdatedDate;
            _logService.Insert(log);


        }

        public void InsertTransLog(Asset asset, AssetTransactionTypeEnum type)
        {
            AssetTransactionLogService _logService = new AssetTransactionLogService();
            AssetTransactionLog transLog = new AssetTransactionLog();
            transLog.AssetId = asset.AssetId;
            transLog.AssignedTo = asset.AssignedTo;
            transLog.BranchId = asset.BranchId;
            transLog.DepartmentId = asset.DepartmentId;
            transLog.Status = asset.Status;
            transLog.TransactionType = type;
            transLog.UpdatedBy = asset.UpdatedBy;
            transLog.UpdatedDate = asset.UpdatedDate;
            _logService.Insert(transLog); 


        }

        public Asset GetItem(long? id, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.AssetId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public Asset GetItemByAssetNo(string assetNo, string includeProperties)
        {
            return _contextUow.AssetRepository.Get(e => e.AssetNo == assetNo, includeProperties: includeProperties).FirstOrDefault();
        }

        public Asset GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.AssetRepository.GetAsNoTracking(e => e.AssetId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public IEnumerable<TempAsset> GetTempAssets(string userId, string includeProperties)
        {
            return _contextUow.TempAssetRepository.GetAsNoTracking(e => e.EmpNo == userId
            , includeProperties: includeProperties);
        }

        public bool Insert(Asset item)
        {
            try
            {
                _contextUow.AssetRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool InsertTempAsset(TempAsset item)
        {
            try
            {
                _contextUow.TempAssetRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Update(Asset item)
        {
            try
            {
                _contextUow.AssetRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Asset item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.AssetRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteTempAssetsByUserId(string userId)
        {
            var assets = _contextUow.TempAssetRepository.Get(e => e.EmpNo == userId, includeProperties: "");

            try
            {
                foreach (var a in assets)
                {
                    _contextUow.TempAssetRepository.Delete(a.TempAssetId);

                }
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeleteTempAsset(long? id)
        {
            try
            {
                _contextUow.TempAssetRepository.Delete(id);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //public bool HasRelationalData(Asset item)
        //{
        //    if (_contextUow.ItemAssetRepository.Get(e => e.AssetCode == item.ItemId.ToString() && e.IsDeleted == false).Any())
        //        return true;

        //    return false;
        //}


        public int GetAssetIdInfra()
        {

            return 1;
        }
        public int GetAssetIdTelco()
        {

            return 9;
        }

        public int GetAssetTypeIdLaptop()
        {

            return 12;
        }


        public static String GetPriorityColor(PriorityEnum status)
        {
            var color = "default";
            if (status == PriorityEnum.VeryLow || status == PriorityEnum.Low)
                color = "info";
            else if (status == PriorityEnum.Medium)
                color = "success";
            else if (status == PriorityEnum.High)
                color = "warning";
            else if (status == PriorityEnum.VeryHigh)
                color = "danger";
            return color;
        }
    }
}

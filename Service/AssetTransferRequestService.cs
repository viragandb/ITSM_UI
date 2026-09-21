using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AssetTransferRequestService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();
        public IEnumerable<AssetTransferRequest> GetAll(string includeProperties)
        {
            return _contextUow.AssetTransferRequestRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }


        public AssetTransferRequest GetItem(long? id, string includeProperties)
        {
            return _contextUow.AssetTransferRequestRepository.Get(e => e.AssetTransferRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public AssetTransferRequest GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.AssetTransferRequestRepository.GetAsNoTracking(e => e.AssetTransferRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public IEnumerable<AssetTransferRequest> GetPendingItems(long? transactionTypeId, long? branchId, long? departmentId, AssetTransStatusEnum status, string userId, string includeProperties)
        {
            var user = _contextUow.UserRepository.GetAsNoTracking(e => e.EmpNo == userId).FirstOrDefault();
            return _contextUow.AssetTransferRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.Status == status)
             && ((int)e.TransactionType == transactionTypeId || transactionTypeId == 0)
             && (e.BranchIdFrom == branchId || branchId == 0)
             && (e.DepartmentIdFrom == departmentId || departmentId == 0)
             && ((e.BranchIdFrom == user.BranchId && e.DepartmentIdFrom == user.DepartmentId) ||
                  (e.BranchFrom.UserBranches.Any(u => u.EmpNo == userId) && e.DepartmentFrom.UserDepartments.Any(u => u.EmpNo == userId))
                )
            , includeProperties: includeProperties);
        }
        public IEnumerable<AssetTransferRequest> GetPendingToItems(long? transactionTypeId, long? branchId, long? departmentId,AssetTransStatusEnum status, string userId, string includeProperties)
        {
            var user = _contextUow.UserRepository.GetAsNoTracking(e => e.EmpNo == userId).FirstOrDefault();
            return _contextUow.AssetTransferRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.Status == status)
             && ((int)e.TransactionType == transactionTypeId || transactionTypeId == 0)
             && (e.BranchId == branchId || branchId == 0)
             && (e.DepartmentId == departmentId || departmentId == 0)
             && ( (e.BranchId== user.BranchId && e.DepartmentId== user.DepartmentId) ||
                  (e.Branch.UserBranches.Any(u=>u.EmpNo==userId) && e.Department.UserDepartments.Any(u => u.EmpNo == userId) )
                )
            , includeProperties: includeProperties);
        }

        public IEnumerable<AssetTransferRequest> GetItems(long? transactionTypeId, long? statusId, long? branchId, long? departmentId
            ,  DateTime sDate, DateTime eDate
            , string includeProperties)
        {
            return _contextUow.AssetTransferRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && ((Int32)e.TransactionType == transactionTypeId || transactionTypeId == 0)
            && (e.BranchIdFrom== branchId || branchId == 0)
            && (e.DepartmentIdFrom == departmentId || departmentId == 0)
            && (statusId == 0 || (Int32)e.Status == statusId)

            && e.InitiatedDate >= sDate && e.InitiatedDate <= eDate

            , includeProperties: includeProperties);
        }

        public IEnumerable<TransferItem> GetTransferItems(long? transactionTypeId, long? statusId, long? branchId, long? departmentId
           , DateTime sDate, DateTime eDate
           , string includeProperties)
        {
            return _contextUow.TransferItemRepository.GetAsNoTracking(e => 
            ((Int32)e.AssetTransferRequest.TransactionType == transactionTypeId || transactionTypeId == 0)
            && (e.AssetTransferRequest.BranchIdFrom == branchId || branchId == 0)
            && (e.AssetTransferRequest.DepartmentIdFrom == departmentId || departmentId == 0)
            && (statusId == 0 || (Int32)e.AssetTransferRequest.Status == statusId)
            && e.AssetTransferRequest.InitiatedDate >= sDate && e.AssetTransferRequest.InitiatedDate <= eDate

            , includeProperties: includeProperties);
        }

        public IEnumerable<TransferItem> GetTransferItems( long? branchId, long? departmentId
         , DateTime sDate, DateTime eDate
         , string includeProperties)
        {
            return _contextUow.TransferItemRepository.GetAsNoTracking(e =>
            (e.AssetTransferRequest.TransactionType == AssetTransactionTypeEnum.Transfer)
            && (e.AssetTransferRequest.AssignedType == AssignedTypeEnum.Permanent)
            && (e.AssetTransferRequest.Status == AssetTransStatusEnum.Completed)
            && (e.AssetTransferRequest.BranchIdFrom == branchId || branchId == 0)
            && (e.AssetTransferRequest.DepartmentIdFrom == departmentId || departmentId == 0)
            && e.AssetTransferRequest.InitiatedDate >= sDate && e.AssetTransferRequest.InitiatedDate <= eDate

            , includeProperties: includeProperties);
        }


        public IEnumerable<AssetTransferRequest> GetMyPendingItems( string userId, string includeProperties)
        {
            return _contextUow.AssetTransferRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
             && e.AssignedTo==userId && e.Status==AssetTransStatusEnum.InTransit
             && e.TransactionType==AssetTransactionTypeEnum.Assigned
            , includeProperties: includeProperties);
        }

        public IEnumerable<AssetTransferRequest> GetUnderRepairItems(long? vendorId, string includeProperties)
        {
            return _contextUow.AssetTransferRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
             && (e.VendorId == vendorId || vendorId == 0)
             && e.Status == AssetTransStatusEnum.Vendor
             && e.TransactionType == AssetTransactionTypeEnum.SentToRepair
            , includeProperties: includeProperties);
        }

        public IEnumerable<AssetTransferRequest> GetItemsByInitiator(string userId,  string includeProperties)
        {
            return _contextUow.AssetTransferRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            //&& (e.Status == AssetTransStatusEnum.InTransit)
            // && ((int)e.TransactionType == transactionTypeId || transactionTypeId == 0)
            // && (e.BranchId == branchId || branchId == 0)
            // && (e.DepartmentId == departmentId || departmentId == 0)
             && e.InitiatedBy== userId
            , includeProperties: includeProperties);
        }


        public IEnumerable<TransferItem> GetTransferItems(long? assetId , string includeProperties)
        {
            return _contextUow.TransferItemRepository.GetAsNoTracking(e => e.AssetId ==assetId, includeProperties: includeProperties);
        }

        public IEnumerable<AssetTransferRequest> GetDisposedItems(string includeProperties)
        {
            return _contextUow.AssetTransferRequestRepository.GetAsNoTracking(e => e.IsDeleted == false &&
            e.TransactionType == AssetTransactionTypeEnum.ToBeDisposed, includeProperties: includeProperties);
        }

        public IEnumerable<AssetTransferRequest> GetNotifyingAssignedTransferRequests(DateTime targetDate,string includeProperties)
        {
            return _contextUow.AssetTransferRequestRepository.Get(e => e.IsDeleted == false &&
            e.TransactionType == AssetTransactionTypeEnum.Assigned &&
            e.Status == AssetTransStatusEnum.InTransit &&
            e.InitiatedDate == targetDate 
            ,includeProperties: includeProperties);
        }

        public bool Insert(AssetTransferRequest item)
        {
            try
            {
                _contextUow.AssetTransferRequestRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Update(AssetTransferRequest item)
        {
            try
            {
                _contextUow.AssetTransferRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Delete(AssetTransferRequest item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.AssetTransferRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //public bool HasRelationalData(AssetTransferRequest item)
        //{
        //    if (_contextUow.ItemAssetTransferRequestRepository.Get(e => e.AssetTransferRequestCode == item.ItemId.ToString() && e.IsDeleted == false).Any())
        //        return true;

        //    return false;
        //}

        public static String GetTransTypeColor(AssetTransactionTypeEnum status)
        {
            var color = "default";
            //if (status == AssetTransactionTypeEnum.StockIn)
            //    color = "default";
            if (status == AssetTransactionTypeEnum.Transfer)
                color = "info";
            else if (status == AssetTransactionTypeEnum.Assigned)
                color = "success";
            else if (status == AssetTransactionTypeEnum.SentToRepair)
                color = "warning";
            else if (status == AssetTransactionTypeEnum.ToBeDisposed)
                color = "primary";
            //else if (status == AssetTransactionTypeEnum.Unassigned)
            //    color = "danger";
            return color;
        }


    }
}

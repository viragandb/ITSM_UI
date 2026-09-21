using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Domain.AR;
using Domain.CM;
using Domain.DR;
using Domain.RA;

namespace Service.DR
{
    public class DisposalRequestService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<DisposalRequest> GetAll(string includeProperties)
        {
            return _contextUow.DisposalRequestRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public DisposalRequest GetItem(long? id, string includeProperties)
        {
            return _contextUow.DisposalRequestRepository.Get(e => e.DisposalRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public DisposalRequest GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.DisposalRequestRepository.GetAsNoTracking(e => e.DisposalRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }

         public IEnumerable<long> GetLinkedItemsAssetId(string includeProperties)
        {
            return _contextUow.DisposalItemRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties)
                .Select(i => i.AssetId).ToList();
        }



        public DisposalItem GetItemByRequestAndAsset(long requestId, long assetId, string includeProperties)
        {
            return _contextUow.DisposalItemRepository.Get(e => e.IsDeleted == false &&
            e.DisposalRequestId == requestId && 
            e.AssetId == assetId, includeProperties: includeProperties).FirstOrDefault();
        }

          public DisposalItem GetItemByAsset(long? assetId, string includeProperties)
        {
            return _contextUow.DisposalItemRepository.Get(e => e.IsDeleted == false &&             
            e.AssetId == assetId, includeProperties: includeProperties).FirstOrDefault();
        }


        public bool Insert(DisposalRequest item)
        {
            try
            {
                _contextUow.DisposalRequestRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertDoc(DisposalRequestDoc item)
        {
            try
            {
                _contextUow.DisposalDocRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertLog(long id, DisposalStatusEnum Status, string Description, bool IsDeleted, string UpdatedBy, DateTime UpdatedDate)
        {
            try
            {
                DisposalRequestLog log = new DisposalRequestLog();
                log.DisposalRequestId = id;
                log.Status = Status;
                log.Description = Description;
                log.IsDeleted = IsDeleted;
                log.UpdatedBy = UpdatedBy;
                log.UpdatedDate = UpdatedDate;

                _contextUow.DisposalRequestLogRepository.Insert(log);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
  
        public bool InsertUpdateStatus(long id, DisposalStatusEnum Status, string Comment, bool IsDeleted, string UpdatedBy, DateTime UpdatedDate)
        {
            try
            {
                DisposalRequestUpdate update = new DisposalRequestUpdate();
                update.DisposalRequestId = id;
                update.Status = Status;
                update.Comment = Comment;
                update.IsDeleted = IsDeleted;
                update.UpdatedBy = UpdatedBy;
                update.UpdatedDate = UpdatedDate;

                _contextUow.DisposalRequestUpdateRepository.Insert(update);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertDisposalItems(DisposalItem item)
        {
            try
            {

                _contextUow.DisposalItemRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(DisposalRequest item)
        {
            try
            {
                _contextUow.DisposalRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         public bool UpdateDisposalItems(DisposalItem item)
        {
            try
            {
                _contextUow.DisposalItemRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static String GetStatusColor(DisposalStatusEnum status)
        {
            var color = "default";
            if (status == DisposalStatusEnum.Initiated)
                color = "info";
            else if (status == DisposalStatusEnum.Reject)
                color = "danger";
            else if (status == DisposalStatusEnum.Return)
                color = "warning";
            else if (status == DisposalStatusEnum.PendingApproval || status == DisposalStatusEnum.Approved || status == DisposalStatusEnum.InTransit || status == DisposalStatusEnum.AmendAndResubmit)
                color = "primary";
            else if (status == DisposalStatusEnum.Completed)
                color = "success";
            return color;


        }
    }
}

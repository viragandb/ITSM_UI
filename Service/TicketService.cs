using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TicketService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<Ticket> GetAll(string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        //public IEnumerable<Ticket> GetTikcets(long? teamId, string assignUserId, long? ticketType, long? categoryId, long? statusId, DateTime sDate, DateTime eDate, string includeProperties)
        //{
        //    return GetTikcets(teamId, assignUserId, ticketType, categoryId, statusId, 0, sDate, eDate, includeProperties);
        //}

        public IEnumerable<Ticket> GetTikcets(long? teamId, string assignUserId, long? ticketType, long? ticketMedium, long? allocatedTypeId, long? categoryId
            ,long? subCategoryid,long? assetTypeId
            , long? statusId, long? vendorId, DateTime sDate, DateTime eDate,string ticketCode
            , bool sla, bool ongoing
            , string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (ticketCode == "" || e.Code.Contains(ticketCode))
            && (e.AssignedTo == assignUserId.ToString() || assignUserId == "")
            && (e.VendorId == vendorId || vendorId == 0)
            && (e.RequestType.TeamId == teamId || teamId == 0)
            && (ticketType == 0 || (Int32)e.RequestType.TicketType == ticketType)
            && (ticketMedium == 0 || (Int32)e.TicketMedium == ticketMedium)
            && (allocatedTypeId == 0 || (Int32)e.AllocatedType == allocatedTypeId)
            && (e.RequestType.CategoryId == categoryId || categoryId == 0)
            && (e.RequestType.SubCategoryId == subCategoryid || subCategoryid == 0)
            && (e.RequestType.AssetTypeId == assetTypeId || assetTypeId == 0)

            && (statusId == 0 || (Int32)e.Status == statusId)

            && ((e.RequestedDate >= sDate && e.RequestedDate <= eDate) || ticketCode != "")
             && (e.IsTimeViolated == true || sla == false)
            && (e.Status < TicketStatusEnum.Completed || ongoing == false)
            , includeProperties: includeProperties);
        }

        //public IEnumerable<Ticket> GetTikcetsReopend(long? teamId, long? companyId, long? assignUserId, long? ticketType, long? categoryId, long? statusId, DateTime sDate, DateTime eDate, string includeProperties)
        //{
        //    return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
        //    && (e.AssignedTo == assignUserId.ToString() || assignUserId == 0)
        //    //&& (e.CompanyId == companyId || companyId == 0)
        //    && (e.RequestType.TeamId == teamId || teamId == 0)
        //    && (ticketType == 0 || (Int32)e.RequestType.TicketType == ticketType)
        //    && (e.RequestType.CategoryId == categoryId || categoryId == 0)
        //    && (statusId == 0 || (Int32)e.Status == statusId)
        //    && e.RequestedDate >= sDate && e.RequestedDate < eDate
        //    && e.TicketUpdates.Any(u => u.Status == TicketStatusEnum.Reopened)
        //    , includeProperties: includeProperties);
        //}
        public IEnumerable<Ticket> GetTikcetsCriticalAsset(long? teamId, long? companyId, long? assignUserId, long? ticketType, long? categoryId, DateTime sDate, DateTime eDate, string includeProperties)
        {

            var q = (from t in _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.AssignedTo == assignUserId.ToString() || assignUserId == 0)
          //&& (e.CompanyId == companyId || companyId == 0)
          && (e.RequestType.TeamId == teamId || teamId == 0)
          && (ticketType == 0 || (Int32)e.RequestType.TicketType == ticketType)
          && (e.RequestType.CategoryId == categoryId || categoryId == 0)
          && e.Status >= TicketStatusEnum.Completed
          && e.RequestedDate >= sDate && e.RequestedDate < eDate
          , includeProperties: includeProperties)
                     join a in _contextUow.ItemAssetRepository.GetAsNoTracking(e => e.IsDeleted == false && e.ItemType == ItemTypeEnum.Ticket && e.Asset.IsCritical == true) on t.TicketId equals a.ItemId

                     select t);

            return q;


        }


        public IEnumerable<Ticket> GetTikcets(long? requestTypeId, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.RequestTypeId == requestTypeId || requestTypeId == 0)
            && e.RequestedDate >= sDate && e.RequestedDate < eDate
            , includeProperties: includeProperties);
        }

        public IEnumerable<Ticket> GetTikcets(string ticketCode, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Code.Contains(ticketCode)
            , includeProperties: includeProperties);
        }


        public IEnumerable<Ticket> GetTikcetsByAssetType(long? assetTypeId, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.RequestType.AssetTypeId == assetTypeId)
            && e.RequestedDate >= sDate && e.RequestedDate < eDate
            , includeProperties: includeProperties);
        }
        public IEnumerable<Ticket> GetItemsPendingByTeam(string userId, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == TicketStatusEnum.Waiting
            && e.PendingTeam.UserTeams.Any(t => t.EmpNo == userId)
            , includeProperties: includeProperties);
        }

        public IEnumerable<Ticket> GetItemsNotCompletedByTeam(string userId, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status <= TicketStatusEnum.Completed
            && e.PendingTeam.UserTeams.Any(t => t.EmpNo == userId)
            , includeProperties: includeProperties);
        }

        public IEnumerable<Ticket> GetItemsByCreatedByPending(string createdBy, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && ((e.CreatedBy == createdBy && (e.Status == TicketStatusEnum.Pending || e.Status == TicketStatusEnum.Waiting))
                     || (e.AssignedTo == createdBy && e.Status < TicketStatusEnum.Completed))
            // && (e.CreatedBy == createdBy || e.AssignedTo== createdBy)
            // && (e.Status == TicketStatusEnum.Pending || e.Status == TicketStatusEnum.Waiting)
            , includeProperties: includeProperties);
        }
        public IEnumerable<Ticket> GetItemsByAssignedByPending(string assignedBy, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false && e.AssignedTo == assignedBy
            && (e.Status == TicketStatusEnum.Pending || e.Status == TicketStatusEnum.Waiting), includeProperties: includeProperties);
        }

        public IEnumerable<Ticket> GetItemsByAssignedTo(string assignedTo, TicketStatusEnum status, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false && e.AssignedTo == assignedTo && e.Status == status, includeProperties: includeProperties);
        }

        public IEnumerable<Ticket> GetItemsPendingReview(string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == TicketStatusEnum.Rated
            && e.IsReviewed == false
            && e.RequestType.TicketType != TicketTypeEnum.SR
            , includeProperties: includeProperties);


        }
        public IEnumerable<Ticket> GetItemsPendingReviewSLA(string userId,string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.AssignedTo == userId
            && e.IsTimeViolated == true && (e.ReasonForSLA == "" || e.ReasonForSLA == null)

            , includeProperties: includeProperties);
        }
        public IEnumerable<Ticket> GetItemsByAssignedToPending(string assignedTo, long? statusId, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false && e.AssignedTo == assignedTo
            && (statusId == 0 || (Int32)e.Status == statusId)
            && e.Status >= TicketStatusEnum.Waiting && e.Status < TicketStatusEnum.Completed, includeProperties: includeProperties);
        }
        public IEnumerable<Ticket> GetItemsByAssignedSLA(long? categoryId, long? companyId, long? statusId, string assignedTo, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false && e.AssignedTo == assignedTo
            && (e.RequestType.CategoryId == categoryId || categoryId == 0)
            //&& (e.CompanyId == companyId || companyId == 0)
            && (statusId == 0 || (Int32)e.Status == statusId)

            && e.IsTimeViolated == true && (e.ReasonForSLA == "" || e.ReasonForSLA == null)
            && e.Status > TicketStatusEnum.Waiting && e.Status < TicketStatusEnum.Completed, includeProperties: includeProperties);
        }
        public IEnumerable<Ticket> GetItemsPendingNotAssignedToMe(long? categoryId, long? companyId, string assignedTo, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status >= TicketStatusEnum.Waiting && e.Status < TicketStatusEnum.Completed
             && e.AssignedTo != assignedTo
            && (e.RequestType.CategoryId == categoryId || categoryId == 0)
            //&& (e.CompanyId == companyId || companyId == 0)

             , includeProperties: includeProperties).Where(s => _contextUow.UserTeamRepository.Get(e => e.IsDeleted == false
             && e.EmpNo == assignedTo && e.TeamId == s.RequestType.TeamId, includeProperties: "").Any());
        }

        public IEnumerable<Ticket> GetItemsByAssignedTo(string assignedTo, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false && e.AssignedTo == assignedTo, includeProperties: includeProperties);
        }

        public IEnumerable<Ticket> GetItemsByRequestedUserOpen(string requestedBy, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false && e.RequestedBy == requestedBy
            && e.Status < TicketStatusEnum.Completed, includeProperties: includeProperties);
        }
        public IEnumerable<Ticket> GetItemsByRequestedUserCompleted(string requestedBy, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false && e.RequestedBy == requestedBy
            && e.Status == TicketStatusEnum.Completed, includeProperties: includeProperties);
        }

        public IEnumerable<Ticket> GetItemsNotCompleted(string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false 
            && e.Status <= TicketStatusEnum.Completed, 
            includeProperties: includeProperties);
        }

        public IEnumerable<Ticket> GetItemsNotCompletedUser(string userId,string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status <= TicketStatusEnum.Completed && e.RequestedBy == userId,
            includeProperties: includeProperties);
        }

        public IEnumerable<Ticket> GetItemsPending(string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status < TicketStatusEnum.Completed,
            includeProperties: includeProperties);
        }

        public IEnumerable<Ticket> GetItemsByRequestedUserClosed(string requestedBy, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false && e.RequestedBy == requestedBy
            && e.Status > TicketStatusEnum.Completed, includeProperties: includeProperties);
        }


        public IEnumerable<Ticket> GetItemsWaiting(string userId, string includeProperties)
        {

            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false && (e.Status == TicketStatusEnum.Waiting || (e.Status == TicketStatusEnum.Pending))

            , includeProperties: includeProperties).Where(s => _contextUow.UserTeamRepository.Get(e => e.IsDeleted == false
             && e.EmpNo == userId && e.TeamId == s.RequestType.TeamId, includeProperties: "").Any());
        }

        public IEnumerable<Ticket> GetItemsWaitingPending(string userId, string includeProperties)
        {

            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false && (e.Status == TicketStatusEnum.Waiting || e.Status == TicketStatusEnum.Pending)

            , includeProperties: includeProperties).Where(s => _contextUow.UserTeamRepository.Get(e => e.IsDeleted == false
             && e.EmpNo == userId && e.TeamId == s.RequestType.TeamId, includeProperties: "").Any());
        }
        public IEnumerable<Ticket> GetItemsChangeCat(long? categoryId, long? companyId, string userId, string includeProperties)
        {

            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false && (e.Status < TicketStatusEnum.Completed)
             && (e.RequestType.CategoryId == categoryId || categoryId == 0)
            , includeProperties: includeProperties).Where(s => _contextUow.UserTeamRepository.Get(e => e.IsDeleted == false
             && e.EmpNo == userId && e.TeamId == s.RequestType.TeamId, includeProperties: "").Any());
        }

        public IEnumerable<Ticket> GetNotRatedTickets(DateTime date, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.UpdatedDate < date
            && e.Status == TicketStatusEnum.Completed
            , includeProperties: includeProperties);
        }

        public int GetPendingTicketCountByUserId(string requestedBy)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == TicketStatusEnum.Completed && e.RequestedBy== requestedBy,
            includeProperties: "").Count();
        }

        #region Day End EMails


        public IEnumerable<Ticket> GetAll(DateTime date, string includeProperties)
        {
            DateTime endDate = date.AddDays(1);
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && !(e.TicketUpdates.Any(u => u.UpdatedDate >= date && u.UpdatedDate < endDate))
            // && e.RequestedDate >= date && e.RequestedDate < endDate
            //&& e.Status < TicketStatusEnum.Completed
            , includeProperties: includeProperties);
        }

        public IEnumerable<Ticket> GetAllCompleted(DateTime date, string includeProperties)
        {
            DateTime endDate = date.AddDays(1);
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && !(e.TicketUpdates.Any(u => u.UpdatedDate >= date && u.UpdatedDate < endDate))
            // && e.RequestedDate >= date && e.RequestedDate < endDate
            && e.Status == TicketStatusEnum.Completed
            , includeProperties: includeProperties);
        }

        public IEnumerable<Ticket> GetAllPendings(DateTime date, string includeProperties)
        {
            DateTime endDate = date.AddDays(1);
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && !(e.TicketUpdates.Any(u => u.UpdatedDate >= date && u.UpdatedDate < endDate))
            // && e.RequestedDate >= date && e.RequestedDate < endDate
            && e.Status < TicketStatusEnum.Completed
            , includeProperties: includeProperties);
        }


        #endregion

        #region KPI
        public int GetUserCreatedTicketsCount(string userId, DateTime date, string includeProperties)
        {
            DateTime endDate = date.AddDays(1);
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.CreatedBy == userId
            && e.RequestedDate >= date && e.RequestedDate < endDate
            , includeProperties: includeProperties).Count();
        }

        //public int GetTotalNoOfTickets(string userId, DateTime date, string includeProperties)
        //{
        //    DateTime endDate = date.AddDays(1);

        //    return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
        //    , includeProperties: includeProperties).Where(s => _contextUow.TicketUpdateRepository.Get(e => e.IsDeleted == false
        //    && e.TicketId == s.TicketId && e.UpdatedBy == userId
        //    && e.UpdatedDate >= date && e.UpdatedDate < endDate
        //     , includeProperties: "").Any()).Count();
        //}

        public int GetTotalNoOfTickets(string userId, DateTime date, string includeProperties)
        {
            DateTime endDate = date.AddDays(1);

            //return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            //, includeProperties: includeProperties).Where(s => _contextUow.TicketUpdateRepository.Get(e => e.IsDeleted == false
            //&& e.TicketId == s.TicketId && e.UpdatedBy == userId
            //&& e.UpdatedDate >= date && e.UpdatedDate < endDate
            // , includeProperties: "").Any()).Count();

            //return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            //&& e.TicketUpdates.All(u => u.IsDeleted == false
            //&& e.UpdatedBy == userId
            //&& e.UpdatedDate >= date && e.UpdatedDate < endDate)
            //, includeProperties: "").Count();

            return _contextUow.TicketUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.UpdatedBy == userId
            && e.UpdatedDate >= date && e.UpdatedDate < endDate
            , includeProperties: "").Select(o => o.TicketId).Distinct().Count();
        }

        public int GetNoOfTicketsSLViolated(string userId, DateTime date, string includeProperties)
        {
            DateTime endDate = date.AddDays(1);

            //return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            //&& e.IsTimeViolated == true
            //, includeProperties: includeProperties).Where(s => _contextUow.TicketUpdateRepository.Get(e => e.IsDeleted == false
            //&& e.TicketId == s.TicketId && e.UpdatedBy == userId
            //&& e.UpdatedDate >= date && e.UpdatedDate < endDate
            // , includeProperties: "").Any()).Count();

            return _contextUow.TicketUpdateRepository.GetAsNoTracking(e => e.IsDeleted == false
                    && e.UpdatedBy == userId
                    && e.UpdatedDate >= date && e.UpdatedDate < endDate
                    && e.Ticket.IsTimeViolated == true
                    , includeProperties: "").Select(o => o.TicketId).Distinct().Count();

        }
        public int GetTotalAssignedTickets(string userId, DateTime date, string includeProperties)
        {
            DateTime endDate = date.AddDays(1);
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.AssignedBy == userId
            && e.RequestedDate >= date && e.RequestedDate < endDate
           , includeProperties: includeProperties).Count();
        }

        public int GetFirstLevelResolveTickets(string userId, DateTime date, string includeProperties)
        {
            DateTime endDate = date.AddDays(1);
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
           && e.AssignedBy == userId
            && e.RequestedDate >= date && e.RequestedDate < endDate
            && e.SpentTime <= 0.5 && e.Status >= TicketStatusEnum.Completed
           , includeProperties: includeProperties).Count();
        }
        #endregion




        public IEnumerable<Ticket> GetItemsBeforeComleted(string userId, string includeProperties)
        {

            return _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false && (e.Status < TicketStatusEnum.Completed)

            , includeProperties: includeProperties).Where(s => _contextUow.UserTeamRepository.Get(e => e.IsDeleted == false
             && e.EmpNo == userId && e.TeamId == s.RequestType.TeamId, includeProperties: "").Any());
        }

        public Ticket GetItem(long? id, string includeProperties)
        {
            return _contextUow.TicketRepository.Get(e => e.TicketId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public Ticket GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.TicketId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public Ticket GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.TicketId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public Ticket GetItemByTicketCode(string code, string includeProperties)
        {
            return _contextUow.TicketRepository.GetAsNoTracking(e => e.Code == code, includeProperties: includeProperties).FirstOrDefault();
        }
        public long GetItemByCode(string code, string includeProperties)
        {
            return _contextUow.TicketRepository.Get(e => e.Code == code, includeProperties: includeProperties).FirstOrDefault().TicketId;
        }


        public bool Insert(Ticket item)
        {
            try
            {
                _contextUow.TicketRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Ticket item)
        {
            try
            {
                if (item.SpentTime > item.Resolve)
                    item.IsTimeViolated = true;
                else
                    item.IsTimeViolated = false;

                _contextUow.TicketRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Ticket item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.TicketRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Ticket item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.TicketId == item.TicketId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

        public static String GetTicketTypeColor(TicketTypeEnum status)
        {
            var color = "default";
            if (status == TicketTypeEnum.IN)
                color = "danger";
            else if (status == TicketTypeEnum.SR)
                color = "success";
            return color;
        }
        public static String GetTicketMediumColor(TicketMediumEnum status)
        {
            var color = "default";
            if (status == TicketMediumEnum.Call)
                color = "success";
            else if (status == TicketMediumEnum.Email)
                color = "primary";
            else if (status == TicketMediumEnum.User)
                color = "warning";
            //else if (status == TaskTypeEnum.LeaveFull || status == TaskTypeEnum.LeaveHalf || status == TaskTypeEnum.LeaveShort)
            //    color = "danger";
            return color;
        }

        public static String GetTicketStatusColor(TicketStatusEnum status)
        {
            var color = "default";
            if (status == TicketStatusEnum.Pending || status == TicketStatusEnum.Waiting)
                color = "danger";
            else if (status == TicketStatusEnum.InProgress || status==TicketStatusEnum.UnderProcurement || status == TicketStatusEnum.ReadyToSend || status ==TicketStatusEnum.WaitingFor)
                color = "primary";
            else if (status == TicketStatusEnum.InTransit || status == TicketStatusEnum.Vendor || status == TicketStatusEnum.UnderRepair || status == TicketStatusEnum.WarrantyClaim)
                color = "orange";
            else if (status == TicketStatusEnum.Completed)
                color = "success";
            else if (status == TicketStatusEnum.Rated)
                color = "warning";
            else if (status == TicketStatusEnum.BackupDeviceProvided)
                color = "info";
            return color;


        }

        public static String GetTicketPriorityColor(TicketPriorityEnum status)
        {
            // danger success warning info primary
            var color = "default";
            if (status == TicketPriorityEnum.VeryLow)
                color = "info";
            else if (status == TicketPriorityEnum.Low)
                color = "primary";
            else if (status == TicketPriorityEnum.Medium)
                color = "success";
            else if (status == TicketPriorityEnum.High)
                color = "warning";
            else if (status == TicketPriorityEnum.VeryHigh)
                color = "danger";
            return color;
        }

        public static String GetTicketRateColor(RateTypeEnum status)
        {
            var color = "default";
            if (status == RateTypeEnum.Rate1)
                color = "danger";
            else if (status == RateTypeEnum.Rate2)
                color = "warning";
            else if (status == RateTypeEnum.Rate3)
                color = "info";
            else if (status == RateTypeEnum.Rate4)
                color = "success";
            else if (status == RateTypeEnum.Rate5)
                color = "primary";
            return color;
        }

        public static String GetTicketRate(RateTypeEnum status)
        {
            var color = "";
            int sVal = 0;
            try
            {
                sVal = Convert.ToInt32(status);
            }
            catch (Exception ex) { }
            for (int i = 0; i < sVal; i++)
            {
                color += "<i class='fa fa-star'></i>";
            }


            return color;
        }
        public void InsertTicketStatus(Ticket item, string comment)
        {
            TicketUpdateService _ticketUpdateService = new TicketUpdateService();
            TicketUpdate ticketUpdate = new TicketUpdate();
            ticketUpdate.UpdatedBy = item.UpdatedBy;
            ticketUpdate.UpdatedDate = item.UpdatedDate;
            ticketUpdate.Status = item.Status;
            ticketUpdate.TeamId = item.PendingTeamId;
            ticketUpdate.Comment = comment;
            ticketUpdate.TicketId = item.TicketId;
            ticketUpdate.SpentTime = item.SpentTime;
            ticketUpdate.AllocatedType = item.AllocatedType;
            _ticketUpdateService.Insert(ticketUpdate);

        }

        public void InsertTicketLog(Ticket ticket, string comment)
        {
            TicketLogService _logService = new TicketLogService();
            TicketLog log = new TicketLog();
            log.TicketId = ticket.TicketId;
            log.Comment = comment;
            log.Status = ticket.Status;
            log.UpdatedBy = ticket.UpdatedBy;
            log.UpdatedDate = ticket.UpdatedDate;
            _logService.Insert(log);

        }
        public void UpdateStatus(Ticket item)
        {
            Ticket ticket = GetItem(item.TicketId, "");
            ticket.Status = item.Status;

            if (item.Status == TicketStatusEnum.Rated)
                ticket.Rate = item.Rate;

            ticket.SpentTime += item.SpentTime;
            if (ticket.SpentTime > ticket.Resolve)
                ticket.IsTimeViolated = true;

            ticket.UpdatedBy = item.UpdatedBy;
            ticket.UpdatedDate = item.UpdatedDate;

            if (item.VendorId > 0)
            {
                ticket.VendorId = item.VendorId;
                ticket.VendorRefNo = item.VendorRefNo;
            }
            Update(ticket);
        }
    }
}
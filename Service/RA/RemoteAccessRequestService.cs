using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Data;
using Domain;
using Domain.AR;
using Domain.RA;

namespace Service.RA
{
    public class RemoteAccessRequestService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<RemoteAccessRequest> GetAll(string includeProperties)
        {
            return _contextUow.RemoteAccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public RemoteAccessRequest GetItem(long? id, string includeProperties)
        {
            return _contextUow.RemoteAccessRequestRepository.Get(e => e.RemoteAccessRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public RemoteAccessRequest GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.RemoteAccessRequestRepository.GetAsNoTracking(e => e.RemoteAccessRequestId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        //Approve

        public IEnumerable<RemoteAccessRequest> GetItemsSupervisorApprovals(string userId, string includeProperties)
        {
            return _contextUow.RemoteAccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.FinalApprovalBy == userId && e.ApprovalStage == RemoteAccessApprovalStageEnum.SupervisorApprovalStage
            , includeProperties: includeProperties);
        }

        public IEnumerable<RemoteAccessRequest> GetItemsVPAVPApprovals(string userId, string includeProperties)
        {
            return _contextUow.RemoteAccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.FinalApprovalBy == userId && e.ApprovalStage == RemoteAccessApprovalStageEnum.VPOrAVPApprovalStage
            , includeProperties: includeProperties);
        }

        public IEnumerable<RemoteAccessRequest> GetItemsTeamApprovals(long teamId, string includeProperties)
        {
            return _contextUow.RemoteAccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.TeamId == teamId && e.ApprovalStage == RemoteAccessApprovalStageEnum.TeamApprovalStage
            , includeProperties: includeProperties);
        }

        public IEnumerable<RemoteAccessRequest> GetItemsOngoing(string includeProperties)
        {
            return _contextUow.RemoteAccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.Status < AccessRequestStatusEnum.Completed)

            , includeProperties: includeProperties);
        }

        public IEnumerable<RemoteAccessRequest> GetItemsPendingRevokeConfirmation(string includeProperties)
        {
            return _contextUow.RemoteAccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.Status == AccessRequestStatusEnum.RevokeConfirmationPending)

            , includeProperties: includeProperties);
        }

        // called by dayend to change "revoke confirmation pending" --> "to be revoked" status when two weeks has passed
        // from revoke confirmation pending status witout a user reply
        public IEnumerable<RemoteAccessRequest> GetItemsPendingRevokeConfirmationPastTwoWeeks(string includeProperties)
        {
            
            DateTime thresholdDate = DateTime.Now.AddDays(-14);

            return _contextUow.RemoteAccessRequestRepository.Get(
                e => e.IsDeleted == false
                && e.Status == AccessRequestStatusEnum.RevokeConfirmationPending
                && e.UpdatedDate <= thresholdDate, 
                includeProperties: includeProperties
            ).ToList();
        }

        public IEnumerable<RemoteAccessRequest> GetItemsToBeRevoked(string includeProperties)
        {
            return _contextUow.RemoteAccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.Status == AccessRequestStatusEnum.ToBeRevoked)

            , includeProperties: includeProperties);
        }


        //Tictket

        public IEnumerable<RemoteAccessRequest> GetTikcets(long? branchId, long? departmentId
            , long? accessTypeId
            , long? statusId, DateTime startDate, DateTime endDate, string requestId
            , string includeProperties, string userId = null, long? teamId = 0, bool filterByEndDate = false, bool completed = false)
        {
            long RemoteAccessRequestId = 0;
            try
            {
                RemoteAccessRequestId = Convert.ToInt64(requestId);
            }
            catch (Exception ex) { }


            return _contextUow.RemoteAccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (string.IsNullOrEmpty(userId) || e.RequestedBy == userId)
            && (e.TeamId == teamId || teamId == 0)
            && (e.BranchId == branchId || branchId == 0)
            && (e.DepartmentId == departmentId || departmentId == 0)
            && (accessTypeId == 0 || (Int32)e.RemoteAccessType == accessTypeId)
            && (                
            
                completed? (e.Status == AccessRequestStatusEnum.Completed) : (statusId == 0 || (Int32)e.Status == statusId)

            )
            && (e.RemoteAccessRequestId == RemoteAccessRequestId || RemoteAccessRequestId == 0)

            //&& ((e.RequestedDate >= startDate && e.RequestedDate <= endDate) || RemoteAccessRequestId > 0)
            &&
            (
                (
                    !filterByEndDate
                    && e.RequestedDate >= startDate
                    && e.RequestedDate <= endDate
                )

                ||

                (
                    filterByEndDate
                    && e.EndDate >= startDate
                    && e.EndDate <= endDate
                )

                ||

                RemoteAccessRequestId > 0
            )
            , includeProperties: includeProperties).OrderByDescending(e => e.RemoteAccessRequestId);
        }

       
        //My Request
        public IEnumerable<RemoteAccessRequest> GetItemsMy(string userId, string includeProperties)
        {
            return _contextUow.RemoteAccessRequestRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.CreatedBy == userId
            , includeProperties: includeProperties);
        }

        public IEnumerable<RemoteAgreement> AnnexureTwo(string includeProperties)
        {
            return _contextUow.RemoteAgreementRepository.GetAsNoTracking(e => e.IsDeleted == false
             , includeProperties: includeProperties);
        }
        public RemoteAgreement GetAnnexureByRequestId(long RequestId, string includeProperties)
        {
            return _contextUow.RemoteAgreementRepository.GetAsNoTracking(e => e.IsDeleted == false && e.RemoteAccessRequestId == RequestId
             , includeProperties: includeProperties).FirstOrDefault();
        }

        //public long Insert(RemoteAccessRequest item)
        //{
        //    try
        //    {
        //        _contextUow.RemoteAccessRequestRepository.Insert(item);
        //        _contextUow.Save();
        //        return item.RemoteAccessRequestId;
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}
        public bool Insert(RemoteAccessRequest item)
        {
            try
            {
                _contextUow.RemoteAccessRequestRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertLog(long id, AccessRequestStatusEnum Status, string Description, bool IsDeleted, string UpdatedBy, DateTime UpdatedDate)
        {
            try
            {
                RemoteAccessRequestLog log = new RemoteAccessRequestLog();
                log.RemoteAccessRequestId = id;
                log.Status = Status;
                log.Description = Description;
                log.IsDeleted = IsDeleted;
                log.UpdatedBy = UpdatedBy;
                log.UpdatedDate = UpdatedDate;

                _contextUow.RemoteAccessRequestLogRepository.Insert(log);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //public bool InsertUpdateStatus(RemoteAccessRequestUpdate update)
        //{
        //    try
        //    {

        //        _contextUow.RemoteAccessRequestUpdateRepository.Insert(update);
        //        _contextUow.Save();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}  
        
        public bool InsertUpdateStatus(long id, long? TeamId, AccessRequestStatusEnum Status, string Comment, double SpentTime, 
            bool IsDeleted, string UpdatedBy , DateTime UpdatedDate, long? ClarificationRequestedByTeamId = null, 
            long? ClarificationAssignedToTeamId = null, ClarificationType? clarificationType = null)
        {
            try
            {
                RemoteAccessRequestUpdate update = new RemoteAccessRequestUpdate();
                update.RemoteAccessRequestId = id;
                update.TeamId = TeamId;
                update.Status = Status;
                update.Comment = Comment;
                update.SpentTime = SpentTime;
                update.IsDeleted = IsDeleted;
                update.UpdatedBy = UpdatedBy;
                update.UpdatedDate = UpdatedDate;
                update.ClarificationRequestedByTeamId = ClarificationRequestedByTeamId;
                update.ClarificationAssignedToTeamId = ClarificationAssignedToTeamId;
                update.ClarificationType = clarificationType;

                _contextUow.RemoteAccessRequestUpdateRepository.Insert(update);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertRequiredSystems(RemoteAccessRequiredSystem RequiredSystem)
        {
            try
            {

                _contextUow.RemoteAccessRequiredSystems.Insert(RequiredSystem);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool Update(RemoteAccessRequest item)
        {
            try
            {
                _contextUow.RemoteAccessRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(RemoteAccessRequest item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.RemoteAccessRequestRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public static String GetUserTypeColor(AllocatedTypeEnum type)
        {
            // danger success warning info primary
            var color = "default";
            if (type == AllocatedTypeEnum.Internal)
                color = "info";
            else if (type == AllocatedTypeEnum.External)
                color = "warning";

            return color;
        }



        public static String GetStatusColor(AccessRequestStatusEnum status)
        {
            var color = "default";
            if (status == AccessRequestStatusEnum.Initiated)
                color = "info";
            else if (status == AccessRequestStatusEnum.Rejected || status == AccessRequestStatusEnum.Revoked)
                color = "danger";
            else if (status == AccessRequestStatusEnum.Pending)
                color = "primary";
            else if (status == AccessRequestStatusEnum.Completed)
                color = "success";
            return color;
        }


        //method to get the team a user belong to
        public IEnumerable<RemoteAccessRequest> GetITSecITNetRemoteAccessApproval(List<long> teamId, string includeProperties)
        {

            return _contextUow.RemoteAccessRequestRepository.Get(
                r => r.IsDeleted == false && teamId.Contains((long)r.TeamId), includeProperties: includeProperties
                );

        }


        public bool HasOpenClarification(long RequestId, string includeProperties)
        {
            var updates = _contextUow.RemoteAccessRequestUpdateRepository.Get(u => !u.IsDeleted && u.RemoteAccessRequestId == RequestId, includeProperties: includeProperties)
                            .OrderBy(u => u.UpdatedDate)
                            .ToList();

            var lastClarificationRequest = updates
                .LastOrDefault(u => u.Status == AccessRequestStatusEnum.ClarificationRequested);

            if (lastClarificationRequest == null)
                return false;

            var clarificationProvidedAfter = updates.Any(u =>
                u.Status == AccessRequestStatusEnum.ClarificationProvided &&
                u.UpdatedDate > lastClarificationRequest.UpdatedDate
            );

            return !clarificationProvidedAfter;
        }


        public IEnumerable<RemoteAccessRequestUpdate> GetClarificationUpdates(long? RequestId, string includeProperties)
        {
            return _contextUow.RemoteAccessRequestUpdateRepository.Get(
                u => !u.IsDeleted && u.RemoteAccessRequestId == RequestId
                && (u.Status == AccessRequestStatusEnum.ClarificationRequested || u.Status == AccessRequestStatusEnum.ClarificationProvided), includeProperties: includeProperties)
                .OrderBy(u => u.UpdatedDate)
                .ToList();

        }


        // method returning the number people who are usng remote access from a given department
        // later need to account fr revoked ones
        public int GetCompletedRemoteAccessCountByDepartment(long DepartmentId)
        {
            return _contextUow.RemoteAccessRequestRepository.Get(r => !r.IsDeleted
            && r.DepartmentId == DepartmentId && r.Status == AccessRequestStatusEnum.Completed, includeProperties: "").Count();

        }



        //get request going to expire in two weeks from now if target date is two weeks from now else get expired request if target date is today
        public IEnumerable<RemoteAccessRequest> GetExpiringRequests(DateTime targetDate, string includeProperties, params AccessRequestStatusEnum[] allowedStatuses)
        {
            //var allowedStatuses = new[] {
            //    AccessRequestStatusEnum.Completed,
            //    AccessRequestStatusEnum.RevokeConfirmationPending,
            //    AccessRequestStatusEnum.RevokeConfirmedContinue 
            //};
          
            return _contextUow.RemoteAccessRequestRepository.Get(r => !r.IsDeleted 
            && allowedStatuses.Contains(r.Status)
            && r.EndDate == targetDate,
            includeProperties: includeProperties).ToList();


        }

        // functions to return team emails for use in remote access

        public String GetRiskTeamEmail()     
        {
            return "amanda.fernando@12345.com";
        }

        public String GetHRTeamEmail()       
        {
            return "amanda.fernando@12345.com";
        }

        public String GetITSecHeadEmail()   
        {
            return "amanda.fernando@12345.com";
        }
        public String GetITSVPEmail()   
        {
            return "amanda.fernando@12345.com";
        }
        public String GetDCEOEmail()   
        {
            return "amanda.fernando@12345.com";
        }

        public String GetNetworkImplementerTeamEmail()
        {
            return "amanda.fernando@12345.com";
        }

        public String GetSecurityImplementerTeamEmail()
        {
            return "amanda.fernando@12345.com";
        }

        public String GetHelpDeskEmail()
        {
            return "amanda.fernando@12345.com";
        }


    }
}

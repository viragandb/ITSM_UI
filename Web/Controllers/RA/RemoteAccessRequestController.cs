using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain;
using Domain.AR;
using Domain.RA;
using log4net.Core;
using Microsoft.Ajax.Utilities;
using Microsoft.Win32;
using Newtonsoft.Json;
using Service;
using Service.AR;
using Service.RA;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using Web.Models;
using Web.Utility;

namespace Web.Controllers.RA
{
    public class RemoteAccessRequestController : Controller
    {
        private readonly RemoteAccessRequestService _service = new RemoteAccessRequestService();

        private readonly RemoteAgreementService _remoteaggrementservice = new RemoteAgreementService();

        private readonly UserTeamService _userTeamService = new UserTeamService();

        private readonly UserDepartmentService _userDepartmentService = new UserDepartmentService();

        private readonly DepartmentService _departmentService = new DepartmentService();

        private readonly BranchService _branchService = new BranchService();

        private readonly UserService _userService = new UserService();

        private readonly TeamService _teamService = new TeamService();

        private DateTimeService _dateTimeService = new DateTimeService();


        // GET: RemoteAccessRequest
        [AccessAuthorize]
        [HttpGet]
        public ActionResult Index(long? teamId, long? branchId, long? departmentId, long? accessTypeId
           , long? statusId, string startDate, string endDate, string checkBoxOnGoing, string requestId)
        {

            TicketService _ticketService = new TicketService();
            DateTime sDate = UserDateTime.GetUserDateOnly();
            DateTime eDate = UserDateTime.GetUserDateOnly();
            if (startDate != null && startDate != "" && endDate != null && endDate != "")
            {
                try
                {
                    sDate = UserDateTime.ConvertToAppDateTime(startDate);
                    eDate = UserDateTime.ConvertToAppDateTime(endDate);
                }
                catch (Exception ex) { }
            }
            if (teamId == null)
                teamId = 0;

            if (branchId == null)
                branchId = 0;
            if (departmentId == null)
                departmentId = 0;

            if (accessTypeId == null)
                accessTypeId = 0;
            if (statusId == null)
                statusId = 0;

            ViewBag.RequestId = requestId;
            ViewBag.TeamId = teamId;
            ViewBag.AccessTypeId = accessTypeId;
            ViewBag.BranchId = branchId;
            ViewBag.DepartmentId = departmentId;
            ViewBag.StatusId = statusId;
            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");

            var items = _service.GetTikcets(branchId, departmentId, accessTypeId, statusId, sDate, eDate.AddDays(1), requestId,
                  "Team,Branch,Department,CreatedUser,RequestedUser", teamId: teamId).OrderByDescending(o => o.RequestedDate).ToList();


            return View(items);

        }


        // get create page
        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {
            var item = new RemoteAccessRequestVM();
            ApplyDefaultDates(item);


            return View(item);
        }




        // create new remote access request
        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(RemoteAccessRequestVM item)
        {
            ApplyDefaultDates(item);

            //validation
           
            if (item.SelectedDepartmentId == 0 || string.IsNullOrWhiteSpace(item.LAN_ID) || item.BranchId == 0)
            {
                TempData["ErrorMessage"] = "Please Fill Requester's Details.";
                return View(item);
            }
            if (item.LAN_ID.Equals(item.ApprovalBy))
            {
                TempData["ErrorMessage"] = "Approval user cannot be same as requester.";
                return View(item);
            }
            if (_departmentService.GetItem(item.SelectedDepartmentId, "") == null)
            {
                TempData["ErrorMessage"] = "No Such Department Exists.";
                return View(item);
            }
            if (_branchService.GetItem(item.BranchId, "") == null)
            {
                TempData["ErrorMessage"] = "No Such Branch Exists.";
                return View(item);
            }

            //check if chosen department has tolerance level then can proceed
            var SelectedDep = _departmentService.GetAsNoTrackingItem(item.SelectedDepartmentId, "");

            if (SelectedDep.ToleranceRate <= 0)
            {
                TempData["ErrorMessage"] = "The Chosen Department has no tolerance level, hence request cannot be made";
                return View(item);
            }

            if (string.IsNullOrWhiteSpace(item.BusinessJustification) || string.IsNullOrWhiteSpace(item.BusinessImpact))
            {
                TempData["ErrorMessage"] = "Please Enter Business justification/Impact.";
                return View(item);
            }

            if (!Enum.IsDefined(typeof(PlannedSheduleEnum), item.PlannedSchedule))
            {
                TempData["ErrorMessage"] = "Please Enter a planned schedule.";
                return View(item);
            }

            if (string.IsNullOrWhiteSpace(item.RemoteWorkLocation))
            {
                TempData["ErrorMessage"] = "Please Enter Address of Remote Location .";
                return View(item);
            }


            if (!Enum.IsDefined(typeof(RemoteAccessTypeEnum), item.RemoteAccessType))
            {
                TempData["ErrorMessage"] = "Please Choose an access type.";
                return View(item);
            }
            if (item.SelectedSystems == null || !item.SelectedSystems.Any())
            {
                TempData["ErrorMessage"] = "At least one application/system must be added";
                return View(item);
            }
            if (item.AssetId == 0)
            {
                TempData["ErrorMessage"] = "Please enter Asset Number";
                return View(item);
            }
            if (item.EndDate <= item.StartDate)
            {
                TempData["ErrorMessage"] = "End date must be greater than start date";
                return View(item);
            }
            var maxEndDate = item.StartDate.AddMonths(6);

            if (item.EndDate > maxEndDate)
            {
                TempData["ErrorMessage"] = "Remote access period cannot exceed 6 months";
                return View(item);
            }
            if (string.IsNullOrWhiteSpace(item.ApprovalBy))
            {
                TempData["ErrorMessage"] = "Please choose an Approver";
                return View(item);
            }

            // validate general commitemnts section and the overseas section and some annexure location details
            if (!Enum.IsDefined(typeof(LocationTypeEnum), item.LocationType))
            {
                TempData["ErrorMessage"] = "Please Choose an location type.";
                return View(item);
            }
            if (!Enum.IsDefined(typeof(AssignedTypeEnum), item.LocationStatus))
            {
                TempData["ErrorMessage"] = "Please specify if locaion is permanent or temporary type.";
                return View(item);
            }
            if (string.IsNullOrWhiteSpace(item.CurrentlyResidingAddress))
            {
                TempData["ErrorMessage"] = "Please Enter the currently residing address.";
                return View(item);
            }

            if (item.WillTravelOverseas)
            {
                if (item.DestinationCountry.IsNullOrWhiteSpace() || item.DurationOfTravel.IsNullOrWhiteSpace() || item.ReasonForTtravel.IsNullOrWhiteSpace() || item.OverseasConnectivityConfirm == false)
                {
                    TempData["ErrorMessage"] = "Please complete the Overseas Travel section if travelling overseas";
                    return View(item);
                }
            }

            if (item.AdherenceToInfoSecConfidentiality == false || item.Notifysupervisorofchanges == false || item.Preserveconfidentiality == false)
            {
                TempData["ErrorMessage"] = "Please fill the commitments section";
                return View(item);
            }


            //fill request details
            var request = new RemoteAccessRequest
            {
                DepartmentId = item.SelectedDepartmentId,
                BranchId = item.BranchId,
                CreatedBy = Session["UserId"].ToString(),
                RequestedBy = item.LAN_ID,
                RequestedDate = UserDateTime.GetUserDate(),
                FinalApprovalBy = item.ApprovalBy,
                BusinessImpact = item.BusinessImpact,
                BusinessJustification = item.BusinessJustification,
                PlannedSchedule = item.PlannedSchedule,
                AssetId = item.AssetId,
                RemoteWorkLocation = item.RemoteWorkLocation,
                RemoteAccessType = item.RemoteAccessType,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                BelowAMJustification = null,
                ToleranceLimitExceededJustification = null,
                HRClarification = null,
                ReasonForHRClarification = null,
                IsClarificationNeeded = false,
                Status = AccessRequestStatusEnum.Initiated,
                TeamId = null,
                Resolve = 0,
                SpentTime = 0,
                SpentTimeStatus = 0,
                SpentTimeSync = UserDateTime.GetUserDate(),
                IsTimeViolated = false,
                UpdatedDate = UserDateTime.GetUserDate(),
                UpdatedBy = Session["UserId"].ToString(),
                IsDeleted = false,
                IsAddedToGroup = false,
                IsCertificateProvided = false,
                IsWifiAccessAllow = false,
                ClarificationRequestedByTeamId = null,
                ClarificationAssignedToTeamId = null,
                ClarificationType =null
            };

            // if tolerance for dept exists then check if exceed and flag it else no flag
            int activeRemoteCount = _service.GetCompletedRemoteAccessCountByDepartment(request.DepartmentId);
            if (activeRemoteCount >= SelectedDep.ToleranceRate)
            {
                request.IsToleranceLimitExceeded = true;
            }
            else
            {
                request.IsToleranceLimitExceeded = false;
            }

            // check if requester designation AVP or VP if so skip supervisor page
            string pattern = @"\b(assistant\s*vice\s*president|asst\.?\s*vp|avp|vp|vice\s*president)\b";

            bool isMatch = Regex.IsMatch(item.Designation.ToLower().Trim(), pattern, RegexOptions.IgnoreCase);

            if (isMatch)
            {
                request.ApprovalStage = RemoteAccessApprovalStageEnum.VPOrAVPApprovalStage;
                request.IsRequesterAVPOrVP = true;
                request.ApprovedAVPOrVP = item.ApprovalBy;
                request.ApprovedSupervisor = null;
            }
            else
            {
                request.ApprovalStage = RemoteAccessApprovalStageEnum.SupervisorApprovalStage;
                request.IsRequesterAVPOrVP = false;
                request.ApprovedAVPOrVP = null;
                request.ApprovedSupervisor = item.ApprovalBy;
            }

            // save the request and get back the id
            //var SavedRemoteAccessRequestId = _service.Insert(request);
            //if (SavedRemoteAccessRequestId <= 0)
            //{
            //    Console.Write("failed to add request");
            //    return View(item);
            //}
            //
            //request.RemoteAccessRequiredSystems

            Collection<RemoteAccessRequiredSystem> systems = new Collection<RemoteAccessRequiredSystem>();
            foreach (var system in item.SelectedSystems)
            {
                var link = new RemoteAccessRequiredSystem
                {
                    AssetTypeId = system.AssetTypeId,
                    UpdatedDate = UserDateTime.GetUserDate(),
                    IsDeleted = false

                };

                systems.Add(link);

            }
            request.RemoteAccessRequiredSystems = systems;
            if (_service.Insert(request))
            {

                if (!_service.InsertUpdateStatus(request.RemoteAccessRequestId, null, AccessRequestStatusEnum.Initiated, "New Request has been initiated", 0, false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
                {
                    TempData["ErrorMessage"] = "Could not add new remote access update status record";
                    return View(item);
                }
                if (!_service.InsertLog(request.RemoteAccessRequestId, AccessRequestStatusEnum.Initiated, "New Remote Access request has been initiated.", false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
                {
                    TempData["ErrorMessage"] = "Could not add new remote access log record";
                    return View(item);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Could not add new request, something went wrong";
                return View(item);
            }



            // fill annexure data
            var annexureOne = new RemoteAgreement
            {
                RemoteAccessRequestId = request.RemoteAccessRequestId,
                LocationType = item.LocationType,
                CurrentlyResidingAddress = item.CurrentlyResidingAddress,
                LocationStatus = item.LocationStatus,
                IsNoiseFree = item.IsNoiseFree,

                WillTravelOverseas = item.WillTravelOverseas,
                DestinationCountry = item.DestinationCountry,
                DurationOfTravel = item.DurationOfTravel,
                ReasonForTtravel = item.ReasonForTtravel,
                OverseasConnectivityConfirm = item.OverseasConnectivityConfirm,

                IsFreeFromHazards = item.IsFreeFromHazards,
                IsLightingVentilation = item.IsLightingVentilation,
                IsErgonomicSeating = item.IsErgonomicSeating,
                IsLocationSecure = item.IsLocationSecure,
                IsFireSafety = item.IsFireSafety,

                IsConfidentialDiscussion = item.IsConfidentialDiscussion,
                IsSeperateFromFamilySpace = item.IsSeperateFromFamilySpace,
                IsDocStoredSecurely = item.IsDocStoredSecurely,
                IsCabinetAvailable = item.IsCabinetAvailable,
                IsUnauthorizedVisibility = item.IsUnauthorizedVisibility,
                IsUseBankApprovedDevice = item.IsUseBankApprovedDevice,
                IsUseBankApprovedVPN = item.IsUseBankApprovedVPN,
                IsSecureWifiNetwork = item.IsSecureWifiNetwork,
                IsAntivirusProtected = item.IsAntivirusProtected,
                IsPublicWifiUse = item.IsPublicWifiUse,

                IsInternetConnStable = item.IsInternetConnStable,
                IsBackupPower = item.IsBackupPower,

                IsOfficeMaterialUsed = item.IsOfficeMaterialUsed,
                IsLockAndKeyStorage = item.IsLockAndKeyStorage,
                IsShared = item.IsShared,
                IsReturnedPromptly = item.IsReturnedPromptly,

                AdherenceToInfoSecConfidentiality = item.AdherenceToInfoSecConfidentiality,
                Notifysupervisorofchanges = item.Notifysupervisorofchanges,
                Preserveconfidentiality = item.Preserveconfidentiality,

                SignedByEmployer = null,
                DateOfSigning = UserDateTime.GetUserDate(), // store now time fr temporary
                CreatedDate = UserDateTime.GetUserDate(),
                UpdatedBy = Session["UserId"].ToString(),
                IsDeleted = false,
                UpdatedDate = UserDateTime.GetUserDate(),

            };
            var status_3 = _remoteaggrementservice.Insert(annexureOne);
            if (!status_3)
            {
                return View(item);
            }

            // send ticket initiation email, if normal flow send first to supervisor else jump to avp/vp
            var LatestSavedRequest = _service.GetAsNoTrackingItem(request.RemoteAccessRequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser");

            try
            {
                EmailTemplate emailTemplate = new EmailTemplate();
                EmailClient emailClient = new EmailClient();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();

                //if requester is avp/vp TO: approval avp/ vp
                if (isMatch)
                {
                    ToRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);

                }
                // TO: immediate supervisor 
                else
                {
                    ToRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                }

                // CC: initiator
                CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);

                String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New Remote Work Request - " + LatestSavedRequest.RemoteAccessRequestId + ": " + LatestSavedRequest.RequestedUser.FullName,
                            emailTemplate.RemoteAcessInitiated(LatestSavedRequest)).Trim();
            }
            catch (Exception ex) { }


            TempData["SuccessMessage"] = "Request has been successfully created.";
            return RedirectToAction("Create");


        }

        private void ApplyDefaultDates(RemoteAccessRequestVM item)
        {
            if (item.StartDate == default)
                item.StartDate = UserDateTime.GetUserDate().Date;

            if (item.EndDate == default)
                item.EndDate = item.StartDate;

            item.MaxEndDate = item.StartDate.AddMonths(6);
        }

  
        [HttpGet]
        [AccessLogin]
        [EncryptedActionParameter]
        public ActionResult Info(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = _service.GetItem(id, "Department,RequestedUser,CreatedUser,FinalApprovalByUser,Asset,Team," +
                "RemoteAccessRequestUpdates," +
                "RemoteAccessRequestUpdates.UpdatedUser," + "RemoteAccessRequestUpdates.Team," + "RemoteAccessRequestLogs," +
                "RemoteAccessRequiredSystems," +
                "RemoteAccessRequiredSystems.AssetType,"+
                "RemoteAgreements"
                );


            if (item == null)
            {
                return HttpNotFound();
            }

            var ClarificationUpdates = _service.GetClarificationUpdates(id, "");

            //var latestRequest = ClarificationUpdates
            //    .LastOrDefault(u => u.Status == AccessRequestStatusEnum.ClarificationRequested);

            //// matching reply AFTER the request
            //RemoteAccessRequestUpdate latestReply = null;

            //if (latestRequest != null)
            //{
            //    latestReply = ClarificationUpdates
            //        .FirstOrDefault(u =>
            //            u.Status == AccessRequestStatusEnum.ClarificationProvided &&
            //            u.UpdatedDate > latestRequest.UpdatedDate);
            //}

            //ViewBag.LatestClarificationRequest = latestRequest;
            //ViewBag.LatestClarificationReply = latestReply;

            var hrRequest = ClarificationUpdates
            .Where(x =>
                x.Status == AccessRequestStatusEnum.ClarificationRequested &&
                x.ClarificationRequestedByTeamId == _teamService.GetTeamIdHR())
            .OrderByDescending(x => x.UpdatedDate)
            .FirstOrDefault();

            RemoteAccessRequestUpdate hrReply = null;

            if (hrRequest != null)
            {
                hrReply = ClarificationUpdates
                    .Where(x =>
                        x.Status == AccessRequestStatusEnum.ClarificationProvided &&
                        x.UpdatedDate > hrRequest.UpdatedDate)
                    .OrderBy(x => x.UpdatedDate)
                    .FirstOrDefault();
            }

            var itsvpRequest = ClarificationUpdates
            .Where(x =>
                x.Status == AccessRequestStatusEnum.ClarificationRequested &&
                x.ClarificationRequestedByTeamId == _teamService.GetTeamIdITSVP())
            .OrderByDescending(x => x.UpdatedDate)
            .FirstOrDefault();

            RemoteAccessRequestUpdate itsvpReply = null;

            if (itsvpRequest != null)
            {
                itsvpReply = ClarificationUpdates
                    .Where(x =>
                        x.Status == AccessRequestStatusEnum.ClarificationProvided &&
                        x.UpdatedDate > itsvpRequest.UpdatedDate)
                    .OrderBy(x => x.UpdatedDate)
                    .FirstOrDefault();
            }


            var dceoRequest = ClarificationUpdates
            .Where(x =>
                x.Status == AccessRequestStatusEnum.ClarificationRequested &&
                x.ClarificationRequestedByTeamId == _teamService.GetTeamIdDCEO())
            .OrderByDescending(x => x.UpdatedDate)
            .FirstOrDefault();


            RemoteAccessRequestUpdate dceoReply = null;

            if (dceoRequest != null)
            {
                dceoReply = ClarificationUpdates
                    .Where(x =>
                        x.Status == AccessRequestStatusEnum.ClarificationProvided &&
                        x.UpdatedDate > dceoRequest.UpdatedDate)
                    .OrderBy(x => x.UpdatedDate)
                    .FirstOrDefault();
            }

            ViewBag.HRRequest = hrRequest;
            ViewBag.HRReply = hrReply;

            ViewBag.ITSVPRequest = itsvpRequest;
            ViewBag.ITSVPReply = itsvpReply;

            ViewBag.DCEORequest = dceoRequest;
            ViewBag.DCEOReply = dceoReply;

            ViewBag.WorkflowTeams = new List<long>
            {
                _teamService.GetTeamIdRisk(),
                _teamService.GetTeamIdHR(),
                _teamService.GetTeamIdITSecHead(),
                _teamService.GetTeamIdITSVP(),
                _teamService.GetTeamIdDCEO(),
                _teamService.GetTeamIdITRemoteNet(),
                _teamService.GetTeamIdITSec(),
                TeamService.GetTeamIdHelpDesk()
            };
            ViewBag.RiskTeamId = _teamService.GetTeamIdRisk();
            ViewBag.HRTeamId = _teamService.GetTeamIdHR();

            return View(item);

        }


        #region requester

        // method for showing my requests
        [HttpGet]
        [AccessAuthorize]
        public ActionResult MyRequest()
        {
            var items = _service.GetItemsMy(Session["UserId"].ToString(),
                        "Department,RemoteAccessRequestUpdates").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }


            return View(items);
        }



        // method for reject a request submmitted by requester
        [AccessAuthorize]
        [HttpPost]
        public ActionResult MyRequestReject(long RequestId)
        {
            int ErrorCode = 0;

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            //checking if suitable to reject
            if (Request.Status == AccessRequestStatusEnum.Initiated)
            {
                // getting last updated date
                var LastUpdated = Request.UpdatedDate;

                //setting new updated date
                var NewUpdatedDate = UserDateTime.GetUserDate();

                // updating fields in the request
                Request.Status = AccessRequestStatusEnum.Rejected;


                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

                Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
                Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
                Request.SpentTimeSync = NewUpdatedDate;

                Request.UpdatedDate = NewUpdatedDate;
                Request.UpdatedBy = NewUpdatedBy;
                Request.ApprovalStage = RemoteAccessApprovalStageEnum.RejectedStage;

           
                if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, null, AccessRequestStatusEnum.Rejected, "Request was Cancelled By Requester.", Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
                {
                    
                    if (!_service.Update(Request))
                    {
                        ErrorCode = 2;
                        return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                    }

                     
                    if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Rejected, "Request was Cancelled By Requester.", false, NewUpdatedBy, NewUpdatedDate))
                    {
                        ErrorCode = 2;
                        return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                ErrorCode = 1;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult MyRequestReplyClarification(long RequestId, string Reply)
        {
            return ReplyClarification(RequestId, Reply);
        }


        #endregion



        #region supervisor Approve

        // method  to show all requests pending approval for supervisor
        [HttpGet]
        [AccessAuthorize]
        public ActionResult SupervisorApprovalPending()
        {
            try
            {
                if (TempData["ActiveApproved"].ToString() == "active")
                    TempData["ActivePending"] = "";
                else
                    TempData["ActivePending"] = "active";
            }
            catch (Exception ex)
            {
                TempData["ActivePending"] = "active";
            }
            var items = _service.GetItemsSupervisorApprovals(Session["UserId"].ToString(),
                "Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }

            var clarificationRequests = _service.GetAll("Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser")
                                       .Where(r => r.IsClarificationNeeded == true && r.ApprovedSupervisor == Session["UserId"].ToString() && r.ClarificationType == ClarificationType.Supervisor)
                                       .OrderByDescending(o => o.RequestedDate).ToList();

            ViewBag.clarificationRequests = clarificationRequests;
            return View(items);
        }

        // method for the approval logic 
        [AccessAuthorize]
        [HttpPost]
        public ActionResult SupervisorApprove(long RequestId, int ApprovalStage, string ApprovalById, string Comment, string Justification)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }
            //if (Justification == " ")
            //{
            //    ErrorCode = 4;
            //    return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            //}
            if (ApprovalById.IsNullOrWhiteSpace())
            {
                ErrorCode = 5;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }
            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

         

            // updating request related fields
            Request.Status = AccessRequestStatusEnum.Pending;
            Request.BelowAMJustification = Justification;
            Request.ApprovalStage = RemoteAccessApprovalStageEnum.VPOrAVPApprovalStage;
            Request.FinalApprovalBy = ApprovalById;       //next approver is set here
            Request.ApprovedAVPOrVP = ApprovalById;

          


            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, null, AccessRequestStatusEnum.Pending, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
                
                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Pending, "Approved By Supervisor.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                //send email when supervisor approve for a normal requester
                var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser");

                try
                {
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: line avp / vp
                    ToRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);

                    // CC: initiator, immediate supervisor
                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Supervisor Approval - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteSupervisorApprovalRejection(LatestSavedRequest, Comment, NewUpdatedDate, false)).Trim();
                }
                catch (Exception ex) { }

            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }


            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);

        }


        // method for the supervisor rejection logic 
        [AccessAuthorize]
        [HttpPost]
        public ActionResult SupervisorReject(long RequestId, string Comment, long? teamId)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating fields in the request
            Request.FinalApprovalBy = NewUpdatedBy;
            Request.Status = AccessRequestStatusEnum.Rejected;
            Request.TeamId = teamId;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;
            Request.ApprovalStage = RemoteAccessApprovalStageEnum.RejectedStage; 

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, teamId, AccessRequestStatusEnum.Rejected, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId,AccessRequestStatusEnum.Rejected, "Request is Rejected.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                //send email when supervisor reject for a normal requester
                var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser");

                try
                {
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: initiator as no avp is chosen when rejecting
                    ToRecipients.Add(LatestSavedRequest.RequestedUser.Email);


                    // CC: immediate supervisor
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Supervisor Rejection - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteSupervisorApprovalRejection(LatestSavedRequest, Comment, NewUpdatedDate, true)).Trim();
                }
                catch (Exception ex) { }

            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);



        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult SupervisorReplyClarification(long RequestId, string Reply)
        {
            return ReplyClarification(RequestId, Reply);

        }


        #endregion

        #region avp/vp approve

        // method to show request pending approval for avp/vp
        [HttpGet]
        [AccessAuthorize]
        public ActionResult VPAVPApprovalPending()
        {
            try
            {
                if (TempData["ActiveApproved"].ToString() == "active")
                    TempData["ActivePending"] = "";
                else
                    TempData["ActivePending"] = "active";
            }
            catch (Exception ex)
            {
                TempData["ActivePending"] = "active";
            }
            var items = _service.GetItemsVPAVPApprovals(Session["UserId"].ToString(),
                "Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }


            var clarificationRequests = _service.GetAll("Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser")
                                        .Where(r => r.IsClarificationNeeded == true && r.ApprovedAVPOrVP == Session["UserId"].ToString() && r.ClarificationType == ClarificationType.AVPOrVP)
                                        .OrderByDescending(o => o.RequestedDate).ToList();

            ViewBag.clarificationRequests = clarificationRequests;
            return View(items);
        }

        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetDepartmentTolerance(long requestId)
        {
            //getting the request's department tolerance level
            var request = _service.GetAsNoTrackingItem(requestId, "Department");
            var TOLERANCE_LIMIT = request.Department.ToleranceRate;

            //getting the current number of active remote access users from the dept where new request has come from
            int activeRemoteCount = _service.GetCompletedRemoteAccessCountByDepartment(request.DepartmentId);
            return Json(new
            {
                currentCount = activeRemoteCount,
                threshold = TOLERANCE_LIMIT,
                isExceeded = activeRemoteCount >= TOLERANCE_LIMIT
            }, JsonRequestBehavior.AllowGet);
        }

        // method for the approval logic for avp/vp
        [AccessAuthorize]
        [HttpPost]
        public ActionResult VPAVPApprove(long RequestId, long? ToleranceLimit, bool? IsToleranceLimitAccepted, int ApprovalStage, string ApprovalById, string Comment, string Justification)
        {
            int ErrorCode = 0;


            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

        
            // updating request related fields ,approver and status remain same
            Request.ApprovalStage = RemoteAccessApprovalStageEnum.TeamApprovalStage;

            string updateComment;

            //if tolerance exceeded them move to risk else HR               

            if (Request.IsToleranceLimitExceeded)
            {                               // but if not exceed then go to HR
                if (Justification.IsNullOrWhiteSpace())
                {
                    ErrorCode = 4;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);

                }
                Request.TeamId = _teamService.GetTeamIdRisk();
                Request.ToleranceLimitExceededJustification = Justification;
                updateComment = "Justification Provided For limit exceedance";
            }
            else
            {
                if (Comment.IsNullOrWhiteSpace())
                {
                    ErrorCode = 3;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);

                }
                updateComment = Comment;
                Request.TeamId = _teamService.GetTeamIdHR();
                Request.ToleranceLimitExceededJustification = null;
            }        


            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, null, AccessRequestStatusEnum.Pending, updateComment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Pending, "Approved By VP / AVP.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
                var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                //sending email for tolerance exceeded
                if (LatestSavedRequest.IsToleranceLimitExceeded)
                {

                    try
                    {
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();

                        // TO: Risk team - no mail 
                        ToRecipients.Add(_service.GetRiskTeamEmail());

                        // CC: initiator, immediate supervisor, Line AVP/VP
                        CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                        CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                        CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);

                        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Justification for Exceeding Risk Tolerance - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                    emailTemplate.RemoteavpvpToleranceExceeded(LatestSavedRequest)).Trim();
                    }
                    catch (Exception ex) { }
                }
                else
                {
                    //sending email for approve from line avp/vp

                    try
                    {
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();

                        // TO: HR Work Administrator - no mail
                        ToRecipients.Add(_service.GetHRTeamEmail());

                        // CC: initiator, immediate supervisor, Line AVP/VP
                        CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                        CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                        CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);

                        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Line AVP/VP Approval - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                    emailTemplate.RemoteavpvpApprovalRejection(LatestSavedRequest, updateComment, NewUpdatedDate, false)).Trim();
                    }
                    catch (Exception ex) { }
                }


            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }


            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);

        }


        // method for the avp/vp rejection logic 
        [AccessAuthorize]
        [HttpPost]
        public ActionResult VPAVPReject(long RequestId, string Comment, long? teamId)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating fields in the request
            Request.FinalApprovalBy = NewUpdatedBy;
            Request.Status = AccessRequestStatusEnum.Rejected;
            Request.TeamId = teamId;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;
            Request.ApprovalStage = RemoteAccessApprovalStageEnum.RejectedStage;

     
             if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, teamId, AccessRequestStatusEnum.Rejected, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Rejected, "Request is Rejected.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                //sending email for rejection from line avp/vp

                try
                {
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: HR Work Administrator - no mail
                    ToRecipients.Add(_service.GetHRTeamEmail());

                    // CC: initiator, immediate supervisor, Line AVP/VP
                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Line AVP/VP Rejection - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteavpvpApprovalRejection(LatestSavedRequest, Comment, NewUpdatedDate, true)).Trim();
                }
                catch (Exception ex) { }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        [AccessAuthorize]
        public ActionResult VPAVPReplyClarification(long RequestId, string Reply)
        {
            return ReplyClarification(RequestId, Reply);

        }

        #endregion

        #region risk team

        [AccessAuthorize]
        [HttpGet]
        public ActionResult RiskApprovalPending()
        {
            try
            {
                if (TempData["ActiveApproved"].ToString() == "active")
                    TempData["ActivePending"] = "";
                else
                    TempData["ActivePending"] = "active";
            }
            catch (Exception ex)
            {
                TempData["ActivePending"] = "active";
            }
            var items = _service.GetItemsTeamApprovals(_teamService.GetTeamIdRisk(),
               "Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }

            var clarificationRequests = _service.GetAll("Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser")
                                        .Where(r => r.IsClarificationNeeded == true && r.ClarificationAssignedToTeamId == _teamService.GetTeamIdRisk() && r.ClarificationType == ClarificationType.Team)
                                        .OrderByDescending(o => o.RequestedDate).ToList();

            ViewBag.clarificationRequests = clarificationRequests;

            return View(items);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult RiskApprove(long RequestId, int ApprovalStage,
            string Comment)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

         
            // updating request related fields
            Request.FinalApprovalBy = NewUpdatedBy;
            Request.TeamId = _teamService.GetTeamIdHR();


            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, _teamService.GetTeamIdRisk(), AccessRequestStatusEnum.Pending, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Pending, "Approved By Risk Team.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                //send approve email from risk team
                try
                {
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: HR Work Administrator - no mail
                    ToRecipients.Add(_service.GetHRTeamEmail());

                    // CC: initiator, immediate supervisor, Line AVP/VP, risk team
                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                    CCRecipients.Add(_service.GetRiskTeamEmail());

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Risk Review Decision - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteRiskTeamApprovalRejection(LatestSavedRequest, Comment, false)).Trim();
                }
                catch (Exception ex) { }

            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult RiskReject(long RequestId, string Comment)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating fields in the request
            Request.FinalApprovalBy = NewUpdatedBy;
            Request.Status = AccessRequestStatusEnum.Rejected;
            Request.TeamId = _teamService.GetTeamIdRisk();

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;
            Request.ApprovalStage = RemoteAccessApprovalStageEnum.RejectedStage;

      
            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, _teamService.GetTeamIdRisk(), AccessRequestStatusEnum.Rejected, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Rejected, "Request is Rejected.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                // send email for risk team rejection
                try
                {
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: HR Work Administrator - no mail
                    ToRecipients.Add(_service.GetHRTeamEmail());

                    // CC: initiator, immediate supervisor, Line AVP/VP, risk team
                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                    CCRecipients.Add(_service.GetRiskTeamEmail());

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Risk Review Decision - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteRiskTeamApprovalRejection(LatestSavedRequest,Comment, true)).Trim();
                }
                catch (Exception ex) { }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }




        [HttpPost]
        [AccessAuthorize]
        public ActionResult RiskReplyClarification(long RequestId, string Reply)
        {
            return ReplyClarification(RequestId, Reply);

        }

        #endregion

        #region HR Remote Work admin

        [AccessAuthorize]
        [HttpGet]
        public ActionResult HRApprovalPending()
        {
            try
            {
                if (TempData["ActiveApproved"].ToString() == "active")
                    TempData["ActivePending"] = "";
                else
                    TempData["ActivePending"] = "active";
            }
            catch (Exception ex)
            {
                TempData["ActivePending"] = "active";
            }
            var items = _service.GetItemsTeamApprovals(_teamService.GetTeamIdHR(),
               "Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }


            var clarificationRequests = _service.GetAll("Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser")
                                           .Where(r => r.IsClarificationNeeded == true && r.ClarificationAssignedToTeamId == _teamService.GetTeamIdHR() && r.ClarificationType == ClarificationType.Team)
                                           .OrderByDescending(o => o.RequestedDate).ToList();

            ViewBag.clarificationRequests = clarificationRequests;

            return View(items);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult HRApprove(long RequestId, int ApprovalStage,
            string Comment, long? DaysOfWeek, string NoticePeriod)
        {
            int ErrorCode = 0;
            // anexure two validation
            if (Comment.IsNullOrWhiteSpace() || NoticePeriod.IsNullOrWhiteSpace() || !DaysOfWeek.HasValue)
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }
            if (DaysOfWeek.Value < 1 || DaysOfWeek.Value > 7)
            {
                ErrorCode = 10;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }
            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            Request.FinalApprovalBy = NewUpdatedBy;
            Request.TeamId = _teamService.GetTeamIdITSecHead();


            //check if hr clarification is needed and given if  then allow to proceed
            //if (_service.HasOpenClarification(RequestId, ""))
            //{
            //    ErrorCode = 4;
            //    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            //}

            //update annexure two
            var agreement = _remoteaggrementservice.GetAgreementForRequest(RequestId);
            agreement.RemoteAccessRequestId = RequestId;
            agreement.SignedByEmployerId = NewUpdatedBy;
            agreement.DateOfSigning = NewUpdatedDate;
            agreement.UpdatedBy = NewUpdatedBy;
            agreement.UpdatedDate = NewUpdatedDate;
            agreement.DaysOfWeek = DaysOfWeek;
            agreement.NoticePeriod = NoticePeriod;

            if (!_remoteaggrementservice.Update(agreement))
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, _teamService.GetTeamIdHR(), AccessRequestStatusEnum.Pending, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Pending, "Approved by HR Remotework admin", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }


                var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");


                //if reccommended by HR
                try
                {

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: head of IT security
                    ToRecipients.Add(_service.GetITSecHeadEmail());

                    // CC: Initiator, immediate supervisor, Line AVP/VP, Risk Department
                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                    CCRecipients.Add(_service.GetRiskTeamEmail());

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "HR Recommendation - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteHRRecommendation(LatestSavedRequest, Comment)).Trim();
                }
                catch (Exception ex) { }



            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }
       
            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }


        [HttpPost]
        [AccessAuthorize]
        public ActionResult HRReplyClarification(long RequestId, string Reply)
        {
            return ReplyClarification(RequestId, Reply);

        }


        [HttpPost]
        [AccessAuthorize]
        public ActionResult HRRaiseClarification(long RequestId, string Reason, long? ClarificationRequestedByTeamId, long? ClarificationAssignedToTeamId, ClarificationType clarificationType)
        {
            return RaiseClarification(RequestId,Reason,ClarificationRequestedByTeamId,ClarificationAssignedToTeamId,clarificationType);
        }

        #endregion

        #region IT Sec Head

        [AccessAuthorize]
        [HttpGet]
        public ActionResult ITSecHeadApprovalPending()
        {
            try
            {
                if (TempData["ActiveApproved"].ToString() == "active")
                    TempData["ActivePending"] = "";
                else
                    TempData["ActivePending"] = "active";
            }
            catch (Exception ex)
            {
                TempData["ActivePending"] = "active";
            }
            var items = _service.GetItemsTeamApprovals(_teamService.GetTeamIdITSecHead(),
               "Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }

            var clarificationRequests = _service.GetAll("Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser")
                                           .Where(r => r.IsClarificationNeeded == true && r.ClarificationAssignedToTeamId == _teamService.GetTeamIdITSecHead() && r.ClarificationType == ClarificationType.Team)
                                           .OrderByDescending(o => o.RequestedDate).ToList();

            ViewBag.clarificationRequests = clarificationRequests;

            return View(items);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult ITSecHeadApprove(long RequestId, int ApprovalStage,
            string Comment)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;      

            // updating request related fields
            Request.FinalApprovalBy = NewUpdatedBy;
            Request.TeamId = _teamService.GetTeamIdITSVP();       


            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, _teamService.GetTeamIdITSecHead(), AccessRequestStatusEnum.Pending, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Pending, "Approved By IT sec head Team.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                // send email when IT sec head approve
                try
                {
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: itsvp
                    ToRecipients.Add(_service.GetITSVPEmail());

                    // CC: Initiator, immediate supervisor, Line AVP/VP, HR team
                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                    CCRecipients.Add(_service.GetHRTeamEmail());

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, " IT Security Approval - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteITSecHeadApprovalRejection(LatestSavedRequest, Comment, NewUpdatedDate, false)).Trim();
                }
                catch (Exception ex) { }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult ITSecHeadReject(long RequestId, string Comment)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating fields in the request
            Request.FinalApprovalBy = NewUpdatedBy;
            Request.Status = AccessRequestStatusEnum.Rejected;
            Request.TeamId = _teamService.GetTeamIdITSecHead();

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;
            Request.ApprovalStage = RemoteAccessApprovalStageEnum.RejectedStage;

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, _teamService.GetTeamIdITSecHead(), AccessRequestStatusEnum.Rejected, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Rejected, "Request is Rejected.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                // send email if reject by IT sec head
                try
                {
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: itsvp
                    ToRecipients.Add(_service.GetITSVPEmail());

                    // CC: Initiator, immediate supervisor, Line AVP/VP, HR team
                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                    CCRecipients.Add(_service.GetHRTeamEmail());

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, " IT Security Rejection - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteITSecHeadApprovalRejection(LatestSavedRequest, Comment, NewUpdatedDate, true)).Trim();
                }
                catch (Exception ex) { }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult ITSecHeadReplyClarification(long RequestId, string Reply)
        {
            return ReplyClarification(RequestId, Reply);

        }

        #endregion


        #region IT SVP

        [HttpGet]
        [AccessAuthorize]
        public ActionResult ITSVPApprovalPending()
        {
            try
            {
                if (TempData["ActiveApproved"].ToString() == "active")
                    TempData["ActivePending"] = "";
                else
                    TempData["ActivePending"] = "active";
            }
            catch (Exception ex)
            {
                TempData["ActivePending"] = "active";
            }
            var items = _service.GetItemsTeamApprovals(_teamService.GetTeamIdITSVP(),
               "Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,"
               +"RemoteAccessRequestUpdates.UpdatedUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }

            var clarificationRequests = _service.GetAll("Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,"
                +"RemoteAccessRequestUpdates.UpdatedUser")
                                            .Where(r => r.IsClarificationNeeded == true && r.ClarificationAssignedToTeamId == _teamService.GetTeamIdITSVP() && r.ClarificationType == ClarificationType.Team)
                                            .OrderByDescending(o => o.RequestedDate).ToList();

            ViewBag.clarificationRequests = clarificationRequests;

            return View(items);
        }


        [HttpPost]
        [AccessAuthorize]        
        public ActionResult ITSVPApprove(long RequestId,
           string Comment, bool EscalateToDCEO)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;


            // updating request related fields
            Request.FinalApprovalBy = NewUpdatedBy;

            if (EscalateToDCEO)
            {
                Request.TeamId = _teamService.GetTeamIdDCEO();
            }
            else
            {
                Request.TeamId = _teamService.GetTeamIdITRemoteNet();
            }
            string logDescription;

            if (EscalateToDCEO)
            {
                logDescription = "Approved by IT SVP and escalated to DCEO.";
            }
            else
            {
                logDescription = "Approved by IT SVP.";
            }

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, _teamService.GetTeamIdITSVP(), AccessRequestStatusEnum.Pending, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Pending, logDescription, false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    if (EscalateToDCEO)
                    {
                        ToRecipients.Add(_service.GetDCEOEmail());
                    }
                    else
                    {
                        ToRecipients.Add(_service.GetNetworkImplementerTeamEmail());
                    }


                    // CC: initiator, immediate supervisor, Line AVP/VP
                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                    CCRecipients.Add(_service.GetITSVPEmail());

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "IT SVP Approval - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteITSVPApprovalRejection(LatestSavedRequest, false, EscalateToDCEO)).Trim();
                }
                catch (Exception ex) { }

            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult ITSVPReject(long RequestId, string Comment)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating fields in the request
            Request.FinalApprovalBy = NewUpdatedBy;
            Request.Status = AccessRequestStatusEnum.Rejected;
            Request.TeamId = _teamService.GetTeamIdITSVP();

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;
            Request.ApprovalStage = RemoteAccessApprovalStageEnum.RejectedStage;

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, _teamService.GetTeamIdITSVP(), AccessRequestStatusEnum.Rejected, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Rejected, "Request is Rejected.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: implementer network
                    ToRecipients.Add(_service.GetNetworkImplementerTeamEmail());

                    // CC: Initiator, immediate supervisor, Line AVP/VP, HR team
                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                    CCRecipients.Add(_service.GetITSVPEmail());

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, " IT SVP Rejection - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteITSVPApprovalRejection(LatestSavedRequest, true, false)).Trim();
                }
                catch (Exception ex) { }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult ITSVPRaiseClarification(long RequestId, string Reason, long? ClarificationRequestedByTeamId, long? ClarificationAssignedToTeamId, ClarificationType clarificationType)
        {
            return RaiseClarification(RequestId, Reason, ClarificationRequestedByTeamId, ClarificationAssignedToTeamId, clarificationType);

        }


        [HttpPost]
        [AccessAuthorize]
        public ActionResult ITSVPReplyClarification(long RequestId, string Reply)
        {
            return ReplyClarification(RequestId, Reply);

        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult ITSVPRouteClarification(long RequestId,long? ClarificationAssignedToTeamId, ClarificationType clarificationType)
        {
            return RouteClarification(RequestId, ClarificationAssignedToTeamId, clarificationType);
        }





        #endregion



        #region DCEO

        [HttpGet]
        [AccessAuthorize]
        public ActionResult DCEOApprovalPending()
        {
            try
            {
                if (TempData["ActiveApproved"].ToString() == "active")
                    TempData["ActivePending"] = "";
                else
                    TempData["ActivePending"] = "active";
            }
            catch (Exception ex)
            {
                TempData["ActivePending"] = "active";
            }
            var items = _service.GetItemsTeamApprovals(_teamService.GetTeamIdDCEO(),
               "Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }

            var clarificationRequests = _service.GetAll("Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser")
                                            .Where(r => r.IsClarificationNeeded == true && r.ClarificationAssignedToTeamId == _teamService.GetTeamIdDCEO() && r.ClarificationType == ClarificationType.Team)
                                            .OrderByDescending(o => o.RequestedDate).ToList();

            ViewBag.clarificationRequests = clarificationRequests;

            return View(items);
        }



        [HttpPost]
        [AccessAuthorize]
        public ActionResult DCEOApprove(long RequestId,
         string Comment)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;


            // updating request related fields
            Request.FinalApprovalBy = NewUpdatedBy;
            Request.TeamId = _teamService.GetTeamIdITRemoteNet();
         

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, _teamService.GetTeamIdDCEO(), AccessRequestStatusEnum.Pending, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Pending, "Approved by DCEO", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                 try
                {
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: HR Work Administrator - no mail
                    ToRecipients.Add(_service.GetNetworkImplementerTeamEmail());

                    // CC: initiator, immediate supervisor, Line AVP/VP, risk team
                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                    CCRecipients.Add(_service.GetITSVPEmail());

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "DCEO Approval - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteDCEOApprovalRejection(LatestSavedRequest, false)).Trim();
                }
                catch (Exception ex) { }

            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult DCEOReject(long RequestId, string Comment)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating fields in the request
            Request.FinalApprovalBy = NewUpdatedBy;
            Request.Status = AccessRequestStatusEnum.Rejected;
            Request.TeamId = _teamService.GetTeamIdDCEO();

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;
            Request.ApprovalStage = RemoteAccessApprovalStageEnum.RejectedStage;

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, _teamService.GetTeamIdDCEO(), AccessRequestStatusEnum.Rejected, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Rejected, "Request is Rejected.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: HR Work Administrator - no mail
                    ToRecipients.Add(_service.GetNetworkImplementerTeamEmail());

                    // CC: initiator, immediate supervisor, Line AVP/VP, risk team
                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                    CCRecipients.Add(_service.GetITSVPEmail());

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "DCEO Rejection - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteDCEOApprovalRejection(LatestSavedRequest, true)).Trim();
                }
                catch (Exception ex) { }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }



        [HttpPost]
        [AccessAuthorize]
        public ActionResult DCEORaiseClarification(long RequestId, string Reason, long? ClarificationRequestedByTeamId, long? ClarificationAssignedToTeamId, ClarificationType clarificationType)
        {
            return RaiseClarification(RequestId, Reason, ClarificationRequestedByTeamId, ClarificationAssignedToTeamId, clarificationType);

        }



        #endregion



        #region Implementer (IT Network,IT Sec)

        [HttpGet]
        [AccessAuthorize]
        public ActionResult ImplementerApprovalPending()
        {
            var userId = Session["UserId"].ToString();
            var teamIds = new List<long>()
            {
                _teamService.GetTeamIdITSec(),
                _teamService.GetTeamIdITRemoteNet()
            };

            var items = _service.GetITSecITNetRemoteAccessApproval(
                teamIds,
                "Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser"
            )
            .OrderByDescending(o => o.RequestedDate)
            .ToList();

            ViewBag.NetworkTeam = _teamService.GetTeamIdITRemoteNet();
            ViewBag.SecurityTeam = _teamService.GetTeamIdITSec();

            return View(items);
        }

        [HttpPost]
        [AccessAuthorize]
        public JsonResult UpdateImplementerCheckboxFields(long RequestId, string field, bool status)
        {
            var Request = _service.GetItem(RequestId, "");
            if (Request == null) return Json(new { success = false, message = "Not found" });

            switch (field)
            {
                case "AddToSelectedGroup":
                    Request.IsAddedToGroup = status;
                    break;
                case "AllowWifiAccess":
                    Request.IsWifiAccessAllow = status;
                    break;
                case "ProvideCertificate":
                    Request.IsCertificateProvided = status;
                    break;
                default:
                    return Json(new { success = false, message = "Invalid field" });
            }
            var status_1 = _service.Update(Request);
            if (!status_1)
            {
                return Json(new { success = false, message = "Could not update checkbox field" });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult ImplementerApprove(long RequestId, int ApprovalStage,
            string Comment )
        {


            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }
            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            string logDescription;
            long? updateTeamId;
            var currentApproverTeamId = Request.TeamId;

            if (currentApproverTeamId == _teamService.GetTeamIdITRemoteNet())
            {
                // check if added to selected group else block
                if (!Request.IsAddedToGroup)
                {
                    ErrorCode = 8;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                //check if GP and if so is wifi allowed by network
                if (Request.RemoteAccessType == RemoteAccessTypeEnum.GlobalProtect && !Request.IsWifiAccessAllow)
                {
                    ErrorCode = 8;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                // if citrix then send to helpdesk no need security
                if (Request.RemoteAccessType == RemoteAccessTypeEnum.Citrix)
                {

                    Request.TeamId = TeamService.GetTeamIdHelpDesk();
                }
                else
                {
                    Request.TeamId = _teamService.GetTeamIdITSec();
                 

                }
                // updating request related fields
                Request.FinalApprovalBy = NewUpdatedBy;

                // adding fields to upadate object and log object
                updateTeamId = currentApproverTeamId;

                logDescription = "Implemented By IT Network Team.";

            }
            else
            {
                //check if GP and if so is certificate provided by security
                if (Request.RemoteAccessType == RemoteAccessTypeEnum.GlobalProtect && !Request.IsCertificateProvided)
                {
                    ErrorCode = 8;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                // updating request related fields
                Request.FinalApprovalBy = NewUpdatedBy;
                Request.TeamId = TeamService.GetTeamIdHelpDesk();

                // adding fields to upadate object and log object
                updateTeamId = currentApproverTeamId;

                logDescription = "Implemented By IT Security Team.";

            }


            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, updateTeamId, AccessRequestStatusEnum.Pending, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Pending, logDescription, false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }


                var LatestSavedRequest =  _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");
                //sending email basd on the team id 
                //if currently approved person was from network team then 

                
                if (currentApproverTeamId == _teamService.GetTeamIdITRemoteNet())
                {
                    try
                    {

                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();

                        // if request was citrix send email to helpdesk
                        if (LatestSavedRequest.RemoteAccessType == RemoteAccessTypeEnum.Citrix)
                        {
                            // TO: implementer security
                            ToRecipients.Add(_service.GetHelpDeskEmail());
                        }
                        else
                        {
                            // TO: implementer security
                            ToRecipients.Add(_service.GetSecurityImplementerTeamEmail());

                        }


                        // CC: Initiator, immediate supervisor, Line AVP/VP, HR team,Implementer – Network 
                        CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                        CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                        CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                        CCRecipients.Add(_service.GetHRTeamEmail());
                        CCRecipients.Add(_service.GetNetworkImplementerTeamEmail());

                        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Remote Work Access Provisioning - " + LatestSavedRequest.RemoteAccessRequestId,
                                    emailTemplate.RemoteImplementerNetwork(LatestSavedRequest, Comment)).Trim();
                    }
                    catch (Exception ex) { }
                }
                else
                {
                    //send email to helpdesk
                    try
                    {

                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();

                        // TO: implementer network
                        ToRecipients.Add(_service.GetHelpDeskEmail());

                        // CC: Initiator, immediate supervisor, Line AVP/VP, HR team, Implementer - NW, Implementer - Sec
                        CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                        CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                        CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                        CCRecipients.Add(_service.GetHRTeamEmail());
                        CCRecipients.Add(_service.GetNetworkImplementerTeamEmail());
                        CCRecipients.Add(_service.GetSecurityImplementerTeamEmail());

                        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Certificate Issued – Remote Work Access " + LatestSavedRequest.RemoteAccessRequestId,
                                    emailTemplate.RemoteImplementerSecurity(LatestSavedRequest)).Trim();
                    }
                    catch (Exception ex) { }
                }

            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }


        [HttpPost]
        [AccessAuthorize]
        public ActionResult ImplementerReject(long RequestId, string Comment)
        {

            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }
            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating fields in the request
            Request.FinalApprovalBy = NewUpdatedBy;
            Request.Status = AccessRequestStatusEnum.Rejected;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;
            Request.ApprovalStage = RemoteAccessApprovalStageEnum.RejectedStage;         

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, Request.TeamId, AccessRequestStatusEnum.Rejected, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Rejected, "Request is Rejected.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult ImplementerRevoke(long RequestId)
        {
            int ErrorCode = 0;
            
            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating fields in the request
            Request.FinalApprovalBy = NewUpdatedBy;
            Request.Status = AccessRequestStatusEnum.Revoked;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;
            Request.ApprovalStage = RemoteAccessApprovalStageEnum.RevokedStage;

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, Request.TeamId, AccessRequestStatusEnum.Revoked, $"Request #{Request.RemoteAccessRequestId} has been revoked by implementer.", Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Revoked, $"Request #{Request.RemoteAccessRequestId} has been revoked by implementer.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);
        }
        

        #endregion

        #region Help desk

        [AccessAuthorize]
        [HttpGet]
        public ActionResult HelpDeskApprovalPending()
        {
            try
            {
                if (TempData["ActiveApproved"].ToString() == "active")
                    TempData["ActivePending"] = "";
                else
                    TempData["ActivePending"] = "active";
            }
            catch (Exception ex)
            {
                TempData["ActivePending"] = "active";
            }
            var items = _service.GetItemsTeamApprovals(TeamService.GetTeamIdHelpDesk(),
               "Department,RequestedUser,FinalApprovalByUser,RemoteAccessRequestUpdates,RemoteAccessRequestUpdates.UpdatedUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult HelpDeskApprove(long RequestId, int ApprovalStage,
            string Comment)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            // updating request related fields
            Request.FinalApprovalBy = NewUpdatedBy;
            Request.ApprovalStage = RemoteAccessApprovalStageEnum.CompletedStage;
            Request.TeamId = TeamService.GetTeamIdHelpDesk();
            Request.Status = AccessRequestStatusEnum.Completed;       

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, TeamService.GetTeamIdHelpDesk(), AccessRequestStatusEnum.Completed, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Completed, "Completed By HelpDesk.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                // send completion email to initiator
                try
                {
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: initiator
                    ToRecipients.Add(LatestSavedRequest.RequestedUser.Email);

                    // CC: Initiator, immediate supervisor, Line AVP/VP, HR team, Implementer - NW, Implementer - Sec, Helpdesk 
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                    CCRecipients.Add(_service.GetHRTeamEmail());
                    CCRecipients.Add(_service.GetNetworkImplementerTeamEmail());
                    CCRecipients.Add(_service.GetSecurityImplementerTeamEmail());
                    CCRecipients.Add(_service.GetHelpDeskEmail());

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Remote Work Request Closed - " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteHelpDeskCompletion(LatestSavedRequest)).Trim();
                }
                catch (Exception ex) { }

            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult HelpDeskReject(long RequestId, string Comment)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating fields in the request
            Request.FinalApprovalBy = NewUpdatedBy;
            Request.Status = AccessRequestStatusEnum.Rejected;
            Request.TeamId = TeamService.GetTeamIdHelpDesk();

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;
            Request.ApprovalStage = RemoteAccessApprovalStageEnum.RejectedStage;          

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, TeamService.GetTeamIdHelpDesk(), AccessRequestStatusEnum.Rejected, Comment, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.Rejected, "Request is Rejected.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }

        #endregion


        // method to insert user given clarification upon HR request

        [AccessAuthorize]
        [HttpPost]
        public ActionResult UpdateClarification(long RequestId, string clarification)
        {
            int ErrorCode = 0;
            if (clarification.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // updating common fields in the request
            Request.HRClarification = clarification;
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;
            Request.IsClarificationNeeded = false;    

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, _teamService.GetTeamIdHR(), AccessRequestStatusEnum.ClarificationProvided, clarification, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.ClarificationProvided, "Clarification is provided upon request.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                // send email to inform that clarification is provided
                try
                {
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: HR team
                    ToRecipients.Add(_service.GetHRTeamEmail());

                    // CC: Initiator, immediate supervisor, Line AVP/VP
                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);


                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Clarifications Provided – Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RemoteClarificationProvided(LatestSavedRequest)).Trim();
                }
                catch (Exception ex) { }

            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);

        }



        // GET: RemoteAccessAnnexure
        [HttpGet]
        [AccessAuthorize]
        public ActionResult RemoteAccessAnnexure()
        {
            var result = _service.AnnexureTwo("RemoteAccessRequest");
            return View(result);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult GetAnnexure(long RequestId)
        {
            var annexure = _service.GetAnnexureByRequestId(RequestId, "RemoteAccessRequest,RemoteAccessRequest.CreatedUser,SignedByEmployer");

            if (annexure == null)
                return HttpNotFound();

            return PartialView("_AnnexurePartial", annexure);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult RevokeRequests(string LAN_ID, long? branchId, long? departmentId, long? accessTypeId
           , long? statusId, string endDate, string checkBoxOnGoing, string requestId)
        {
            TicketService _ticketService = new TicketService();
            DateTime eDate = UserDateTime.GetUserDateOnly();
            DateTime sDate = UserDateTime.GetUserDateOnly();
            if (endDate != null && endDate != "")
            {
                try
                {
                    //sDate = UserDateTime.ConvertToAppDateTime(startDate);
                    eDate = UserDateTime.ConvertToAppDateTime(endDate);
                    sDate = eDate;
                }
                catch (Exception ex) { }
            }

            if (branchId == null)
                branchId = 0;
            if (departmentId == null)
                departmentId = 0;

            if (accessTypeId == null)
                accessTypeId = 0;
            if (statusId == null)
                statusId = 0;

            var pendingReply = _service.GetItemsPendingRevokeConfirmation("Team,Branch,Department,CreatedUser,RequestedUser").OrderByDescending(o => o.RemoteAccessRequestId).ToList();
            var toBeRevoked = _service.GetItemsPendingRevokeConfirmationPastTwoWeeks("Team,Branch,Department,CreatedUser,RequestedUser").OrderByDescending(o => o.RemoteAccessRequestId).ToList();

            ViewBag.RequestId = requestId;
            ViewBag.LAN_ID = LAN_ID;
            ViewBag.AccessTypeId = accessTypeId;
            ViewBag.BranchId = branchId;
            ViewBag.DepartmentId = departmentId;
            ViewBag.StatusId = statusId;
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");
            ViewBag.PendingRevokeRequests = pendingReply;
            ViewBag.toBeRevoked = toBeRevoked;

            var items = _service.GetTikcets(branchId, departmentId, accessTypeId, statusId,sDate, eDate, requestId,
                "Team,Branch,Department,CreatedUser,RequestedUser", userId: LAN_ID, filterByEndDate :true,completed:true).ToList();

            return View(items);

        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult RevokeNotifyUser(long requestId)
        {
            int ErrorCode = 0;


            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(requestId, "");

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;
            Request.Status = AccessRequestStatusEnum.RevokeConfirmationPending;  

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, null, AccessRequestStatusEnum.RevokeConfirmationPending, "Revocation confirmation request sent to user due to inactivity.", Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.RevokeConfirmationPending, "Revocation confirmation request sent to user due to inactivity.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }


                //send inactivity email to user

                try
                {
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(requestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                    // check if requester designation AVP or VP if so ski supervisor page
                    var RequesterDesignation = LatestSavedRequest.RequestedUser.DesignationName.ToLower().Trim();
                    string pattern = @"\b(assistant\s*vice\s*president|asst\.?\s*vp|avp|vp|vice\s*president)\b";

                    bool isMatch = Regex.IsMatch(RequesterDesignation, pattern, RegexOptions.IgnoreCase);

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    // TO: initiator and supervisor else if requtser is avp/vp then to initiator and approval vp/avp
                    if (isMatch)
                    {
                        ToRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                    }
                    else
                    {
                        ToRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    }
                    ToRecipients.Add(LatestSavedRequest.RequestedUser.Email);

                    // CC: network and security implementer
                    CCRecipients.Add(_service.GetNetworkImplementerTeamEmail());
                    CCRecipients.Add(_service.GetSecurityImplementerTeamEmail());


                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Remote Work Access Inactivity Alert - " + LatestSavedRequest.RequestedUser.FullName,
                                emailTemplate.RemoteAccessInactiveEmail(LatestSavedRequest)).Trim();
                }
                catch (Exception ex) { }

            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult ContinueAccess(long requestId)
        {
            int ErrorCode = 0;


            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(requestId, "");

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;
            Request.Status = AccessRequestStatusEnum.RevokeConfirmedContinue;
            

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, null, AccessRequestStatusEnum.RevokeConfirmedContinue, "User confirmed intent to retain access; revocation cancelled.", Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.RevokeConfirmedContinue, "User confirmed intent to retain access; revocation cancelled.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);

        }

        // general clarification request method 
        public ActionResult RaiseClarification(long RequestId,string Reason,long? ClarificationRequestedByTeamId, long? ClarificationAssignedToTeamId, ClarificationType clarificationType)
        {
            int ErrorCode = 0;
            if (Reason.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

        


            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            Request.FinalApprovalBy = NewUpdatedBy;
            Request.IsClarificationNeeded = true;

            Request.ClarificationRequestedByTeamId = ClarificationRequestedByTeamId;
            Request.ClarificationAssignedToTeamId = ClarificationAssignedToTeamId;
            Request.ClarificationType = clarificationType;

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId, ClarificationRequestedByTeamId, AccessRequestStatusEnum.ClarificationRequested, Reason, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate, ClarificationRequestedByTeamId, ClarificationAssignedToTeamId, clarificationType))
            {
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.ClarificationRequested, "Request for clarification.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
                
                // email sent when HR,ITSVP and DCEO ask clarifcation
                try
                {
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    if (ClarificationRequestedByTeamId == _teamService.GetTeamIdHR())
                    {
                        ToRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                        CCRecipients.Add(_service.GetHRTeamEmail());

                    }
                    else if (ClarificationRequestedByTeamId == _teamService.GetTeamIdITSVP())
                    {
                        if (clarificationType == ClarificationType.Requester)
                        {
                            ToRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                        }
                        else if (clarificationType == ClarificationType.Supervisor)
                        {
                            ToRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                        }
                        else if (clarificationType == ClarificationType.AVPOrVP)
                        {
                            ToRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                        }
                        else if (clarificationType == ClarificationType.Team)
                        {
                            if (ClarificationAssignedToTeamId == _teamService.GetTeamIdRisk())
                            {
                                ToRecipients.Add(_service.GetRiskTeamEmail());
                            }
                            else if (ClarificationAssignedToTeamId == _teamService.GetTeamIdHR())
                            {
                                ToRecipients.Add(_service.GetHRTeamEmail());
                            }
                            else
                            {
                                ToRecipients.Add(_service.GetITSecHeadEmail());
                            }
                        }
                        CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                        CCRecipients.Add(_service.GetITSVPEmail());

                    }
                    else  // if dceo requests
                    {
                        ToRecipients.Add(_service.GetITSVPEmail());
                        CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                        CCRecipients.Add(_service.GetDCEOEmail());

                    }


                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, " Clarification Required - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.RaiseClarificationEmail(LatestSavedRequest, Reason)).Trim();
                }
                catch (Exception ex) { }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }


        // general clarification reply method

        public ActionResult ReplyClarification(long RequestId, string Reply)
        {
            int ErrorCode = 0;
            if (Reply.IsNullOrWhiteSpace())
            {
                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);

            }

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");



            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            Request.FinalApprovalBy = NewUpdatedBy;
            Request.IsClarificationNeeded = false;

            var ClarificationRequestedByTeamId = Request.ClarificationRequestedByTeamId;
            var ClarificationAssignedToTeamId = Request.ClarificationAssignedToTeamId;
            var ClarificationType = Request.ClarificationType;

            if (_service.InsertUpdateStatus(Request.RemoteAccessRequestId,Request.ClarificationAssignedToTeamId , AccessRequestStatusEnum.ClarificationProvided, Reply, Request.SpentTime, false, NewUpdatedBy, NewUpdatedDate, Request.ClarificationRequestedByTeamId, Request.ClarificationAssignedToTeamId, Request.ClarificationType))
            {
                Request.ClarificationRequestedByTeamId = null;
                Request.ClarificationAssignedToTeamId = null;
                Request.ClarificationType = null;

                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.ClarificationProvided, "Clarification is provided upon request.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    RemoteAccessRequestUpdate latestRequest = null;
                    var requestedBy = "";
                    var assignedTo = "";
                    var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department");
                    var ClarificationUpdates = _service.GetClarificationUpdates(RequestId, "");

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();

                    if (ClarificationRequestedByTeamId == _teamService.GetTeamIdHR())
                    {
                        ToRecipients.Add(_service.GetHRTeamEmail());
                        requestedBy = "HR Remote Work Admin";
                        assignedTo = LatestSavedRequest.RequestedUser.FullName;
                     
                    }
                    else if (ClarificationRequestedByTeamId == _teamService.GetTeamIdITSVP())
                    {
                        ToRecipients.Add(_service.GetITSVPEmail());
                        requestedBy = "IT SVP";
                        if (ClarificationType == Domain.RA.ClarificationType.Team)
                        {
                            if (ClarificationAssignedToTeamId == _teamService.GetTeamIdHR())
                            {
                                CCRecipients.Add(_service.GetHRTeamEmail());
                                assignedTo = "HR Remote Work Admin";

                            }
                            else if (ClarificationAssignedToTeamId == _teamService.GetTeamIdRisk())
                            {
                                CCRecipients.Add(_service.GetRiskTeamEmail());
                                assignedTo = "Risk Team";
                            }
                            else // it sec head
                            {
                                CCRecipients.Add(_service.GetITSecHeadEmail());
                                assignedTo = "IT Security Head";
                            }
                        }
                        else if (ClarificationType == Domain.RA.ClarificationType.Supervisor)
                        {
                            assignedTo = "Supervisor";
                        }
                        else if (ClarificationType == Domain.RA.ClarificationType.AVPOrVP)
                        {
                            assignedTo = "AVP / VP";
                        }
                        else
                        {
                            assignedTo = "Requester";
                        }

                    }
                    else
                    {
                        ToRecipients.Add(_service.GetDCEOEmail());
                        requestedBy = "DCEO";
                        if (ClarificationType == Domain.RA.ClarificationType.Team)
                        {
                            if (ClarificationAssignedToTeamId == _teamService.GetTeamIdHR())
                            {
                                CCRecipients.Add(_service.GetHRTeamEmail());
                                assignedTo = "HR Remote Work Admin";

                            }
                            else if (ClarificationAssignedToTeamId == _teamService.GetTeamIdRisk())
                            {
                                CCRecipients.Add(_service.GetRiskTeamEmail());
                                assignedTo = "Risk Team";

                            }
                            else // it sec head
                            {
                                CCRecipients.Add(_service.GetITSecHeadEmail());
                                assignedTo = "IT Security Head";

                            }
                        }
                        else if (ClarificationType == Domain.RA.ClarificationType.Supervisor)
                        {
                            assignedTo = "Supervisor";
                        }
                        else if (ClarificationType == Domain.RA.ClarificationType.AVPOrVP)
                        {
                            assignedTo = "AVP / VP";
                        }
                        else
                        {
                            assignedTo = "Requester";
                        }
                    }

                    latestRequest = ClarificationUpdates
                     .Where(x =>
                         x.Status == AccessRequestStatusEnum.ClarificationRequested &&
                         x.ClarificationRequestedByTeamId == ClarificationRequestedByTeamId)
                     .OrderByDescending(x => x.UpdatedDate)
                     .FirstOrDefault();

                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);

                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, " Clarifications Provided  - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                                emailTemplate.ReplyClarificationEmail(LatestSavedRequest,latestRequest, Reply, requestedBy,assignedTo)).Trim();
                }
                catch (Exception ex) { }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }

        public ActionResult RouteClarification(long RequestId,long? ClarificationAssignedToTeamId, ClarificationType clarificationType)
        {
            int ErrorCode = 0;
         

            var NewUpdatedBy = Session["UserId"].ToString();

            // capturing the request to update its values
            var Request = _service.GetItem(RequestId, "");

            // getting last updated date
            var LastUpdated = Request.UpdatedDate;

            //setting new updated date
            var NewUpdatedDate = UserDateTime.GetUserDate();

            // updating common fields in the request
            Request.UpdatedDate = NewUpdatedDate;
            Request.UpdatedBy = NewUpdatedBy;

            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(LastUpdated, NewUpdatedDate);

            Request.SpentTime = Request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
            Request.SpentTimeStatus = Request.SpentTime;  //time since last stat change
            Request.SpentTimeSync = NewUpdatedDate;

            Request.FinalApprovalBy = NewUpdatedBy;

            Request.ClarificationAssignedToTeamId = ClarificationAssignedToTeamId;
            Request.ClarificationType = clarificationType;

            
                if (!_service.Update(Request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                if (!_service.InsertLog(Request.RemoteAccessRequestId, AccessRequestStatusEnum.ClarificationRequested, "Clarification routed to another recipient.", false, NewUpdatedBy, NewUpdatedDate))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
            try
            {
                var LatestSavedRequest = _service.GetAsNoTrackingItem(RequestId, "RequestedUser,ApprovedSupervisorUser,ApprovedAVPOrVPUser,FinalApprovalByUser,Department,RemoteAccessRequestUpdates");

                EmailTemplate emailTemplate = new EmailTemplate();
                EmailClient emailClient = new EmailClient();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();

              
               
                    if (clarificationType == ClarificationType.Requester)
                    {
                        ToRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    }
                    else if (clarificationType == ClarificationType.Supervisor)
                    {
                        ToRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                    }
                    else if (clarificationType == ClarificationType.AVPOrVP)
                    {
                        ToRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);
                    }
                    else if (clarificationType == ClarificationType.Team)
                    {
                        if (ClarificationAssignedToTeamId == _teamService.GetTeamIdRisk())
                        {
                            ToRecipients.Add(_service.GetRiskTeamEmail());
                        }
                        else if (ClarificationAssignedToTeamId == _teamService.GetTeamIdHR())
                        {
                            ToRecipients.Add(_service.GetHRTeamEmail());
                        }
                        else
                        {
                            ToRecipients.Add(_service.GetITSecHeadEmail());
                        }
                    }
                    CCRecipients.Add(LatestSavedRequest.RequestedUser.Email);
                    CCRecipients.Add(_service.GetITSVPEmail());

                
            


                CCRecipients.Add(LatestSavedRequest.ApprovedSupervisorUser.Email);
                CCRecipients.Add(LatestSavedRequest.ApprovedAVPOrVPUser.Email);

                var Reason = LatestSavedRequest.RemoteAccessRequestUpdates
                                    .Where(u =>
                                        u.Status == Domain.AR.AccessRequestStatusEnum.ClarificationRequested)
                                    .OrderByDescending(u => u.UpdatedDate)
                                    .FirstOrDefault().Comment;

                String res = emailClient.SendEmail(ToRecipients, CCRecipients, " Clarification Required - Remote Work Request " + LatestSavedRequest.RemoteAccessRequestId,
                            emailTemplate.RaiseClarificationEmail(LatestSavedRequest, Reason)).Trim();
            }
            catch (Exception ex) { }


            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetAccessTypesJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();
            Array values = Enum.GetValues(typeof(RemoteAccessTypeEnum));

            foreach (RemoteAccessTypeEnum val in values)
            {
                EnumStatusVM item = new EnumStatusVM();
                item.Id = Convert.ToInt32(val);
                item.Name = GlobalStaticService.EnumDisplayName(val);
                items.Add(item);
            }

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetStatusJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();
            Array values = Enum.GetValues(typeof(AccessRequestStatusEnum));

            foreach (AccessRequestStatusEnum val in values)
            {
                EnumStatusVM item = new EnumStatusVM();
                item.Id = Convert.ToInt32(val);
                item.Name = GlobalStaticService.EnumDisplayName(val);
                items.Add(item);
            }

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

    }
}
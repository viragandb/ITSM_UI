using Domain;
using Domain.AR;
using Newtonsoft.Json;
using Service;
using Service.AR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers.AR
{
    public class AccessRequestController : Controller
    {
        private readonly AccessRequestService _service = new AccessRequestService();
        private readonly AccessRequestTypeService _accessRequestTypeService = new AccessRequestTypeService();

        // GET: AccessRequest
        [AccessAuthorize]
        public ActionResult Index(long? teamId, long? accessRequestTypeId, long? branchId, long? departmentId, long? durationTypeId, long? accessTypeId
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
            if (accessRequestTypeId == null)
                accessRequestTypeId = 0;
            if (branchId == null)
                branchId = 0;
            if (departmentId == null)
                departmentId = 0;
            if (durationTypeId == null)
                durationTypeId = 0;
            if (accessTypeId == null)
                accessTypeId = 0;
            if (statusId == null)
                statusId = 0;

            ViewBag.RequestId = requestId;
            ViewBag.TeamId = teamId;
            ViewBag.AccessRequestTypeId = accessRequestTypeId;
            ViewBag.BranchId = branchId;
            ViewBag.DepartmentId = departmentId;
            ViewBag.DurationTypeId = durationTypeId;
            ViewBag.AccessTypeId = accessTypeId;
            ViewBag.StatusId = statusId;
            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");

            var items = _service.GetTikcets(teamId, accessRequestTypeId, branchId, departmentId, durationTypeId, accessTypeId, statusId, sDate, eDate.AddDays(1), requestId,
                "AccessRequestType,Team,Branch,Department,CreatedUser,RequestedUser").OrderByDescending(o => o.RequestedDate).ToList();

            return View(items);

        }


        [AccessAuthorize]
        [HttpGet]
        public ActionResult NewRequest()
        {

            AccessRequestTypeService _accessRequestTypeService = new AccessRequestTypeService();
            UserTeamService _userTeamService = new UserTeamService();


            if (_userTeamService.IsTeamUserIT(Session["UserId"].ToString()))
            {
                var items = _accessRequestTypeService.GetAllActive("");
                return View(items);

            }
            else
            {
                var items = _accessRequestTypeService.GetAllNoIT("");
                return View(items);
            }

        }

        [AccessAuthorize]
        [HttpGet]
        [EncryptedActionParameter]
        public ActionResult Create(long? accessType, long? accessRequestTypeId)
        {

            var item = new NewAccessRequestVM();
            item.AccessTypeId = (int)accessType;
            if (accessType == 1)
                item.AccessType = AllocatedTypeEnum.Internal;
            else if (accessType == 2)
                item.AccessType = AllocatedTypeEnum.External;

            if (item.AccessType == AllocatedTypeEnum.External)
            {
                item.DurationType = AssignedTypeEnum.Temporary;
            }
            var requestType = _accessRequestTypeService.GetAsNoTrackingItem(accessRequestTypeId, "");
            item.AccessRequestTypeId = requestType.AccessRequestTypeId;
            item.AccessRequestName = requestType.Name;
            item.RequestedUser = new User();
            item.ExpireDate = UserDateTime.GetUserDateOnly().ToString("dd/MM/yyyy");
            item.Instructions = requestType.RequestorInstructions;
            ViewBag.ExpireDate = item.ExpireDate;

            Collection<SystemAccessItem> systemAccessItems = new Collection<SystemAccessItem>();
            item.SystemAccessItems = systemAccessItems.ToList();

            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(NewAccessRequestVM item, HttpPostedFileBase fileInputLegalIdDoc, HttpPostedFileBase fileInputNonEmpAcknowledgementDoc)
        {
            ViewBag.BranchId = item.BranchId;
            ViewBag.DepartmentId = item.DepartmentId;
            var dateExpire = UserDateTime.GetUserDateOnly();

            if (item.AccessType == AllocatedTypeEnum.External)
                item.DurationType = AssignedTypeEnum.Temporary;

            if (item.Subject == null || item.Justification == null || item.Description == null
                || item.DurationType == 0 || item.BranchId == 0 || item.DepartmentId == 0 || (item.RequestedBy == null && item.ExternalUser == null) || item.ApprovalBy == null)
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View("Create", item);
            }

            if (item.AccessType == AllocatedTypeEnum.Internal && (item.RequestedBy == null || item.RequestedBy == ""))
            {
                TempData["ErrorMessage"] = "Please enter requested user. ";
                return View("Create", item);
            }
            if (item.AccessType == AllocatedTypeEnum.External && (item.ExternalUser == null || item.ExternalUser == "" || item.Organization == null || item.Organization == ""))
            {
                TempData["ErrorMessage"] = "Please enter requested user. ";
                return View("Create", item);
            }

            if (item.AccessType == AllocatedTypeEnum.External)
                item.DurationType = AssignedTypeEnum.Temporary;

            if (item.DurationType == AssignedTypeEnum.Temporary)
            {
                try
                {
                    dateExpire = UserDateTime.ConvertToAppDateTime(item.ExpireDate);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Please enter valid expire date. ";
                    return View(item);
                }

                if (dateExpire > UserDateTime.GetUserDate().AddMonths(6))
                {
                    TempData["ErrorMessage"] = "Please enter valid expire date. ";
                    return View(item);
                }
            }


            if (item.AccessRequestTypeId == AccessRequestTypeService.GetSystemAccessId())
            {
                if (item.SystemAccessItems == null)
                {
                    TempData["ErrorMessage"] = "Please add systems. ";
                    return View(item);
                }
            }
            else if (item.AccessRequestTypeId == AccessRequestTypeService.GetUserPrivilegeAccessId())
            {
                if (item.UserPrivilegeAccessItems == null)
                {
                    TempData["ErrorMessage"] = "Please add User Privilege Accesses. ";
                    return View(item);
                }
            }
            else if (item.AccessRequestTypeId == AccessRequestTypeService.GetPhysicalAccessId())
            {
                if (item.PhysicalAccessItems == null)
                {
                    TempData["ErrorMessage"] = "Please add Physical Access areas.";
                    return View(item);
                }
            }
            else if (item.AccessRequestTypeId == AccessRequestTypeService.GetUserAccessIdInternal() || item.AccessRequestTypeId == AccessRequestTypeService.GetUserAccessIdExternal())
            {
                if (item.UserAccessItems == null || item.UserAccessType == 0)
                {
                    TempData["ErrorMessage"] = "Please add User Access data. ";
                    return View(item);
                }

                if (item.AccessType == AllocatedTypeEnum.External)
                {
                    if (fileInputLegalIdDoc == null || fileInputLegalIdDoc.ContentLength == 0
                        || fileInputNonEmpAcknowledgementDoc == null || fileInputNonEmpAcknowledgementDoc.ContentLength == 0)
                    {
                        TempData["ErrorMessage"] = "Please add external user documents. ";
                        return View(item);
                    }

                    else if (fileInputLegalIdDoc.ContentLength > 2097152 || fileInputNonEmpAcknowledgementDoc.ContentLength > 2097152) //2MB
                    {
                        TempData["ErrorMessage"] = "Please reduce file size (Max. 2MB). ";
                        return View(item);
                    }

                    var supportedTypes = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".docx", ".doc", ".xlsx", ".xls" };
                    if (!supportedTypes.Contains(Path.GetExtension(fileInputLegalIdDoc.FileName).ToLower())
                        || !supportedTypes.Contains(Path.GetExtension(fileInputNonEmpAcknowledgementDoc.FileName).ToLower()))
                    {
                        TempData["ErrorMessage"] = "Please upload a valid format. ";
                        return View(item);
                    }

                }
            }
            else if (item.AccessRequestTypeId == AccessRequestTypeService.GetDeviceAccessId())
            {
                if (item.DeviceAccessItems == null)
                {
                    TempData["ErrorMessage"] = "Please add Device Access items.";
                    return View(item);
                }
                else if (item.AssetId == 0)
                {
                    TempData["ErrorMessage"] = "Please type Asset #.";
                    return View(item);
                }
            }
            else if (item.AccessRequestTypeId == AccessRequestTypeService.GetRemoteAccessId())
            {
                if (item.RemoteAccessType == 0)
                {
                    TempData["ErrorMessage"] = "Select the Remote Access Type. ";
                    return View(item);
                }
                if (item.Impact == "" || item.Impact == "")
                {
                    TempData["ErrorMessage"] = "Please enter impact.";
                    return View(item);
                }
                if (item.RemoteAccessItems == null)
                {
                    TempData["ErrorMessage"] = "Please add System Access items.";
                    return View(item);
                }
                else if (item.RemoteAssetId == 0)
                {
                    TempData["ErrorMessage"] = "Please type Asset #.";
                    return View(item);
                }
            }

            UserService _userService = new UserService();
            User requestedUser = new User();
            if (item.AccessType == AllocatedTypeEnum.Internal && item.RequestedBy != "" && item.RequestedBy != null)
            {
                requestedUser = _userService.GetUserByEmpNo_UpdateByAD(item.RequestedBy, Session["UserId"].ToString(), UserDateTime.GetUserDate(), "");
                if (requestedUser.EmpNo == null)
                {
                    TempData["ErrorMessage"] = "Invalid User. ";
                    return View("Create", item);
                }

            }

            User approvalUser = new User();
            if (item.ApprovalBy != "" && item.ApprovalBy != null)
            {
                approvalUser = _userService.GetUserByEmpNo_UpdateByAD(item.ApprovalBy, Session["UserId"].ToString(), UserDateTime.GetUserDate(), "");
                if (approvalUser.EmpNo == null || item.ApprovalBy == Session["UserId"].ToString())
                {
                    TempData["ErrorMessage"] = "Invalid Approval User. ";
                    return View("Create", item);
                }

            }

            AccessRequest newTicket = new AccessRequest();
            newTicket.UpdatedBy = Session["UserId"].ToString();
            newTicket.UpdatedDate = UserDateTime.GetUserDate();
            newTicket.CreatedBy = newTicket.UpdatedBy;
            newTicket.RequestedDate = newTicket.UpdatedDate;
            newTicket.AccessRequestTypeId = item.AccessRequestTypeId;
            newTicket.AccessType = item.AccessType;
            newTicket.ApprovalBy = item.ApprovalBy;
            newTicket.Subject = item.Subject;
            newTicket.Description = item.Description;
            newTicket.Justification = item.Justification;
            newTicket.DurationType = item.DurationType;
            newTicket.BranchId = item.BranchId;
            newTicket.DepartmentId = item.DepartmentId;
            newTicket.ExpireDate = dateExpire;

            if (newTicket.AccessType == AllocatedTypeEnum.Internal)
            {
                newTicket.RequestedBy = item.RequestedBy;
            }
            else
            {
                newTicket.RequestedBy = newTicket.CreatedBy;
                newTicket.ExternalUser = item.ExternalUser;
                newTicket.LegalId = item.LegalId;
                newTicket.Organization = item.Organization;
            }
            //SerialService _serialService = new SerialService();
            //newTicket.Code = _serialService.GetTicketSerial().ToString("00000000");

            var requestType = _accessRequestTypeService.GetAsNoTrackingItem(item.AccessRequestTypeId, "Workflow,Workflow.WorkflowLevels");

            newTicket.Status = AccessRequestStatusEnum.Initiated;
            newTicket.WorkflowId = requestType.WorkflowId;
            newTicket.NextWorkflowlevelNo = 1;
            // newTicket.TeamId = requestType.Workflow.WorkflowLevels.FirstOrDefault(l => l.LevelNo == 1).TeamId;
            newTicket.WorkflowLevelType = requestType.Workflow.WorkflowLevels.FirstOrDefault(l => l.LevelNo == 1).WorkflowLevelType;


            newTicket.SpentTimeSync = newTicket.RequestedDate;
            newTicket.SpentTime = 0;
            //TicketDocService _ticketDocService = new TicketDocService();
            //var tempDocs = _ticketDocService.GetTempDocs(newTicket.UpdatedBy, "");

            var entitySaved = _service.Insert(newTicket);
            if (entitySaved)
            {

                if (newTicket.AccessRequestTypeId == AccessRequestTypeService.GetSystemAccessId())
                {
                    SystemAccessService _systemAccessService = new SystemAccessService();
                    SystemAccess systemAccess = new SystemAccess();
                    systemAccess.AccessRequestId = newTicket.AccessRequestId;
                    systemAccess.UpdatedBy = newTicket.UpdatedBy;
                    systemAccess.UpdatedDate = newTicket.UpdatedDate;

                    var systems = new Collection<SystemAccessItem>();
                    foreach (var sys in item.SystemAccessItems)
                    {
                        SystemAccessItem sysItem = new SystemAccessItem();
                        sysItem.Description = sys.Description;
                        sysItem.Justification = sys.Justification;
                        sysItem.MonitoredBy = sys.MonitoredBy;
                        sysItem.PrivilegeLevel = sys.PrivilegeLevel;
                        sysItem.SystemEnvironmentId = sys.SystemEnvironmentId;
                        sysItem.UpdatedBy = newTicket.UpdatedBy;
                        sysItem.UpdatedDate = newTicket.UpdatedDate;
                        systems.Add(sysItem);
                    }
                    systemAccess.SystemAccessItems = systems;
                    _systemAccessService.Insert(systemAccess);

                    newTicket.SystemAccessId = systemAccess.SystemAccessId;
                }
                else if (newTicket.AccessRequestTypeId == AccessRequestTypeService.GetUserPrivilegeAccessId())
                {
                    UserPrivilegeAccessService _userPrivilegeAccessService = new UserPrivilegeAccessService();
                    UserPrivilegeAccess _access = new UserPrivilegeAccess();
                    //SystemAccessService _systemAccessService = new SystemAccessService();
                    //SystemAccess systemAccess = new SystemAccess();
                    _access.AccessRequestId = newTicket.AccessRequestId;
                    _access.UpdatedBy = newTicket.UpdatedBy;
                    _access.UpdatedDate = newTicket.UpdatedDate;

                    var accesses = new Collection<UserPrivilegeAccessItem>();
                    foreach (var aItem in item.UserPrivilegeAccessItems)
                    {
                        UserPrivilegeAccessItem acessItem = new UserPrivilegeAccessItem();
                        acessItem.AccessPrivilegeCategoryId = aItem.AccessPrivilegeCategoryId;
                        acessItem.AccessApplicationId = aItem.AccessApplicationId;
                        acessItem.AccessLevelId = aItem.AccessLevelId;
                        acessItem.UpdatedBy = newTicket.UpdatedBy;
                        acessItem.UpdatedDate = newTicket.UpdatedDate;
                        accesses.Add(acessItem);
                    }
                    _access.UserPrivilegeAccessItems = accesses;
                    _userPrivilegeAccessService.Insert(_access);

                    newTicket.UserPrivilegeAccessId = _access.UserPrivilegeAccessId;
                }
                else if (newTicket.AccessRequestTypeId == AccessRequestTypeService.GetPhysicalAccessId())
                {
                    PhysicalAccessService _physicalAccessService = new PhysicalAccessService();
                    PhysicalAccess _access = new PhysicalAccess();
                    _access.AccessRequestId = newTicket.AccessRequestId;
                    _access.UpdatedBy = newTicket.UpdatedBy;
                    _access.UpdatedDate = newTicket.UpdatedDate;

                    var accesses = new Collection<PhysicalAccessItem>();
                    foreach (var aItem in item.PhysicalAccessItems)
                    {
                        PhysicalAccessItem acessItem = new PhysicalAccessItem();
                        acessItem.PhysicalAreaId = aItem.PhysicalAreaId;
                        acessItem.UpdatedBy = newTicket.UpdatedBy;
                        acessItem.UpdatedDate = newTicket.UpdatedDate;
                        accesses.Add(acessItem);
                    }
                    _access.PhysicalAccessItems = accesses;
                    _physicalAccessService.Insert(_access);

                    newTicket.PhysicalAccessId = _access.PhysicalAccessId;
                }
                else if (newTicket.AccessRequestTypeId == AccessRequestTypeService.GetUserAccessIdInternal() || newTicket.AccessRequestTypeId == AccessRequestTypeService.GetUserAccessIdExternal())
                {
                    UserAccessService _accessService = new UserAccessService();
                    UserAccess _access = new UserAccess();
                    _access.AccessRequestId = newTicket.AccessRequestId;
                    _access.UpdatedBy = newTicket.UpdatedBy;
                    _access.UpdatedDate = newTicket.UpdatedDate;

                    _access.UserAccessType = item.UserAccessType;
                    #region Upload Doc
                    if (item.AccessType == AllocatedTypeEnum.External)
                    {
                        string dirname = GlobalStaticService.GetFileUploadPath();
                        string _FileName = Guid.NewGuid().ToString() + Path.GetExtension(fileInputLegalIdDoc.FileName);
                        string _path = Path.Combine(Server.MapPath("~/" + dirname), _FileName);

                        fileInputLegalIdDoc.SaveAs(_path);
                        _access.LegalIdDoc = "/" + dirname + _FileName;

                        _FileName = Guid.NewGuid().ToString() + Path.GetExtension(fileInputNonEmpAcknowledgementDoc.FileName);
                        _path = Path.Combine(Server.MapPath("~/" + dirname), _FileName);

                        fileInputNonEmpAcknowledgementDoc.SaveAs(_path);
                        _access.NonEmpAcknowledgementDoc = "/" + dirname + _FileName;
                    }
                    #endregion

                    var accesses = new Collection<UserAccessItem>();
                    foreach (var aItem in item.UserAccessItems)
                    {
                        UserAccessItem acessItem = new UserAccessItem();
                        acessItem.UserAccessItemTypeId = aItem.UserAccessItemTypeId;
                        acessItem.Description = aItem.Description;
                        acessItem.Justification = aItem.Justification;
                        acessItem.UpdatedBy = newTicket.UpdatedBy;
                        acessItem.UpdatedDate = newTicket.UpdatedDate;
                        accesses.Add(acessItem);
                    }
                    _access.UserAccessItems = accesses;
                    _accessService.Insert(_access);

                    newTicket.UserAccessId = _access.UserAccessId;
                }
                else if (newTicket.AccessRequestTypeId == AccessRequestTypeService.GetDeviceAccessId())
                {
                    DeviceAccessService _accessService = new DeviceAccessService();
                    DeviceAccess _access = new DeviceAccess();
                    _access.AccessRequestId = newTicket.AccessRequestId;
                    _access.UpdatedBy = newTicket.UpdatedBy;
                    _access.UpdatedDate = newTicket.UpdatedDate;

                    _access.AssetId = item.AssetId;

                    var accesses = new Collection<DeviceAccessItem>();
                    foreach (var aItem in item.DeviceAccessItems)
                    {
                        DeviceAccessItem acessItem = new DeviceAccessItem();
                        acessItem.DeviceAccessItemTypeId = aItem.DeviceAccessItemTypeId;
                        acessItem.UpdatedBy = newTicket.UpdatedBy;
                        acessItem.UpdatedDate = newTicket.UpdatedDate;
                        accesses.Add(acessItem);
                    }
                    _access.DeviceAccessItems = accesses;
                    _accessService.Insert(_access);

                    newTicket.DeviceAccessId = _access.DeviceAccessId;
                }
                else if (newTicket.AccessRequestTypeId == AccessRequestTypeService.GetRemoteAccessId())
                {
                    RemoteAccessService _accessService = new RemoteAccessService();
                    RemoteAccess _access = new RemoteAccess();
                    _access.AccessRequestId = newTicket.AccessRequestId;
                    _access.UpdatedBy = newTicket.UpdatedBy;
                    _access.UpdatedDate = newTicket.UpdatedDate;

                    _access.AssetId = item.RemoteAssetId;
                    _access.RemoteAccessType = item.RemoteAccessType;
                    _access.Impact = item.Impact;

                    var accesses = new Collection<RemoteAccessItem>();
                    foreach (var aItem in item.RemoteAccessItems)
                    {
                        RemoteAccessItem acessItem = new RemoteAccessItem();
                        acessItem.AssetTypeId = aItem.AssetTypeId;
                        acessItem.Description = aItem.Description;
                        acessItem.UpdatedBy = newTicket.UpdatedBy;
                        acessItem.UpdatedDate = newTicket.UpdatedDate;
                        accesses.Add(acessItem);
                    }
                    _access.RemoteAccessItems = accesses;
                    _accessService.Insert(_access);

                    newTicket.RemoteAccessId = _access.RemoteAccessId;
                }

                _service.UpdateStatus(newTicket);
                _service.InsertLog(newTicket, "New request initiated");
                _service.InsertUpdateStatus(newTicket, "New Ticket was initiated", 1);

                #region Email
                try
                {
                    var emailRequest = _service.GetItemAsNoTracking(newTicket.AccessRequestId, "ApprovalByUser,CreatedUser,AccessRequestType,RequestedUser");
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();
                    if (!string.IsNullOrEmpty(emailRequest.ApprovalByUser.Email))
                    {
                        ToRecipients.Add(emailRequest.ApprovalByUser.Email);
                        if (!string.IsNullOrEmpty(emailRequest.CreatedUser.Email))
                            CCRecipients.Add(emailRequest.CreatedUser.Email);

                        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New " + emailRequest.AccessRequestType.Name + " request pending approval | Request # " + emailRequest.AccessRequestId,
                                    emailTemplate.AccessRequestCreate(emailRequest)).Trim();
                    }
                }
                catch (Exception ex) { }
                #endregion

                TempData["SuccessMessage"] = "Request has been successfully created.";
                return RedirectToAction("NewRequest");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View("Create", item);
            }



        }

        [HttpGet]
        [AccessLogin]
        [EncryptedActionParameter]
        public ActionResult Info(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = _service.GetItem(id, "AccessRequestType,Workflow,Workflow.WorkflowLevels,Workflow.WorkflowLevels.Team,Team,Branch,Department,CreatedUser,RequestedUser,ApprovalByUser" +
                ",SystemAccess,SystemAccess.SystemAccessItems,SystemAccess.SystemAccessItems.SystemEnvironment,SystemAccess.SystemAccessItems.SystemEnvironment.System" +
                ",UserPrivilegeAccess,UserPrivilegeAccess.UserPrivilegeAccessItems,UserPrivilegeAccess.UserPrivilegeAccessItems.AccessApplication.System,UserPrivilegeAccess.UserPrivilegeAccessItems.AccessPrivilegeCategory,UserPrivilegeAccess.UserPrivilegeAccessItems.AccessLevel" +
                ",PhysicalAccess,PhysicalAccess.PhysicalAccessItems,PhysicalAccess.PhysicalAccessItems.PhysicalArea,PhysicalAccess.PhysicalAccessItems.PhysicalArea.Branch" +
                ",UserAccess,UserAccess.UserAccessItems,UserAccess.UserAccessItems.UserAccessItemType" +
                ",DeviceAccess,DeviceAccess.Asset,DeviceAccess.DeviceAccessItems,DeviceAccess.DeviceAccessItems.DeviceAccessItemType" +
                ",RemoteAccess,RemoteAccess.Asset,RemoteAccess.RemoteAccessItems,RemoteAccess.RemoteAccessItems.AssetType" +
                ",AccessRequestUpdates,AccessRequestUpdates.UpdatedUser,AccessRequestUpdates.Team,AccessRequestLogs,AccessRequestLogs.UpdatedUser");


            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult MyRequest()
        {
            var items = _service.GetItemsMy(Session["UserId"].ToString(),
                "AccessRequestType,Team,Branch,Department,CreatedUser,RequestedUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult MyRequestReject(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == AccessRequestStatusEnum.Initiated && request.CreatedBy == request.UpdatedBy)
            {
                request.Status = AccessRequestStatusEnum.Rejected;

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    //_service.InsertLog(newTicket, "New request initiated");
                    _service.InsertUpdateStatus(request, comment, 1);

                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.AccessRequestId, "CreatedUser,ApprovalByUser,AccessRequestType,RequestedUser");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.ApprovalByUser.Email))
                        {
                            ToRecipients.Add(emailRequest.ApprovalByUser.Email);
                            if (!string.IsNullOrEmpty(emailRequest.CreatedUser.Email))
                                CCRecipients.Add(emailRequest.CreatedUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, emailRequest.AccessRequestType.Name + " Request Rejected | Request # " + emailRequest.AccessRequestId,
                                        emailTemplate.AccessRequestCancel(emailRequest, comment)).Trim();
                        }
                    }
                    catch (Exception ex) { }
                    #endregion

                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }

        #region Approve

        [HttpGet]
        [AccessAuthorize]
        public ActionResult ApprovalPending()
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
            var items = _service.GetItemsApprovals(Session["UserId"].ToString(),
                "AccessRequestType,Team,Branch,Department,CreatedUser,RequestedUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        //[AccessAuthorize]
        [HttpPost]
        public ActionResult Approve(long requestId, string comment, bool checkboxValue, HttpPostedFileBase file)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == AccessRequestStatusEnum.Initiated && request.ApprovalBy == request.UpdatedBy)
            {
                var requestType = _accessRequestTypeService.GetAsNoTrackingItem(request.AccessRequestTypeId, "Workflow,Workflow.WorkflowLevels");

                request.Status = AccessRequestStatusEnum.Pending;

                request.TeamId = requestType.Workflow.WorkflowLevels.FirstOrDefault(l => l.LevelNo == 1).TeamId;

                if (checkboxValue && file == null)
                    errorCode = 1;
                else
                {
                    try
                    {
                        string dirname = GlobalStaticService.GetFileUploadPath();
                        string _FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string _path = Path.Combine(Server.MapPath("~/" + dirname), _FileName);

                        file.SaveAs(_path);
                        request.ApprovalAttachment = "/" + dirname + _FileName;
                        request.IsApprovalTerm = checkboxValue;
                        if (checkboxValue)
                            comment += " (AVP/VP Approval Attached)";
                    }
                    catch (Exception ex) { }
                    var entitySaved = _service.Update(request);
                    if (entitySaved)
                    {
                        //_service.InsertLog(newTicket, "New request initiated");
                        _service.InsertUpdateStatus(request, comment , 1);

                        TempData["SuccessMessage"] = "Request has been updated.";
                        errorCode = 1;

                        #region Email
                        try
                        {
                            var emailRequest = _service.GetItemAsNoTracking(request.AccessRequestId, "AccessRequestType,RequestedUser,Team");
                            EmailTemplate emailTemplate = new EmailTemplate();
                            EmailClient emailClient = new EmailClient();
                            List<string> ToRecipients = new List<string>();
                            List<string> CCRecipients = new List<string>();
                            if (!string.IsNullOrEmpty(emailRequest.Team.TeamEmail))
                            {
                                ToRecipients.Add(emailRequest.Team.TeamEmail);

                                String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New " + emailRequest.AccessRequestType.Name + " Request Pending | Request # " + emailRequest.AccessRequestId,
                                            emailTemplate.AccessRequestPending(emailRequest)).Trim();
                            }
                        }
                        catch (Exception ex) { }
                        #endregion

                    }
                    else
                        errorCode = 2;
                }
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Reject(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == AccessRequestStatusEnum.Initiated && request.ApprovalBy == request.UpdatedBy)
            {
                request.Status = AccessRequestStatusEnum.Rejected;

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    //_service.InsertLog(newTicket, "New request initiated");
                    _service.InsertUpdateStatus(request, comment, 1);

                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.AccessRequestId, "CreatedUser,ApprovalByUser,AccessRequestType,RequestedUser");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.CreatedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.CreatedUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, emailRequest.AccessRequestType.Name + " Request Rejected | Request # " + emailRequest.AccessRequestId,
                                        emailTemplate.AccessRequestReject(emailRequest, comment)).Trim();
                        }
                    }
                    catch (Exception ex) { }
                    #endregion

                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Authorizer

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Authorizer()
        {

            var items = _service.GetItemsWorkflow(Session["UserId"].ToString(), WorkflowLevelTypeEnum.Authorizer,
                "AccessRequestType,Team,Branch,Department,CreatedUser,RequestedUser,ApprovalByUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult AuthorizerApprove(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "Workflow,Workflow.WorkflowLevels,Team.UserTeams");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == AccessRequestStatusEnum.Pending && request.WorkflowLevelType == WorkflowLevelTypeEnum.Authorizer
                && request.Team.UserTeams.Any(u => u.EmpNo == request.UpdatedBy))
            {
                //request.Status = AccessRequestStatusEnum.Pending;

                var teamId = request.TeamId;
                var workFlowLevel = request.Workflow.WorkflowLevels.FirstOrDefault(l => l.LevelNo == request.NextWorkflowlevelNo + 1);
                if (workFlowLevel != null)
                {
                    request.TeamId = workFlowLevel.TeamId;
                    request.NextWorkflowlevelNo = workFlowLevel.LevelNo;
                    request.WorkflowLevelType = workFlowLevel.WorkflowLevelType;
                }
                else
                {
                    request.NextWorkflowlevelNo = 0;
                    request.WorkflowLevelType = WorkflowLevelTypeEnum.NA;
                    request.Status = AccessRequestStatusEnum.Completed;
                }
                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    _service.InsertUpdateStatus(request, comment, teamId);

                    TempData["SuccessMessage"] = "Request has been updated.";
                    errorCode = 1;
                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.AccessRequestId, "AccessRequestType,RequestedUser,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.Team.TeamEmail))
                        {
                            ToRecipients.Add(emailRequest.Team.TeamEmail);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New " + emailRequest.AccessRequestType.Name + " Request Pending | Request # " + emailRequest.AccessRequestId,
                                        emailTemplate.AccessRequestPending(emailRequest)).Trim();
                        }
                    }
                    catch (Exception ex) { }
                    #endregion
                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult AuthorizerReject(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "Team.UserTeams");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == AccessRequestStatusEnum.Pending && request.WorkflowLevelType == WorkflowLevelTypeEnum.Authorizer
            && request.Team.UserTeams.Any(u => u.EmpNo == request.UpdatedBy))
            {
                request.Status = AccessRequestStatusEnum.Rejected;
                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    _service.InsertUpdateStatus(request, comment, request.TeamId);

                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.AccessRequestId, "CreatedUser,ApprovalByUser,AccessRequestType,RequestedUser,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.CreatedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.CreatedUser.Email);
                            if (!string.IsNullOrEmpty(emailRequest.ApprovalByUser.Email))
                                CCRecipients.Add(emailRequest.ApprovalByUser.Email);
                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, emailRequest.AccessRequestType.Name + " Request Rejected | Request # " + emailRequest.AccessRequestId,
                                        emailTemplate.AccessRequestRejectWorkflow(emailRequest, comment)).Trim();
                        }
                    }
                    catch (Exception ex) { }
                    #endregion
                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Inputter

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Inputter()
        {

            var items = _service.GetItemsWorkflow(Session["UserId"].ToString(), WorkflowLevelTypeEnum.Inputter,
                "AccessRequestType,Team,Branch,Department,CreatedUser,RequestedUser,ApprovalByUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult InputterUpdate(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "Workflow,Workflow.WorkflowLevels,Team.UserTeams");
            var teamId = request.TeamId;
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == AccessRequestStatusEnum.Pending && request.WorkflowLevelType == WorkflowLevelTypeEnum.Inputter
            && request.Team.UserTeams.Any(u => u.EmpNo == request.UpdatedBy))
            {
                //request.Status = AccessRequestStatusEnum.Pending;

                var workFlowLevel = request.Workflow.WorkflowLevels.FirstOrDefault(l => l.LevelNo == request.NextWorkflowlevelNo + 1);
                if (workFlowLevel != null)
                {
                    request.TeamId = workFlowLevel.TeamId;
                    request.NextWorkflowlevelNo = workFlowLevel.LevelNo;
                    request.WorkflowLevelType = workFlowLevel.WorkflowLevelType;
                }
                else
                {
                    request.NextWorkflowlevelNo = 0;
                    request.WorkflowLevelType = WorkflowLevelTypeEnum.NA;
                    request.Status = AccessRequestStatusEnum.Completed;
                }
                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    _service.InsertUpdateStatus(request, comment, teamId);

                    TempData["SuccessMessage"] = "Request has been updated.";
                    errorCode = 1;
                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.AccessRequestId, "AccessRequestType,RequestedUser,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.Team.TeamEmail))
                        {
                            ToRecipients.Add(emailRequest.Team.TeamEmail);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New " + emailRequest.AccessRequestType.Name + " Request Pending | Request # " + emailRequest.AccessRequestId,
                                        emailTemplate.AccessRequestPending(emailRequest)).Trim();
                        }
                    }
                    catch (Exception ex) { }
                    #endregion
                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Checker

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Checker()
        {

            var items = _service.GetItemsWorkflow(Session["UserId"].ToString(), WorkflowLevelTypeEnum.Checker,
                "AccessRequestType,Team,Branch,Department,CreatedUser,RequestedUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult CheckerUpdate(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "Workflow,Workflow.WorkflowLevels,Team.UserTeams,ApprovalByUser");
            var teamId = request.TeamId;
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == AccessRequestStatusEnum.Pending && request.WorkflowLevelType == WorkflowLevelTypeEnum.Checker
                && request.Team.UserTeams.Any(u => u.EmpNo == request.UpdatedBy))
            {
                //request.Status = AccessRequestStatusEnum.Pending;

                var workFlowLevel = request.Workflow.WorkflowLevels.FirstOrDefault(l => l.LevelNo == request.NextWorkflowlevelNo + 1);
                if (workFlowLevel != null)
                {
                    request.TeamId = workFlowLevel.TeamId;
                    request.NextWorkflowlevelNo = workFlowLevel.LevelNo;
                    request.WorkflowLevelType = workFlowLevel.WorkflowLevelType;
                }
                else
                {
                    request.NextWorkflowlevelNo = 0;
                    request.WorkflowLevelType = WorkflowLevelTypeEnum.NA;
                    request.Status = AccessRequestStatusEnum.Completed;
                }
                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    _service.InsertUpdateStatus(request, comment, teamId);

                    TempData["SuccessMessage"] = "Request has been updated.";
                    errorCode = 1;
                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.AccessRequestId, "AccessRequestType,RequestedUser,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.Team.TeamEmail))
                        {
                            ToRecipients.Add(emailRequest.Team.TeamEmail);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New " + emailRequest.AccessRequestType.Name + " Request Pending | Request # " + emailRequest.AccessRequestId,
                                        emailTemplate.AccessRequestPending(emailRequest)).Trim();
                        }
                    }
                    catch (Exception ex) { }
                    #endregion
                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult CheckerReject(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "Team.UserTeams");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == AccessRequestStatusEnum.Pending && request.WorkflowLevelType == WorkflowLevelTypeEnum.Checker
            && request.Team.UserTeams.Any(u => u.EmpNo == request.UpdatedBy))
            {
                request.Status = AccessRequestStatusEnum.Rejected;
                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    _service.InsertUpdateStatus(request, comment, request.TeamId);

                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.AccessRequestId, "CreatedUser,ApprovalByUser,AccessRequestType,RequestedUser,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.CreatedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.CreatedUser.Email);
                            if (!string.IsNullOrEmpty(emailRequest.ApprovalByUser.Email))
                                CCRecipients.Add(emailRequest.ApprovalByUser.Email);
                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, emailRequest.AccessRequestType.Name + " Request Rejected | Request # " + emailRequest.AccessRequestId,
                                        emailTemplate.AccessRequestRejectWorkflow(emailRequest, comment)).Trim();
                        }
                    }
                    catch (Exception ex) { }
                    #endregion
                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Implementer

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Implementer()
        {

            var items = _service.GetItemsWorkflow(Session["UserId"].ToString(), WorkflowLevelTypeEnum.Implementer,
                "AccessRequestType,Team,Branch,Department,CreatedUser,RequestedUser,ApprovalByUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }

            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult ImplementerUpdate(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "Workflow,Workflow.WorkflowLevels,RequestedUser,CreatedUser,AccessRequestType,Team,Team.UserTeams");
            var teamId = request.TeamId;
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();

            if (request.Status == AccessRequestStatusEnum.Pending && request.WorkflowLevelType == WorkflowLevelTypeEnum.Implementer
                && request.Team.UserTeams.Any(u => u.EmpNo == request.UpdatedBy))
            {
                //request.Status = AccessRequestStatusEnum.Pending;

                var workFlowLevel = request.Workflow.WorkflowLevels.FirstOrDefault(l => l.LevelNo == request.NextWorkflowlevelNo + 1);
                if (workFlowLevel != null)
                {
                    request.TeamId = workFlowLevel.TeamId;
                    request.NextWorkflowlevelNo = workFlowLevel.LevelNo;
                    request.WorkflowLevelType = workFlowLevel.WorkflowLevelType;
                }
                else
                {
                    request.NextWorkflowlevelNo = 0;
                    request.WorkflowLevelType = WorkflowLevelTypeEnum.NA;
                    request.Status = AccessRequestStatusEnum.Completed;
                }
                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    _service.InsertUpdateStatus(request, comment, teamId);

                    if (request.Status == AccessRequestStatusEnum.Completed)
                    {
                        #region Email
                        UserService _userService = new UserService();
                        var implementedUser = _userService.GetAsNoTrackingUserByEmpNo(request.UpdatedBy, "");
                        String emailBody = "";
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();

                        string reserverEmail = "";
                        if (request.AccessType == AllocatedTypeEnum.Internal)
                            reserverEmail = request.RequestedUser.Email;
                        else
                            reserverEmail = request.CreatedUser.Email;

                        if (!string.IsNullOrEmpty(reserverEmail))
                        {
                            ToRecipients.Add(reserverEmail);
                            if (!string.IsNullOrEmpty(request.CreatedUser.Email))
                            {
                                if (request.CreatedUser.Email != reserverEmail)
                                    CCRecipients.Add(request.CreatedUser.Email);

                            }
                            emailBody = emailTemplate.AccessRequestComplete(implementedUser.FullName, request, comment);
                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, request.AccessRequestType.Name + " Completed | Request # " + request.AccessRequestId,
                                emailBody).Trim();
                        }
                        #endregion
                    }
                    else
                    {
                        #region Email
                        try
                        {
                            var emailRequest = _service.GetItemAsNoTracking(request.AccessRequestId, "AccessRequestType,RequestedUser,Team");
                            EmailTemplate emailTemplate = new EmailTemplate();
                            EmailClient emailClient = new EmailClient();
                            List<string> ToRecipients = new List<string>();
                            List<string> CCRecipients = new List<string>();
                            if (!string.IsNullOrEmpty(emailRequest.Team.TeamEmail))
                            {
                                ToRecipients.Add(emailRequest.Team.TeamEmail);

                                String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New " + emailRequest.AccessRequestType.Name + " Request Pending | Request # " + emailRequest.AccessRequestId,
                                            emailTemplate.AccessRequestPending(emailRequest)).Trim();
                            }
                        }
                        catch (Exception ex) { }
                        #endregion
                    }
                    TempData["SuccessMessage"] = "Request has been updated.";
                    errorCode = 1;

                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult ImplementerReject(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "Team.UserTeams");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == AccessRequestStatusEnum.Pending && request.WorkflowLevelType == WorkflowLevelTypeEnum.Implementer
            && request.Team.UserTeams.Any(u => u.EmpNo == request.UpdatedBy))
            {
                request.Status = AccessRequestStatusEnum.Rejected;
                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    _service.InsertUpdateStatus(request, comment, request.TeamId);

                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.AccessRequestId, "CreatedUser,ApprovalByUser,AccessRequestType,RequestedUser,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.CreatedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.CreatedUser.Email);
                            if (!string.IsNullOrEmpty(emailRequest.ApprovalByUser.Email))
                                CCRecipients.Add(emailRequest.ApprovalByUser.Email);
                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, emailRequest.AccessRequestType.Name + " Request Rejected | Request # " + emailRequest.AccessRequestId,
                                        emailTemplate.AccessRequestRejectWorkflow(emailRequest, comment)).Trim();
                        }
                    }
                    catch (Exception ex) { }
                    #endregion
                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }


        #endregion


        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetDurationTypesJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();
            Array values = Enum.GetValues(typeof(AssignedTypeEnum));

            foreach (AssignedTypeEnum val in values)
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
        public JsonResult GetAccessTypesJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();
            Array values = Enum.GetValues(typeof(AllocatedTypeEnum));

            foreach (AllocatedTypeEnum val in values)
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
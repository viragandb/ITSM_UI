using Domain;
using Domain.CM;
using log4net;
using Newtonsoft.Json;
using Service;
using Service.CM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers.CM
{
    public class ChangeRequestController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AssetController));
        private readonly ChangeRequestService _service = new ChangeRequestService();
        private readonly UserService _userService = new UserService();

        [AccessAuthorize]
        public ActionResult Index(long? teamId, long? requestCategoryId, long? durationTypeId, long? changeAreaId, long? changeCategoryId
          , long? statusId, string startDate, string endDate, string requestId)
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
            if (requestCategoryId == null)
                requestCategoryId = 0;
            if (changeAreaId == null)
                changeAreaId = 0;
            if (changeCategoryId == null)
                changeCategoryId = 0;
            if (durationTypeId == null)
                durationTypeId = 0;
            if (statusId == null)
                statusId = 0;

            ViewBag.RequestId = requestId;
            ViewBag.TeamId = teamId;
            ViewBag.RequestCategoryId = requestCategoryId;
            ViewBag.ChangeAreaId = changeAreaId;
            ViewBag.ChangeCategoryId = changeCategoryId;
            ViewBag.DurationTypeId = durationTypeId;
            ViewBag.StatusId = statusId;
            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");

            var items = _service.GetItems(teamId, requestCategoryId, durationTypeId, changeAreaId, changeCategoryId, statusId, sDate, eDate.AddDays(1), requestId,
                "ChangeRequestCategory,RequestedUser,ChangeImplementData,ChangeImplementData.ChangeArea,Team").OrderBy(o=>o.ChangeRequestId).ToList();
            return View(items);

        }



        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {
            var item = new NewChangeRequestVM();
            item.ExpireDate = UserDateTime.GetUserDateOnly().ToString("dd/MM/yyyy");
            ViewBag.ExpireDate = item.ExpireDate;
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(NewChangeRequestVM item)
        {
            var dateExpire = UserDateTime.GetUserDateOnly();

            ViewBag.ChangeRequestCategoryId = item.ChangeRequestCategoryId;
            if (item.Subject == null || item.Justification == null || item.Description == null
                || item.DurationType == 0 || item.ApprovalBy == null || item.ChangeRequestCategoryId == 0)
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View("Create", item);
            }

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

            UserService _userService = new UserService();
            User requestedUser = new User();

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

            ChangeRequest newTicket = new ChangeRequest();
            newTicket.UpdatedBy = Session["UserId"].ToString();
            newTicket.UpdatedDate = UserDateTime.GetUserDate();
            newTicket.RequestedBy = newTicket.UpdatedBy;
            newTicket.RequestedDate = newTicket.UpdatedDate;
            newTicket.ApprovalBy = item.ApprovalBy;
            newTicket.Subject = item.Subject;
            newTicket.Description = item.Description;
            newTicket.Justification = item.Justification;
            newTicket.DurationType = item.DurationType;
            newTicket.ExpireDate = dateExpire;
            newTicket.Status = ChangeRequestStatusEnum.Initiated;
            newTicket.ChangeRequestCategoryId = item.ChangeRequestCategoryId;
            newTicket.SpentTimeSync = newTicket.RequestedDate;
            newTicket.SpentTime = 0;

            ChangeRequestCategoryService _changeRequestCategoryService = new ChangeRequestCategoryService();
            var changeCategory = _changeRequestCategoryService.GetAsNoTrackingItem(item.ChangeRequestCategoryId, "");
            newTicket.TeamId = changeCategory.TeamId;

            var entitySaved = _service.Insert(newTicket);
            if (entitySaved)
            {
                _service.InsertLog(newTicket, "New request initiated");
                _service.InsertUpdateStatus(newTicket, 0, "New Ticket was initiated");

                TicketDocService _ticketDocService = new TicketDocService();
                var tempDocs = _ticketDocService.GetTempDocs(newTicket.UpdatedBy, "");
                foreach (TempDoc _doc in tempDocs)
                {
                    ChangeRequestDoc doc = new ChangeRequestDoc();
                    doc.FileName = _doc.FileName;
                    doc.FileUrl = _doc.FileUrl;
                    doc.ChangeRequestId = newTicket.ChangeRequestId;
                    doc.DocType = DocTypeEnum.Attachment;
                    doc.DocumentName = _doc.DocumentName;
                    doc.UpdatedDate = UserDateTime.GetUserDate();
                    doc.UpdatedBy = Session["UserId"].ToString();
                    _service.InsertDoc(doc);
                    newTicket.UpdatedDate = doc.UpdatedDate;

                    _service.InsertLog(newTicket, "Document (" + doc.DocumentName + ") was uploaded");
                  
                    ItemDocService _docService = new ItemDocService();
                    _docService.DeleteTempDoc(_doc.TempDocId);
                }

                #region Email
                try
                {
                    var emailRequest = _service.GetItemAsNoTracking(newTicket.ChangeRequestId, "ApprovalByUser,RequestedUser");
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();
                    if (!string.IsNullOrEmpty(emailRequest.ApprovalByUser.Email))
                    {
                        ToRecipients.Add(emailRequest.ApprovalByUser.Email);
                        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New Change Request pending approval | Request # " + emailRequest.ChangeRequestId,
                                    emailTemplate.ChangeRequestCreate(emailRequest)).Trim();
                    }
                }
                catch (Exception ex) { }
                #endregion

                TempData["SuccessMessage"] = "Request has been successfully created.";
                return RedirectToAction("MyRequest");
            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View("Create", item);
            }

        }

        #region Temp Doc

        [AccessLogin]
        [HttpPost]
        public ActionResult UploadFiles()
        {
            try
            {
                string dirname = "UploadedFiles/";
                TicketDocService _ticketDocService = new TicketDocService();
                //string _FileName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(itemFile.FileName);
                //string _path = Path.Combine(Server.MapPath("~/" + dirname), _FileName);
                string _fileName = "";
                string path = Server.MapPath("~/" + dirname);
                HttpFileCollectionBase files = Request.Files;
                int vaildFilesCount = 0;
                string errorMsg = "";
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    _fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var supportedTypes = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".docx", ".doc", ".xlsx", ".xls" };
                    if (supportedTypes.Contains(Path.GetExtension(file.FileName).ToLower()))
                    {
                        if (file.ContentLength > 3145728) //3MB
                        {
                            errorMsg += "Please reduce file size (Max. 3MB) of " + file.FileName + "<br>";
                        }
                        else
                        {
                            file.SaveAs(path + _fileName);
                            var doc = new TempDoc();
                            doc.FileName = _fileName;
                            doc.FileUrl = "../" + dirname + _fileName;
                            doc.EmpNo = Session["UserId"].ToString();
                            doc.DocumentName = file.FileName;
                            doc.UpdatedDate = UserDateTime.GetUserDate();
                            doc.UpdatedBy = Session["UserId"].ToString();
                            _ticketDocService.InsertTempDoc(doc);

                            vaildFilesCount++;
                            TempData["SuccessMessage"] = file.FileName + " file has been uploaded.";
                        }
                    }
                    else
                    {
                        errorMsg += file.FileName + " is not a valid file type " + "<br>";

                    }
                }
                return Json(errorMsg + vaildFilesCount + " Files Uploaded!");
            }
            catch (Exception ex)
            {

                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString() + " Change Request File Upload ", ex);
                return Json(ex.Message.ToString());

            }
        }

        [HttpGet]
        [AccessLogin]
        public JsonResult GetTempDocListJsonResult()
        {
            TicketDocService _ticketDocService = new TicketDocService();
            var items = _ticketDocService.GetTempDocs(Session["UserId"].ToString(), "").ToList();

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [AccessLogin]
        [HttpGet]
        public ActionResult DeleteTempDoc(long? id)
        {
            int errorCode = 0;
            TicketDocService _ticketDocService = new TicketDocService();
            string fullPath = Server.MapPath("~/" + GlobalStaticService.GetFileUploadPath());

            if (_ticketDocService.DeleteTempDoc(id, fullPath))
                errorCode = 1;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }
        #endregion

        [HttpGet]
        [AccessLogin]
        [EncryptedActionParameter]
        public ActionResult Info(long? id)
        {
            if (id == null)
                return HttpNotFound();
            var item = _service.GetItem(id, "RequestedUser,ApprovalByUser,Team,ImplementedByUser,ChangeRequestCategory,ChangeApprovedByUser," +
                "ChangeImplementData,ChangeImplementData.ChangeArea,ChangeImplementData.ChangeRequestTasks,ChangeImplementData.ChangeRequestTasks.ChangeType,ChangeImplementData.DowntimeAlertEmails," +
                "ChangeRequestUpdates,ChangeRequestUpdates.UpdatedUser," +
                "ChangeRequestLogs,ChangeRequestLogs.UpdatedUser,ChangeRequestDocs,Assets,Assets.Asset,Assets.Asset.AssetType,Assets.Asset.AssetMake");

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
                "ChangeRequestCategory,RequestedUser").OrderByDescending(o => o.RequestedDate).ToList();

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
            if (request.Status == ChangeRequestStatusEnum.Initiated && request.RequestedBy == request.UpdatedBy)
            {
                request.Status = ChangeRequestStatusEnum.Rejected;

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    //_service.InsertLog(newTicket, "New request initiated");
                    _service.InsertUpdateStatus(request, 0, comment);

                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "ApprovalByUser,RequestedUser");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.ApprovalByUser.Email))
                        {
                            ToRecipients.Add(emailRequest.ApprovalByUser.Email);


                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Request Rejected | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.ChangeRequestCancel(emailRequest, comment)).Trim();
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
                "ChangeRequestCategory,RequestedUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Approve(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.Initiated && request.ApprovalBy == request.UpdatedBy)
            {

                request.Status = ChangeRequestStatusEnum.Pending;

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    _service.InsertUpdateStatus(request, 0, comment);

                    TempData["SuccessMessage"] = "Request has been updated.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.RequestedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.RequestedUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Request Approved | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.ChangeRequestApprove(emailRequest)).Trim();
                        }
                        ToRecipients.Clear();
                        if (!string.IsNullOrEmpty(emailRequest.Team.TeamEmail))
                        {
                            ToRecipients.Add(emailRequest.Team.TeamEmail);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New Change Pending | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.ChangeRequestPending(emailRequest)).Trim();
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
        public ActionResult Reject(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.Initiated && request.ApprovalBy == request.UpdatedBy)
            {
                request.Status = ChangeRequestStatusEnum.Rejected;

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    _service.InsertUpdateStatus(request, 0, comment);

                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.RequestedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.RequestedUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Request Rejected | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.ChangeRequestReject(emailRequest, comment)).Trim();
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

        #region Change Update

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Pending()
        {

            var items = _service.GetItemsPending(Session["UserId"].ToString(),
                "ChangeRequestCategory,RequestedUser,ApprovalByUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }


        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult Update(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = new ChangeImplementVM();
            item.ChangeRequest = _service.GetItemAsNoTracking(id, "ChangeRequestCategory,RequestedUser,ApprovalByUser,Team");
            item.ChangeRequestId = item.ChangeRequest.ChangeRequestId;

            //ViewBag.BackController = Session["BackController"].ToString();
            //ViewBag.BackAction = Session["BackAction"].ToString();

            //TicketTaskService _taskService = new TicketTaskService();
            //if (_taskService.IsTaskNotCompleted(item.TicketId))
            //{
            //    TempData["ErrorMessage"] = "Please complete all tasks";
            //    return RedirectToAction("Pending");
            //}
            Collection<AssetVM> assets = new Collection<AssetVM>();
            item.Assets = assets.ToList();

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult Update(ChangeImplementVM item)
        {
            item.ChangeRequest = _service.GetItemAsNoTracking(item.ChangeRequestId, "ChangeRequestCategory,RequestedUser,ApprovalByUser,Team,ChangeRequestUpdates");
            //ViewBag.BackController = Session["BackController"].ToString();
            //ViewBag.BackAction = Session["BackAction"].ToString();
            ViewBag.ChangeAreaId = item.ChangeAreaId;


            if (item.ChangeCategory == 0 || item.ChangePriority == 0 || item.ChangeAreaId == 0 || item.PlannedChange == null || item.PlannedChange == ""
             || item.RiskIdentification == null || item.RiskIdentification == ""
             || item.RollbackPlan == null || item.RollbackPlan == "" || item.Comment == null || item.Comment == "" || item.ChangeRequestTasks == null
             || (item.IsDowntimeRequired == true && (item.Downtime == "" || item.Downtime == null || item.DowntimeImpactedAreas == "" || item.DowntimeImpactedAreas == null))
             )
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View("Update", item);
                //  return View(item);
            }

            var change = _service.GetItem(item.ChangeRequestId, "");
            if (change.Status != ChangeRequestStatusEnum.Pending)
            {
                TempData["ErrorMessage"] = "Invalid Request.";
                return RedirectToAction("Pending");

            }
            var newItem = new ChangeImplementData();
            newItem.ChangeRequestId = item.ChangeRequestId;
            newItem.ChangeCategory = item.ChangeCategory;
            newItem.ChangePriority = item.ChangePriority;
            newItem.ChangeAreaId = item.ChangeAreaId;
            if (item.IsDowntimeRequired)
            {
                newItem.IsDowntimeRequired = true;
                newItem.Downtime = item.Downtime;
                newItem.DowntimeImpactedAreas = item.DowntimeImpactedAreas;
            }
            else
                newItem.IsDowntimeRequired = false;

            newItem.PlannedChange = item.PlannedChange;
            newItem.RiskAssessment = item.RiskAssessment;
            newItem.RiskIdentification = item.RiskIdentification;
            newItem.RollbackPlan = item.RollbackPlan;
            newItem.RollbackTestResults = item.RollbackTestResults;
            newItem.UpdatedBy = Session["UserId"].ToString();
            newItem.UpdatedDate = UserDateTime.GetUserDate();


            change.UpdatedBy = newItem.UpdatedBy;
            change.UpdatedDate = newItem.UpdatedDate;
            change.ImplementedBy = newItem.UpdatedBy;

            var lastUpdate = item.ChangeRequest.ChangeRequestUpdates.OrderByDescending(o => o.UpdatedDate).FirstOrDefault();
            DateTimeService _dateTimeService = new DateTimeService();

            //TicketUpdateService _ticketUpdateService = new TicketUpdateService();
            //var lastTicketUpdate = _ticketUpdateService.GetLastItem(item.TicketId, "");
            // double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastTicketUpdate.UpdatedDate, ticket.UpdatedDate);
            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, change.UpdatedDate);

            change.SpentTime = change.SpentTimeStatus + spentTimeStatus;
            change.SpentTimeStatus = change.SpentTime;
            change.SpentTimeSync = change.UpdatedDate;

            ChangeTypeService _changeTypeService = new ChangeTypeService();
            double sla = 0;

            Collection<ChangeRequestTask> tasks = new Collection<ChangeRequestTask>();

            foreach (var task in item.ChangeRequestTasks)
            {
                sla += _changeTypeService.GetAsNoTrackingItem(task.ChangeTypeId, "").Sla;

                ChangeRequestTask _t = new ChangeRequestTask();
                _t.ChangeTypeId = task.ChangeTypeId;
                _t.UpdatedBy = newItem.UpdatedBy;
                _t.UpdatedDate = newItem.UpdatedDate;
                tasks.Add(_t);

            }
            newItem.ChangeRequestTasks = tasks;
            change.Resolve = sla;
            ChangeImplementDataService _changeImplementDataService = new ChangeImplementDataService();

            change.ChangeImplementData = newItem;
            change.Status = ChangeRequestStatusEnum.PendingApproval;

            var assets = new Collection<ChangeRequestAsset>();
            if (item.Assets != null)
            {
                foreach (var asset in item.Assets)
                {
                    var _asset = new ChangeRequestAsset();
                    _asset.AssetId = asset.AssetId;
                    assets.Add(_asset);
                }
                change.Assets = assets;
            }

            var entitySaved = _service.Update(change);

            if (entitySaved)
            {

                _service.InsertUpdateStatus(change, spentTimeStatus, item.Comment);

                //_changeImplementDataService.Insert(newItem);
                //var statusUpdate = change;
                //statusUpdate.SpentTime = spentTimeStatus;
                //statusUpdate.Status = item.ChangeRequest.Status;

                #region EMail
                String emailBody = "";

                //var assigedUser = _userService.GetAsNoTrackingUserByEmpNo(item.AssignedTo, "");
                EmailTemplate emailTemplate = new EmailTemplate();
                EmailClient emailClient = new EmailClient();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                if (!String.IsNullOrEmpty(item.ChangeRequest.Team.TeamEmail))
                {
                    ToRecipients.Add(item.ChangeRequest.Team.TeamEmail);

                    emailBody = emailTemplate.ChangeRequestChangeApprove(item.ChangeRequest);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Approve Change Request | Request # " + item.ChangeRequestId,
                        emailBody).Trim();

                }
                #endregion

                TempData["SuccessMessage"] = "Request has been successfully updated.";
                return RedirectToAction("Pending");
                //return RedirectToAction(ViewBag.BackAction, ViewBag.BackController);

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View(item);
            }

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult PendingReject(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "Team.UserTeams");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();

            if (request.Status == ChangeRequestStatusEnum.Pending && request.Team.UserTeams.Any(t => t.EmpNo == request.UpdatedBy))
            {
                request.Status = ChangeRequestStatusEnum.Rejected;
                request.ImplementedBy = request.UpdatedBy;
                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    _service.InsertUpdateStatus(request, 0, comment);
                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ImplementedByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.RequestedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.RequestedUser.Email);
                            if (!string.IsNullOrEmpty(emailRequest.ApprovalByUser.Email))
                                CCRecipients.Add(emailRequest.ApprovalByUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Request Rejected | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.ChangeRequestChangeReject(emailRequest, comment)).Trim();
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

        #region Change Approval Pending

        [HttpGet]
        [AccessAuthorize]
        public ActionResult ChangeApprovalPending()
        {
            var items = _service.GetItemsPendingApproval(Session["UserId"].ToString(),
                "ChangeImplementData,ChangeImplementData.ChangeArea,ChangeRequestCategory,RequestedUser,ApprovalByUser,ImplementedByUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult ChangeApprove(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();

            if (request.Status == ChangeRequestStatusEnum.PendingApproval)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
             
                request.Status = ChangeRequestStatusEnum.Approved;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);

                request.ChangeApprovedBy = request.UpdatedBy;

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    TempData["SuccessMessage"] = "Request has been approved.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.Team.TeamEmail))
                        {
                            ToRecipients.Add(emailRequest.Team.TeamEmail);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Approved | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeApproved(emailRequest)).Trim();
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
        public ActionResult ChangeReject(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.PendingApproval)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                request.Status = ChangeRequestStatusEnum.Rejected;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.RequestedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.RequestedUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Rejected | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeRejected(emailRequest, comment)).Trim();
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
        public ActionResult ChangeSendToAVP(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.PendingApproval)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);
                request.Status = ChangeRequestStatusEnum.PendingApprovalAVP;
                request.ChangeApprovedBy = request.UpdatedBy;

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Request has been sent for higher approval.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ChangeRequestCategory,Team");
                        var emailUser = _userService.GetAsNoTrackingUserByEmpNo(UserService.GetUserId_IT_Infra_AVP(), "");

                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailUser.Email))
                        {
                            ToRecipients.Add(emailUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Approval Pending | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeSentHigherApproval(emailRequest)).Trim();
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
        public ActionResult ChangeSendToVP(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.PendingApproval)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);
                request.Status = ChangeRequestStatusEnum.PendingApprovalVP;

                request.ChangeApprovedBy = request.UpdatedBy;
                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Request has been sent for higher approval.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                        var emailUser = _userService.GetAsNoTrackingUserByEmpNo(UserService.GetUserId_IT_VP(), "");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailUser.Email))
                        {
                            ToRecipients.Add(emailUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Approval Pending | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeSentHigherApproval(emailRequest)).Trim();
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
        public ActionResult ChangeSendToCMC(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.PendingApproval)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);
                request.Status = ChangeRequestStatusEnum.PendingApprovalCMC;
                request.ChangeApprovedBy = request.UpdatedBy;

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Request has been sent for higher approval.";
                    errorCode = 1;

                    //#region Email
                    //try
                    //{
                    //    var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                    //    var emailUser = _userService.GetAsNoTrackingUserByEmpNo(UserService.GetUserId_IT_Infra_AVP(), "");
                    //    EmailTemplate emailTemplate = new EmailTemplate();
                    //    EmailClient emailClient = new EmailClient();
                    //    List<string> ToRecipients = new List<string>();
                    //    List<string> CCRecipients = new List<string>();
                    //    if (!string.IsNullOrEmpty(emailUser.Email))
                    //    {
                    //        ToRecipients.Add(emailUser.Email);

                    //        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Approval Pending | Request # " + emailRequest.ChangeRequestId,
                    //                    emailTemplate.CRChangeSentHigherApproval(emailRequest)).Trim();
                    //    }

                    //}
                    //catch (Exception ex) { }
                    //#endregion

                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region AVP Approval Pending

        [HttpGet]
        [AccessAuthorize]
        public ActionResult AVPApprovalPending()
        {
            var items = _service.GetItemsPendingApprovalAVP(
                "ChangeRequestCategory,RequestedUser,ApprovalByUser,ImplementedByUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult AVPApprove(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.PendingApprovalAVP)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                request.Status = ChangeRequestStatusEnum.Approved;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Request has been approved.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.Team.TeamEmail))
                        {
                            ToRecipients.Add(emailRequest.Team.TeamEmail);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Approved | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeApproved(emailRequest)).Trim();
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
        public ActionResult AVPReject(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.PendingApprovalAVP)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                request.Status = ChangeRequestStatusEnum.Rejected;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.RequestedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.RequestedUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Rejected | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeRejected(emailRequest, comment)).Trim();
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
        public ActionResult AVPChangeSendToVP(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.PendingApprovalAVP)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);
                request.Status = ChangeRequestStatusEnum.PendingApprovalVP;

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Request has been sent for higher approval.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                        var emailUser = _userService.GetAsNoTrackingUserByEmpNo(UserService.GetUserId_IT_VP(), "");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailUser.Email))
                        {
                            ToRecipients.Add(emailUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Approval Pending | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeSentHigherApproval(emailRequest)).Trim();
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

        #region VP Approval Pending

        [HttpGet]
        [AccessAuthorize]
        public ActionResult VPApprovalPending()
        {
            var items = _service.GetItemsPendingApprovalVP(
                "ChangeRequestCategory,RequestedUser,ApprovalByUser,ImplementedByUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult VPApprove(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.PendingApprovalVP)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                request.Status = ChangeRequestStatusEnum.Approved;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Request has been approved.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.Team.TeamEmail))
                        {
                            ToRecipients.Add(emailRequest.Team.TeamEmail);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Approved | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeApproved(emailRequest)).Trim();
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
        public ActionResult VPReject(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.PendingApprovalVP)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                request.Status = ChangeRequestStatusEnum.Rejected;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.RequestedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.RequestedUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Rejected | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeRejected(emailRequest, comment)).Trim();
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

        #region CMC Approval Pending

        [HttpGet]
        [AccessAuthorize]
        public ActionResult CMCApprovalPending()
        {
            var items = _service.GetItemsPendingApprovalCMC(
                "ChangeRequestCategory,RequestedUser,ApprovalByUser,ImplementedByUser").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult CMCApprove(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.PendingApprovalCMC)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                request.Status = ChangeRequestStatusEnum.Approved;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Request has been approved.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.Team.TeamEmail))
                        {
                            ToRecipients.Add(emailRequest.Team.TeamEmail);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Approved | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeApproved(emailRequest)).Trim();
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
        public ActionResult CMCReject(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.PendingApprovalCMC)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                request.Status = ChangeRequestStatusEnum.Rejected;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.RequestedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.RequestedUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Rejected | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeRejected(emailRequest, comment)).Trim();
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

        #region Implement


        [HttpGet]
        [AccessAuthorize]
        public ActionResult ImplementPending()
        {
            var items = _service.GetItemsPendingImplement(Session["UserId"].ToString(),
                "ChangeRequestCategory,RequestedUser,ApprovalByUser,ImplementedByUser,ChangeImplementData,ChangeImplementData.ChangeArea").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }


        [AccessAuthorize]
        [HttpGet]
        public ActionResult Complete(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.Approved)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                request.Status = ChangeRequestStatusEnum.Completed;

                _service.InsertUpdateStatus(request, spentTimeStatus, comment);

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Request has been approved.";
                    errorCode = 1;

                    //#region Email
                    //try
                    //{
                    //    var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                    //    EmailTemplate emailTemplate = new EmailTemplate();
                    //    EmailClient emailClient = new EmailClient();
                    //    List<string> ToRecipients = new List<string>();
                    //    List<string> CCRecipients = new List<string>();
                    //    if (!string.IsNullOrEmpty(emailRequest.Team.TeamEmail))
                    //    {
                    //        ToRecipients.Add(emailRequest.Team.TeamEmail);

                    //        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Approved | Request # " + emailRequest.ChangeRequestId,
                    //                    emailTemplate.CRChangeApproved(emailRequest)).Trim();
                    //    }

                    //}
                    //catch (Exception ex) { }

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
        public ActionResult ImplementReject(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.Completed)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                request.Status = ChangeRequestStatusEnum.Rejected;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.RequestedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.RequestedUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Rejected | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeRejectedByImplementer(emailRequest, comment)).Trim();
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
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult Downtime(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = new ChangeDowntimeVM();
            ChangeImplementDataService _imlementDataService = new ChangeImplementDataService();
            item.ChangeImplementData = _imlementDataService.GetAsNoTrackingItem(id, "ChangeArea,ChangeRequestTasks,ChangeRequestTasks.ChangeType,ChangeRequest,ChangeRequest.ChangeRequestCategory,ChangeRequest.RequestedUser" +
                ",ChangeRequest.ApprovalByUser,ChangeRequest.Team");

            item.DowntimePlannedDate = UserDateTime.GetUserDateOnly().ToString("dd/MM/yyyy");
            item.ChangeImplementDataId = item.ChangeImplementData.ChangeImplementDataId;
            item.ChangeRequestId = item.ChangeImplementData.ChangeRequestId;

            item.Downtime = item.ChangeImplementData.Downtime;
            item.DowntimeImpactedAreas = item.ChangeImplementData.DowntimeImpactedAreas;

            if (!item.ChangeImplementData.IsDowntimeRequired || item.ChangeImplementData.SendDowntimeAlert)
            {
                TempData["ErrorMessage"] = "Invalid Request.";
                return RedirectToAction("ImplementPending");
            }

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult Downtime(ChangeDowntimeVM item)
        {
            ChangeImplementDataService _imlementDataService = new ChangeImplementDataService();

            item.ChangeImplementData = _imlementDataService.GetAsNoTrackingItem(item.ChangeImplementDataId, "ChangeArea,ChangeRequestTasks,ChangeRequestTasks.ChangeType,ChangeRequest,ChangeRequest.ChangeRequestCategory,ChangeRequest.RequestedUser" +
              ",ChangeRequest.ApprovalByUser,ChangeRequest.Team,ChangeRequest.ChangeRequestCategory");
            //   item.ChangeRequest = _service.GetItemAsNoTracking(item.ChangeRequestId, "ChangeRequestCategory,RequestedUser,ApprovalByUser,Team,ChangeRequestUpdates");
            //  ViewBag.ChangeAreaId = item.ChangeAreaId;


            if (item.Downtime == null || item.Downtime == ""
             || item.DowntimeImpactedAreas == null || item.DowntimeImpactedAreas == ""
             || item.DowntimePlannedDate == null || item.DowntimePlannedDate == ""
             || item.DowntimeTimePeriod == null || item.DowntimeTimePeriod == "" || item.DowntimeAlertEmails == null
             || string.IsNullOrEmpty(item.DowntimeTimePurpose)
             )
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View("Downtime", item);
            }

            var _date = UserDateTime.GetUserDateOnly();

            try
            {
                _date = UserDateTime.ConvertToAppDateTime(item.DowntimePlannedDate);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Please enter valid date. ";
                return View("Downtime", item);
            }

            if (_date < UserDateTime.GetUserDateOnly())
            {
                TempData["ErrorMessage"] = "Please enter valid date. ";
                return View("Downtime", item);
            }

            //var change = _service.GetItem(item.ChangeRequestId, "");
            if (!item.ChangeImplementData.IsDowntimeRequired || item.ChangeImplementData.SendDowntimeAlert)
            {
                TempData["ErrorMessage"] = "Invalid Request.";
                return RedirectToAction("ImplementPending");

            }
            var implementData = _imlementDataService.GetItem(item.ChangeImplementDataId, "");
            implementData.Downtime = item.Downtime;
            implementData.DowntimeImpactedAreas = item.DowntimeImpactedAreas;
            implementData.DowntimeTimePeriod = item.DowntimeTimePeriod;
            implementData.DowntimeTimePurpose = item.DowntimeTimePurpose;
            implementData.DowntimePlannedDate = _date;
            implementData.SendDowntimeAlert = true;
            implementData.UpdatedBy = Session["UserId"].ToString();
            implementData.UpdatedDate = UserDateTime.GetUserDate();

            Collection<DowntimeAlertEmail> emails = new Collection<DowntimeAlertEmail>();
            foreach (var _e in item.DowntimeAlertEmails)
            {
                DowntimeAlertEmail email = new DowntimeAlertEmail();
                if (Regex.IsMatch(_e.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                {
                    email.Email = _e.Email;
                    emails.Add(email);
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid email - " + _e.Email;
                    return View("Downtime", item);
                }
            }
            implementData.DowntimeAlertEmails = emails;
            var entitySaved = _imlementDataService.Update(implementData);
            if (entitySaved)
            {

                #region EMail
                String emailBody = "";

                EmailTemplate emailTemplate = new EmailTemplate();
                EmailClient emailClient = new EmailClient();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                if (!String.IsNullOrEmpty(item.ChangeImplementData.ChangeRequest.Team.TeamEmail))
                {
                    foreach (var _e in item.DowntimeAlertEmails)
                    {
                        ToRecipients.Add(_e.Email);
                    }
                    ToRecipients.Add(item.ChangeImplementData.ChangeRequest.Team.TeamEmail);

                    emailBody = emailTemplate.Downtime(implementData);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Planned System Outage", emailBody).Trim();

                }
                #endregion

                _service.InsertLog(item.ChangeImplementData.ChangeRequest, "Downtime alert sent");

                TempData["SuccessMessage"] = "Downtime alert has been successfully sent.";

                return RedirectToAction("ImplementPending");


            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View(item);
            }

        }






        #region Review

        [HttpGet]
        [AccessAuthorize]
        public ActionResult ReviewPending()
        {
            var items = _service.GetItemsPendingReview(Session["UserId"].ToString(),
                "ChangeRequestCategory,RequestedUser,ApprovalByUser,ImplementedByUser,ChangeImplementData,ChangeImplementData.ChangeArea").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Review(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");

            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();

            if (request.Status == ChangeRequestStatusEnum.Completed)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);
                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                request.Status = ChangeRequestStatusEnum.Closed;

                _service.InsertUpdateStatus(request, spentTimeStatus, comment);

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Request has been reviewed.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ImplementedByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.RequestedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.RequestedUser.Email);
                            if (!string.IsNullOrEmpty(emailRequest.ApprovalByUser.Email))
                                CCRecipients.Add(emailRequest.ApprovalByUser.Email);
                            if (!string.IsNullOrEmpty(emailRequest.ImplementedByUser.Email))
                                CCRecipients.Add(emailRequest.ImplementedByUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Reviewed | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeClosed(emailRequest)).Trim();
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
        public ActionResult ReviewReject(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _service.GetItem(requestId, "");
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            if (request.Status == ChangeRequestStatusEnum.Completed)
            {
                var lastUpdate = _service.GetLastItem(requestId, "");
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;
                request.Status = ChangeRequestStatusEnum.Rejected;
                _service.InsertUpdateStatus(request, spentTimeStatus, comment);

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                    #region Email
                    try
                    {
                        var emailRequest = _service.GetItemAsNoTracking(request.ChangeRequestId, "RequestedUser,ApprovalByUser,ImplementedByUser,ChangeRequestCategory,Team");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(emailRequest.RequestedUser.Email))
                        {
                            ToRecipients.Add(emailRequest.RequestedUser.Email);
                            if (!string.IsNullOrEmpty(emailRequest.ApprovalByUser.Email))
                                CCRecipients.Add(emailRequest.ApprovalByUser.Email);
                            if (!string.IsNullOrEmpty(emailRequest.ImplementedByUser.Email))
                                CCRecipients.Add(emailRequest.ImplementedByUser.Email);

                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Change Rejected | Request # " + emailRequest.ChangeRequestId,
                                        emailTemplate.CRChangeRejectedByReviewer(emailRequest, comment)).Trim();
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
        public JsonResult GetChangeCategoriesJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();
            Array values = Enum.GetValues(typeof(ChangeCategoryEnum));

            foreach (ChangeCategoryEnum val in values)
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
            Array values = Enum.GetValues(typeof(ChangeRequestStatusEnum));

            foreach (ChangeRequestStatusEnum val in values)
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
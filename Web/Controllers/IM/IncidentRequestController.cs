using Domain;
using Domain.IM;
using log4net;
using Newtonsoft.Json;
using Service;
using Service.IM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers.IM
{
    public class IncidentRequestController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AssetController));
        private readonly IncidentRequestService _service = new IncidentRequestService();
        private readonly UserService _userService = new UserService();

        // GET: IncidentRequest

        [AccessAuthorize]
        public ActionResult Index(long? teamId, long? statusId, string startDate, string endDate)
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
            if (statusId == null)
                statusId = 0;

            ViewBag.TeamId = teamId;
            ViewBag.StatusId = statusId;
            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");

            var items = _service.GetItems(teamId, statusId, sDate, eDate.AddDays(1),
                "Level01Team,PendingTeam,RequestedUser").OrderByDescending(o => o.RequestedDate).ToList();
            return View(items);

        }



        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {
            var item = new NewIncidentRequestVM();
            item.OccurredDate = UserDateTime.GetUserDateOnly().ToString("dd/MM/yyyy");
            item.OccurredTime = UserDateTime.GetUserDate().ToShortTimeString();
            ViewBag.OccurredTime = item.OccurredTime;
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(NewIncidentRequestVM item, string Occurred_Time)
        {
            var dateOccurredDate = UserDateTime.GetUserDate();

            ViewBag.TeamId = item.TeamId;
            ViewBag.OccurredTime = Occurred_Time;
            if (item.Subject == null || item.Description == null || item.BusinessImpact == null
                || item.Impact == 0 || item.TeamId == 0)
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View("Create", item);
            }
            try
            {
                dateOccurredDate = UserDateTime.ConvertToAppDateTime(item.OccurredDate + " " + Occurred_Time);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Please enter valid Occurred Date/Time. ";
                return View(item);
            }

            if (dateOccurredDate > UserDateTime.GetUserDate())
            {
                TempData["ErrorMessage"] = "Please enter valid Occurred Date/Time. ";
                return View(item);
            }

            IncidentRequest newTicket = new IncidentRequest();
            newTicket.UpdatedBy = Session["UserId"].ToString();
            newTicket.UpdatedDate = UserDateTime.GetUserDate();
            newTicket.RequestedBy = newTicket.UpdatedBy;
            newTicket.RequestedDate = newTicket.UpdatedDate;
            newTicket.Subject = item.Subject;
            newTicket.Description = item.Description;
            newTicket.BusinessImpact = item.BusinessImpact;
            newTicket.Impact = item.Impact;
            newTicket.OccurredDate = dateOccurredDate;
            newTicket.Status = IncidentStatusEnum.Pending;
            newTicket.Level01TeamId = item.TeamId;
            newTicket.SpentTimeSync = newTicket.RequestedDate;
            newTicket.SpentTime = 0;
            newTicket.CreatedBy = newTicket.UpdatedBy;
            newTicket.PendingTeamId = newTicket.Level01TeamId;
            if (item.Tickets != null)
            {
                Collection<IncidentRequestTicket> tickets = new Collection<IncidentRequestTicket>();
                foreach (var ticket in item.Tickets)
                {
                    IncidentRequestTicket _t = new IncidentRequestTicket();
                    _t.TicketId = ticket.TicketId;
                    _t.UpdatedBy = newTicket.UpdatedBy;
                    _t.UpdatedDate = newTicket.UpdatedDate;
                    tickets.Add(_t);

                }
                newTicket.Tickets = tickets;
            }
            var entitySaved = _service.Insert(newTicket);
            if (entitySaved)
            {
                _service.InsertLog(newTicket, "New Incident was created");
                _service.InsertUpdateStatus(newTicket, 0, "New Incident was created");

                TicketDocService _ticketDocService = new TicketDocService();
                var tempDocs = _ticketDocService.GetTempDocs(newTicket.UpdatedBy, "");
                foreach (TempDoc _doc in tempDocs)
                {
                    IncidentRequestDoc doc = new IncidentRequestDoc();
                    doc.FileName = _doc.FileName;
                    doc.FileUrl = _doc.FileUrl;
                    doc.IncidentRequestId = newTicket.IncidentRequestId;
                    doc.DocType = DocTypeEnum.Attachment;
                    doc.DocumentName = _doc.DocumentName;
                    doc.UpdatedDate = UserDateTime.GetUserDate();
                    doc.UpdatedBy = Session["UserId"].ToString();
                    _service.InsertDoc(doc);

                    newTicket.UpdatedDate = doc.UpdatedDate;

                    _service.InsertLog(newTicket, "Document (" + doc.DocumentName + ") was uploaded");
                    string fullPath = Server.MapPath("~/" + GlobalStaticService.GetFileUploadPath());
                    _ticketDocService.DeleteTempDoc(_doc.TempDocId, fullPath);
                }

                #region Email
                try
                {
                    var emailRequest = _service.GetItemAsNoTracking(newTicket.IncidentRequestId, "Level01Team,RequestedUser");
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();
                    if (!string.IsNullOrEmpty(emailRequest.Level01Team.TeamEmail))
                    {
                        CCRecipients.Add("ITSM-Incident@12345.com");

                        ToRecipients.Add(emailRequest.Level01Team.TeamEmail);
                        if (!string.IsNullOrEmpty(emailRequest.RequestedUser.Email))
                            CCRecipients.Add(emailRequest.RequestedUser.Email);

                        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New Incident | Request # " + emailRequest.IncidentRequestId,
                                  emailTemplate.NewIncident(emailRequest)).Trim();
                    }
                }
                catch (Exception ex) { }
                #endregion

                TempData["SuccessMessage"] = "Request has been successfully submited.";
                return RedirectToAction("Create");
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
            var item = _service.GetItem(id, "Level01Team,PendingTeam,RequestedUser,IncidentRequestUpdates,IncidentRequestUpdates.UpdatedUser" +
                ",IncidentRequestLogs,IncidentRequestLogs.UpdatedUser,IncidentRequestDocs,Assets,Tickets,Tickets.Ticket," +
                "Assets.Asset,Assets.Asset.AssetType,Assets.Asset.AssetMake");

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }


        #region Pending

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Pending()
        {

            var items = _service.GetItemsPending(Session["UserId"].ToString(),
                "RequestedUser,Level01Team").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Update(long requestId, string comment)
        {
            UserTeamService _userTeamService = new UserTeamService();
            int errorCode = 0;
            TempData["ActiveRequestId"] = requestId;
            if (requestId > 0 && comment != "")
            {
                var userId = Session["UserId"].ToString();
                var request = _service.GetItem(requestId, "PendingTeam.UserTeams,IncidentRequestUpdates");
                if (request.Status < IncidentStatusEnum.Completed && request.PendingTeam.UserTeams.Any(u => u.EmpNo == userId))
                {
                    request.UpdatedBy = userId;
                    request.UpdatedDate = UserDateTime.GetUserDate();
                    request.Status = IncidentStatusEnum.InProgress;

                    var lastUpdate = request.IncidentRequestUpdates.OrderByDescending(o => o.UpdatedDate).FirstOrDefault();
                    DateTimeService _dateTimeService = new DateTimeService();
                    double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                    request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                    request.SpentTimeStatus = request.SpentTime;
                    request.SpentTimeSync = request.UpdatedDate;

                    var entitySaved = _service.Update(request);
                    if (entitySaved)
                    {
                        _service.InsertUpdateStatus(request, spentTimeStatus, comment);

                        errorCode = 1;
                        TempData["SuccessMessage"] = "Request has been successfully updated.";
                    }
                    else
                    {
                        errorCode = 2;
                        TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                    }
                }
                else
                {
                    errorCode = 2;
                    TempData["ErrorMessage"] = "Invalid request";
                }
            }
            else
            {
                errorCode = 2;
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Escalation(long requestId, long teamId, string comment)
        {
            UserTeamService _userTeamService = new UserTeamService();
            int errorCode = 0;
            TempData["ActiveRequestId"] = requestId;
            if (requestId > 0 && teamId > 0 && comment != "")
            {
                var userId = Session["UserId"].ToString();
                var request = _service.GetItem(requestId, "PendingTeam.UserTeams,IncidentRequestUpdates");
                if (request.Status < IncidentStatusEnum.Completed && request.PendingTeam.UserTeams.Any(u => u.EmpNo == userId))
                {
                    request.PendingTeamId = teamId;
                    request.UpdatedBy = userId;
                    request.UpdatedDate = UserDateTime.GetUserDate();
                    request.Status = IncidentStatusEnum.Pending;

                    var lastUpdate = request.IncidentRequestUpdates.OrderByDescending(o => o.UpdatedDate).FirstOrDefault();
                    DateTimeService _dateTimeService = new DateTimeService();
                    double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                    request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                    request.SpentTimeStatus = request.SpentTime;
                    request.SpentTimeSync = request.UpdatedDate;

                    var entitySaved = _service.Update(request);
                    if (entitySaved)
                    {
                        _service.InsertUpdateStatus(request, spentTimeStatus, "Escalated - " + comment);

                        errorCode = 1;
                        TempData["SuccessMessage"] = "Request has been successfully escalated.";

                        #region Email
                        try
                        {
                            // var emailRequest = _service.GetItemAsNoTracking(newTicket.IncidentRequestId, "Level01Team,RequestedUser");
                            EmailTemplate emailTemplate = new EmailTemplate();
                            EmailClient emailClient = new EmailClient();
                            List<string> ToRecipients = new List<string>();
                            List<string> CCRecipients = new List<string>();
                            if (!string.IsNullOrEmpty(request.PendingTeam.TeamEmail))
                            {
                                ToRecipients.Add(request.PendingTeam.TeamEmail);


                                String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Incident Escalation | Request # " + request.IncidentRequestId,
                                            emailTemplate.IncidentEscalation(request)).Trim();
                            }
                        }
                        catch (Exception ex) { }
                        #endregion
                    }
                    else
                    {
                        errorCode = 2;
                        TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                    }
                }
                else
                {
                    errorCode = 2;
                    TempData["ErrorMessage"] = "Invalid request";
                }
            }
            else
            {
                errorCode = 2;
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Impact(long requestId, long impactId, string comment)
        {
            UserTeamService _userTeamService = new UserTeamService();
            int errorCode = 0;
            TempData["ActiveRequestId"] = requestId;
            if (requestId > 0 && impactId > 0 && comment != "")
            {
                var userId = Session["UserId"].ToString();
                var request = _service.GetItem(requestId, "PendingTeam.UserTeams,IncidentRequestUpdates");
                var oldImpact = request.Impact;
                if (request.Status < IncidentStatusEnum.Completed && request.PendingTeam.UserTeams.Any(u => u.EmpNo == userId))
                {
                    request.Impact = (IncidentImpactEnum)impactId;
                    request.UpdatedBy = userId;
                    request.UpdatedDate = UserDateTime.GetUserDate();

                    //var lastUpdate = request.IncidentRequestUpdates.OrderByDescending(o => o.UpdatedDate).FirstOrDefault();
                    //DateTimeService _dateTimeService = new DateTimeService();
                    //double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                    //request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                    //request.SpentTimeStatus = request.SpentTime;
                    //request.SpentTimeSync = request.UpdatedDate;

                    var entitySaved = _service.Update(request);
                    if (entitySaved)
                    {
                        //_service.InsertUpdateStatus(request, spentTimeStatus, "Impact Changed - " + comment);
                        _service.InsertLog(request, "Impact Changed (" + oldImpact.EnumDisplayName() + " -> " + request.Impact.EnumDisplayName() + "). " + comment);
                        errorCode = 1;
                        TempData["SuccessMessage"] = "Request has been successfully updated.";
                    }
                    else
                    {
                        errorCode = 2;
                        TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                    }
                }
                else
                {
                    errorCode = 2;
                    TempData["ErrorMessage"] = "Invalid request";
                }
            }
            else
            {
                errorCode = 2;
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Vendor(long requestId, string comment)
        {
            UserTeamService _userTeamService = new UserTeamService();
            int errorCode = 0;
            TempData["ActiveRequestId"] = requestId;
            if (requestId > 0 && comment != "")
            {
                var userId = Session["UserId"].ToString();
                var request = _service.GetItem(requestId, "PendingTeam.UserTeams,IncidentRequestUpdates");
                var oldImpact = request.Impact;
                if (request.Status < IncidentStatusEnum.Completed && request.PendingTeam.UserTeams.Any(u => u.EmpNo == userId))
                {
                    request.UpdatedBy = userId;
                    request.UpdatedDate = UserDateTime.GetUserDate();
                    request.Status = IncidentStatusEnum.Vendor;
                    var lastUpdate = request.IncidentRequestUpdates.OrderByDescending(o => o.UpdatedDate).FirstOrDefault();
                    DateTimeService _dateTimeService = new DateTimeService();
                    double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                    request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                    request.SpentTimeStatus = request.SpentTime;
                    request.SpentTimeSync = request.UpdatedDate;

                    var entitySaved = _service.Update(request);
                    if (entitySaved)
                    {
                        _service.InsertUpdateStatus(request, spentTimeStatus, comment);
                        _service.InsertLog(request, "Escalated to the vendor.");
                        errorCode = 1;
                        TempData["SuccessMessage"] = "Request has been successfully updated.";
                    }
                    else
                    {
                        errorCode = 2;
                        TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                    }
                }
                else
                {
                    errorCode = 2;
                    TempData["ErrorMessage"] = "Invalid request";
                }
            }
            else
            {
                errorCode = 2;
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult Complete(long? id)
        {

            //ViewBag.BackController = Session["BackController"].ToString();
            //ViewBag.BackAction = Session["BackAction"].ToString();
            TempData["ActiveTicketId"] = id;

            if (id == null)
                return HttpNotFound();

            var item = new IncidentCompleteVM();
            item.IncidentRequest = _service.GetItemAsNoTracking(id, "RequestedUser,Level01Team,PendingTeam.UserTeams");
            item.IncidentRequestId = item.IncidentRequest.IncidentRequestId;
            if (item == null)
            {
                return HttpNotFound();
            }
            var userId = Session["UserId"].ToString();
            if (item.IncidentRequest.Status < IncidentStatusEnum.Completed && item.IncidentRequest.PendingTeam.UserTeams.Any(u => u.EmpNo == userId))
            {
                return View(item);
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid Request.";
                return RedirectToAction("Pending");

            }
        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult Complete(IncidentCompleteVM item)
        {
            //ViewBag.BackController = Session["BackController"].ToString();
            //ViewBag.BackAction = Session["BackAction"].ToString();
            var request = _service.GetItem(item.IncidentRequestId, "RequestedUser,Level01Team,IncidentRequestUpdates,PendingTeam.UserTeams");
            item.IncidentRequest = request;

            if (string.IsNullOrEmpty(item.RootCause) || string.IsNullOrEmpty(item.CorrectiveAction)
                || string.IsNullOrEmpty(item.PreventiveAction) || string.IsNullOrEmpty(item.LessonsLearnt) || string.IsNullOrEmpty(item.Comment))
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }
            var userId = Session["UserId"].ToString();
            if (request.Status < IncidentStatusEnum.Completed && request.PendingTeam.UserTeams.Any(u => u.EmpNo == userId))
            {
                request.RootCause = item.RootCause;
                request.CorrectiveAction = item.CorrectiveAction;
                request.PreventiveAction = item.PreventiveAction;
                request.LessonsLearnt = item.LessonsLearnt;

                request.UpdatedBy = Session["UserId"].ToString();
                request.UpdatedDate = UserDateTime.GetUserDate();
                request.Status = IncidentStatusEnum.Completed;

                var lastUpdate = request.IncidentRequestUpdates.OrderByDescending(o => o.UpdatedDate).FirstOrDefault();
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;

                var assets = new Collection<IncidentRequestAsset>();
                if (item.Assets != null)
                {
                    foreach (var asset in item.Assets)
                    {
                        var _asset = new IncidentRequestAsset();
                        _asset.AssetId = asset.AssetId;
                        _asset.UpdatedBy = request.UpdatedBy;
                        _asset.UpdatedDate = request.UpdatedDate;
                        assets.Add(_asset);
                    }
                    request.Assets = assets;
                }


                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    _service.InsertUpdateStatus(request, spentTimeStatus, item.Comment);
                    _service.InsertLog(request, "Completed");
                    TempData["SuccessMessage"] = "Request has been successfully updated.";

                    #region Email
                    try
                    {
                        // var emailRequest = _service.GetItemAsNoTracking(newTicket.IncidentRequestId, "Level01Team,RequestedUser");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(request.PendingTeam.TeamEmail))
                        {
                            ToRecipients.Add(request.PendingTeam.TeamEmail);
                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Incident Escalation | Request # " + request.IncidentRequestId,
                                        emailTemplate.IncidentCompleted(request)).Trim();
                        }
                    }
                    catch (Exception ex) { }
                    #endregion

                    return RedirectToAction("Pending");

                }
                else
                {
                    TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                    return View(item);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid Request.";
                return RedirectToAction("Pending");

            }


        }


        #endregion

        #region Review

        [HttpGet]
        [AccessAuthorize]
        public ActionResult PendingReview()
        {

            var items = _service.GetItemsReview(Session["UserId"].ToString(),
                "RequestedUser,Level01Team,PendingTeam").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }
        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult Review(long? id)
        {
            var item = new IncidentCompleteVM();
            item.IncidentRequest = _service.GetItemAsNoTracking(id, "RequestedUser,Level01Team,PendingTeam.UserTeams");
            var userId = Session["UserId"].ToString();
            if (item.IncidentRequest.Status == IncidentStatusEnum.Completed && item.IncidentRequest.PendingTeam.UserTeams.Any(u => u.EmpNo == userId))
            {
                TempData["ActiveTicketId"] = id;

                if (id == null)
                    return HttpNotFound();


                item.IncidentRequestId = item.IncidentRequest.IncidentRequestId;
                item.RootCause = item.IncidentRequest.RootCause;
                item.CorrectiveAction = item.IncidentRequest.CorrectiveAction;
                item.PreventiveAction = item.IncidentRequest.PreventiveAction;
                item.LessonsLearnt = item.IncidentRequest.LessonsLearnt;
                if (item == null)
                {
                    return HttpNotFound();
                }

                return View(item);
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid request";
                return RedirectToAction("PendingReview");

            }
        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult Review(IncidentCompleteVM item)
        {
            var request = _service.GetItem(item.IncidentRequestId, "IncidentRequestUpdates,RequestedUser,Level01Team,PendingTeam.UserTeams");
            item.IncidentRequest = request;

            if (string.IsNullOrEmpty(item.RootCause) || string.IsNullOrEmpty(item.CorrectiveAction)
                || string.IsNullOrEmpty(item.PreventiveAction) || string.IsNullOrEmpty(item.LessonsLearnt) || string.IsNullOrEmpty(item.Comment))
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }
            var userId = Session["UserId"].ToString();
            if (request.Status == IncidentStatusEnum.Completed && request.PendingTeam.UserTeams.Any(u => u.EmpNo == userId))
            {

                bool isRootCause = false;
                bool isCorrectiveAction = false;
                bool isPreventiveAction = false;
                bool isLessonsLearnt = false;
                if (request.RootCause != item.RootCause)
                    isRootCause = true;
                if (request.CorrectiveAction != item.CorrectiveAction)
                    isCorrectiveAction = true;
                if (request.PreventiveAction != item.PreventiveAction)
                    isPreventiveAction = true;
                if (request.LessonsLearnt != item.LessonsLearnt)
                    isLessonsLearnt = true;

                request.RootCause = item.RootCause;
                request.CorrectiveAction = item.CorrectiveAction;
                request.PreventiveAction = item.PreventiveAction;
                request.LessonsLearnt = item.LessonsLearnt;

                request.UpdatedBy = Session["UserId"].ToString();
                request.UpdatedDate = UserDateTime.GetUserDate();
                request.Status = IncidentStatusEnum.Reviewed;

                var lastUpdate = request.IncidentRequestUpdates.OrderByDescending(o => o.UpdatedDate).FirstOrDefault();
                DateTimeService _dateTimeService = new DateTimeService();
                // below method get the gap
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus; //total spend time plus gap
                request.SpentTimeStatus = request.SpentTime;  //time since last stat change
                request.SpentTimeSync = request.UpdatedDate;

                var assets = new Collection<IncidentRequestAsset>();
                if (item.Assets != null)
                {
                    foreach (var asset in item.Assets)
                    {
                        var _asset = new IncidentRequestAsset();
                        _asset.AssetId = asset.AssetId;
                        assets.Add(_asset);
                    }
                    request.Assets = assets;
                }

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    _service.InsertUpdateStatus(request, spentTimeStatus, item.Comment);
                    if (isRootCause)
                        _service.InsertLog(request, "Root Cause updated.");
                    if (isCorrectiveAction)
                        _service.InsertLog(request, "Corrective Action updated.");
                    if (isPreventiveAction)
                        _service.InsertLog(request, "Preventive Action updated.");
                    if (isLessonsLearnt)
                        _service.InsertLog(request, "Lessons Learnt updated.");

                    TempData["SuccessMessage"] = "Request has been successfully reviewed.";

                    #region Email
                    try
                    {
                        // var emailRequest = _service.GetItemAsNoTracking(newTicket.IncidentRequestId, "Level01Team,RequestedUser");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        ToRecipients.Add("ITSM-Incident@12345.com");
                        CCRecipients.Add(request.RequestedUser.Email);
                        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Incident Pending Closure| Request # " + request.IncidentRequestId,
                                    emailTemplate.IncidentReviewed(request)).Trim();
                    }
                    catch (Exception ex) { }
                    #endregion
                    return RedirectToAction("PendingReview");

                }
                else
                {
                    TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                    return View(item);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid request";
                return RedirectToAction("PendingReview");

            }

        }

        #endregion

        #region Close
        [HttpGet]
        [AccessAuthorize]
        public ActionResult PendingClose()
        {

            var items = _service.GetItemsClose(Session["UserId"].ToString(),
                "RequestedUser,Level01Team,PendingTeam").OrderByDescending(o => o.RequestedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }
        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult Close(long? id)
        {

            TempData["ActiveTicketId"] = id;

            if (id == null)
                return HttpNotFound();

            var item = new IncidentCompleteVM();
            item.IncidentRequest = _service.GetItemAsNoTracking(id, "RequestedUser,Level01Team");
            item.IncidentRequestId = item.IncidentRequest.IncidentRequestId;
            if (item.IncidentRequest.Status == IncidentStatusEnum.Reviewed)
            {
                item.RootCause = item.IncidentRequest.RootCause;
                item.CorrectiveAction = item.IncidentRequest.CorrectiveAction;
                item.PreventiveAction = item.IncidentRequest.PreventiveAction;
                item.LessonsLearnt = item.IncidentRequest.LessonsLearnt;
                if (item == null)
                {
                    return HttpNotFound();
                }

                return View(item);
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid request";
                return RedirectToAction("PendingClose");

            }
        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult Close(IncidentCompleteVM item)
        {
            var request = _service.GetItem(item.IncidentRequestId, "IncidentRequestUpdates,RequestedUser,Level01Team");
            item.IncidentRequest = request;

            if (string.IsNullOrEmpty(item.RootCause) || string.IsNullOrEmpty(item.CorrectiveAction)
                || string.IsNullOrEmpty(item.PreventiveAction) || string.IsNullOrEmpty(item.LessonsLearnt) || string.IsNullOrEmpty(item.Comment))
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }

            if (item.IncidentRequest.Status == IncidentStatusEnum.Reviewed)
            {
                bool isRootCause = false;
                bool isCorrectiveAction = false;
                bool isPreventiveAction = false;
                bool isLessonsLearnt = false;
                if (request.RootCause != item.RootCause)
                    isRootCause = true;
                if (request.CorrectiveAction != item.CorrectiveAction)
                    isCorrectiveAction = true;
                if (request.PreventiveAction != item.PreventiveAction)
                    isPreventiveAction = true;
                if (request.LessonsLearnt != item.LessonsLearnt)
                    isLessonsLearnt = true;

                request.RootCause = item.RootCause;
                request.CorrectiveAction = item.CorrectiveAction;
                request.PreventiveAction = item.PreventiveAction;
                request.LessonsLearnt = item.LessonsLearnt;

                request.UpdatedBy = Session["UserId"].ToString();
                request.UpdatedDate = UserDateTime.GetUserDate();
                request.Status = IncidentStatusEnum.Closed;

                var lastUpdate = request.IncidentRequestUpdates.OrderByDescending(o => o.UpdatedDate).FirstOrDefault();
                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastUpdate.UpdatedDate, request.UpdatedDate);

                request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                request.SpentTimeStatus = request.SpentTime;
                request.SpentTimeSync = request.UpdatedDate;

                var assets = new Collection<IncidentRequestAsset>();
                if (item.Assets != null)
                {
                    foreach (var asset in item.Assets)
                    {
                        var _asset = new IncidentRequestAsset();
                        _asset.AssetId = asset.AssetId;
                        assets.Add(_asset);
                    }
                    request.Assets = assets;
                }

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    _service.InsertUpdateStatus(request, spentTimeStatus, item.Comment);
                    if (isRootCause)
                        _service.InsertLog(request, "Root Cause updated.");
                    if (isCorrectiveAction)
                        _service.InsertLog(request, "Corrective Action updated.");
                    if (isPreventiveAction)
                        _service.InsertLog(request, "Preventive Action updated.");
                    if (isLessonsLearnt)
                        _service.InsertLog(request, "Lessons Learnt updated.");

                    TempData["SuccessMessage"] = "Request has been successfully reviewed.";
                    #region Email
                    try
                    {
                        // var emailRequest = _service.GetItemAsNoTracking(newTicket.IncidentRequestId, "Level01Team,RequestedUser");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!string.IsNullOrEmpty(request.RequestedUser.Email))
                        {
                            CCRecipients.Add("ITSM-Incident@12345.com");

                            ToRecipients.Add(request.RequestedUser.Email);
                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Incident Closed | Request # " + request.IncidentRequestId,
                                        emailTemplate.IncidentClosed(request)).Trim();
                        }
                    }
                    catch (Exception ex) { }
                    #endregion

                    return RedirectToAction("PendingClose");

                }
                else
                {
                    TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                    return View(item);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid request";
                return RedirectToAction("PendingClose");

            }

        }

        #endregion

        [HttpGet]
        [AccessLogin]
        public JsonResult GetImpactTypesJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();

            Array values = Enum.GetValues(typeof(IncidentImpactEnum));

            foreach (IncidentImpactEnum val in values)
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
            Array values = Enum.GetValues(typeof(IncidentStatusEnum));

            foreach (IncidentStatusEnum val in values)
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
using Domain;
using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Utility;

namespace Web.Controllers
{
    public class TicketTaskController : Controller
    {
        private readonly TicketTaskService _service = new TicketTaskService();
        // GET: TicketTask
        public ActionResult Index()
        {
            return View();
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create(long ticketId, long teamId, string taskName, string desc)
        {
            UserTeamService _userTeamService = new UserTeamService();
            int errorCode = 0;
            TempData["ActiveTicketId"] = ticketId;
            if (ticketId > 0 && teamId > 0 && taskName != "")
            {

                var task = new TicketTask();
                task.AllocatedTeamId = teamId;
                task.CreatedBy = Session["UserId"].ToString();
                task.CreatedDate = UserDateTime.GetUserDate();
                task.Description = desc;
                task.Subject = taskName;
                task.TicketId = ticketId;
                task.UpdatedBy = task.CreatedBy;
                task.UpdatedDate = task.CreatedDate;
                task.Status = TaskStatusEnum.Pending;
                task.SpentTime = 0;
                task.IsMyTeam = _userTeamService.IsTeamUser(task.CreatedBy, task.AllocatedTeamId);

                var entitySaved = _service.Insert(task);
                if (entitySaved)
                {
                    errorCode = 1;
                    TempData["SuccessMessage"] = "Task has been successfully created.";

                    TeamService _teamService = new TeamService();
                    var newTeam = _teamService.GetItemAsNoTracking(teamId, "");

                    #region EMail
                    String emailBody = "";
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();
                    if (!String.IsNullOrEmpty(newTeam.TeamEmail))
                    {
                        ToRecipients.Add(newTeam.TeamEmail);

                        emailBody = emailTemplate.TaskCreated(newTeam.TeamName, task);
                        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New Task Created | Task # " + task.TicketTaskId,
                            emailBody).Trim();

                    }
                    #endregion

                    // return RedirectToAction("Pending");
                }
                else
                {
                    errorCode = 2;
                    TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                }

            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Delete(long id)
        {
            UserTeamService _userTeamService = new UserTeamService();
            int errorCode = 0;
            var task = _service.GetItemAsNoTracking(id, "");
            task.UpdatedDate = UserDateTime.GetUserDate();
            task.UpdatedBy = Session["UserId"].ToString();
            TempData["ActiveTicketId"] = task.TicketId;
            var entitySaved = _service.Delete(id);
            if (entitySaved)
            {
                errorCode = 1;
                TempData["SuccessMessage"] = "Task has been successfully deleted.";
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
        public ActionResult Assign(long taskId, string userId)
        {
            int errorCode = 0;

            var task = _service.GetItem(taskId, "");
            TempData["ActiveTicketId"] = task.TicketId;
            if (taskId > 0 && userId != "")
            {
                task.AssignedBy = Session["UserId"].ToString();
                task.AssignedTo = userId;
                task.UpdatedBy = Session["UserId"].ToString();
                task.UpdatedDate = UserDateTime.GetUserDate();
                task.Status = TaskStatusEnum.Assigned;

                var entitySaved = _service.Update(task);
                if (entitySaved)
                {
                    errorCode = 1;
                    TempData["SuccessMessage"] = "Task has been successfully assigned.";

                    UserService _userService = new UserService();
                    var assigedUser = _userService.GetAsNoTrackingUserByEmpNo(task.AssignedTo, "");

                    #region EMail
                    String emailBody = "";
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();
                    if (!String.IsNullOrEmpty(assigedUser.Email))
                    {
                        ToRecipients.Add(assigedUser.Email);

                        emailBody = emailTemplate.TaskAssigned(assigedUser.FullName, task);
                        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New Task Assigned | Task # " + task.TicketTaskId,
                            emailBody).Trim();

                    }
                    #endregion

                }
                else
                {
                    errorCode = 2;
                    TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                }

            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Pending()
        {

            var items = _service.GetItemsPending(Session["UserId"].ToString(),
            "Ticket,AllocatedTeam,CreatedUser,AssignedByUser,AssignedToUser").OrderByDescending(o => o.CreatedDate).ToList();

            Session["BackController"] = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            Session["BackAction"] = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();


            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult PendingAssign(long? ticketTaskId, string userId)
        {
            int errorCode = 0;

            var item = _service.GetItem(ticketTaskId, "");


            if (item != null)
            {

                item.UpdatedBy = Session["UserId"].ToString();
                item.UpdatedDate = UserDateTime.GetUserDate();

                item.AssignedBy = item.UpdatedBy;
                item.AssignedTo = userId;
                item.Status = TaskStatusEnum.Assigned;

                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(item.CreatedDate, item.UpdatedDate);

                item.SpentTime = spentTimeStatus;

                var entitySaved = _service.Update(item);
                if (entitySaved)
                {
                    errorCode = 1;

                    TempData["SuccessMessage"] = "Ticket has been successfully assigned.";

                }
                else
                {
                    errorCode = 2;
                    TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                }

            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult AssignMe(long? ticketTaskId)
        {
            int errorCode = 0;

            var item = _service.GetItem(ticketTaskId, "");


            if (item != null)
            {
                UserService _userService = new UserService();

                item.UpdatedBy = Session["UserId"].ToString();
                item.UpdatedDate = UserDateTime.GetUserDate();

                item.AssignedBy = item.UpdatedBy;
                item.AssignedTo = item.UpdatedBy;
                item.Status = TaskStatusEnum.Assigned;

                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(item.CreatedDate, item.UpdatedDate);

                item.SpentTime = spentTimeStatus;

                var entitySaved = _service.Update(item);
                if (entitySaved)
                {
                    errorCode = 1;

                    TempData["SuccessMessage"] = "Ticket has been successfully assigned.";

                }
                else
                {
                    errorCode = 2;
                    TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                }

            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Reject(long? ticketTaskId, string comment)
        {
            int errorCode = 0;

            var item = _service.GetItem(ticketTaskId, "");

            if (item != null)
            {
                item.UpdatedBy = Session["UserId"].ToString();
                item.UpdatedDate = UserDateTime.GetUserDate();
                item.Comment = comment;
                item.Status = TaskStatusEnum.Rejected;

                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(item.CreatedDate, item.UpdatedDate);

                item.SpentTime = spentTimeStatus;
                var entitySaved = _service.Update(item);
                if (entitySaved)
                {
                    errorCode = 1;
                    TempData["SuccessMessage"] = "Task has been successfully rejected.";
                }
                else
                {
                    errorCode = 2;
                    TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                }

            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Assigned()
        {

            var items = _service.GetItemsAssigned(Session["UserId"].ToString(),
            "Ticket,AllocatedTeam,CreatedUser,AssignedByUser,AssignedToUser").OrderByDescending(o => o.CreatedDate).ToList();

            Session["BackController"] = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            Session["BackAction"] = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Complete(long? ticketTaskId, string comment)
        {
            int errorCode = 0;

            var item = _service.GetItem(ticketTaskId, "");

            if (item != null)
            {
                item.UpdatedBy = Session["UserId"].ToString();
                item.UpdatedDate = UserDateTime.GetUserDate();
                item.Comment = comment;
                item.Status = TaskStatusEnum.Completed;

                DateTimeService _dateTimeService = new DateTimeService();
                double spentTimeStatus = _dateTimeService.GetTicketLogTimes(item.CreatedDate, item.UpdatedDate);

                item.SpentTime = spentTimeStatus;
                var entitySaved = _service.Update(item);
                if (entitySaved)
                {
                    errorCode = 1;
                    TempData["SuccessMessage"] = "Task has been successfully completed.";
                }
                else
                {
                    errorCode = 2;
                    TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                }

            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

    }
}
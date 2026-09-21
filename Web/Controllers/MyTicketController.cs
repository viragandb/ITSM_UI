using Domain;
using Service;
using System;
using System.Linq;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers
{
    public class MyTicketController : Controller
    {
        private readonly TicketService _service = new TicketService();
        // GET: MyTicket
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Open()
        {
            var items = _service.GetItemsByRequestedUserOpen(Session["UserId"].ToString(),
               "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser,AssignedToUser,Branch,Department").OrderByDescending(o => o.RequestedDate).ToList();
            TicketStatusService _statusService = new TicketStatusService();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Complete()
        {
            var items = _service.GetItemsByRequestedUserCompleted(Session["UserId"].ToString(),
               "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser,Branch,AssignedToUser,Department").OrderByDescending(o => o.RequestedDate).ToList();
            TicketStatusService _statusService = new TicketStatusService();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [HttpGet]
        public ActionResult RateByEmail(string id)
        {

            if (id == null || id == "")
                return RedirectToAction("Index", "Login");

            id = GlobalStaticService.Decrypt(id.ToString());

            var ticket = _service.GetItem(Convert.ToInt64(id), "RequestedUser");

            if (ticket.Status == TicketStatusEnum.Completed)
            {
                UserService _userService = new UserService();
                bool isLogingSucess = _userService.IsUserLoged(ticket.RequestedBy, false, UserDateTime.GetUserDate());

            }
            //var user = _userService.GetUserByEmpNo(ticket.RequestedBy, "");

            //if (user.UserStatus == UserStatusEnum.Active)
            //{


            //    int timeOut =30;
            //    var formTicket = new FormsAuthenticationTicket(user.EmpNo, false, timeOut);
            //    string formEncrypt = FormsAuthentication.Encrypt(formTicket);

            //    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, formEncrypt)
            //    {
            //        Expires = DateTime.Now.AddMinutes(timeOut),
            //        HttpOnly = true
            //    };

            //    Response.Cookies.Add(cookie);

            //    Session["UserId"] = user.EmpNo;
            //    Session["UserName"] = user.FullName;
            //    Session["CompanyId"] = user.CompanyId;
            //    Session["CompanyName"] = user.CompanyName;
            //    if (!string.IsNullOrEmpty(user.ProImageName))
            //    {
            //        Session["ProfileImage"] = user.ProImageName;
            //    }
            //    user.LastActiveDate = UserDateTime.GetUserDate();
            //    _userService.Update(user);

            //}

            string queryString = "id=" + ticket.TicketId;
            string enSrt = GlobalStaticService.Encrypt(queryString, DateTime.Now.ToString("dd-MM-yyyy"));
            return RedirectToAction("Rate", "MyTicket", new { q = enSrt });

        }

        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult Rate(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = new TicketRateVM();
            item.Ticket = _service.GetItemAsNoTracking(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,TicketUpdates,RequestedUser,Branch,Department");
            if (item.Ticket.Status != TicketStatusEnum.Completed)
                return RedirectToAction("Complete");

            item.TicketId = item.Ticket.TicketId;
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }


        [HttpPost]
        [AccessAuthorize]
        public ActionResult Rate(TicketRateVM item, string switchReopen)
        {
            //if (ModelState.IsValid)
            //{

            bool reopen = false;
            if (switchReopen == "on")
                reopen = true;


            item.Ticket = _service.GetItem(item.TicketId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,TicketUpdates,RequestedUser,Branch,Department");
            if (item.Comment == null || item.Rate == 0)
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }

            var ticket = _service.GetItem(item.TicketId, "");

            ticket.UpdatedBy = Session["UserId"].ToString();
            ticket.UpdatedDate = UserDateTime.GetUserDate();
            if (reopen)
                ticket.Status = TicketStatusEnum.Pending;
            else
            {
                ticket.Status = TicketStatusEnum.Rated;
                ticket.IsReopened = true;
                ticket.Rate = item.Rate;
            }
            double spentTimeStatus = 0;

            var entitySaved = _service.Update(ticket);
            if (entitySaved)
            {
                TeamService _teamService = new TeamService();
                var ticketStatus = ticket;
                ticketStatus.SpentTime = spentTimeStatus;
                ticketStatus.PendingTeamId = _teamService.GetTeamIdDefault();
                _service.InsertTicketStatus(ticketStatus, item.Comment);

                if (reopen)
                    _service.InsertTicketLog(ticket, "Ticket - Reopened by requester");
                else
                    _service.InsertTicketLog(ticket, "Ticket - closed ( Rate : " + item.Rate.EnumDisplayName() + "  )");
                //#region Email

                //String emailBody = "";
                //EmailTemplate emailTemplate = new EmailTemplate();
                //EmailClient emailClient = new EmailClient();
                //List<string> ToRecipients = new List<string>();
                //List<string> CCRecipients = new List<string>();
                //if (emailTicket.RequestedUser.Email != null && emailTicket.RequestedUser.Email != "")
                //{
                //    ToRecipients.Add(emailTicket.RequestedUser.Email);

                //    emailBody = emailTemplate.TicketUpdate(emailTicket.RequestedUser.FullName
                //        , emailTicket.Code, item.Status.EnumDisplayName(), item.Comment);
                //    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Ticket Updates",
                //        emailBody).Trim();

                //}
                //#endregion

                if (!reopen)
                    TempData["SuccessMessage"] = "Ticket has been closed.";
                else
                    TempData["SuccessMessage"] = "Ticket has been reopened.";

                return RedirectToAction("Complete");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View(item);
            }

            //***********

        }


        [HttpGet]
        [AccessAuthorize]
        public ActionResult Closed()
        {
            var items = _service.GetItemsByRequestedUserClosed(Session["UserId"].ToString(),
               "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser,TicketPriority,AssignedToUser,RequestedUser,Branch,Department").OrderByDescending(o => o.RequestedDate).ToList();
            TicketStatusService _statusService = new TicketStatusService();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }


    }
}
using Domain;
using Service;
using Service.AR;
using Service.CM;
using Service.IM;
using Service.RA;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Utility;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        [AccessLogin]
        public ActionResult Index()
        {
           

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            string userId = Session["UserId"].ToString();
        
            try
            {
                string controller = Session["NavController"].ToString();
                string action = Session["NavAction"].ToString();

                Session["NavController"] = null;
                Session["NavAction"] = null;
                return RedirectToAction( action, controller);

            }
            catch (Exception ex) { }
            TicketService _ticketService = new TicketService();
            UserTeamService _userTeamService = new UserTeamService();


            if (_userTeamService.IsTeamUser(userId))
            {
                var items = _ticketService.GetItemsNotCompleted("RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory" +
              ",RequestType.Team,Branch,Department").OrderByDescending(o => o.RequestedDate).ToList();
                return View(items);
             
            }
            else
            {
                var items = _ticketService.GetItemsNotCompletedUser(userId,"RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory" +
            ",RequestType.Team,Branch,Department").OrderByDescending(o => o.RequestedDate).ToList();
                return View(items);

                
            }

          
           
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }


        //[AccessLogin]
        [ChildActionOnly]
        public ActionResult AdminMenuLayout()
        {
            string userId = "";
            try
            {
                userId = Session["UserId"].ToString();
            }
            catch (Exception ex) { }
            var mvcArea = "";
            //try
            //{
            //    mvcArea = System.Web.HttpContext.Current.Request.RequestContext.RouteData.DataTokens["Area"].ToString();
            //}
            //catch (NullReferenceException)
            //{
            //    mvcArea = "";
            //}
            MenuItemFunctionService _menuItemFunctionService = new MenuItemFunctionService();
            MenuService _menuService = new MenuService();
            var menus = _menuService.GetMenus(userId, mvcArea, "");
            //var menus = _menuService.GetMenus(Convert.ToString(Session["UserId"]), mvcArea, "MenuItems");

            MenuItemService _menuItemService = new MenuItemService();
            foreach (Domain.Menu m in menus)
            {
                m.MenuItems = _menuItemService.GetMenuItems(m.MenuId, userId, "").OrderBy(o => o.MenuItemOrder).ToList();
            }

            return PartialView("_MenuLayout", menus);
        }

        //[AccessLogin]
        [ChildActionOnly]
        public ActionResult AdminMenuTitleLayout()
        {
            var mvcArea = "";
            try
            {
                mvcArea = System.Web.HttpContext.Current.Request.RequestContext.RouteData.DataTokens["Area"].ToString();
            }
            catch (NullReferenceException)
            {
                mvcArea = "";
            }
            MenuItemFunctionService _menuItemFunctionService = new MenuItemFunctionService();

            var mvcAction = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            var mvcConroller = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();

            var menuItem = _menuItemFunctionService.GetMenuItemFunction(mvcArea, mvcConroller, mvcAction, "MenuItem, MenuItem.Menu");

            return PartialView("_MenuTitleLayout", menuItem);
        }

        [ChildActionOnly]
        //[AccessLogin]
        public ActionResult NoticationPanel()
        {

            string userId = "";
            try {
                userId = Session["UserId"].ToString();
            }
            catch(Exception ex) { }
            NotificationService _notificationService = new NotificationService();
            var _logs = _notificationService.GetNotificationsByUser(userId, "NotificationCategory");

            var query = _logs.GroupBy(g => new { g.NotificationCategoryId, g.NotificationCategory.Subject, g.NotificationCategory.Icon, g.NotificationCategory.Action, g.NotificationCategory.Controller, g.NotificationCategory.Color })
                .Select(l => new
                {
                    log = l.Key
                 ,
                    NCount = l.Count()
                });

            Collection<NotificationCategory> notifications = new Collection<NotificationCategory>();

            foreach (var n in query)
            {
                NotificationCategory _n = new NotificationCategory();
                _n.Action = n.log.Action;
                _n.Controller = n.log.Controller;
                _n.Icon = n.log.Icon;
                _n.Subject = n.log.Subject;
                _n.NotificationCount = n.NCount;
                _n.Color = n.log.Color;
                notifications.Add(_n);
            }

            return PartialView("_NotificationPanel", notifications.ToList());
        }


        [ChildActionOnly]
        //[AccessLogin]
        public ActionResult CRNotification()
        {

            ChangeRequestService _requestService = new ChangeRequestService();
            var items = _requestService.GetItemsOngoing("Team.UserTeams");

              return PartialView("_CRNotification", items);
        }

        [ChildActionOnly]
        //[AccessLogin]
        public ActionResult IMNotification()
        {

            IncidentRequestService _requestService = new IncidentRequestService();
            var items = _requestService.GetItemsOngoing("PendingTeam.UserTeams");

            return PartialView("_IMNotification", items);
        }

        [ChildActionOnly]
        //[AccessLogin]
        public ActionResult ARNotification()
        {

            AccessRequestService _requestService = new AccessRequestService();
            var items = _requestService.GetItemsOngoing("Team.UserTeams");

            return PartialView("_ARNotification", items);
        }
        
        [ChildActionOnly]
        //[AccessLogin]
        public ActionResult RANotification()
        {
            TeamService _teamService = new TeamService();
            var teamIds = new Dictionary<string, long>
            {
                ["Risk"] = _teamService.GetTeamIdRisk(),
                ["HR"] = _teamService.GetTeamIdHR(),
                ["ITSecHead"] = _teamService.GetTeamIdITSecHead(),
                ["ITRemoteNet"] = _teamService.GetTeamIdITRemoteNet(),
                ["ITSec"] = _teamService.GetTeamIdITSec(),
                ["HelpDesk"] = TeamService.GetTeamIdHelpDesk()
            };

            ViewBag.TeamIds = teamIds;
            RemoteAccessRequestService _requestService = new RemoteAccessRequestService();
            var items = _requestService.GetItemsOngoing("Team");

            return PartialView("_RANotification", items);
        }
    }
}
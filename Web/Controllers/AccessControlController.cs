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
    public class AccessControlController : Controller
    {
        private readonly AccessControlService _accessControlService = new AccessControlService();
        private readonly GroupService _groupService = new GroupService();


        [AccessAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            var selectedGroup = Convert.ToInt64(form["GroupId"]);
            var selectedAccess = form["GroupAcess"];
            _accessControlService.GrantAccess(selectedGroup, selectedAccess, Session["UserId"].ToString());
            TempData["SuccessMessage"] = "Records have been successfully updated. ";
            ViewBag.GroupId = selectedGroup;
            return View();
        }
        [AllowAnonymous]
        public ActionResult GetGroup()
        {
            return Json(JsonConvert.SerializeObject(_groupService.GetGroups(""), Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetAccessListJsonResult(long groupId)
        {
            var accessList = new List<JsTreeViewModel>();
            string userId = Convert.ToString(Session["UserId"]);
            //var menus = _accessControlService.GetMenus(userId, "MenuItems,MenuItems.MenuItemFunctions");
            // var menus = _accessControlService.GetMenus(userId, "MenuItems,MenuItems.MenuItemFunctions");
            var menus = _accessControlService.GetMenus("MenuItems,MenuItems.MenuItemFunctions");

            var groupAccess = _accessControlService.GetUserGroupAccess("MenuItemFunction,MenuItemFunction.MenuItem", groupId);


            foreach (var menu in menus)
            {
                if (!menu.IsDeleted)
                {
                    var accessMenu = new JsTreeViewModel
                    {
                        Id = "U" + menu.MenuId.ToString("D"),
                        Text = menu.MenuName,
                        Icon = "fa-fw fa " + menu.IconName,
                        Parent = "#",
                        State = new State()
                    };
                    accessList.Add(accessMenu);
                    foreach (var menuItem in menu.MenuItems)
                    {
                        if (!menuItem.IsDeleted)
                        {
                            var accessMenuItems = new JsTreeViewModel
                            {
                                Id = "W" + menuItem.MenuItemId.ToString("D"),
                                Text = menuItem.MenuItemName,
                                Icon = "fa-fw fa " + menuItem.IconName,
                                Parent = "U" + menu.MenuId.ToString("D"),
                                State = new State()
                            };
                            accessList.Add(accessMenuItems);

                            foreach (var menuItemFunction in menuItem.MenuItemFunctions)
                            {
                                var accessMenuItemFunctions = new JsTreeViewModel
                                {
                                    Id = menuItemFunction.MenuItemFunctionId.ToString("D"),
                                    Text = menuItemFunction.MenuItemFunctionName,
                                    Icon = "fa-fw fa " + menuItem.IconName,
                                    Parent = "W" + menuItem.MenuItemId.ToString("D"),
                                    State =
                                        new State
                                        {
                                            Selected =
                                                groupAccess.Any(
                                                    e => e.MenuItemFunctionId == menuItemFunction.MenuItemFunctionId)
                                        }
                                };

                                accessList.Add(accessMenuItemFunctions);
                            }
                        }
                    }
                }
            }

            return Json(JsonConvert.SerializeObject(accessList.ToList()), JsonRequestBehavior.AllowGet);

        }

    }
}
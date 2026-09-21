using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Utility
{
    public class AccessAuthorizeAttribute : AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userService = new UserService();

            //var loggedUser = httpContext.User;
            //var loggedUserIdentity = loggedUser.Identity;

            //if (!loggedUserIdentity.IsAuthenticated)
            //{
            //    return false;
            //}

            bool isAuthorized = false;

            var routeData = httpContext.Request.RequestContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            var currentArea = routeData.DataTokens["Area"] as string ?? "";

            // var tenantId = Convert.ToInt64((httpContext.Session["TenantId"]) ?? 0);
            //var userName = loggedUserIdentity.Name;
            //try
            //{
            //    userName = userName.Split('\\')[1];
            //}
            //catch(Exception ex) { }
                
            //var systemUser = userService.GetUserByEmpNo(userName, "");
            
            //if (systemUser != null)
            //{
            //    isAuthorized = userService.HasAccessToFunction("", systemUser, currentArea, currentController, currentAction);
            //}

            var userName = "";
            try
            {
                userName = httpContext.Session["UserId"].ToString();
            }
            catch (Exception ex) { }

            if (userName != null)
            {
                isAuthorized = userService.HasAccessToFunction("", userName, currentArea, currentController, currentAction);
            }

            return isAuthorized;
        }


        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var routeData = filterContext.HttpContext.Request.RequestContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            var currentArea = routeData.DataTokens["Area"] as string ?? "";

            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                if (string.IsNullOrEmpty(currentArea))
                {
                    filterContext.Result = new RedirectResult("~/Home/AccessDenied");
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/" + currentArea + "/Dashboard/AccessDenied");
                }
            }

            if (filterContext.HttpContext.Session["UserId"] == null || Convert.ToString(filterContext.HttpContext.Session["UserId"]) == "")
            {
                filterContext.Result = new RedirectResult("~/Login");
            }
            //Remove Notifications
            NotificationService _notificationService = new NotificationService();
            _notificationService.RemoveNotifications(Convert.ToString(filterContext.HttpContext.Session["UserId"]), currentController, currentAction);

        }
    }
}
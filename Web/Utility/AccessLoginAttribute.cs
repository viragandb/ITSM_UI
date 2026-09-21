using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Utility
{
    public class AccessLoginAttribute : AuthorizeAttribute
    {
     
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var routeData = filterContext.HttpContext.Request.RequestContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            var currentArea = routeData.DataTokens["Area"] as string ?? "";

            base.OnAuthorization(filterContext);

           

            //if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            //{
            //    filterContext.Result = new RedirectResult("~/Login");
            //    return;
            //}
            //else
            //{
            //     Session["UserId"].ToString() = filterContext.HttpContext.User.Identity.Name;
            //}
            // string userId = filterContext.HttpContext.User.Identity.GetUserId();

            if (filterContext.HttpContext.Session["UserId"] == null || Convert.ToString(filterContext.HttpContext.Session["UserId"]) == "")
            {
                filterContext.Result = new RedirectResult("~/Login");
            }
        }
    }
}
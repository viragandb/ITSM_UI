using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Controller));
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);


            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.Configure();

            HtmlHelper.ClientValidationEnabled = true;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;

        }

        protected void Application_Error()
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

            //try
            //{
            //    var routeData = HttpContext.Current.Request.RequestContext.RouteData;
            //    var currentAction = routeData.GetRequiredString("action");
            //    var currentController = routeData.GetRequiredString("controller");

            //    Log.Error(currentController + "/" + currentAction + " - " + Session["UserId"].ToString() + " Application_Error", HttpContext.Current.Request.RequestContext.HttpContext.Error);

            //    Response.Redirect(urlHelper.Action("Error", "Home"));
            //}
            //catch (Exception ex) { }
        }
    }
}

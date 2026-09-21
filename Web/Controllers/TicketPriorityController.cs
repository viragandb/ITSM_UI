using Domain;
using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class TicketPriorityController : Controller
    {
        private readonly TicketPriorityService _service = new TicketPriorityService();

        // GET: TicketPriority
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetTicketPriorityListJsonResult(string urgency, string impact)
        {

            int urgencyId = 0;
            int impactId = 0;
            try
            {
                urgencyId = Convert.ToInt32(urgency);
                impactId = Convert.ToInt32(impact);
            }
            catch (Exception ex) { }
            if (urgencyId > 0 && impactId > 0)
            {
                var items = _service.GetAllByUugencynImpact((TicketUrgencyEnum)urgencyId, (TicketImpactEnum)impactId, "");
                //foreach(var item in items)
                //{
                //    item.TicketPriorityName = item.Priority.EnumDisplayName();
                //}
                return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(JsonConvert.SerializeObject(null), JsonRequestBehavior.AllowGet);
        }
    }
}
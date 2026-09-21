using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class DeliverySLAController : Controller
    {
        private readonly DeliverySLAService _service = new DeliverySLAService();

        // GET: DeliverySLA
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetItemListJsonResult()
        {

            var items = _service.GetAll("");
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

    }
}
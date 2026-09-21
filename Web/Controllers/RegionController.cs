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
    public class RegionController : Controller
    {
        private readonly RegionService _service = new RegionService();
        // GET: Region
        //public ActionResult Index()
        //{
        //    return View();
        //}


        [HttpGet]
        [AccessLogin]
        public JsonResult GetItemsJsonResult()
        {
            var items = _service.GetAll().ToList();

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }
    }
}
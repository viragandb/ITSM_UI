using Domain;
using log4net;
using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers
{
    public class SystemEnvironmentController : Controller
    {
        private readonly SystemEnvironmentService _service = new SystemEnvironmentService();
        private static readonly ILog Log = LogManager.GetLogger(typeof(AssetController));
        // GET: Workflow
        [AccessAuthorize]
        public ActionResult Index()
        {
            var items = _service.GetAll("System");

            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {
            var item = new SystemEnvironmentVM();

            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SystemEnvironmentVM item)
        {

            if (ModelState.IsValid)
            {
                ViewBag.AssetTypeId = item.AssetTypeId;


                if (item.EnvironmentType == 0  || item.AssetTypeId == 0)
                {
                    TempData["ErrorMessage"] = "Please enter required data. ";
                    return View("Create", item);
                }

                if (_service.ItemAvilable(item.EnvironmentType, item.AssetTypeId, ""))
                {
                    TempData["ErrorMessage"] = "Invalid, Data already exists.";
                    return View(item);

                }


                var newItem = new SystemEnvironment();
                newItem.EnvironmentType = item.EnvironmentType;
                newItem.AssetTypeId = item.AssetTypeId;
                newItem.UpdatedDate = UserDateTime.GetUserDate();
                newItem.UpdatedBy = Session["UserId"].ToString();
              
                var entitySaved = _service.Insert(newItem);
                if (entitySaved)
                {
                    TempData["SuccessMessage"] = "Data Saved successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry, something went wrong, Record couldn't save, Please contact the IT support.";
                }
            }

            return View(item);
        }


        [AccessAuthorize]
        [HttpGet]
        public ActionResult Delete(long? id)
        {
            int errorCode = 0;

            var item = _service.GetItem(id, "");
            item.UpdatedDate = UserDateTime.GetUserDate();
            item.UpdatedBy = Session["UserId"].ToString();
            if (item != null)
            {
                if (_service.HasRelationalData(item))
                    errorCode = 2;
                else if (_service.Delete(item))
                    errorCode = 1;
            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetSystemListJsonResult(int? envTypeId)
        {
            int envType = 0;
            try
            {
                envType = Convert.ToInt32(envTypeId);
            }
            catch (Exception ex) { }

            var items = _service.GetAllByEnvType((SystemEnvironmentTypeEnum)envType, "System").OrderBy(o => o.System.AssetTypeName);
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetEnvirnmentTypesJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();
            Array values = Enum.GetValues(typeof(SystemEnvironmentTypeEnum));

            foreach (SystemEnvironmentTypeEnum val in values)
            {

                Console.WriteLine(String.Format("{0}: {1}", Enum.GetName(typeof(SystemEnvironmentTypeEnum), val), val));
                EnumStatusVM item = new EnumStatusVM();
                item.Id = Convert.ToInt32(val);
                item.Name = val.EnumDisplayName();
                items.Add(item);
            }

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }


    }
}
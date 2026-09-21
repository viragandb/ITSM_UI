using Domain.AR;
using log4net;
using Newtonsoft.Json;
using Service;
using Service.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers.AR
{
    public class AccessApplicationController : Controller
    {
        private readonly AccessApplicationService _service = new AccessApplicationService();
        private static readonly ILog Log = LogManager.GetLogger(typeof(AssetController));

        [AccessAuthorize]
        public ActionResult Index()
        {
            var items = _service.GetAll("System,AccessPrivilegeCategory");

            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {
            var item = new AccessApplicationVM();

            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccessApplicationVM item)
        {

            ViewBag.AssetTypeId = item.AssetTypeId;
            ViewBag.AccessPrivilegeCategoryId = item.AccessPrivilegeCategoryId;
            if (ModelState.IsValid)
            {

                if (item.AssetTypeId == 0 || item.AccessPrivilegeCategoryId == 0)
                {
                    TempData["ErrorMessage"] = "Please enter required data. ";
                    return View("Create", item);
                }

                if (_service.ItemAvilable(item.AssetTypeId, ""))
                {
                    TempData["ErrorMessage"] = "Invalid, Data already exists.";
                    return View(item);

                }

                var newItem = new AccessApplication();
                newItem.AssetTypeId = item.AssetTypeId;
                newItem.AccessPrivilegeCategoryId = item.AccessPrivilegeCategoryId;
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
        public JsonResult GetItemsJsonResult()
        {
            var items = _service.GetAll("System").OrderBy(o => o.System.AssetTypeName);
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetItemsByCatJsonResult(long? privilegeCategoryId)
        {
            var items = _service.GetAllByPrivilegeCategoryId(privilegeCategoryId,"System").OrderBy(o => o.System.AssetTypeName);
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }
    }
}
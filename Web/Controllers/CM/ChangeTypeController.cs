using Domain.CM;
using log4net;
using Newtonsoft.Json;
using Service.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Utility;

namespace Web.Controllers.CM
{
     public class ChangeTypeController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AssetController));
        private readonly ChangeTypeService _service = new ChangeTypeService();

        [AccessAuthorize]
        public ActionResult Index()
        {
            var items = _service.GetAll("ChangeArea").OrderBy(o=>o.ChangeArea.Name).ToList();

            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChangeType item)
        {

            if (ModelState.IsValid)
            {
                if (item.Name == null || item.Name == "" || item.ChangeAreaId == 0)
                {
                    TempData["ErrorMessage"] = "Please enter required data. ";
                    return View(item);
                }

                item.UpdatedDate = UserDateTime.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();
                var entitySaved = _service.Insert(item);
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
            else
            {
                TempData["ErrorMessage"] = "Sorry, something went wrong, Record couldn't save, Please contact the IT support.";
            }

            return View(item);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Edit(long? id)
        {
            if (id == null)
                return HttpNotFound();

            var item = _service.GetItem(id, "ChangeArea");

            ViewBag.ChangeAreaId = item.ChangeAreaId;

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChangeType item)
        {
            if (ModelState.IsValid)
            {
                if (item.Name == null || item.Name == "" || item.ChangeAreaId == 0)
                {
                    TempData["ErrorMessage"] = "Please enter required data. ";
                    return View(item);
                }
                item.UpdatedDate = UserDateTime.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();

                var entityUpdated = _service.Update(item);
                if (entityUpdated)
                {
                    TempData["SuccessMessage"] = "Data updated successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry, something went wrong, Record couldn't update, Please contact the IT support.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Sorry, something went wrong, Record details couldn't update, Please contact the IT support.";
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
            var items = _service.GetAll("").OrderBy(o => o.Name);
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetItemsByAreaJsonResult(long? changeAreaId)
        {
            var items = _service.GetAllByArea(changeAreaId, "").OrderBy(o => o.Name);
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }
    }
}
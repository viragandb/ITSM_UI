using Domain;
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
    public class AssetTypeController : Controller
    {
        private readonly AssetTypeService _service = new AssetTypeService();

        [AccessAuthorize]
        public ActionResult Index(long? assetCategoryId)
        {
            if (assetCategoryId == null)
                assetCategoryId = 0;
            var items = _service.GetAll(assetCategoryId,"");

            ViewBag.AssetCategoryId = assetCategoryId;

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
        public ActionResult Create( AssetType item)
        {

            if (ModelState.IsValid)
            {
                item.UpdatedDate = UserDateTime.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();
                if(_service.IsUpdated(item.AssetTypeName))
                {
                    TempData["ErrorMessage"] = "Sorry, something went wrong, Record already exists.";
                    return View(item);

                }
                var entitySaved = _service.Insert(item);
                if (entitySaved)
                {
                    TempData["SuccessMessage"] = "Data Saved successfully";
                    ViewBag.AssetCategoryId = Convert.ToInt32(item.AssetCategory);
                    return RedirectToAction("Index", new { assetCategoryId = (int)item.AssetCategory });
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

            var item = _service.GetItem(id, "");
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( AssetType item)
        {
            if (ModelState.IsValid)
            {
                item.UpdatedDate = UserDateTime.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();
                if (_service.IsUpdated(item.AssetTypeName))
                {
                    TempData["ErrorMessage"] = "Sorry, something went wrong, Record already exists.";
                    return View(item);

                }
                var entityUpdated = _service.Update(item);
                if (entityUpdated)
                {
                    TempData["SuccessMessage"] = "Data updated successfully";
                    //return RedirectToAction("Index");
                    return RedirectToAction("Index", new { assetCategoryId = (int)item.AssetCategory });
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
        public JsonResult GetAssetTypeListJsonResult()
        {

            var items = _service.GetAll("").OrderBy(o=> o.AssetTypeName);
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetAssetTypeByTicketTypeListJsonResult(string ticketType, long? categoryId)
        {
            int ticketTypeId = 0;
            try
            {
                ticketTypeId = Convert.ToInt32(ticketType);
            }
            catch (Exception ex) { }

            var items = _service.GetAllByTicketType((TicketTypeEnum)ticketTypeId, categoryId, "").OrderBy(o => o.AssetTypeName);
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetAssetTypeByAssetCategoryListJsonResult(string assetCategory)
        {
            int assetCat = 0;
            try
            {
                assetCat = Convert.ToInt32(assetCategory);
            }
            catch (Exception ex) { }

            var items = _service.GetAllByAssetCategory((AssetCategoryEnum)assetCat, "").OrderBy(o => o.AssetTypeName);
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetAssetTypeSystemAppListJsonResult()
        {

            var items = _service.GetAllByAssetCategory(AssetCategoryEnum.System, "").OrderBy(o => o.AssetTypeName);
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetAssetCategoryJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();
            Array values = Enum.GetValues(typeof(AssetCategoryEnum));

            foreach (AssetCategoryEnum val in values)
            {

                Console.WriteLine(String.Format("{0}: {1}", Enum.GetName(typeof(AssetCategoryEnum), val), val));
                EnumStatusVM item = new EnumStatusVM();
                item.Id = Convert.ToInt32(val);
                item.Name = val.EnumDisplayName();
                items.Add(item);
            }

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetAssetOwnershipTypeJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();
            Array values = Enum.GetValues(typeof(AllocatedTypeEnum));

            foreach (AllocatedTypeEnum val in values)
            {
                Console.WriteLine(String.Format("{0}: {1}", Enum.GetName(typeof(AllocatedTypeEnum), val), val));
                EnumStatusVM item = new EnumStatusVM();
                item.Id = Convert.ToInt32(val);
                item.Name = val.EnumDisplayName();
                items.Add(item);
            }

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }


    }
}
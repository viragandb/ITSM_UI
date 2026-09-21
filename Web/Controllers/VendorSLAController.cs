using Domain;
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
    public class VendorSLAController : Controller
    {
        private readonly VendorSLAService _service = new VendorSLAService();

        [AccessAuthorize]
        public ActionResult Index(long? vendorId)
        {
            if (vendorId == null)
                vendorId = 0;
            ViewBag.VendorId = vendorId;
            var items = _service.GetAll(vendorId, "Vendor,AssetType");
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
        public ActionResult Create(VendorSLA item)
        {

            if (ModelState.IsValid)
            {
                if (item.VendorId == 0 || item.AssetTypeId ==0 ||  item.SLA==0)
                {
                    TempData["ErrorMessage"] = "Please enter valid data.";
                    return View(item);
                }
                item.UpdatedDate = UserDateTime.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();

                if (_service.IsAvilable(item))
                {
                    TempData["ErrorMessage"] = "Invalid, Record already exists.";
                    return View(item);

                }

                var entitySaved = _service.Insert(item);
                if (entitySaved)
                {
                    TempData["SuccessMessage"] = "Data Saved successfully";
                    return RedirectToAction("Index", new { vendorId = item.VendorId });
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

            var item = _service.GetItem(id, "Vendor,AssetType");
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( VendorSLA item)
        {
            if (ModelState.IsValid)
            {
                if (item.SLA == 0)
                {
                    TempData["ErrorMessage"] = "Please enter valid data.";
                    return View(item);
                }

                var updateItem = _service.GetItem(item.VendorSLAId, "");
                updateItem.SLA = item.SLA;
                updateItem.UpdatedDate = UserDateTime.GetUserDate();
                updateItem.UpdatedBy = Session["UserId"].ToString();
                var entityUpdated = _service.Update(updateItem);
                if (entityUpdated)
                {
                    TempData["SuccessMessage"] = "Data updated successfully";
                    return RedirectToAction("Index", new { vendorId = updateItem.VendorId });
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

    }
}
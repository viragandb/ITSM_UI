using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Domain;
using Newtonsoft.Json;
using Service;
using Web.Models;
using Web.Utility;

namespace Web.Controllers
{
    public class ItemController : Controller
    {
        private readonly ItemService _service = new ItemService();

        [AccessAuthorize]
        public ActionResult Index()
        {
            var items = _service.GetAll("");

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
        public ActionResult Create([Bind(Include = "ItemId,ItemName,ItemSerialNo,ItemBarCode,ItemModelNo,ItemMake,ItemFixedAssetNo,ItemPurchaseDate,ItemExpiryDate,CompanyId,LocationId,DepartmentId,VendorId,AssetTypeId")] Item item)
        {
            if (ModelState.IsValid)
            {
                item.ItemInputDate = DateTime.Today;
                var entitySaved = _service.Insert(item);
                if (entitySaved)
                {
                    TempData["SuccessMessage"] = "Data Saved successfully";
                    return RedirectToAction("Create");
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

            ViewBag.LocationId = item.LocationId;
            ViewBag.CompanyId = item.CompanyId;
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemId,ItemName,ItemSerialNo,ItemBarCode,ItemModelNo,CompanyId,LocationId")] Item item)
        {
            if (ModelState.IsValid)
            {
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

            if (item != null)
            {
                _service.Delete(item);
                errorCode = 1;
            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        

            [HttpGet]
        [AllowAnonymous]
        public JsonResult GetItemJsonResult(string BarCode)
        {
            ItemService _itemService = new ItemService();
            Item item = _itemService.GetItemsByItemBarCode(BarCode, "Location,Department");

            ItemVM itemVM = new ItemVM();
            itemVM.ItemId = item.ItemId;
            itemVM.ItemName = item.ItemName;
            itemVM.ItemMake = item.ItemMake;
            itemVM.ItemBarCode = item.ItemBarCode;
            itemVM.ItemModelNo = item.ItemModelNo;
            itemVM.ItemSerialNo = item.ItemSerialNo;
            itemVM.ItemFixedAssetNo = item.ItemFixedAssetNo;
            itemVM.ItemCost = item.ItemCost;
            itemVM.ItemPurchaseDate = item.ItemPurchaseDate;
            itemVM.ItemExpiryDate = item.ItemExpiryDate;
            itemVM.ItemInputDate = item.ItemInputDate;
            itemVM.DepartmentName = item.Department.DepartmentName;

            return Json(JsonConvert.SerializeObject(itemVM), JsonRequestBehavior.AllowGet);

        }
    }
}

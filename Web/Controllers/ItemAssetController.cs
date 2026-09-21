using Domain;
using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Utility;

namespace Web.Controllers
{
    public class ItemAssetController : Controller
    {
        private readonly ItemAssetService _service = new ItemAssetService();

        // GET: ItemAsset
        public ActionResult Index()
        {
            return View();
        }


        [AccessLogin]
        [HttpGet]
        public ActionResult AssignAssetTicket(long itemId, long assetId)
        {
            int errorCode = 0;

            ItemAsset item = new ItemAsset();
            item.AssetId = assetId;
            item.ItemId = itemId;
            item.TicketId = item.ItemId;
            item.ItemType = ItemTypeEnum.Ticket;
            item.UpdatedBy = Session["UserId"].ToString();
            item.UpdatedDate = UserDateTime.GetUserDate();
            if (!_service.IsAvailable(item))
            {
                AssetService _assetService = new AssetService();
                var asset = _assetService.GetItemAsNoTracking(assetId, "");
                TicketService _ticketService = new TicketService();
                var ticket = _ticketService.GetAsNoTrackingItem(itemId, "");
                ticket.UpdatedBy = item.UpdatedBy;
                ticket.UpdatedDate = item.UpdatedDate;
                _ticketService.InsertTicketLog(ticket, asset.AssetNo +  " asset assigned.");

                var entitySaved = _service.Insert(item);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Asset has been updated.";
                    errorCode = 1;

                }
                else
                    errorCode = 2;

            }
            else
            {
                errorCode = 2;
                TempData["ErrorMessage"] = "Asset has already been added.";
            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }

        [AccessLogin]
        [HttpGet]
        public ActionResult RemoveAssetTicket(long itemId)
        {
            int errorCode = 0;

            var item = _service.GetItem(itemId, "");
            var entitySaved = _service.Delete(item);
            if (entitySaved)
            {
                AssetService _assetService = new AssetService();
                var asset = _assetService.GetItemAsNoTracking(item.AssetId, "");
                TicketService _ticketService = new TicketService();
                var ticket = _ticketService.GetAsNoTrackingItem(item.ItemId, "");
                ticket.UpdatedBy = item.UpdatedBy;
                ticket.UpdatedDate = item.UpdatedDate;
                _ticketService.InsertTicketLog(ticket, asset.AssetNo + " asset unassigned.");

                TempData["SuccessMessage"] = "Asset has been Unassigned.";
                errorCode = 1;

            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }

        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult AssignAssetJS(long? assetCode, long? ticketId, long? assetCategory)
        //{
        //    int errorCode = 0;
        //    ItemAssetService _itemAssetService = new ItemAssetService();
        //    if (assetCategory == 0 || assetCategory == null)
        //    {
        //        TempData["ErrorMessage"] = "Select a asset category.";
        //    }
        //    else
        //    {

        //        AssetItemDTO _item = new AssetItemDTO();

        //        if ((AssetCategoryEnum)assetCategory == AssetCategoryEnum.ITAsset || (AssetCategoryEnum)assetCategory == AssetCategoryEnum.DataCenter)
        //        {
        //            IMSItemService _imsService = new IMSItemService();
        //            _item = _imsService.GetItem(assetCode.ToString());
        //        }
        //        else if ((AssetCategoryEnum)assetCategory == AssetCategoryEnum.Telco)
        //        {
        //            TelcoItemService _telcoService = new TelcoItemService();
        //            _item = _telcoService.GetItem(assetCode.ToString());
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Invalid Asset Category.";
        //            errorCode = 2;
        //        }

        //        if (_item.IMSCode == null)
        //        {
        //            TempData["ErrorMessage"] = "Invalid Asset Code.";
        //            errorCode = 3;
        //        }
        //        else
        //        {
        //            AssetService _assetService = new AssetService();
        //            Asset asset = new Asset();


        //            asset = _assetService.GetItemsByCode(_item.IMSCode, "");

        //            if (asset == null)
        //            {
        //                asset = new Asset();
        //                asset.AssetCode = _item.IMSCode;
        //                asset.AssetName = _item.ItemName;
        //                asset.AssetTypeName = _item.ItemTypeName;
        //                asset.AssetCategory = (AssetCategoryEnum)assetCategory;
        //                asset.Description = _item.Description;
        //                asset.ModelName = _item.ModelNo;
        //                asset.SerialNo = _item.SerialNo;
        //                asset.UpdatedDate = UserDateTime.GetUserDate();
        //                asset.UpdatedBy = Session["UserId"].ToString();
        //                asset.Location = _item.Location;
        //                asset.Company = _item.Company;

        //                _assetService.Insert(asset);
        //            }
        //            else
        //            {
        //                asset.AssetName = _item.ItemName;
        //                asset.AssetTypeName = _item.ItemTypeName;
        //                asset.Description = _item.Description;
        //                asset.ModelName = _item.ModelNo;
        //                asset.SerialNo = _item.SerialNo;
        //                asset.UpdatedDate = UserDateTime.GetUserDate();
        //                asset.UpdatedBy = Session["UserId"].ToString();
        //                asset.Location = _item.Location;
        //                asset.Company = _item.Company;
        //                _assetService.Update(asset);

        //            }

        //            ItemAsset newItem = new ItemAsset();
        //            newItem.AssetId = asset.AssetId;
        //            newItem.ItemId = Convert.ToInt64(ticketId);
        //            newItem.ItemType = ItemTypeEnum.Ticket;
        //            newItem.UpdatedDate = UserDateTime.GetUserDate();
        //            newItem.UpdatedBy = Session["UserId"].ToString();



        //            if (!_itemAssetService.IsAvailable(newItem))
        //            {
        //                var entitySaved = _itemAssetService.Insert(newItem);
        //                if (entitySaved)
        //                {
        //                    ItemLogService _logService = new ItemLogService();
        //                    ItemLog log = new ItemLog();
        //                    log.ItemType = ItemTypeEnum.Ticket;
        //                    log.ItemId = newItem.ItemId;
        //                    log.Comment = "Asset " + newItem.AssetName + "(" + newItem.AssetCode + ")" + " has been assigned";
        //                    log.UpdatedBy = newItem.UpdatedBy;
        //                    log.UpdatedDate = newItem.UpdatedDate;
        //                    _logService.Insert(log);

        //                    TempData["SuccessMessage"] = "Asset has been successfully assigned.";
        //                    errorCode = 1;

        //                }
        //            }
        //            else
        //            {
        //                TempData["ErrorMessage"] = "Asset already exists.";
        //                errorCode = 4;
        //            }
        //        }
        //    }


        //    return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        //}


        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult AssignAssetJS(long? assetCode, long? ticketId, long? assetCategory)
        //{
        //    int errorCode = 0;
        //    ItemAssetService _itemAssetService = new ItemAssetService();
        //    if (assetCategory == 0 || assetCategory == null)
        //    {
        //        TempData["ErrorMessage"] = "Select a asset category.";
        //    }
        //    else
        //    {
        //        errorCode = AssignAsset(assetCode, ticketId, assetCategory, ItemTypeEnum.Ticket);

        //        if (errorCode == 1)
        //            TempData["SuccessMessage"] = "Asset has been successfully assigned.";
        //        else if (errorCode == 2)
        //            TempData["ErrorMessage"] = "Invalid Asset Category.";
        //        else if (errorCode == 3)
        //            TempData["ErrorMessage"] = "Invalid Asset Code.";
        //        else if (errorCode == 4)
        //            TempData["ErrorMessage"] = "Asset already exists.";
        //    }

        //    return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        //}

        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult AssignAssetJSChange(long? assetCode, long? itemId, long? assetCategory)
        //{
        //    int errorCode = 0;
        //    ItemAssetService _itemAssetService = new ItemAssetService();
        //    if (assetCategory == 0 || assetCategory == null)
        //    {
        //        TempData["ErrorMessage"] = "Select a asset category.";
        //    }
        //    else
        //    {
        //        errorCode = AssignAsset(assetCode, itemId, assetCategory, ItemTypeEnum.Change);

        //        if (errorCode == 1)
        //            TempData["SuccessMessage"] = "Asset has been successfully assigned.";
        //        else if (errorCode == 2)
        //            TempData["ErrorMessage"] = "Invalid Asset Category.";
        //        else if (errorCode == 3)
        //            TempData["ErrorMessage"] = "Invalid Asset Code.";
        //        else if (errorCode == 4)
        //            TempData["ErrorMessage"] = "Asset already assigned.";
        //    }

        //    return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        //}

        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult AssignAssetJSRelease(long? assetCode, long? itemId, long? assetCategory)
        //{
        //    int errorCode = 0;
        //    ItemAssetService _itemAssetService = new ItemAssetService();
        //    if (assetCategory == 0 || assetCategory == null)
        //    {
        //        TempData["ErrorMessage"] = "Select a asset category.";
        //    }
        //    else
        //    {
        //        errorCode = AssignAsset(assetCode, itemId, assetCategory, ItemTypeEnum.Release);

        //        if (errorCode == 1)
        //            TempData["SuccessMessage"] = "Asset has been successfully assigned.";
        //        else if (errorCode == 2)
        //            TempData["ErrorMessage"] = "Invalid Asset Category.";
        //        else if (errorCode == 3)
        //            TempData["ErrorMessage"] = "Invalid Asset Code.";
        //        else if (errorCode == 4)
        //            TempData["ErrorMessage"] = "Asset already assigned.";
        //    }

        //    return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        //}

        //public int AssignAsset(long? assetCode, long? ticketId, long? assetCategory, ItemTypeEnum type)
        //{
        //    int res = 0;
        //    ItemAssetService _itemAssetService = new ItemAssetService();
        //    AssetItemDTO _item = new AssetItemDTO();

        //    //if ((AssetCategoryEnum)assetCategory == AssetCategoryEnum.ITAsset || (AssetCategoryEnum)assetCategory == AssetCategoryEnum.DataCenter)
        //    //{
        //    //    IMSItemService _imsService = new IMSItemService();
        //    //    _item = _imsService.GetItem(assetCode.ToString());
        //    //}
        //    //else if ((AssetCategoryEnum)assetCategory == AssetCategoryEnum.Telco)
        //    //{
        //    //    TelcoItemService _telcoService = new TelcoItemService();
        //    //    _item = _telcoService.GetItem(assetCode.ToString());
        //    //}
        //    //else
        //    //{
        //    //    res = 2;
        //    //}

        //    if (_item.IMSCode == null)
        //    {
        //        res = 3;
        //    }
        //    else
        //    {
        //        AssetService _assetService = new AssetService();
        //        Asset asset = new Asset();


        //        //asset = _assetService.GetItemsByCode(_item.IMSCode, "");

        //        //if (asset == null)
        //        //{
        //        //    asset = new Asset();
        //        //    asset.AssetCode = _item.IMSCode;
        //        //    asset.AssetName = _item.ItemName;
        //        //    asset.AssetTypeName = _item.ItemTypeName;
        //        //    asset.AssetCategory = (AssetCategoryEnum)assetCategory;
        //        //    asset.Description = _item.Description;
        //        //    asset.ModelName = _item.ModelNo;
        //        //    asset.SerialNo = _item.SerialNo;
        //        //    asset.UpdatedDate = UserDateTime.GetUserDate();
        //        //    asset.UpdatedBy = Session["UserId"].ToString();
        //        //    asset.Location = _item.Location;
        //        //    asset.Company = _item.Company;

        //        //    _assetService.Insert(asset);
        //        //}
        //        //else
        //        //{

        //        //    asset.AssetName = _item.ItemName;
        //        //    asset.AssetTypeName = _item.ItemTypeName;
        //        //    asset.AssetCategory = (AssetCategoryEnum)assetCategory;
        //        //    asset.Description = _item.Description;
        //        //    asset.ModelName = _item.ModelNo;
        //        //    asset.SerialNo = _item.SerialNo;
        //        //    asset.UpdatedDate = UserDateTime.GetUserDate();
        //        //    asset.UpdatedBy = Session["UserId"].ToString();
        //        //    asset.Location = _item.Location;
        //        //    asset.Company = _item.Company;
        //        //    _assetService.Update(asset);

        //        //}

        //        ItemAsset newItem = new ItemAsset();
        //        newItem.AssetId = asset.AssetId;
        //        newItem.ItemId = Convert.ToInt64(ticketId);
        //        newItem.ItemType = type;
        //        newItem.UpdatedDate = UserDateTime.GetUserDate();
        //        newItem.UpdatedBy = Session["UserId"].ToString();



        //        if (!_itemAssetService.IsAvailable(newItem))
        //        {
        //            var entitySaved = _itemAssetService.Insert(newItem);
        //            if (entitySaved)
        //            {
        //                ItemLogService _logService = new ItemLogService();
        //                ItemLog log = new ItemLog();
        //                log.ItemType = type;
        //                log.ItemId = newItem.ItemId;
        //                //log.Comment = "Asset " + asset.AssetTypeName + " (" + asset.AssetCode + ")" + " has been assigned";
        //                log.UpdatedBy = newItem.UpdatedBy;
        //                log.UpdatedDate = newItem.UpdatedDate;
        //                _logService.Insert(log);

        //                res = 1;

        //            }
        //        }
        //        else
        //        {
        //            res = 4;
        //        }
        //    }


        //    return res;
        //}



    }
}
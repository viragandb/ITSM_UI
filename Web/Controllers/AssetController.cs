using Domain;
using Domain.DR;
using log4net;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Service;
using Service.DR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers
{
    public class AssetController : Controller
    {
        // GET: Asset
        private static readonly ILog Log = LogManager.GetLogger(typeof(AssetController));
        private readonly AssetService _service = new AssetService();
        private readonly AssetTransferRequestService _assetTransferService = new AssetTransferRequestService();
        private readonly GRNoteService _grnService = new GRNoteService();
        private readonly AssetLinkService _assetLinkService = new AssetLinkService();
        private readonly AssetLinkLogService _assetLinkLogService = new AssetLinkLogService();


        [AccessAuthorize]
        public ActionResult Index(long? assetCategoryId, long? assetTypeId, long? allocatedTypeId
            , long? branchId, long? departmentId,int[] statusId, string checkboxWarranty, string checkboxMaintenance, string checkboxToBeReturned
            ,string assetNo,string serialNo, string barcodeNo,string assignTo)
        {
            if (assetCategoryId == null)
                assetCategoryId = 0;
            if (assetTypeId == null)
                assetTypeId = 0;
            //if (statusId == null)
            //    statusId = 0;
            if (allocatedTypeId == null)
                allocatedTypeId = 0;
            if (branchId == null)
                branchId = 0;
            if (departmentId == null)
                departmentId = 0;

            bool warranty = false;
            ViewBag.Warranty = "";
            if (checkboxWarranty == "on")
            {
                warranty = true;
                ViewBag.Warranty = "checked";
            }

            bool maintenance = false;
            ViewBag.Maintenance = "";
            if (checkboxMaintenance == "on")
            {
                maintenance = true;
                ViewBag.Maintenance = "checked";
            }

            bool toBeReturned = false;
            ViewBag.ToBeReturned = "";
            if (checkboxToBeReturned == "on")
            {
                toBeReturned = true;
                ViewBag.ToBeReturned = "checked";
            }

            ViewBag.AssetNo = assetNo;
            ViewBag.SerialNo = serialNo;
            ViewBag.BarcodeNo = barcodeNo;
            ViewBag.AssignTo = assignTo;

            ViewBag.AssetCategoryId = assetCategoryId;
            ViewBag.BranchId = branchId;
            ViewBag.DepartmentId = departmentId;
            ViewBag.AssetTypeId = assetTypeId;
            ViewBag.StatusId = statusId;
            ViewBag.AllocatedTypeId = allocatedTypeId;
            var items = _service.GetItemsAdv(assetCategoryId, assetTypeId, allocatedTypeId, branchId, departmentId, statusId, UserDateTime.GetUserDate()
                , warranty,maintenance,toBeReturned, assetNo, serialNo, barcodeNo, assignTo,
                "AssetType,AssetMake,Branch,Department,AssignedToUser,Vendor,ResponsibleTeam");

            return View(items);


        }


        [AccessAuthorize]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult Update(long? id)
        {
            if (id == null)
                return HttpNotFound();

            var item = _service.GetItem(id, "AssetType,AssetMake,Branch,Department,AssignedToUser,Vendor,ResponsibleTeam");
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssetMakeId = item.AssetMakeId;
            ViewBag.VendorId = item.VendorId;
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Asset item)
        {
            ViewBag.AssetMakeId = item.AssetMakeId;
            ViewBag.VendorId = item.VendorId;

            if (ModelState.IsValid)
            {
                if (item.AssetMakeId == 0 || item.VendorId == 0 || item.AllocatedType == 0
                || item.ModelName == null || item.ModelName == "" || item.SerialNo == null || item.SerialNo == ""
                || item.Barcode == null || item.Barcode == "" || item.Description == null || item.Description == "")
                {
                    TempData["ErrorMessage"] = "Please enter required data. ";
                    return View(item);
                }

                var updateItem = _service.GetItem(item.AssetId, "");
                updateItem.AssetMakeId = item.AssetMakeId;
                updateItem.VendorId = item.VendorId;
                updateItem.ModelName = item.ModelName;
                updateItem.SerialNo = item.SerialNo;
                updateItem.Barcode = item.Barcode;
                updateItem.AllocatedType = item.AllocatedType;
                updateItem.Description = item.Description;
                updateItem.UpdatedDate = UserDateTime.GetUserDate();
                updateItem.UpdatedBy = Session["UserId"].ToString();
                var entityUpdated = _service.Update(updateItem);
                if (entityUpdated)
                {
                    TempData["SuccessMessage"] = "Data updated successfully";
                    return RedirectToAction("Index",new { assetCategoryId = (long)updateItem.AssetCategory});
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
        public ActionResult StockIn()
        {
            var item = new StockInAssetVM();
            Collection<TempAsset> assets = new Collection<TempAsset>();
            item.Assets = assets.ToList();
            item.PurchaseDate = UserDateTime.GetUserDateOnly().ToString("dd/MM/yyyy");
            item.ReceivedDate = UserDateTime.GetUserDateOnly().ToString("dd/MM/yyyy");

            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult StockIn(StockInAssetVM item)
        {

            ViewBag.VendorId = item.VendorId;
            ViewBag.PurchaseDate = item.PurchaseDate;
            ViewBag.ReceivedDate = item.ReceivedDate;
            if (item.AllocatedType == 0 || item.VendorId == 0
                || item.PONumber == null || item.PONumber == "" || item.GRNNumber == null || item.GRNNumber == ""
                || item.InvoiceNumber == null || item.InvoiceNumber == "")
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }
            var purchaseDate = UserDateTime.GetUserDate();
            var receivedDate = UserDateTime.GetUserDate();

            try
            {
                purchaseDate = UserDateTime.ConvertToAppDateTime(item.PurchaseDate);
                receivedDate = UserDateTime.ConvertToAppDateTime(item.ReceivedDate);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Please enter a valid Date. ";
                return View(item);
            }

            if (item.Assets.Count() == 0)
            {
                TempData["ErrorMessage"] = "Please add assets. ";
                return View(item);
            }

            ViewBag.PurchaseDate = purchaseDate.ToString("dd/MM/yyyy");
            ViewBag.ReceivedDate = receivedDate.ToString("dd/MM/yyyy");
            GRNote newItem = new GRNote();
            newItem.Description = item.Description;
            newItem.GRNNumber = item.GRNNumber;
            newItem.InvoiceNumber = item.InvoiceNumber;
            newItem.PONumber = item.PONumber;
            newItem.PurchaseDate = purchaseDate;
            newItem.ReceivedDate = receivedDate;
            newItem.VendorId = item.VendorId;
            newItem.UpdatedBy = Session["UserId"].ToString();
            newItem.UpdatedDate = UserDateTime.GetUserDate();

            SerialService _serialService = new SerialService();

            var assets = new Collection<Asset>();
            foreach (var asset in item.Assets)
            {
                if(_service.IsAssetNoAvb(asset.AssetNo))
                {
                    TempData["ErrorMessage"] = "Invalid Asset No. : "+ asset.AssetNo;
                    return View(item);
                }
                var _asset = new Asset();
                _asset.Code = _serialService.GetAssetSerial().ToString("00000");

                _asset.AssetCategory = (AssetCategoryEnum)asset.AssetCategoryId;
                _asset.AllocatedType = item.AllocatedType;
                _asset.AssetMakeId = asset.AssetMakeId;
                _asset.AssetName = asset.AssetMakeName + " " + asset.ModelName;
                _asset.AssetNo = asset.AssetNo;
                _asset.AssetTypeId = asset.AssetTypeId;
                _asset.Barcode = asset.Barcode;
                _asset.BranchId = BranchService.GetAssetStockInBranchId();
                _asset.DepartmentId = DepartmentService.GetAssetStockInDeptId();
                _asset.Description = asset.Description;
                _asset.ModelName = asset.ModelName;
                _asset.MaintenanceFrequency = MaintenanceFrequencyEnum.N_A;
                _asset.NextMaintenanceDate = receivedDate;
                _asset.Priority = PriorityEnum.Low;
                _asset.PurchasePrice = asset.PurchasePrice;
                _asset.ResponsibleTeamId = TeamService.GetTeamIdHelpDesk();
                _asset.SerialNo = asset.SerialNo;
                _asset.StockInDate = newItem.ReceivedDate;
                _asset.ToBeReturnedDate = _asset.StockInDate.AddMonths(asset.ToBeReturned);
                _asset.VendorId = newItem.VendorId;
                _asset.WarrantyPeriod = asset.WarrantyPeriod;
                _asset.WarrantyExpire = newItem.PurchaseDate.AddMonths(asset.WarrantyPeriod);
                _asset.UpdatedBy = newItem.UpdatedBy;
                _asset.UpdatedDate = newItem.UpdatedDate;
                _asset.Status = AssetStatusEnum.Opened;
                _asset.AssignedType = AssignedTypeEnum.Permanent;
                _asset.VerifiedDate = _asset.UpdatedDate;
                assets.Add(_asset);
            }

            newItem.Assets = assets;
            
            ItemDocService _docService = new ItemDocService();
            var tempDocs = _docService.GetTempDocs(newItem.UpdatedBy, "");

            var entitySaved = _grnService.Insert(newItem);
            if (entitySaved)
            {
                foreach (var a in newItem.Assets)
                {
                    _service.InsertLog(a, "Stock In");
                }
                _service.DeleteTempAssetsByUserId(Session["UserId"].ToString());

                foreach (TempDoc _doc in tempDocs)
                {
                    ItemDoc doc = new ItemDoc();
                    doc.ItemType = ItemTypeEnum.GRN;
                    doc.ItemId = newItem.GRNoteId;
                    doc.DocumentName = _doc.DocumentName;
                    doc.FileName = _doc.FileName;
                    doc.FileUrl = _doc.FileUrl;
                    doc.UpdatedDate = UserDateTime.GetUserDate();
                    doc.UpdatedBy = Session["UserId"].ToString();
                    _docService.Insert(doc);

                    //_service.InsertTicketLog(newTicket, "Document (" + doc.DocumentName + ") was uploaded");

                    _docService.DeleteTempDoc(_doc.TempDocId);
                }

                TempData["SuccessMessage"] = "GRN has been successfully updated.";
                return RedirectToAction("StockIn");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString() + " GRN - " + newItem.GRNNumber);
                return View(item);
            }

            // return View();
        }


        [HttpGet]
        [AccessLogin]
        [EncryptedActionParameter]
        public ActionResult Info(long? id)
        {


            if (id == null)
                return HttpNotFound();
            var item = _service.GetItemAsNoTracking(id, "GRNote,AssetType,AssetMake,Vendor,Branch,Department,ResponsibleTeam,AssignedToUser" +
                ",AssetLogs,AssetTransactionLogs,AssetLogs.UpdatedUser,AssetTransactionLogs.UpdatedUser,"+
                "ChildLinks,ChildLinks.ChildAsset,ParentLinks,ParentLinks.ParentAsset,ChildLinks.ChildAsset.AssetType,ParentLinks.ParentAsset.AssetType");

            AssetTransferRequestService _assetTransferService = new AssetTransferRequestService();
            item.TransferItemRequests = _assetTransferService.GetTransferItems(id, 
                "AssetTransferRequest,"+
                "AssetTransferRequest.InitiatedUser,"+
                "AssetTransferRequest.AuthorizedUser,"+
                "AssetTransferRequest.CompletedUser,"+
                "AssetTransferRequest.AssignedToUser,"+
                "AssetTransferRequest.Branch,"+
                "AssetTransferRequest.Department").ToList();

            ItemAssetService _itemAssetService = new ItemAssetService();
            item.Tickets = _itemAssetService.GetTickets(item.AssetId, "Ticket,Ticket.RequestType.Category,Ticket.RequestType.AssetType,Ticket.RequestType.SubCategory,Ticket.Branch,Ticket.RequestedUser").ToList();
            if (item == null)
            {
                return HttpNotFound();
            }

            // find the disposal request if any related to chosen asset
            DisposalRequestService disposalRequestService = new DisposalRequestService();
            var disposalItem = disposalRequestService.GetItemByAsset(id, "DisposalRequest");

            if (disposalItem != null)
            {
                ViewBag.DisposalRequestId = disposalItem.DisposalRequestId;
                ViewBag.Title = disposalItem.DisposalRequest?.Title;
                ViewBag.Status = disposalItem.DisposalRequest?.Status;
                ViewBag.RequestedDate = disposalItem.DisposalRequest?.RequestCreatedDate;
            }


            return View(item);

        }


        [HttpGet]
        [AccessLogin]
        [EncryptedActionParameter]
        public ActionResult GRNInfo(long? id)
        {
            GRNoteService _grnService = new GRNoteService();
            ItemDocService _docService = new ItemDocService();
            if (id == null)
                return HttpNotFound();
            var item = _grnService.GetItemAsNoTracking(id, "Vendor,Assets,Assets.AssetType,Assets.AssetMake,Assets.Branch,Assets.Department");
            item.ItemDocs = _docService.GetGRNDocs(item.GRNoteId, "").ToList();

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult MyAssets()
        {

            var item = new MyAssetsVM();

            item.Assets = _service.GetMyAssets(Session["UserId"].ToString(), "AssetType,AssetMake,Vendor,Branch,Department").ToList();
            item.AssetTransferRequests = _assetTransferService.GetMyPendingItems(Session["UserId"].ToString(), "InitiatedUser,AuthorizedUser,CompletedUser" +
                ",BranchFrom,DepartmentFrom,Branch,Department,AssignedToUser,TransferItems,TransferItems.AssignedToUser,TransferItems.Asset" +
                ",TransferItems.Asset.AssetType,TransferItems.Asset.AssetMake").ToList();
            return View(item);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Accept(long requestId, string comment)
        {
            int errorCode = 0;
            var request = _assetTransferService.GetItem(requestId, "TransferItems");
            if (request.Status == AssetTransStatusEnum.InTransit)
            {
                request.Status = AssetTransStatusEnum.Completed;
                request.CompletedBy = Session["UserId"].ToString();
                request.CompletedDate = UserDateTime.GetUserDate();
                request.UpdatedBy = request.AuthorizedBy;
                request.UpdatedDate = request.AuthorizedDate;
                request.CommentComplete = comment;
                var entitySaved = _assetTransferService.Update(request);
                if (entitySaved)
                {
                    foreach (var transferItem in request.TransferItems)
                    {
                        var asset = _service.GetItem(transferItem.AssetId, "");
                        asset.AssignedTo = request.AssignedTo;
                        asset.Status = AssetStatusEnum.Assigned;
                        asset.UpdatedBy = request.UpdatedBy;
                        asset.UpdatedDate = request.UpdatedDate;
                        asset.BranchId = request.BranchId;
                        asset.DepartmentId = request.DepartmentId;
                        asset.AssignedType = request.AssignedType;
                        _service.Update(asset);
                        _service.InsertLog(asset, "Assigned to " + asset.AssignedTo);

                    }

                    TempData["SuccessMessage"] = "Request has been completed.";
                    errorCode = 1;

                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }


        //[HttpGet]
        //[AccessAuthorize]
        //public ActionResult AssetCritical(long? assetCategoryId)
        //{
        //    if (assetCategoryId == null)
        //        assetCategoryId = 0;

        //    //var item = new AssignAssetVM();
        //    //item.ItemId = Convert.ToInt64(id);

        //    //ItemAssetService _itemAssetService = new ItemAssetService();
        //    //item.AssignedItems = _itemAssetService.GetAssets(ItemTypeEnum.Release, id, "Asset").ToList();

        //    var items = _service.GetCriticalAssets(assetCategoryId, "");

        //    ViewBag.AssetCategoryId = assetCategoryId;

        //    return View(items);
        //}



        //[HttpGet]
        //[AccessAuthorize]
        //public ActionResult AddCriticalAsset()
        //{

        //    //if (id == null)
        //    //    return HttpNotFound();
        //    var item = new Asset();
        //    return View(item);

        //}

        //[HttpPost]
        //[AccessAuthorize]
        //public ActionResult AddCriticalAsset(long? assetCode, long? assetCategory)
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


        //    if (_item.IMSCode == null)
        //    {
        //        TempData["ErrorMessage"] = "Please select valid asset category and item";
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
        //        //    asset.IsCritical = true;
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
        //        //    asset.IsCritical = true;
        //        //    _assetService.Update(asset);

        //        //}

        //        TempData["SuccessMessage"] = "Asset has been successfully created.";
        //        return RedirectToAction("AssetCritical", new { assetCategoryId = assetCategory });

        //    }


        //    return View();

        //}


        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult DeleteCriticalAsset(long? id)
        //{
        //    int errorCode = 0;

        //    var item = _service.GetItem(id, "");

        //    if (item != null)
        //    {
        //        item.IsCritical = false;
        //        _service.Update(item);
        //        errorCode = 1;
        //    }

        //    return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        //}


        #region Temp Asset

        [AccessLogin]
        [HttpGet]
        public ActionResult AddTempAsset(long assetCategoryId, string assetCategoryName, long assetTypeId, string assetTypeName,
            long assetMakeId, string assetMakeName, string modelName,string assetNo, string serialNo,string barcode
            , string warranty, string toBeReturned, string purchasedPrice, string description)
        {
            int errorCode = 0;
            var asset = new TempAsset();
            asset.AssetCategoryId = assetCategoryId;
            asset.AssetCategoryName = assetCategoryName;
            asset.AssetTypeId = assetTypeId;
            asset.AssetTypeName = assetTypeName;
            asset.AssetMakeId = assetMakeId;
            asset.AssetMakeName = assetMakeName;
            asset.ModelName = modelName;
            asset.AssetNo = assetNo;
            asset.SerialNo = serialNo;
            asset.Barcode = barcode;
            try
            {
                asset.WarrantyPeriod = Convert.ToInt32(warranty);
            }
            catch (Exception ex) { }
            try
            {
                asset.ToBeReturned = Convert.ToInt32(toBeReturned);
            }
            catch (Exception ex) { }
            try
            {
                asset.PurchasePrice = Convert.ToDouble(purchasedPrice);
            }
            catch (Exception ex) { }
            asset.Description = description;
            asset.EmpNo = Session["UserId"].ToString();
            var entitySaved = _service.InsertTempAsset(asset);
            if (entitySaved)
            {
                errorCode = 1;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);


        }

        [AccessLogin]
        [HttpGet]
        public ActionResult DeleteTempAsset(long? id)
        {
            int errorCode = 0;
            //TicketDocService _ticketDocService = new TicketDocService();
            if (_service.DeleteTempAsset(id))
                errorCode = 1;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AccessLogin]
        public JsonResult GetTempAssetListJsonResult()
        {
            //TicketDocService _ticketDocService = new TicketDocService();
            var items = _service.GetTempAssets(Session["UserId"].ToString(), "").ToList();

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [AccessLogin]
        [HttpPost]
        public ActionResult UploadFilesTempAssets(long assetCategoryId, string assetCategoryName
            , long assetTypeId, string assetTypeName,
            long assetMakeId, string assetMakeName, string modelName
            , string warranty, string toBeReturned, string purchasedPrice, string description
            )
        {
            try
            {
                string dirname = "UploadedFiles/";
                TicketDocService _ticketDocService = new TicketDocService();
                //string _FileName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(itemFile.FileName);
                //string _path = Path.Combine(Server.MapPath("~/" + dirname), _FileName);
                string _FileName = "";
                string path = Server.MapPath("~/" + dirname);
                HttpFileCollectionBase files = Request.Files;
                int vaildFilesCount = 0;
                string errorMsg = "";
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    _FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    //file.FileName
                    var supportedTypes = new[] { ".xlsx", ".xls" };
                    if (supportedTypes.Contains(Path.GetExtension(file.FileName).ToLower()))
                    {
                        if (file.ContentLength > 3145728) //3MB
                        {
                            errorMsg += "Please reduce file size (Max. 3MB) of " + file.FileName + "<br>";
                        }
                        else
                        {
                            path += _FileName;
                            file.SaveAs(path);

                            #region Excel Read
                            string connString = "";
                            if (Path.GetExtension(file.FileName).ToLower() == ".xls")
                            {
                                connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                                // result = Service.ImportExceltoDatabase(path1, connString, userId);
                            }
                            else if (Path.GetExtension(file.FileName).ToLower() == ".xlsx")
                            {
                                connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            }

                            OleDbConnection oledbConn = new OleDbConnection(connString);
                            DataTable dt = new DataTable();
                            try
                            {
                                oledbConn.Open();
                                using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", oledbConn))
                                {
                                    OleDbDataAdapter oleda = new OleDbDataAdapter();
                                    oleda.SelectCommand = cmd;
                                    DataSet ds = new DataSet();
                                    oleda.Fill(ds);

                                    dt = ds.Tables[0];

                                    if (dt.Rows.Count > 0)
                                    {
                                        //table tblObj = new table();
                                        foreach (DataRow row in dt.Rows)
                                        {
                                            //tblObj.Name = row["Name"].ToString();
                                            var asset = new TempAsset();
                                            asset.AssetCategoryId = assetCategoryId;
                                            asset.AssetCategoryName = assetCategoryName;
                                            asset.AssetTypeId = assetTypeId;
                                            asset.AssetTypeName = assetTypeName;
                                            asset.AssetMakeId = assetMakeId;
                                            asset.AssetMakeName = assetMakeName;
                                            asset.ModelName = modelName;
                                            asset.AssetNo = row[0].ToString();
                                            asset.SerialNo = row[1].ToString();
                                            asset.Barcode = row[2].ToString();
                                            //asset.AssetNo = row["AssetNo"].ToString();
                                            //asset.SerialNo = row["SerialNo"].ToString();
                                            //asset.Barcode = row["Barcode"].ToString();
                                            try
                                            {
                                                asset.WarrantyPeriod = Convert.ToInt32(warranty);
                                            }
                                            catch (Exception ex) { }
                                            try
                                            {
                                                asset.ToBeReturned = Convert.ToInt32(toBeReturned);
                                            }
                                            catch (Exception ex) { }
                                            try
                                            {
                                                asset.PurchasePrice = Convert.ToDouble(purchasedPrice);
                                            }
                                            catch (Exception ex) { }
                                            asset.Description = description;
                                            asset.EmpNo = Session["UserId"].ToString();
                                           
                                            var entitySaved = _service.InsertTempAsset(asset);
                                            if(!entitySaved)
                                            {
                                                errorMsg += asset.AssetNo + " Not updated <br>";

                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                errorMsg += ex.Message.ToString() + "<br>";

                            }
                            finally
                            {
                                oledbConn.Close();
                            }

                            #endregion



                            vaildFilesCount++;
                            TempData["SuccessMessage"] = file.FileName + " file has been uploaded.";
                        }
                    }
                    else
                    {
                        errorMsg += file.FileName + " is not a valid file type " + "<br>";

                    }
                }
                return Json(errorMsg + vaildFilesCount + " Files Uploaded!");
            }
            catch (Exception ex)
            {

                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString() + " Ticket File Upload ", ex);
                return Json(ex.Message.ToString());

            }
        }



        #endregion


        #region Link Assets

        [HttpGet]
        [AccessAuthorize]
        public ActionResult LinkAssets()
        {
            var model = new AssetLinkVM();
            return View(model);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult LinkAssets(AssetLinkVM model)
        {
            // update the items of the request

            if (model.ParentAssetId == 0)
            {
                TempData["ErrorMessage"] = "Please select a Parent to link";
                return View(model);
            }            

            // delete removed items
            if (model.RemovedAssets != null)
            {
                foreach (var asset in model.RemovedAssets)
                {
                    var item = _assetLinkService
                        .GetAssetLinkItem(model.ParentAssetId, asset.AssetId, "");

                    if (item != null)
                    {
                        item.IsDeleted = true;
                        item.UpdatedDate = UserDateTime.GetUserDate();
                        item.UpdatedBy = Session["UserId"].ToString();
                        
                        if(!_assetLinkService.Update(item))
                        {
                            TempData["ErrorMessage"] = "Could not remove link.";
                            return View(model);
                        }

                        // add log
                        if (!_assetLinkLogService.Insert(
                            model.ParentAssetId,
                            asset.AssetId,
                            AssetLinkActionEnum.Unlinked,
                            $"Unlinked Child Asset #{asset.AssetId} from Parent Asset #{model.ParentAssetId}.",
                            Session["UserId"].ToString(),
                            UserDateTime.GetUserDate(),
                            false
                            ))  
                        {
                            TempData["ErrorMessage"] = "Could not add log for link removal.";
                            return View(model);
                        }
                    }                   
                }

            }

            // add the newly selected items

            if (model.Assets != null)
            {
                foreach (var asset in model.Assets)
                {
                    var item = new AssetLink
                    {
                        ParentAssetId = model.ParentAssetId,
                        ChildAssetId = asset.AssetId,
                        UpdatedBy = Session["UserId"].ToString(),
                        UpdatedDate = UserDateTime.GetUserDate(),
                        IsDeleted = false
                    };

                    if(!_assetLinkService.Insert(item))
                    {
                        TempData["ErrorMessage"] = "Could not add new link.";
                        return View(model);
                    }

                    // add log
                    if (!_assetLinkLogService.Insert(
                        model.ParentAssetId,
                        asset.AssetId,
                        AssetLinkActionEnum.Linked,
                        $"Linked Child Asset #{asset.AssetId} to Parent Asset #{model.ParentAssetId}.",
                        Session["UserId"].ToString(),
                        UserDateTime.GetUserDate(),
                        false
                        ))
                    {
                        TempData["ErrorMessage"] = "Could not add log for adding new link.";
                        return View(model);
                    }
                }
            }


            //ViewBag.CurrentParentAssetId = model.ParentAssetId;
            //ViewBag.ParentBranchId = branchId;
            //ViewBag.ParentDepartmentId = deptId;
            //ViewBag.ParentAssetCategoryId = assetCatId;
            //ViewBag.ParentAssetTypeId = assetTypeId;
            //ViewBag.ParentAssetNo = assetNo;
            //ViewBag.ChildAssetNo = assetNo;

            TempData["SuccessMessage"] = "Links updated successfully.";
            return View(model);

        }


        [HttpGet]
        [AccessLogin]
        public JsonResult GetParentAssetListJsonResult(long? branchId, long? deptId, long? assetCatId, long? assetTypeId, string assetNo)
        {

            if ((branchId ?? 0) == 0 && (deptId ?? 0) == 0 && (assetCatId ?? 0) == 0 && (assetTypeId ?? 0) == 0 && string.IsNullOrEmpty(assetNo))
            {
                return Json(new List<AssetVM>(), JsonRequestBehavior.AllowGet);
            }

            if (assetCatId == null)
                assetCatId = 0;
            if (assetTypeId == null)
                assetTypeId = 0;
            assetNo = assetNo ?? "";

            var ParentAssetToLink = _service.GetParentAssetsToLink(branchId, deptId, assetCatId, assetTypeId, assetNo, "")
                .Select(e => new
                {
                    e.AssetId,
                    e.AssetNo,
                    e.AssetName,
                    e.SerialNo
                    
                }).ToList();

        

            return Json(ParentAssetToLink, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AccessLogin]
        public JsonResult GetLinkedChildAssetsJsonResult(long parentAssetId)
        {
            var items = _assetLinkService.GetAllChildAsset(parentAssetId,"ChildAsset")
            .Select(d => new
            {
                d.ChildAsset.AssetId,
                d.ChildAsset.AssetNo,
                d.ChildAsset.AssetName,
            }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AccessLogin]
        public JsonResult GetChildAssetListJsonResult(long? branchId, long? deptId, long? assetCatId, long? assetTypeId, string assetNo)
        {

            if ((branchId ?? 0) == 0 && (deptId ?? 0) == 0 && (assetCatId ?? 0) == 0 && (assetTypeId ?? 0) == 0 && string.IsNullOrEmpty(assetNo))
            {
                return Json(new List<AssetVM>(), JsonRequestBehavior.AllowGet);
            }

            if (assetCatId == null)
                assetCatId = 0;
            if (assetTypeId == null)
                assetTypeId = 0;
            assetNo = assetNo ?? "";

            //filter out child asset linked already

            var ChildAssetToLink = _service.GetChildAssetsToLink    (branchId, deptId, assetCatId, assetTypeId, assetNo, "")
                .Select(e => new
                {
                    e.AssetId,
                    e.AssetNo,
                    e.AssetName,
                    e.SerialNo
                    
                }).ToList();

            return Json(ChildAssetToLink, JsonRequestBehavior.AllowGet);
        }



        #endregion


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
        public JsonResult GetAssetStatusJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();
            Array values = Enum.GetValues(typeof(AssetStatusEnum));

            foreach (AssetStatusEnum val in values)
            {

                Console.WriteLine(String.Format("{0}: {1}", Enum.GetName(typeof(AssetStatusEnum), val), val));
                EnumStatusVM item = new EnumStatusVM();
                item.Id = Convert.ToInt32(val);
                item.Name = val.EnumDisplayName();
                items.Add(item);
            }

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        [AccessLogin]
        public JsonResult GetAssetListAvbJsonResult(long? branchId, long? deptId, long? assetCatId, long? assetTypeId)
        {
            if (assetCatId == null)
                assetCatId = 0;
            if (assetTypeId == null)
                assetTypeId = 0;
            var items = _service.GetItemsAvb(branchId, deptId, assetCatId, assetTypeId, "").ToList();

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AccessLogin]
        public JsonResult GetAssetListAvbNewTicketJsonResult(long? branchId,  long? assetTypeId,long? deptId,string userId)
        {
            if (assetTypeId == null)
                assetTypeId = 0;

            //List<Asset> assets = new List<Asset>();
            IEnumerable<Asset> assets = Enumerable.Empty<Asset>();

            var items = _service.GetItemsAvbNewTicket(branchId,  assetTypeId,deptId,  "").ToList();
            //var userAsset = new List<Asset> (items.Where(w => w.AssignedTo == userId).ToList());
            assets = assets.Concat(items.Where(w => w.AssignedTo == userId).ToList());
            assets = assets.Concat(items.Where(w => w.AssignedTo != userId).ToList());

            return Json(JsonConvert.SerializeObject(assets), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AccessLogin]
        public JsonResult GetAssetListAvbByUserDM(long? deviceManagementtype, string userId)
        {
            if (deviceManagementtype == null)
                deviceManagementtype = 0;


            var type = DeviceManagementTypeEnum.Managed;
            if(deviceManagementtype==0)
                type = DeviceManagementTypeEnum.Unmanaged;

            var items = _service.GetItemsAvbByUser(type, userId,(long)AssetCategoryEnum.ITAsset,_service.GetAssetTypeIdLaptop(), "").ToList();

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AccessLogin]
        public JsonResult GetAssetListUserJsonResult(long? branchId, long? assetTypeId, long? deptId, string userId)
        {
            if (assetTypeId == null)
                assetTypeId = 0;

            deptId = 0;
            //List<Asset> assets = new List<Asset>();
            IEnumerable<Asset> assets = Enumerable.Empty<Asset>();

            var items = _service.GetItemsUserId(branchId, assetTypeId, deptId, userId,"");
            //var userAsset = new List<Asset> (items.Where(w => w.AssignedTo == userId).ToList());
            //assets = assets.Concat(items.Where(w => w.AssignedTo == userId).ToList());
            //assets = assets.Concat(items.Where(w => w.AssignedTo != userId).ToList());

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetSearchByIdJsonResult(string term)
        {
            var items = _service.GetItemsSearch(term, "").ToList();
           // return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

       

    }
}
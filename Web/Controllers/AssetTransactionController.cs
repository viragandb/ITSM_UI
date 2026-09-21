using DocumentFormat.OpenXml.EMMA;
using Domain;
using Domain.AR;
using Domain.CM;
using Domain.DR;
using log4net;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Service;
using Service.DR;
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
    public class AssetTransactionController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AssetController));
        private readonly AssetService _assetService = new AssetService();
        private readonly AssetTransferRequestService _service = new AssetTransferRequestService();
        private readonly DisposalRequestService _disposalService = new DisposalRequestService();


        [AccessAuthorize]
        public ActionResult Index(long? transactionTypeId, long? statusId, long? branchId, long? departmentId
            , string startDate, string endDate)
        {
            DateTime sDate = UserDateTime.GetUserDateOnly();
            DateTime eDate = UserDateTime.GetUserDateOnly();
            if (startDate != null && startDate != "" && endDate != null && endDate != "")
            {
                try
                {
                    sDate = UserDateTime.ConvertToAppDateTime(startDate);
                    eDate = UserDateTime.ConvertToAppDateTime(endDate);
                }
                catch (Exception ex) { }
            }
            if (statusId == null)
                statusId = 0;
            if (transactionTypeId == null)
                transactionTypeId = 0;
            if (branchId == null)
                branchId = 0;
            if (departmentId == null)
                departmentId = 0;

            ViewBag.BranchId = branchId;
            ViewBag.DepartmentId = departmentId;
            ViewBag.StatusId = statusId;
            ViewBag.TransactionTypeId = transactionTypeId;
            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");

            var items = _service.GetItems(transactionTypeId, statusId, branchId, departmentId, sDate, eDate.AddDays(1)
                , "InitiatedUser,BranchFrom,DepartmentFrom,Branch,Department,AssignedToUser,Vendor,AuthorizedUser,AssignedToUser");
            //, "InitiatedUser,AuthorizedUser,CompletedUser,BranchFrom,DepartmentFrom,Branch,Department,AssignedToUser,TransferItems,TransferItems.AssignedToUser,TransferItems.Asset,Vendor"");

            return View(items);


        }


        #region Tansfer


        [AccessAuthorize]
        [HttpGet]
        public ActionResult Transfer()
        {
            var item = new TransferVM();
            Collection<AssetVM> assets = new Collection<AssetVM>();
            item.Assets = assets.ToList();
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Transfer(TransferVM item)
        {

            ViewBag.BranchIdFrom = item.BranchIdFrom;
            ViewBag.DepartmentIdFrom = item.DepartmentIdFrom;
            ViewBag.BranchId = item.BranchId;
            ViewBag.DepartmentId = item.DepartmentId;
            //if (item.TransactionType == 0 || item.BranchIdFrom == 0 || item.DepartmentIdFrom == 0
            if (item.AssignedType == 0 || item.BranchIdFrom == 0 || item.DepartmentIdFrom == 0
                || item.BranchId == 0 || item.DepartmentId == 0
                || item.AssignTo == null || item.AssignTo == ""
                || item.Comment == null || item.Comment == "")
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }

            if (item.Assets == null)
            {
                TempData["ErrorMessage"] = "Please add assets. ";
                return View(item);
            }
            UserService _userService = new UserService();
            var assignedToUser = _userService.GetUserByEmpNo_UpdateByAD(item.AssignTo, Session["UserId"].ToString(), UserDateTime.GetUserDate(), "");
            if (assignedToUser == null)
            {
                TempData["ErrorMessage"] = "Invalid Assign To user.";
                return View(item);
            }
            AssetTransferRequest newItem = new AssetTransferRequest();
            newItem.Status = AssetTransStatusEnum.Initiated;
            //newItem.TransactionType = item.TransactionType;
            newItem.TransactionType = AssetTransactionTypeEnum.Transfer;
            newItem.AssignedType = item.AssignedType;
            newItem.UpdatedBy = Session["UserId"].ToString();
            newItem.UpdatedDate = UserDateTime.GetUserDate();
            newItem.InitiatedBy = newItem.UpdatedBy;
            newItem.InitiatedDate = newItem.UpdatedDate;
            newItem.AuthorizedDate = newItem.UpdatedDate;
            newItem.CompletedDate = newItem.UpdatedDate;
            newItem.Comment = item.Comment;
            newItem.BranchIdFrom = item.BranchIdFrom;
            newItem.DepartmentIdFrom = item.DepartmentIdFrom;
            newItem.BranchId = item.BranchId;
            newItem.DepartmentId = item.DepartmentId;
            newItem.AssignedTo = item.AssignTo;
            var assets = new Collection<TransferItem>();
            foreach (var asset in item.Assets)
            {
                var _asset = new TransferItem();
                _asset.AssetId = asset.AssetId;
                //_asset.AssignedTo = asset.AssignedToUserId;
                assets.Add(_asset);
            }

            newItem.TransferItems = assets;
            var entitySaved = _service.Insert(newItem);
            if (entitySaved)
            {
                //_service.InsertTicketLog(newTicket, "New request initiated.");
                foreach (var transferItem in newItem.TransferItems)
                {
                    var asset = _assetService.GetItem(transferItem.AssetId, "");
                    asset.Status = AssetStatusEnum.InTransit;
                    asset.UpdatedBy = newItem.UpdatedBy;
                    asset.UpdatedDate = newItem.UpdatedDate;
                    //if (transferItem.AssignedTo != null)
                    //    asset.AssignedTo = transferItem.AssignedTo;

                    _assetService.Update(asset);
                    _assetService.InsertLog(asset, asset.Status.EnumDisplayName());
                    //if (transferItem.AssignedTo != null)
                    //    _assetService.InsertLog(asset, "Assigned To - " + transferItem.AssignedTo);

                }

                TempData["SuccessMessage"] = "Request has been successfully updated.";
                return RedirectToAction("Transfer");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString());
                return View(item);
            }

        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult PendingAuthorize(long? branchId, long? departmentId)
        {
            //if (transactionTypeId == null)
            //    transactionTypeId = 0;
            if (branchId == null)
                branchId = 0;
            if (departmentId == null)
                departmentId = 0;

            //ViewBag.TransactionTypeId = transactionTypeId;
            ViewBag.BranchId = branchId;
            ViewBag.DepartmentId = departmentId;

            var items = _service.GetPendingItems(0, branchId, departmentId, AssetTransStatusEnum.Initiated, Session["UserId"].ToString(),
                "InitiatedUser,AuthorizedUser,CompletedUser,BranchFrom,DepartmentFrom,Branch,Department,AssignedToUser,TransferItems,TransferItems.AssignedToUser,TransferItems.Asset,Vendor").OrderByDescending(o => o.UpdatedDate).ToList();

            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult ApproveAuthorize(long requestId, string comment)
        {
            int errorCode = 0;

            var request = _service.GetItem(requestId, "TransferItems");
            if (request.Status == AssetTransStatusEnum.Initiated)
            {
                request.AuthorizedBy = Session["UserId"].ToString();
                request.AuthorizedDate = UserDateTime.GetUserDate();
                request.UpdatedBy = request.AuthorizedBy;
                request.UpdatedDate = request.AuthorizedDate;
                request.CommentAuthorize = comment;

                if (request.TransactionType == AssetTransactionTypeEnum.SentToRepair)
                    request.Status = AssetTransStatusEnum.Vendor;
                else if (request.TransactionType == AssetTransactionTypeEnum.ToBeDisposed)
                {
                    request.Status = AssetTransStatusEnum.Completed;
                    request.CompletedBy = Session["UserId"].ToString();
                    request.CompletedDate = UserDateTime.GetUserDate();
                    request.CommentComplete = "N/A";
                }
                //else if (request.TransactionType == AssetTransactionTypeEnum.Assigned)// Requested By Gange 29/02/2024
                //    request.Status = AssetTransStatusEnum.Completed;
                else
                    request.Status = AssetTransStatusEnum.InTransit;

                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    if (request.TransactionType == AssetTransactionTypeEnum.SentToRepair)
                    {
                        foreach (var transferItem in request.TransferItems)
                        {
                            var asset = _assetService.GetItem(transferItem.AssetId, "");
                            asset.Status = AssetStatusEnum.UnderRepair;
                            asset.UpdatedBy = request.UpdatedBy;
                            asset.UpdatedDate = request.UpdatedDate;

                            _assetService.Update(asset);
                            _assetService.InsertLog(asset, asset.Status.EnumDisplayName());
                        }
                    }
                    else if (request.TransactionType == AssetTransactionTypeEnum.ToBeDisposed)
                    {
                        foreach (var transferItem in request.TransferItems)
                        {
                            var asset = _assetService.GetItem(transferItem.AssetId, "");
                            asset.Status = AssetStatusEnum.ToBeDisposed;
                            asset.UpdatedBy = request.UpdatedBy;
                            asset.UpdatedDate = request.UpdatedDate;

                            _assetService.Update(asset);
                            _assetService.InsertLog(asset, asset.Status.EnumDisplayName());
                        }
                    }
                    //else if (request.TransactionType == AssetTransactionTypeEnum.Assigned)// Requested By Gange 29/02/2024
                    //{
                    //    foreach (var transferItem in request.TransferItems)
                    //    {
                    //        var asset = _assetService.GetItem(transferItem.AssetId, "");
                    //        asset.AssignedTo = request.AssignedTo;
                    //        asset.Status = AssetStatusEnum.Assigned;
                    //        asset.UpdatedBy = request.UpdatedBy;
                    //        asset.UpdatedDate = request.UpdatedDate;
                    //        asset.BranchId = request.BranchId;
                    //        asset.DepartmentId = request.DepartmentId;
                    //        asset.AssignedType = request.AssignedType;
                    //        _assetService.Update(asset);
                    //        _assetService.InsertLog(asset, "Assigned to " + asset.AssignedTo);

                    //    }
                    //}


                    TempData["SuccessMessage"] = "Request has been approved.";
                    errorCode = 1;

                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);


        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult RejectAuthorize(long requestId, string comment)
        {
            int errorCode = 0;

            var request = _service.GetItem(requestId, "TransferItems");
            if (request.Status == AssetTransStatusEnum.Initiated)
            {
                request.Status = AssetTransStatusEnum.Rejected;
                request.AuthorizedBy = Session["UserId"].ToString();
                request.AuthorizedDate = UserDateTime.GetUserDate();
                request.UpdatedBy = request.AuthorizedBy;
                request.UpdatedDate = request.AuthorizedDate;
                request.CommentAuthorize = comment;
                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    foreach (var transferItem in request.TransferItems)
                    {
                        var asset = _assetService.GetItem(transferItem.AssetId, "");
                        asset.Status = AssetStatusEnum.Assigned;
                        asset.UpdatedBy = request.UpdatedBy;
                        asset.UpdatedDate = request.UpdatedDate;
                        _assetService.Update(asset);
                        _assetService.InsertLog(asset, asset.Status.EnumDisplayName() + " - Transaction Rejected");

                    }
                    TempData["SuccessMessage"] = "Request has been rejected.";
                    errorCode = 1;

                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult PendingComplete(long? branchId, long? departmentId)
        {
            //if (transactionTypeId == null)
            //    transactionTypeId = 0;
            if (branchId == null)
                branchId = 0;
            if (departmentId == null)
                departmentId = 0;

            //ViewBag.TransactionTypeId = transactionTypeId;
            ViewBag.BranchId = branchId;
            ViewBag.DepartmentId = departmentId;

            var items = _service.GetPendingToItems((int)AssetTransactionTypeEnum.Transfer, branchId, departmentId, AssetTransStatusEnum.InTransit, Session["UserId"].ToString(),
                "InitiatedUser,AuthorizedUser,CompletedUser,BranchFrom,DepartmentFrom,Branch,Department,AssignedToUser,TransferItems,TransferItems.AssignedToUser,TransferItems.Asset").OrderByDescending(o => o.UpdatedDate).ToList();

            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Complete(long requestId, string comment)
        {
            int errorCode = 0;

            var request = _service.GetItem(requestId, "TransferItems");
            if (request.Status == AssetTransStatusEnum.InTransit)
            {
                request.Status = AssetTransStatusEnum.Completed;
                request.CompletedBy = Session["UserId"].ToString();
                request.CompletedDate = UserDateTime.GetUserDate();
                request.UpdatedBy = request.CompletedBy;
                request.UpdatedDate = request.CompletedDate;
                request.CommentComplete = comment;
                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    foreach (var transferItem in request.TransferItems)
                    {
                        var asset = _assetService.GetItem(transferItem.AssetId, "");
                        asset.Status = AssetStatusEnum.Assigned;
                        asset.UpdatedBy = request.UpdatedBy;
                        asset.UpdatedDate = request.UpdatedDate;
                        asset.BranchId = request.BranchId;
                        asset.DepartmentId = request.DepartmentId;
                        asset.AssignedType = request.AssignedType;
                        _assetService.Update(asset);
                        //_assetService.InsertLog(asset, "Transfer Completed");
                        //_assetService.InsertTransLog(asset, request.TransactionType);
                        _assetService.InsertLog(asset, asset.Status.EnumDisplayName() + " - Transferred");

                    }

                    TempData["SuccessMessage"] = "Request has been updated.";
                    errorCode = 1;

                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);


        }

        #endregion


        #region Assign

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Assign()
        {
            var item = new TransferVM();
            Collection<AssetVM> assets = new Collection<AssetVM>();
            item.Assets = assets.ToList();
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Assign(TransferVM item)
        {

            ViewBag.BranchIdFrom = item.BranchIdFrom;
            ViewBag.DepartmentIdFrom = item.DepartmentIdFrom;
            ViewBag.BranchId = item.BranchId;
            ViewBag.DepartmentId = item.DepartmentId;
            if (item.AssignedType == 0 || item.BranchIdFrom == 0 || item.DepartmentIdFrom == 0
                || item.BranchId == 0 || item.DepartmentId == 0
                || item.AssignTo == null || item.AssignTo == ""
                || item.Comment == null || item.Comment == "")
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }

            if (item.Assets.Count() == 0)
            {
                TempData["ErrorMessage"] = "Please add assets. ";
                return View(item);
            }
            UserService _userService = new UserService();
            var assignedToUser = _userService.GetUserByEmpNo_UpdateByAD(item.AssignTo, Session["UserId"].ToString(), UserDateTime.GetUserDate(), "");

            AssetTransferRequest newItem = new AssetTransferRequest();
            //newItem.Status = AssetTransStatusEnum.Completed; 
            newItem.Status = AssetTransStatusEnum.Initiated;
            newItem.TransactionType = AssetTransactionTypeEnum.Assigned;
            newItem.AssignedType = item.AssignedType;
            newItem.UpdatedBy = Session["UserId"].ToString();
            newItem.UpdatedDate = UserDateTime.GetUserDate();
            newItem.InitiatedBy = newItem.UpdatedBy;
            newItem.InitiatedDate = newItem.UpdatedDate;
            newItem.AuthorizedDate = newItem.UpdatedDate;
            newItem.CompletedDate = newItem.UpdatedDate;
            newItem.Comment = item.Comment;
            newItem.BranchIdFrom = item.BranchIdFrom;
            newItem.DepartmentIdFrom = item.DepartmentIdFrom;
            newItem.BranchId = item.BranchId;
            newItem.DepartmentId = item.DepartmentId;
            newItem.AssignedTo = item.AssignTo;



            var assets = new Collection<TransferItem>();
            foreach (var asset in item.Assets)
            {
                var _asset = new TransferItem();
                _asset.AssetId = asset.AssetId;
                _asset.AssignedTo = asset.AssignedToUserId;
                assets.Add(_asset);
            }
            newItem.TransferItems = assets;

            var entitySaved = _service.Insert(newItem);
            if (entitySaved)
            {
                foreach (var transferItem in newItem.TransferItems)
                {
                    var asset = _assetService.GetItem(transferItem.AssetId, "");
                    ////Requested by Gange
                    //asset.Status = AssetStatusEnum.Assigned;
                    //asset.AssignedTo = transferItem.AssignedTo;

                    asset.Status = AssetStatusEnum.InTransit;

                    asset.UpdatedBy = newItem.UpdatedBy;
                    asset.UpdatedDate = newItem.UpdatedDate;
                    _assetService.Update(asset);
                    _assetService.InsertLog(asset, asset.Status.EnumDisplayName());

                }

                TempData["SuccessMessage"] = "Request has been successfully updated.";
                return RedirectToAction("Assign");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString());
                return View(item);
            }

        }


        #endregion

        #region Send for repair

        [AccessAuthorize]
        [HttpGet]
        public ActionResult SendToRepair()
        {
            var item = new TransferVM();
            Collection<AssetVM> assets = new Collection<AssetVM>();
            item.Assets = assets.ToList();
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult SendToRepair(TransferVM item)
        {

            ViewBag.BranchIdFrom = item.BranchIdFrom;
            ViewBag.DepartmentIdFrom = item.DepartmentIdFrom;
            ViewBag.VendorId = item.VendorId;
            if (item.BranchIdFrom == 0 || item.DepartmentIdFrom == 0
                || item.VendorId == 0
                //|| item.BranchId == 0 || item.DepartmentId == 0
                //|| item.AssignTo == null || item.AssignTo == ""
                || item.Comment == null || item.Comment == "")
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }

            if (item.Assets.Count() == 0)
            {
                TempData["ErrorMessage"] = "Please add a asset. ";
                return View(item);
            }

            AssetTransferRequest newItem = new AssetTransferRequest();
            newItem.Status = AssetTransStatusEnum.Initiated;
            newItem.TransactionType = AssetTransactionTypeEnum.SentToRepair;
            newItem.AssignedType = AssignedTypeEnum.Temporary;
            newItem.UpdatedBy = Session["UserId"].ToString();
            newItem.UpdatedDate = UserDateTime.GetUserDate();
            newItem.InitiatedBy = newItem.UpdatedBy;
            newItem.InitiatedDate = newItem.UpdatedDate;
            newItem.AuthorizedDate = newItem.UpdatedDate;
            newItem.CompletedDate = newItem.UpdatedDate;
            newItem.Comment = item.Comment;
            newItem.BranchIdFrom = item.BranchIdFrom;
            newItem.DepartmentIdFrom = item.DepartmentIdFrom;
            newItem.BranchId = item.BranchIdFrom;
            newItem.DepartmentId = item.DepartmentIdFrom;
            //newItem.AssignedTo = item.AssignTo;
            newItem.VendorId = item.VendorId;

            var assets = new Collection<TransferItem>();
            foreach (var asset in item.Assets)
            {
                var _asset = new TransferItem();
                _asset.AssetId = asset.AssetId;
                //_asset.AssignedTo = asset.AssignedToUserId;
                assets.Add(_asset);
            }
            newItem.TransferItems = assets;

            var entitySaved = _service.Insert(newItem);
            if (entitySaved)
            {
                foreach (var transferItem in newItem.TransferItems)
                {
                    var asset = _assetService.GetItem(transferItem.AssetId, "");
                    asset.Status = AssetStatusEnum.InTransit;
                    asset.UpdatedBy = newItem.UpdatedBy;
                    asset.UpdatedDate = newItem.UpdatedDate;
                    _assetService.Update(asset);
                    _assetService.InsertLog(asset, asset.Status.EnumDisplayName());

                }

                TempData["SuccessMessage"] = "Request has been successfully updated.";
                return RedirectToAction("SendToRepair");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString());
                return View(item);
            }

        }


        #endregion

        #region Under repair

        [HttpGet]
        [AccessAuthorize]
        public ActionResult UnderRepair(long? vendorId)
        {
            if (vendorId == null)
                vendorId = 0;

            ViewBag.VendorId = vendorId;

            var items = _service.GetUnderRepairItems(vendorId,
                "InitiatedUser,AuthorizedUser,CompletedUser,BranchFrom,DepartmentFrom,Branch,Department,AssignedToUser,TransferItems,TransferItems.AssignedToUser,TransferItems.Asset,Vendor").OrderByDescending(o => o.UpdatedDate).ToList();

            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult BackToOperation(long requestId, string comment)
        {
            int errorCode = 0;

            var request = _service.GetItem(requestId, "TransferItems");
            if (request.Status == AssetTransStatusEnum.Vendor)
            {
                request.Status = AssetTransStatusEnum.Completed;
                request.CompletedBy = Session["UserId"].ToString();
                request.CompletedDate = UserDateTime.GetUserDate();
                request.UpdatedBy = request.CompletedBy;
                request.UpdatedDate = request.CompletedDate;
                request.CommentComplete = comment;
                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    foreach (var transferItem in request.TransferItems)
                    {
                        var asset = _assetService.GetItem(transferItem.AssetId, "");
                        asset.Status = AssetStatusEnum.Assigned;
                        asset.UpdatedBy = request.UpdatedBy;
                        asset.UpdatedDate = request.UpdatedDate;
                        _assetService.Update(asset);
                        _assetService.InsertLog(asset, asset.Status.EnumDisplayName() + " - Back to operation");

                    }

                    TempData["SuccessMessage"] = "Request has been updated.";
                    errorCode = 1;

                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);


        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult OutOfService(long requestId, string comment)
        {
            int errorCode = 0;

            var request = _service.GetItem(requestId, "TransferItems");
            if (request.Status == AssetTransStatusEnum.Vendor)
            {
                request.Status = AssetTransStatusEnum.Completed;
                request.CompletedBy = Session["UserId"].ToString();
                request.CompletedDate = UserDateTime.GetUserDate();
                request.UpdatedBy = request.CompletedBy;
                request.UpdatedDate = request.CompletedDate;
                request.CommentComplete = comment;
                var entitySaved = _service.Update(request);
                if (entitySaved)
                {
                    foreach (var transferItem in request.TransferItems)
                    {
                        var asset = _assetService.GetItem(transferItem.AssetId, "");
                        asset.Status = AssetStatusEnum.OutOfService;
                        asset.UpdatedBy = request.UpdatedBy;
                        asset.UpdatedDate = request.UpdatedDate;
                        _assetService.Update(asset);
                        _assetService.InsertLog(asset, asset.Status.EnumDisplayName());

                    }

                    #region Dispose Request

                    AssetTransferRequest newItem = new AssetTransferRequest();
                    newItem.Status = AssetTransStatusEnum.Initiated;
                    newItem.TransactionType = AssetTransactionTypeEnum.ToBeDisposed;
                    newItem.AssignedType = AssignedTypeEnum.Permanent;
                    newItem.UpdatedBy = Session["UserId"].ToString();
                    newItem.UpdatedDate = UserDateTime.GetUserDate();
                    newItem.InitiatedBy = newItem.UpdatedBy;
                    newItem.InitiatedDate = newItem.UpdatedDate;
                    newItem.AuthorizedDate = newItem.UpdatedDate;
                    newItem.CompletedDate = newItem.UpdatedDate;
                    newItem.Comment = comment;
                    newItem.BranchIdFrom = request.BranchId;
                    newItem.DepartmentIdFrom = request.DepartmentId;
                    newItem.BranchId = BranchService.GetAssetToBeDisposedBranchId();
                    newItem.DepartmentId = DepartmentService.GetAssetToBeDisposedDeptId();

                    var assets = new Collection<TransferItem>();
                    foreach (var asset in request.TransferItems)
                    {
                        var _asset = new TransferItem();
                        _asset.AssetId = asset.AssetId;
                        assets.Add(_asset);
                    }
                    newItem.TransferItems = assets;

                    _service.Insert(newItem);
                    foreach (var transferItem in newItem.TransferItems)
                    {
                        var asset = _assetService.GetItem(transferItem.AssetId, "");
                        asset.Status = AssetStatusEnum.InTransit;
                        asset.UpdatedBy = newItem.UpdatedBy;
                        asset.UpdatedDate = newItem.UpdatedDate;
                        _assetService.Update(asset);
                        _assetService.InsertLog(asset, asset.Status.EnumDisplayName());

                    }

                    #endregion

                    TempData["SuccessMessage"] = "Request has been updated.";
                    errorCode = 1;

                }
                else
                    errorCode = 2;
            }
            else
                errorCode = 2;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);


        }


        #endregion

        #region To be disposed

        [AccessAuthorize]
        [HttpGet]
        public ActionResult ToBeDisposed()
        {
            var item = new TransferVM();
            Collection<AssetVM> assets = new Collection<AssetVM>();
            item.Assets = assets.ToList();
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult ToBeDisposed(TransferVM item)
        {

            ViewBag.BranchIdFrom = item.BranchIdFrom;
            ViewBag.DepartmentIdFrom = item.DepartmentIdFrom;
            if (item.BranchIdFrom == 0 || item.DepartmentIdFrom == 0
                //|| item.BranchId == 0 || item.DepartmentId == 0
                //|| item.AssignTo == null || item.AssignTo == ""
                || item.Comment == null || item.Comment == "")
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }

            if (item.Assets.Count() == 0)
            {
                TempData["ErrorMessage"] = "Please add a asset. ";
                return View(item);
            }


            AssetTransferRequest newItem = new AssetTransferRequest();
            newItem.Status = AssetTransStatusEnum.Initiated;
            newItem.TransactionType = AssetTransactionTypeEnum.ToBeDisposed;
            newItem.AssignedType = AssignedTypeEnum.Permanent;
            newItem.UpdatedBy = Session["UserId"].ToString();
            newItem.UpdatedDate = UserDateTime.GetUserDate();
            newItem.InitiatedBy = newItem.UpdatedBy;
            newItem.InitiatedDate = newItem.UpdatedDate;
            newItem.AuthorizedDate = newItem.UpdatedDate;
            newItem.CompletedDate = newItem.UpdatedDate;
            newItem.Comment = item.Comment;
            newItem.BranchIdFrom = item.BranchIdFrom;
            newItem.DepartmentIdFrom = item.DepartmentIdFrom;
            newItem.BranchId = BranchService.GetAssetToBeDisposedBranchId();
            newItem.DepartmentId = DepartmentService.GetAssetToBeDisposedDeptId();

            var assets = new Collection<TransferItem>();
            foreach (var asset in item.Assets)
            {
                var _asset = new TransferItem();
                _asset.AssetId = asset.AssetId;
                assets.Add(_asset);
            }
            newItem.TransferItems = assets;

            var entitySaved = _service.Insert(newItem);
            if (entitySaved)
            {
                foreach (var transferItem in newItem.TransferItems)
                {
                    var asset = _assetService.GetItem(transferItem.AssetId, "");
                    asset.Status = AssetStatusEnum.InTransit;
                    asset.UpdatedBy = newItem.UpdatedBy;
                    asset.UpdatedDate = newItem.UpdatedDate;
                    _assetService.Update(asset);
                    _assetService.InsertLog(asset, asset.Status.EnumDisplayName());

                }

                TempData["SuccessMessage"] = "Request has been successfully updated.";
                return RedirectToAction("ToBeDisposed");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString());
                return View(item);
            }

        }


        #endregion

        #region Dispose

        [AccessAuthorize]
        [HttpGet]
        public ActionResult NewDisposalRequest()
        {
            return View();
        }


        [AccessAuthorize]
        [HttpPost]
        public ActionResult NewDisposalRequest(DisposalVM item)
        {

            if (item.Title.IsNullOrWhiteSpace())
            {
                TempData["ErrorMessage"] = "Please Provide a Title";
                return View(item);
            }

            var request = new DisposalRequest
            {
                Title = item.Title,
                Status = DisposalStatusEnum.Initiated,
                RequestCreatedDate = UserDateTime.GetUserDate(),
                BuyerName = null,            
                UpdatedDate = UserDateTime.GetUserDate(),
                UpdatedBy = Session["UserId"].ToString(),
                IsDeleted = false
            };

            if (_disposalService.Insert(request))
            {

                if (!_disposalService.InsertUpdateStatus(request.DisposalRequestId, DisposalStatusEnum.Initiated, "New Disposal Request has been initiated", false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
                {
                    TempData["ErrorMessage"] = "Could not add new disposal update status record";
                    return View(item);
                }
                if (!_disposalService.InsertLog(request.DisposalRequestId, DisposalStatusEnum.Initiated, "New Disposal Request has been initiated.", false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
                {
                    TempData["ErrorMessage"] = "Could not add new disposal log record";
                    return View(item);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Could not add new disposal request, something went wrong";
                return View(item);
            }

            TempData["SuccessMessage"] = "Disposal Request has been successfully created.";
            return RedirectToAction("NewDisposalRequest");

        }


        [AccessAuthorize]
        [HttpGet]
        public ActionResult Disposed()
        {
            var item = new DisposalVM();
            Collection<AssetVM> assets = new Collection<AssetVM>();
            item.Assets = assets.ToList();

            item.AllDisposalRequests = _disposalService.GetAll("DisposalItems.Asset.AssetType").ToList();

            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Disposed(DisposalVM model)
        {
            // update the items of the request

            if (model.DisposalRequestId == 0)
            {
                TempData["ErrorMessage"] = "Please select a request to update items";
                return View(model);
            }

            // delete removed items
            if (model.RemovedAssets != null)
            {
                foreach (var asset in model.RemovedAssets)
                {
                    var item = _disposalService
                        .GetItemByRequestAndAsset(model.DisposalRequestId, asset.AssetId, "");

                    if (item != null)
                    {
                        item.IsDeleted = true;
                        item.UpdatedDate = UserDateTime.GetUserDate();

                        _disposalService.UpdateDisposalItems(item);
                    }
                }
            }

            // add the newly selected items

            if (model.Assets != null)
            {
                foreach (var asset in model.Assets)
                {
                    var item = new DisposalItem
                    {
                        DisposalRequestId = model.DisposalRequestId,
                        AssetId = asset.AssetId,
                        UpdatedDate = UserDateTime.GetUserDate(),
                        IsDeleted = false
                    };

                    _disposalService.InsertDisposalItems(item);
                }
            }


            TempData["SuccessMessage"] = "Disposal request updated.";
            return RedirectToAction("Disposed");

        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult SendDisposalToApproval(long RequestId, String Comment)
        {
            int ErrorCode = 0;
            if (RequestId == 0)
            {
                TempData["ErrorMessage"] = "Please select a request to update items";
                return View();
            }

            var request = _disposalService.GetItem(RequestId, "DisposalItems");

            if (request.DisposalItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Cannot Send to approval as the request has no linked assets for disposal";
                return View();
            }

            // if all valid update request
            request.Status = DisposalStatusEnum.PendingApproval;
            request.UpdatedDate = UserDateTime.GetUserDate();
            request.UpdatedBy = Session["UserId"].ToString();

            if (_disposalService.InsertUpdateStatus(request.DisposalRequestId, DisposalStatusEnum.PendingApproval, Comment, false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
            {

                if (!_disposalService.Update(request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
                if (!_disposalService.InsertLog(request.DisposalRequestId, DisposalStatusEnum.PendingApproval, "Request #" + request.DisposalRequestId + " has been sent for approval.", false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);


        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult PendingDisposalApproval()
        {
            var requests = _disposalService.GetAll("DisposalItems.Asset.AssetType");

            return View(requests);
        }


        [AccessAuthorize]
        [HttpPost]
        public ActionResult DisposalApprove(long RequestId, string Comment)
        {

            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {

                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            var request = _disposalService.GetItem(RequestId, "DisposalItems");

            request.Status = DisposalStatusEnum.Approved;
            request.UpdatedDate = UserDateTime.GetUserDate();
            request.UpdatedBy = Session["UserId"].ToString();

            if (_disposalService.InsertUpdateStatus(request.DisposalRequestId, DisposalStatusEnum.Approved, Comment, false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
            {

                if (!_disposalService.Update(request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
                if (!_disposalService.InsertLog(request.DisposalRequestId, DisposalStatusEnum.Approved, "Request #" + request.DisposalRequestId + " has been approved for disposal.", false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);

        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult DisposalReject(long RequestId, string Comment)
        {
            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {

                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            var request = _disposalService.GetItem(RequestId, "DisposalItems");

            request.Status = DisposalStatusEnum.Reject;
            request.UpdatedDate = UserDateTime.GetUserDate();
            request.UpdatedBy = Session["UserId"].ToString();

            if (_disposalService.InsertUpdateStatus(request.DisposalRequestId, DisposalStatusEnum.Reject, Comment, false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
            {

                if (!_disposalService.Update(request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
                if (!_disposalService.InsertLog(request.DisposalRequestId, DisposalStatusEnum.Reject, "Request #" + request.DisposalRequestId + " has been rejected.", false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);

        }


        [AccessAuthorize]
        [HttpPost]
        public ActionResult DisposalReturn(long RequestId, string Comment)
        {

            int ErrorCode = 0;
            if (Comment.IsNullOrWhiteSpace())
            {

                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            var request = _disposalService.GetItem(RequestId, "DisposalItems");

            request.Status = DisposalStatusEnum.Return;
            request.UpdatedDate = UserDateTime.GetUserDate();
            request.UpdatedBy = Session["UserId"].ToString();

            if (_disposalService.InsertUpdateStatus(request.DisposalRequestId, DisposalStatusEnum.Return, Comment, false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
            {

                if (!_disposalService.Update(request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
                if (!_disposalService.InsertLog(request.DisposalRequestId, DisposalStatusEnum.Return, "Request #" + request.DisposalRequestId + " has been returned.", false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        [AccessAuthorize]
        public ActionResult DisposalResubmit(long RequestId)
        {

            int ErrorCode = 0;
         
            var request = _disposalService.GetItem(RequestId, "");

            request.Status = DisposalStatusEnum.AmendAndResubmit;
            request.UpdatedDate = UserDateTime.GetUserDate();
            request.UpdatedBy = Session["UserId"].ToString();

            if (_disposalService.InsertUpdateStatus(request.DisposalRequestId, DisposalStatusEnum.AmendAndResubmit, "Request #" + request.DisposalRequestId + " has been sent for amendment and resubmission.", false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
            {

                if (!_disposalService.Update(request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
                if (!_disposalService.InsertLog(request.DisposalRequestId, DisposalStatusEnum.AmendAndResubmit, "Request #" + request.DisposalRequestId + " has been sent for amendment and resubmission.", false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult UpdateDisposalRequest(long RequestId, string BuyerName)
        {
            int ErrorCode = 0;
            if (BuyerName.IsNullOrWhiteSpace())
            {

                ErrorCode = 3;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            var request = _disposalService.GetItem(RequestId, "DisposalItems");

            request.BuyerName = BuyerName;
            request.Status = DisposalStatusEnum.InTransit;
            request.UpdatedDate = UserDateTime.GetUserDate();
            request.UpdatedBy = Session["UserId"].ToString();

            if (_disposalService.InsertUpdateStatus(request.DisposalRequestId, DisposalStatusEnum.InTransit, "Assets for Request #" + request.DisposalRequestId + " are in transit and documents have been uploaded successfully.", false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
            {

                if (!_disposalService.Update(request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
                if (!_disposalService.InsertLog(request.DisposalRequestId, DisposalStatusEnum.InTransit, "Assets for Request #" + request.DisposalRequestId + " are in transit and documents have been uploaded successfully.", false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                // insert the files record from tempdoc table into disposal request doc table permanently
                TicketDocService _ticketDocService = new TicketDocService();
                var tempDocs = _ticketDocService.GetTempDocs(request.UpdatedBy, "");
                foreach (TempDoc _doc in tempDocs)
                {
                    DisposalRequestDoc doc = new DisposalRequestDoc();
                    doc.FileName = _doc.FileName;
                    doc.FileUrl = _doc.FileUrl;
                    doc.DisposalRequestId = request.DisposalRequestId;
                    doc.DocType = DocTypeEnum.Attachment;
                    doc.DocumentName = _doc.DocumentName;
                    doc.UpdatedDate = UserDateTime.GetUserDate();
                    doc.UpdatedBy = Session["UserId"].ToString();
                    _disposalService.InsertDoc(doc);


                    _disposalService.InsertLog(request.DisposalRequestId, DisposalStatusEnum.InTransit, "Document (" + doc.DocumentName + ") was uploaded", false, Session["UserId"].ToString(), UserDateTime.GetUserDate());
                    ItemDocService _docService = new ItemDocService();
                    _docService.DeleteTempDoc(_doc.TempDocId); // delete the tempdoc record
                }
                
            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AccessAuthorize]
        public ActionResult CompleteDisposalRequest(long RequestId)
        {

            int ErrorCode = 0;
          
            var request = _disposalService.GetItem(RequestId, "DisposalItems.Asset");

            request.Status = DisposalStatusEnum.Completed;
            request.UpdatedDate = UserDateTime.GetUserDate();
            request.UpdatedBy = Session["UserId"].ToString();

            if (_disposalService.InsertUpdateStatus(request.DisposalRequestId, DisposalStatusEnum.Completed, "Request #" + request.DisposalRequestId + " has been completed.", false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
            {

                if (!_disposalService.Update(request))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }
                if (!_disposalService.InsertLog(request.DisposalRequestId, DisposalStatusEnum.Completed, "Request #" + request.DisposalRequestId + " has been completed.", false, Session["UserId"].ToString(), UserDateTime.GetUserDate()))
                {
                    ErrorCode = 2;
                    return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                }

                var disposedAssets = request.DisposalItems.Where(a => a.IsDeleted == false);
                foreach(var item in disposedAssets)
                {
                    var asset = item.Asset;
                    asset.Status = AssetStatusEnum.Disposed;
                    asset.UpdatedBy = Session["UserId"].ToString();
                    asset.UpdatedDate = UserDateTime.GetUserDate();

                    // update master asset data 
                    if (!_assetService.Update(asset))
                    {
                        ErrorCode = 2;
                        return Json(ErrorCode, JsonRequestBehavior.AllowGet);
                    }

                    // add a log in asset log
                    _assetService.InsertLog(asset, "Asset has been disposed.");                   

                }

            }
            else
            {
                ErrorCode = 2;
                return Json(ErrorCode, JsonRequestBehavior.AllowGet);
            }

            ErrorCode = 1;
            return Json(ErrorCode, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AccessLogin]
        public JsonResult GetDisposedAssetListJsonResult(long? branchId, long? deptId, long? assetCatId, long? assetTypeId)
        {

            if ((branchId ?? 0) == 0 && (deptId ?? 0) == 0 && (assetCatId ?? 0) == 0 && (assetTypeId ?? 0) == 0)
            {
                return Json(new List<AssetVM>(), JsonRequestBehavior.AllowGet);
            }

            if (assetCatId == null)
                assetCatId = 0;
            if (assetTypeId == null)
                assetTypeId = 0;

            var disposedItems = _service.GetDisposedItems("TransferItems.Asset");

            var itemsLinkedToRequests = _disposalService.GetLinkedItemsAssetId("");

            var filteredAssets = disposedItems
                .SelectMany(request => request.TransferItems ?? new List<TransferItem>())
                .Where(transfer =>
                    transfer.Asset != null &&
                    !itemsLinkedToRequests.Contains(transfer.AssetId) &&
                    (assetCatId == 0 || assetCatId == null || (long)transfer.Asset.AssetCategory == assetCatId) &&
                    (assetTypeId == 0 || assetTypeId == null || transfer.Asset.AssetTypeId == assetTypeId) &&
                    (branchId == 0 || branchId == null || transfer.Asset.BranchId == branchId) &&
                    (deptId == 0 || deptId == null || transfer.Asset.DepartmentId == deptId)
                )
                .Select(transfer => new AssetVM
                {
                    AssetId = transfer.AssetId,
                    AssetName = transfer.Asset.AssetName,
                    ModelName = transfer.Asset.ModelName,
                    AssetNo = transfer.Asset.AssetNo
                })
                .ToList();

            return Json(filteredAssets, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [AccessLogin]
        public JsonResult GetDisposalRequestsListJsonResult()
        {
            var allDisposalRequests = _disposalService.GetAll("").Where(r => r.Status == DisposalStatusEnum.Initiated || r.Status == DisposalStatusEnum.AmendAndResubmit).Select(r => new
            {
                r.DisposalRequestId,
                r.Title
            }).ToList();

            return Json(allDisposalRequests, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AccessLogin]
        public JsonResult GetLinkedDisposalItemsJsonResult(long requestId)
        {
            var request = _disposalService.GetAsNoTrackingItem(requestId, "DisposalItems.Asset");

            var items = request.DisposalItems.Where(d => d.IsDeleted == false)
            .Select(d => new
            {
                d.AssetId,
                d.Asset.AssetNo,
                d.Asset.AssetName,
            }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AccessLogin]
        [EncryptedActionParameter]
        public ActionResult DisposalRequestInfo(long? id)
        {
            if (id == null)
                return HttpNotFound();
            var item = _disposalService.GetItem(id, "DisposalRequestUpdates,"+"DisposalRequestUpdates.UpdatedUser," +
                "DisposalRequestLogs,"+"DisposalRequestLogs.UpdatedUser,"+"DisposalItems.Asset," + "DisposalRequestDocs");

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }


        #endregion


        [HttpGet]
        [AccessAuthorize]
        public ActionResult MyRequests()
        {

            var items = _service.GetItemsByInitiator(Session["UserId"].ToString(),
                "InitiatedUser,AuthorizedUser,CompletedUser,BranchFrom,DepartmentFrom,Branch,Department,AssignedToUser,TransferItems,TransferItems.AssignedToUser,TransferItems.Asset,TransferItems.Asset.AssetType,TransferItems.Asset.AssetMake,Vendor").OrderByDescending(o => o.UpdatedDate).ToList();
            return View(items);
        }

        [HttpGet]
        [AccessLogin]
        [EncryptedActionParameter]
        public ActionResult GatePass(long? id)
        {


            if (id == null)
                return HttpNotFound();
            var item = _service.GetItem(id, "InitiatedUser,AuthorizedUser,CompletedUser,BranchFrom,DepartmentFrom,Branch,Department,AssignedToUser,TransferItems,TransferItems.AssignedToUser,TransferItems.Asset,TransferItems.Asset.AssetType,TransferItems.Asset.AssetMake,Vendor");

            return View(item);

        }


        [HttpGet]
        [AccessLogin]
        [EncryptedActionParameter]
        public ActionResult Info(long? id)
        {
            GRNoteService _grnService = new GRNoteService();
            ItemDocService _docService = new ItemDocService();
            if (id == null)
                return HttpNotFound();
            var item = _service.GetItemAsNoTracking(id, "InitiatedUser,AuthorizedUser,CompletedUser,BranchFrom,DepartmentFrom,Branch,Department,AssignedToUser,Vendor," +
                "TransferItems,TransferItems.Asset,TransferItems.Asset.AssetType,TransferItems.Asset.AssetMake,TransferItems.Asset.Branch,TransferItems.Asset.Department");
            //item.ItemDocs = _docService.GetGRNDocs(item., "").ToList();

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }


        [HttpGet]
        [AccessLogin]
        public JsonResult GetTransTypesJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();

            Array values = Enum.GetValues(typeof(AssetTransactionTypeEnum));

            foreach (AssetTransactionTypeEnum val in values)
            {

                EnumStatusVM item = new EnumStatusVM();
                item.Id = Convert.ToInt32(val);
                item.Name = val.EnumDisplayName();
                items.Add(item);
            }

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetTransStatusJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();
            Array values = Enum.GetValues(typeof(AssetTransStatusEnum));

            foreach (AssetTransStatusEnum val in values)
            {

                Console.WriteLine(String.Format("{0}: {1}", Enum.GetName(typeof(AssetTransStatusEnum), val), val));
                EnumStatusVM item = new EnumStatusVM();
                item.Id = Convert.ToInt32(val);
                item.Name = val.EnumDisplayName();
                items.Add(item);
            }

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

    }
}
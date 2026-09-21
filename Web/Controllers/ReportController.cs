using Domain;
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
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult CustomerSat(long? teamId, string userEmpNo, long? ticketTypeId, long? categoryId, string startDate, string endDate)
        {
            TicketService _ticketService = new TicketService();
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
            if (teamId == null)
                teamId = 0;

            if (ticketTypeId == null)
                ticketTypeId = 0;
            if (categoryId == null)
                categoryId = 0;
            if (userEmpNo == null || userEmpNo == "0")
                userEmpNo = "";

            var items = _ticketService.GetTikcets(teamId, userEmpNo, ticketTypeId, 0, 0, categoryId, 0, 0, 0, 0, sDate, eDate.AddDays(1), "", false, false,
             "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser,AssignedToUser").Where(w => w.Rate > 0);

            ViewBag.TeamId = teamId;
            ViewBag.UserEmpNo = userEmpNo;
            ViewBag.TicketTypeId = ticketTypeId;
            ViewBag.CategoryId = categoryId;
            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");
            return View(items);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult SLAViolation(long? teamId, string userEmpNo, long? ticketTypeId, long? categoryId, long? statusId, string startDate, string endDate
            , long? vendorId)
        {
            TicketService _ticketService = new TicketService();
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

            if (teamId == null)
                teamId = 0;
            if (ticketTypeId == null)
                ticketTypeId = 0;
            if (categoryId == null)
                categoryId = 0;
            if (userEmpNo == null || userEmpNo == "0")
                userEmpNo = "";
            if (statusId == null)
                statusId = 0;
            if (vendorId == null)
                vendorId = 0;

            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");
            ViewBag.TeamId = teamId;
            ViewBag.UserEmpNo = userEmpNo;
            ViewBag.TicketTypeId = ticketTypeId;
            ViewBag.CategoryId = categoryId;
            ViewBag.StatusId = statusId;
            ViewBag.VendorId = vendorId;

            var items = _ticketService.GetTikcets(teamId, userEmpNo, ticketTypeId, 0, 0, categoryId, 0, 0, 0, vendorId, sDate, eDate.AddDays(1), "", true, false,
           "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser,AssignedToUser,Branch,Department,Vendor,TicketUpdates");


            return View(items);

        }

        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult Reopend(long? teamId, string userEmpNo, long? ticketTypeId, long? categoryId, long? statusId, string startDate, string endDate)
        //{
        //    TicketService _ticketService = new TicketService();
        //    DateTime sDate = UserDateTime.GetUserDateOnly();
        //    DateTime eDate = UserDateTime.GetUserDateOnly();
        //    if (startDate != null && startDate != "" && endDate != null && endDate != "")
        //    {
        //        try
        //        {
        //            sDate = Convert.ToDateTime(startDate);
        //            eDate = Convert.ToDateTime(endDate);
        //        }
        //        catch (Exception ex) { }
        //    }
        //    if (teamId == null)
        //        teamId = 0;
        //    if (ticketTypeId == null)
        //        ticketTypeId = 0;
        //    if (categoryId == null)
        //        categoryId = 0;
        //    if (userEmpNo == null || userEmpNo == "0")
        //        userEmpNo = "";
        //    if (statusId == null)
        //        statusId = 0;

        //    var items = _ticketService.GetTikcets(teamId, userEmpNo, ticketTypeId, categoryId, 0, 0, 0, 0, sDate, eDate.AddDays(1), "", false, false,
        //   "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser,AssignedToUser,Branch").Where(w => w.Status == TicketStatusEnum.Reopened);

        //    ViewBag.TeamId = teamId;
        //    ViewBag.UserEmpNo = userEmpNo;
        //    ViewBag.TicketTypeId = ticketTypeId;
        //    ViewBag.CategoryId = categoryId;
        //    ViewBag.StatusId = statusId;
        //    ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
        //    ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");
        //    return View(items);

        //}



        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult Vendor(long? teamId, long? companyId, long? userEmpNo, long? ticketTypeId, long? categoryId, long? statusId,long? vendorId, string startDate, string endDate)
        //{
        //    TicketService _ticketService = new TicketService();
        //    DateTime sDate = UserDateTime.GetUserDateOnly();
        //    DateTime eDate = UserDateTime.GetUserDateOnly();
        //    if (startDate != null && startDate != "" && endDate != null && endDate != "")
        //    {
        //        try
        //        {
        //            sDate = Convert.ToDateTime(startDate);
        //            eDate = Convert.ToDateTime(endDate);
        //        }
        //        catch (Exception ex) { }
        //    }
        //    if (teamId == null)
        //        teamId = 0;
        //    if (companyId == null)
        //        companyId = 0;
        //    if (ticketTypeId == null)
        //        ticketTypeId = 0;
        //    if (categoryId == null)
        //        categoryId = 0;
        //    if (userEmpNo == null)
        //        userEmpNo = 0;
        //    if (statusId == null)
        //        statusId = 0;
        //    if (vendorId == null)
        //        vendorId = 0;
        //    var items = _ticketService.GetTikcets(teamId, companyId, userEmpNo, ticketTypeId, categoryId, statusId, vendorId, sDate, eDate.AddDays(1),
        //     "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser,AssignedToUser,Vendor").Where(w => w.VendorId>0).OrderBy(o=> o.RequestedDate);



        //    //TempData["TeamId"] = teamId;

        //    ViewBag.TeamId = teamId;
        //    ViewBag.UserEmpNo = userEmpNo;

        //    ViewBag.CompanyId = companyId;
        //    ViewBag.TicketTypeId = ticketTypeId;
        //    ViewBag.CategoryId = categoryId;
        //    ViewBag.StatusId = statusId;
        //    ViewBag.StartDate = sDate.ToString("MM/dd/yyyy");
        //    ViewBag.EndDate = eDate.ToString("MM/dd/yyyy");
        //    return View(items);

        //}

        [AccessAuthorize]
        [HttpGet]
        public ActionResult GatePass(long? transactionTypeId, long? statusId, long? branchId, long? departmentId
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

            AssetTransferRequestService _service = new AssetTransferRequestService();
            var items = _service.GetTransferItems(transactionTypeId, statusId, branchId, departmentId, sDate, eDate.AddDays(1)
                    , "AssetTransferRequest,AssetTransferRequest.InitiatedUser,AssetTransferRequest.BranchFrom,AssetTransferRequest.DepartmentFrom,AssetTransferRequest.Branch,AssetTransferRequest.Department,AssignedToUser,AssetTransferRequest.Vendor,Asset");
            //, "InitiatedUser,AuthorizedUser,CompletedUser,BranchFrom,DepartmentFrom,Branch,Department,AssignedToUser,TransferItems,TransferItems.AssignedToUser,TransferItems.Asset,Vendor"");

            return View(items);


        }


        [AccessAuthorize]
        [HttpGet]
        public ActionResult AssetTransfers(long? branchId, long? departmentId
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
            if (branchId == null)
                branchId = 0;
            if (departmentId == null)
                departmentId = 0;

            ViewBag.BranchId = branchId;
            ViewBag.DepartmentId = departmentId;
            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");

            AssetTransferRequestService _service = new AssetTransferRequestService();
            var items = _service.GetTransferItems(branchId, departmentId, sDate, eDate.AddDays(1)
                    , "AssetTransferRequest,AssetTransferRequest.BranchFrom" +
                    ",AssetTransferRequest.DepartmentFrom,AssetTransferRequest.Branch,AssetTransferRequest.Department,AssignedToUser,Asset,Asset.AssetType");
            //, "InitiatedUser,AuthorizedUser,CompletedUser,BranchFrom,DepartmentFrom,Branch,Department,AssignedToUser,TransferItems,TransferItems.AssignedToUser,TransferItems.Asset,Vendor"");

            return View(items);


        }


        [AccessAuthorize]
        [HttpGet]
        public ActionResult GRNStockIn(long? vendorId, string startDate, string endDate
            , string gRNNo, string pONo, string invoiceNo)
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
            if (vendorId == null)
                vendorId = 0;

            ViewBag.GRNNo = gRNNo;
            ViewBag.PONo = pONo;
            ViewBag.InvoiceNo = invoiceNo;
            ViewBag.VendorId = vendorId;
            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");

            GRNoteService _grnService = new GRNoteService();
            var items = _grnService.GetGRNs(vendorId, sDate, eDate.AddDays(1), gRNNo, pONo, invoiceNo
                    , "Vendor");

            return View(items);


        }


        [HttpGet]
        [AccessAuthorize]
        public ActionResult AssetDiscrepancy(long? assetCategoryId, long? branchId, long? departmentId, string startDate, string endDate)
        {
            if (branchId == null)
                branchId = 0;
            if (departmentId == null)
                departmentId = 0;
            if (assetCategoryId == null || assetCategoryId == 0)
                assetCategoryId = (long)AssetCategoryEnum.ITAsset;
            TicketService _ticketService = new TicketService();
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

            ViewBag.BranchId = branchId;
            ViewBag.DepartmentId = departmentId;
            ViewBag.AssetCategoryId = assetCategoryId;
            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");

            AssetVerificationRequestService _assetVerificationService = new AssetVerificationRequestService();
            var items = _assetVerificationService.GetItemsDiscrepancy(assetCategoryId, branchId, departmentId, sDate, eDate.AddDays(1),
                "AssetVerificationRequest,AssetVerificationRequest.Branch,AssetVerificationRequest.Department,AssetVerificationRequest.RequestedUser,AssetVerificationRequest.VerifiedUser" +
                ",Asset,Asset.AssetType,Asset.AssetMake").OrderByDescending(o => o.UpdatedDate).ToList();
            return View(items);

        }


        [HttpGet]
        [AccessAuthorize]
        public ActionResult Assets(long? assetCategoryId, long? assetTypeId, long? allocatedTypeId
            , long? branchId, long? departmentId, int[] statusId, string checkboxWarranty, string checkboxMaintenance, string checkboxToBeReturned
            , string assetNo, string serialNo, string barcodeNo, string assignTo)
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
            AssetService _assetService = new AssetService();

            var items = _assetService.GetItemsAdv(assetCategoryId, assetTypeId, allocatedTypeId, branchId, departmentId, statusId, UserDateTime.GetUserDate()
                    , warranty, maintenance, toBeReturned, assetNo, serialNo, barcodeNo, assignTo,
                    "AssetType,AssetMake,Branch,Department,AssignedToUser,Vendor,ResponsibleTeam");

            return View(items);


        }

    }
}
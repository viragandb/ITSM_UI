using Domain;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers
{
    public class ServiceDeskController : Controller
    {
        private readonly TicketService _service = new TicketService();
        // GET: ServiceDesk
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AccessLogin]
        public ActionResult ServiceRequest()
        {
            CategoryService _categoryService = new CategoryService();
            AssetTypeService _assetTypesService = new AssetTypeService();
            SubCategoryService _subCatService = new SubCategoryService();
            var ticketType = TicketTypeEnum.SR;
            var items = _categoryService.GetAllByTicketType(ticketType, "");
            foreach (Category cat in items)
            {
                cat.AssetTypes = _assetTypesService.GetAllByTicketType(ticketType, cat.CategoryId, "").ToList();

                foreach (AssetType asset in cat.AssetTypes)
                {
                    asset.SubCategories = _subCatService.GetAllByTicketType(ticketType, cat.CategoryId, asset.AssetTypeId, "").ToList();
                }
            }
            return View(items);
        }

        [HttpGet]
        [AccessLogin]
        public ActionResult Incident()
        {
            CategoryService _categoryService = new CategoryService();
            AssetTypeService _assetTypesService = new AssetTypeService();
            SubCategoryService _subCatService = new SubCategoryService();
            var ticketType = TicketTypeEnum.IN;
            var items = _categoryService.GetAllByTicketType(ticketType, "");
            foreach (Category cat in items)
            {

                cat.AssetTypes = _assetTypesService.GetAllByTicketType(ticketType, cat.CategoryId, "").ToList();

                foreach (AssetType asset in cat.AssetTypes)
                {
                    asset.SubCategories = _subCatService.GetAllByTicketType(ticketType, cat.CategoryId, asset.AssetTypeId, "").ToList();
                }
            }


            return View(items);
        }

        [AccessLogin]
        [HttpGet]
        [EncryptedActionParameter]
        public ActionResult CreateRequest(long? ticketType, long? categoryId, long? assetTypeId, long? subCategoryId)
        {
            var item = new TicketVM();

            if (ticketType == 0 || categoryId == 0 || assetTypeId == 0 || subCategoryId == 0)
            {
                TempData["ErrorMessage"] = "Sorry, something went wrong, Record couldn't update, Please contact the IT support. ";

                return HttpNotFound();

            }
            var type = new TicketTypeEnum();
            if (ticketType == 1)
                type = TicketTypeEnum.SR;
            else if (ticketType == 2)
                type = TicketTypeEnum.IN;

            RequestTypeService _requestTypeService = new RequestTypeService();
            RequestType requestType = new RequestType();
            requestType = _requestTypeService.GetItemAsNoTracking(type, (Int64)categoryId, (Int64)assetTypeId, (Int64)subCategoryId, "Category,SubCategory,AssetType");

            item.RequestTypeId = requestType.RequestTypeId;
            item.RequestType = requestType;
            item.TicketType = type;
            item.CategoryId = (long)categoryId;
            item.AssetTypeId = (long)assetTypeId;
            item.SubCategoryId = (long)subCategoryId;
            item.TicketTypeId = (long)ticketType;
            item.RequestedBy = Session["UserId"].ToString();
            item.RequestedUser = new User();
            item.RequestedUser.EmpNo = item.RequestedBy;
            item.OccurredDate = UserDateTime.GetUserDateOnly().ToString("dd/MM/yyyy");
            item.OccurredTime = UserDateTime.GetUserDate().ToShortTimeString();

            UserService _userService = new UserService();
            var user = _userService.GetAsNoTrackingUserByEmpNo(item.RequestedBy, "");
            if(user.BranchId!=null)
            item.BranchId = (long)user.BranchId;
            if(user.DepartmentId != null)
            item.DepartmentId = (long)user.DepartmentId;

            item.ContactNo = user.ContactNo;
            item.ContactEmail = user.Email;


            item.PendingTickets = _service.GetPendingTicketCountByUserId(Session["UserId"].ToString());

            ViewBag.OccurredTime = item.OccurredTime;
            ViewBag.CategoryId = item.CategoryId;
            ViewBag.SubCategoryId = item.SubCategoryId;
            ViewBag.AssetTypeId = item.AssetTypeId;
            ViewBag.BranchId = item.BranchId;
            ViewBag.DepartmentId = item.DepartmentId;
            return View(item);
        }

        [AccessLogin]
        [HttpPost]
        public ActionResult CreateRequest(TicketVM item, string Occurred_Time)
        {

            //if (ModelState.IsValid)
            //{

            ViewBag.CategoryId = item.CategoryId;
            ViewBag.SubCategoryId = item.SubCategoryId;
            ViewBag.AssetTypeId = item.AssetTypeId;
            ViewBag.BranchId = item.BranchId;
            ViewBag.DepartmentId = item.DepartmentId;
            ViewBag.OccurredTime = Occurred_Time;
            var dateOccurredDate = UserDateTime.GetUserDate();

            if (item.CategoryId == 0 || item.AssetTypeId == 0
                || item.SubCategoryId == 0 || item.BranchId == 0 || item.DepartmentId == 0 || item.Subject == null
                || item.ContactNo == null || item.ContactNo == "")
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }
            else if (item.TicketType == TicketTypeEnum.IN && item.TicketPriorityId == 0)
            {
                TempData["ErrorMessage"] = "Please select Urgency and Impact levels. ";
                return View(item);
            }

            if (item.TicketType == TicketTypeEnum.IN)
            {

                try
                {
                    dateOccurredDate = UserDateTime.ConvertToAppDateTime(item.OccurredDate + " " + Occurred_Time);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Please enter valid Occurred Date/Time. ";
                    return View(item);
                }

            }

            UserService _userService = new UserService();
            User requestedUser = new User();
            requestedUser = _userService.GetAsNoTrackingUserByEmpNo(Session["UserId"].ToString(), "");
            item.RequestedBy = Session["UserId"].ToString();

            Ticket newTicket = new Ticket();
            newTicket.Subject = item.Subject;
            newTicket.TicketMedium = TicketMediumEnum.User;
            newTicket.Description = item.Description;
            newTicket.DepartmentName = requestedUser.DepartmentName;
            newTicket.BranchId = item.BranchId;
            newTicket.DepartmentId = item.DepartmentId;
            newTicket.OccurredDate = dateOccurredDate;

            newTicket.ContactEmail = item.ContactEmail;
            newTicket.ContactNo = item.ContactNo;

            newTicket.RequestedBy = requestedUser.EmpNo;
            newTicket.UpdatedBy = Session["UserId"].ToString();
            newTicket.CreatedBy = Session["UserId"].ToString();
            newTicket.UpdatedDate = UserDateTime.GetUserDate();
            newTicket.RequestedDate = UserDateTime.GetUserDate();
            newTicket.SpentTimeSync = newTicket.RequestedDate;

            RequestTypeService _requestTypeService = new RequestTypeService();
            RequestType requestType = new RequestType();
            requestType = _requestTypeService.GetItemAsNoTracking(item.TicketType, item.CategoryId, item.AssetTypeId, item.SubCategoryId, "Category,SubCategory,AssetType,Team,RequestTypePriorities");
            newTicket.RequestTypeId = requestType.RequestTypeId;

            SerialService _serialService = new SerialService();
            newTicket.Code = _serialService.GetTicketSerial().ToString("00000000");

            newTicket.Level01TeamId = requestType.TeamId;
            newTicket.PendingTeamId = requestType.TeamId;
            newTicket.AllocatedType = AllocatedTypeEnum.Internal;

            if (item.TicketType == TicketTypeEnum.IN)
            {
                newTicket.Urgency = item.Urgency;
                newTicket.Impact = item.Impact;

                TicketPriorityService _ticketPriorityService = new TicketPriorityService();
                TicketPriority priority = new TicketPriority();
                priority = _ticketPriorityService.GetItem(item.TicketPriorityId, "");
                newTicket.Priority = priority.Priority;

                var requestPriority = requestType.RequestTypePriorities.Where(w => w.Priority == newTicket.Priority).FirstOrDefault();
                newTicket.Resolve = requestPriority.Resolve;
                newTicket.Respond = requestPriority.Respond;

                newTicket.Code = "IN" + newTicket.Code;
            }
            else
            {
                newTicket.Resolve = Convert.ToDouble(requestType.Sla);
                newTicket.Respond = Convert.ToDouble(requestType.Sla);
                newTicket.Code = "SR" + newTicket.Code;
            }

            newTicket.Status = TicketStatusEnum.Waiting;
            newTicket.FlowType = TicketFlowTypeEnum.Initiate;

            newTicket.SpentTime = 0;
            TicketDocService _ticketDocService = new TicketDocService();
            var tempDocs = _ticketDocService.GetTempDocs(newTicket.UpdatedBy, "");

            var entitySaved = _service.Insert(newTicket);
            if (entitySaved)
            {
                _service.InsertTicketLog(newTicket, "New Ticket created");

                foreach (TempDoc _doc in tempDocs)
                {
                    TicketDoc doc = new TicketDoc();
                    doc.FileName = _doc.FileName;
                    doc.FileUrl = _doc.FileUrl;
                    doc.TicketId = newTicket.TicketId;
                    doc.DocType = DocTypeEnum.Attachment;
                    doc.DocumentName = _doc.DocumentName;
                    doc.Status = newTicket.Status;
                    doc.UpdatedDate = UserDateTime.GetUserDate();
                    doc.UpdatedBy = Session["UserId"].ToString();
                    _ticketDocService.Insert(doc);

                    newTicket.UpdatedDate = doc.UpdatedDate;

                    _service.InsertTicketLog(newTicket, "Document (" + doc.DocumentName + ") was uploaded");
                    string fullPath = Server.MapPath("~/" + GlobalStaticService.GetFileUploadPath());

                    _ticketDocService.DeleteTempDoc(_doc.TempDocId,fullPath);
                }

                #region Assign Asset
                ItemAssetService _itemAssetService = new ItemAssetService();
                AssetService _assetService = new AssetService();
                if (item.AssetId != null)
                {
                    ItemAsset assetItem = new ItemAsset();
                    assetItem.AssetId = (Int64)item.AssetId;
                    assetItem.ItemId = newTicket.TicketId;
                    assetItem.TicketId = assetItem.ItemId;
                    assetItem.ItemType = ItemTypeEnum.Ticket;
                    assetItem.UpdatedBy = Session["UserId"].ToString();
                    assetItem.UpdatedDate = UserDateTime.GetUserDate();
                    if (!_itemAssetService.IsAvailable(assetItem))
                    {
                        if (_itemAssetService.Insert(assetItem))
                        {
                            var asset = _assetService.GetItemAsNoTracking(assetItem.AssetId, "");
                            _service.InsertTicketLog(newTicket, asset.AssetNo + " asset assigned.");

                        }
                    }
                }
                #endregion

                TeamService _teamService = new TeamService();
                var ticketStatus = newTicket;
                ticketStatus.Status = TicketStatusEnum.Waiting;
                ticketStatus.PendingTeamId = _teamService.GetTeamIdDefault();
                ticketStatus.SpentTime = 0;
                _service.InsertTicketStatus(ticketStatus, "New Ticket was initiated");

                //Update user 
                if (requestedUser.ContactNo == "" || requestedUser.ContactNo == null 
                    || requestedUser.Email == "" || requestedUser.Email == null
                    || requestedUser.BranchId == 0 || requestedUser.BranchId == null
                    || requestedUser.DepartmentId == 0 || requestedUser.DepartmentId == null)
                {
                    var _user = _userService.GetUserByEmpNo(requestedUser.EmpNo, "");
                    _user.ContactNo = newTicket.ContactNo;
                    _user.Email = newTicket.ContactEmail;
                    _user.BranchId = newTicket.BranchId;
                    _user.DepartmentId = newTicket.DepartmentId;
                    _user.UpdatedBy = newTicket.UpdatedBy;
                    _user.UpdatedDate = newTicket.UpdatedDate;
                    _userService.Update(_user);
                }

                #region UserEMail
                String emailBody = "";
                EmailTemplate emailTemplate = new EmailTemplate();
                EmailClient emailClient = new EmailClient();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                if (newTicket.ContactEmail != null && newTicket.ContactEmail != "")
                {
                    ToRecipients.Add(newTicket.ContactEmail);

                    emailBody = emailTemplate.TicketCreate(requestedUser.FullName, newTicket, requestType);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New Ticket | Ticket # " + newTicket.Code,
                        emailBody).Trim();

                }
                #endregion


                #region TeamEMail

                emailBody = "";
                ToRecipients.Clear();
                CCRecipients.Clear();

                if (!String.IsNullOrEmpty(requestType.Team.TeamEmail))
                {
                    ToRecipients.Add(requestType.Team.TeamEmail);

                    emailBody = emailTemplate.TicketCreateTeam(requestType.Team.TeamName, newTicket, requestType);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New Ticket | Ticket # " + newTicket.Code, emailBody).Trim();

                }
                #endregion
                TempData["SuccessMessage"] = "Ticket has been successfully created.";
                //return View("Create", item);
                return RedirectToAction("Open", "MyTicket");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View(item);
            }



        }
        
        //[AccessLogin]
        //[HttpPost]
        //public ActionResult Create(Ticket item)
        //{



        //    RequestTypeService _requestTypeService = new RequestTypeService();
        //    RequestType requestType = new RequestType();
        //    requestType = _requestTypeService.GetItem(item.RequestTypeId, "Category,SubCategory,AssetType");
        //    item.RequestType = requestType;
        //    if (item.RequestTypeId == 0 || item.Subject == null)
        //    {
        //        TempData["ErrorMessage"] = "Please enter required data. ";
        //        return View("Create", item);
        //    }
        //    else if (item.RequestType.TicketType == TicketTypeEnum.IN
        //        && (item.TicketPriorityId == 0 || item.TicketPriorityId == null))
        //    {
        //        TempData["ErrorMessage"] = "Please select Urgency and Impact levels. ";
        //        return View("Create", item);
        //    }
        //    Ticket newTicket = new Ticket();

        //    newTicket.RequestedBy = Session["UserId"].ToString();
        //    newTicket.UpdatedBy = Session["UserId"].ToString();
        //    newTicket.CreatedBy = Session["UserId"].ToString();
        //    newTicket.UpdatedDate = UserDateTime.GetUserDate();
        //    newTicket.RequestedDate = UserDateTime.GetUserDate();

        //    UserService _userService = new UserService();
        //    User requestedUser = new User();
        //    requestedUser = _userService.GetHRISEmployee(newTicket.RequestedBy);
        //    if (requestedUser.EmpNo == null)
        //    {
        //        TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
        //        return View("Create", item);
        //    }

        //    newTicket.Subject = item.Subject;
        //    newTicket.TicketMedium = TicketMediumEnum.User;
        //    newTicket.Description = item.Description;
        //    newTicket.CompanyId = requestedUser.CompanyId;
        //    newTicket.Company = requestedUser.CompanyName;
        //    newTicket.DepartmentId = requestedUser.DepartmentId;
        //    newTicket.DepartmentName = requestedUser.DepartmentName;
        //    newTicket.LocationId = requestedUser.LocationId;
        //    newTicket.LocationName = requestedUser.LocationName;

        //    newTicket.ContactEmail = requestedUser.Email;
        //    newTicket.ContactNo = requestedUser.ContactNo;

        //    newTicket.RequestTypeId = item.RequestTypeId;

        //    SerialService _serialService = new SerialService();
        //    newTicket.Code = _serialService.GetTicketSerial().ToString("00000000");

        //    if (requestType.TicketType == TicketTypeEnum.IN)
        //    {
        //        newTicket.Urgency = item.Urgency;
        //        newTicket.Impact = item.Impact;
        //        newTicket.TicketPriorityId = item.TicketPriorityId;

        //        TicketPriorityService _ticketPriorityService = new TicketPriorityService();
        //        TicketPriority priority = new TicketPriority();
        //        priority = _ticketPriorityService.GetItem(item.TicketPriorityId, "");

        //        newTicket.Resolve = priority.Resolve;
        //        newTicket.Respond = priority.Respond;

        //        newTicket.Code = "IN" + newTicket.Code;
        //    }
        //    else
        //    {
        //        newTicket.Resolve = Convert.ToDouble(requestType.Sla);
        //        newTicket.Respond = Convert.ToDouble(requestType.Sla);
        //        newTicket.Code = "SR" + newTicket.Code;
        //    }

        //    newTicket.Status = TicketStatusEnum.Waiting;
        //    newTicket.SpentTime = 0;

        //    var entitySaved = _service.Insert(newTicket);
        //    if (entitySaved)
        //    {

        //        //Email
        //        String emailBody = "";
        //        EmailTemplate emailTemplate = new EmailTemplate();
        //        EmailClient emailClient = new EmailClient();
        //        List<string> ToRecipients = new List<string>();
        //        List<string> CCRecipients = new List<string>();
        //        if (requestedUser.Email != null && requestedUser.Email != "")
        //        {
        //            ToRecipients.Add(requestedUser.Email);

        //            emailBody = emailTemplate.TicketCreate(requestedUser.FullName, newTicket, requestType);
        //            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New Ticket",
        //                emailBody).Trim();

        //        }
        //        TempData["SuccessMessage"] = "Ticket has been successfully created.";

        //        return RedirectToAction("AssignAssetDoc", "Ticket");

        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
        //        return View("Create", item);
        //    }


        //}


    }
}
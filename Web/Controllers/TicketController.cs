using Domain;
using log4net;
using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers
{
    public class TicketController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TicketController));
        private readonly TicketService _service = new TicketService();

        // GET: Ticket

        [AccessAuthorize]
        public ActionResult Index(long? teamId, long? companyId, string userEmpNo, long? ticketTypeId, long? ticketMediumId, long? categoryId, long? allocatedTypeId
            , long? statusId, string startDate, string checkBoxSLA, string checkBoxOnGoing, string endDate, string ticketCode, long? vendorId)
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
            if (companyId == null)
                companyId = 0;
            if (ticketTypeId == null)
                ticketTypeId = 0;
            if (ticketMediumId == null)
                ticketMediumId = 0;
            if (allocatedTypeId == null)
                allocatedTypeId = 0;
            if (categoryId == null)
                categoryId = 0;
            if (statusId == null)
                statusId = 0;
            if (vendorId == null)
                vendorId = 0;
            if (userEmpNo == null || userEmpNo == "0")
                userEmpNo = "";

            bool sla = false;
            ViewBag.SLA = "";
            if (checkBoxSLA == "on")
            {
                sla = true;
                ViewBag.SLA = "checked";
            }

            bool onGoing = false;
            ViewBag.OnGoing = "";
            if (checkBoxOnGoing == "on")
            {
                onGoing = true;
                ViewBag.OnGoing = "checked";
            }
            ViewBag.TicketCode = ticketCode;

            ViewBag.TeamId = teamId;
            ViewBag.UserEmpNo = userEmpNo;

            ViewBag.CompanyId = companyId;
            ViewBag.TicketTypeId = ticketTypeId;
            ViewBag.TicketMediumId = ticketMediumId;
            ViewBag.AllocatedTypeId = allocatedTypeId;
            ViewBag.CategoryId = categoryId;
            ViewBag.StatusId = statusId;
            ViewBag.VendorId = vendorId;
            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");

            ItemAssetService _itemAssetService = new ItemAssetService();
            var items = _ticketService.GetTikcets(teamId, userEmpNo, ticketTypeId, ticketMediumId, allocatedTypeId, categoryId, 0, 0,
                statusId, vendorId, sDate, eDate.AddDays(1), ticketCode, sla, onGoing,
                  "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser,CreatedUser,AssignedToUser,Branch,Department,Vendor,TicketUpdates");
            //foreach (Ticket item in items)
            //{
            //    //item.ItemAssets = _itemAssetService.GetAssets(ItemTypeEnum.Ticket, item.TicketId, "Asset").ToList();
            //}
            return View(items);

            //if (ticketCode == "" || ticketCode == null)
            //{
            //    var items = _ticketService.GetTikcets(teamId, companyId, userEmpNo, ticketTypeId, categoryId, statusId, sDate, eDate.AddDays(1),
            //      "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser,AssignedToUser,Branch");
            //    foreach (Ticket item in items)
            //    {
            //        item.ItemAssets = _itemAssetService.GetAssets(ItemTypeEnum.Ticket, item.TicketId, "Asset").ToList();
            //    }
            //    return View(items);
            //}
            //else
            //{
            //    var items = _ticketService.GetTikcets(ticketCode,
            //     "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser,AssignedToUser,Branch");

            //    foreach (Ticket item in items)
            //    {
            //        item.ItemAssets = _itemAssetService.GetAssets(ItemTypeEnum.Ticket, item.TicketId, "Asset").ToList();
            //    }
            //    return View(items);
            //}


            //TempData["TeamId"] = teamId;


        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {
            //HRISService _hrisService = new HRISService();
            //UserService _userService = new UserService();
            var item = new TicketVM();
            item.RequestedUser = new User();
            item.OccurredDate = UserDateTime.GetUserDateOnly().ToString("dd/MM/yyyy");
            item.OccurredTime = UserDateTime.GetUserDate().ToShortTimeString();
            ViewBag.OccurredTime = item.OccurredTime;

            return View(item);
        }


        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(TicketVM item, string Occurred_Time)
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

            if (item.TicketMedium == 0 || item.TicketType == 0 || item.CategoryId == 0 || item.AssetTypeId == 0
                || item.SubCategoryId == 0 || item.BranchId == 0 || item.DepartmentId == 0 || item.Subject == null || item.RequestedBy == null)
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View("Create", item);
            }
            else if (item.TicketType == TicketTypeEnum.IN && item.TicketPriorityId == 0)
            {
                TempData["ErrorMessage"] = "Please select Urgency and Impact levels. ";
                return View("Create", item);
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
            if (item.RequestedBy != "" && item.RequestedBy != null)
            {
                requestedUser = _userService.GetUserByEmpNo_UpdateByAD(item.RequestedBy, Session["UserId"].ToString(), UserDateTime.GetUserDate(), "");
                if (requestedUser.EmpNo == null)
                {
                    TempData["ErrorMessage"] = "Invalid User. ";
                    return View("Create", item);
                }

            }

            var pendingTicketsCount = _service.GetPendingTicketCountByUserId(item.RequestedBy);
            if (pendingTicketsCount>0)
            {
                TempData["ErrorMessage"] = "Invalid user. The user has pending tickets that need to be closed.";
                return View(item);
            }

                Ticket newTicket = new Ticket();
            newTicket.Subject = item.Subject;
            newTicket.TicketMedium = item.TicketMedium;
            newTicket.Description = item.Description;
            // newTicket.DepartmentName = requestedUser.DepartmentName;
            newTicket.BranchId = item.BranchId;
            newTicket.DepartmentId = item.DepartmentId;

            newTicket.OccurredDate = dateOccurredDate;

            if (item.RequestedUser.Email != null && item.RequestedUser.Email != "")
                newTicket.ContactEmail = item.RequestedUser.Email;
            else
                newTicket.ContactEmail = requestedUser.Email;

            if (item.ContactNo != null && item.ContactNo != "")
                newTicket.ContactNo = item.ContactNo;
            else
                newTicket.ContactNo = requestedUser.ContactNo;

            newTicket.RequestedBy = requestedUser.EmpNo;
            newTicket.UpdatedBy = Session["UserId"].ToString();
            newTicket.CreatedBy = Session["UserId"].ToString();
            newTicket.UpdatedDate = UserDateTime.GetUserDate();
            newTicket.RequestedDate = UserDateTime.GetUserDate();
            newTicket.SpentTimeSync = newTicket.RequestedDate;
            //newTicket.AssignedTo = newTicket.UpdatedBy.ToString();

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
                //newTicket.TicketPriorityId = item.TicketPriorityId;

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
            newTicket.SLAIT = newTicket.Resolve;
            newTicket.Status = TicketStatusEnum.Waiting;
            newTicket.FlowType = TicketFlowTypeEnum.Initiate;
            //TeamService _teamService = new TeamService();
            //var userTeams = _teamService.GetTeamByUserId(Session["UserId"].ToString(), "");
            //foreach (Team team in userTeams)
            //{
            //    if (requestType.TeamId == team.TeamId)
            //    {
            //        newTicket.AssignedBy = newTicket.UpdatedBy;
            //        newTicket.AssignedTo = newTicket.UpdatedBy;
            //        newTicket.Status = TicketStatusEnum.Pending;
            //    }
            //}
            //if (requestType.TeamId == _teamService.GetTeamIdInfra() || requestType.TeamId == _teamService.GetTeamIdTelco())
            //{
            //    newTicket.AssignedBy = newTicket.UpdatedBy;
            //    newTicket.AssignedTo = newTicket.UpdatedBy;
            //    newTicket.Status = TicketStatusEnum.Pending;

            //}

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

                    _ticketDocService.DeleteTempDoc(_doc.TempDocId, fullPath);
                }

                TeamService _teamService = new TeamService();
                var ticketStatus = newTicket;
                ticketStatus.Status = TicketStatusEnum.Waiting;
                ticketStatus.PendingTeamId = _teamService.GetTeamIdDefault();
                ticketStatus.SpentTime = 0;
                _service.InsertTicketStatus(ticketStatus, "New Ticket was initiated");

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

                //Update user contact Number
                if (requestedUser.ContactNo == "" || requestedUser.ContactNo == null)
                {
                    requestedUser.ContactNo = newTicket.ContactNo;
                    _userService.UpdateContactNo(requestedUser.EmpNo, requestedUser.ContactNo);
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
                return RedirectToAction("AssignAssetDoc");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View("Create", item);
            }



        }


        [HttpGet]
        [AccessAuthorize]
        public ActionResult AssignAssetDoc()
        {

            var items = _service.GetItemsByCreatedByPending(Session["UserId"].ToString(),
                "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,RequestedUser,Branch,Department").OrderByDescending(o => o.RequestedDate).ToList();
            ItemDocService _itemDocService = new ItemDocService();
            foreach (Ticket t in items)
            {
                // t.TicketDocs = _itemDocService.GetTicketDocs(t.TicketId, "").ToList();
            }

            Session["BackController"] = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            Session["BackAction"] = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();


            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult DocUpload(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = new DocUploadVM();
            TicketDocService _ticketDocService = new TicketDocService();
            item.Ticket = _service.GetItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,RequestedUser,Branch,Department");
            item.TicketDocs = _ticketDocService.GetTicketDocsByUserStatus(item.Ticket.TicketId, Session["UserId"].ToString(), "").ToList();

            try
            {
                item.BackAction = Session["BackAction"].ToString();
                item.BackController = Session["BackController"].ToString();
            }
            catch (Exception ex)
            {
                MenuItem backMenu = _ticketDocService.GetBackMenuItem(item.Ticket.Status, false);
                item.BackController = backMenu.Controller;
                item.BackAction = backMenu.Action;
            }

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }

        [AccessLogin]
        [HttpPost]
        public async Task<ActionResult> DocUpload(HttpPostedFileBase itemFile, DocUploadVM item)
        {
            TicketDocService _ticketDocService = new TicketDocService();
            //ItemDocService _itemDocService = new ItemDocService();
            item.Ticket = _service.GetItem(item.Ticket.TicketId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,RequestedUser,Branch,Department");
            item.TicketDocs = _ticketDocService.GetTicketDocsByUserStatus(item.Ticket.TicketId, Session["UserId"].ToString(), "").ToList();

            if (item.FileName == null || item.FileName == "")
            {
                TempData["ErrorMessage"] = "Please enter file name. ";
                return View(item);
            }

            try
            {
                if (itemFile != null && itemFile.ContentLength != 0)
                {
                    if (itemFile.ContentLength > 2097152) //2MB
                    {
                        TempData["ErrorMessage"] = "Please reduce file size (Max. 2MB). ";
                        return View(item);
                    }
                    var supportedTypes = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".docx", ".doc", ".xlsx", ".xls" };
                    if (supportedTypes.Contains(Path.GetExtension(itemFile.FileName).ToLower()))
                    {
                        string dirname = GlobalStaticService.GetFileUploadPath();
                        string _FileName = Guid.NewGuid().ToString() + Path.GetExtension(itemFile.FileName);
                        string _path = Path.Combine(Server.MapPath("~/" + dirname), _FileName);
                        itemFile.SaveAs(_path);
                        TicketDoc doc = new TicketDoc();
                        doc.FileName = _FileName;
                        doc.FileUrl = "/" + dirname + _FileName;
                        doc.TicketId = item.Ticket.TicketId;
                        doc.DocumentName = item.FileName;
                        doc.Status = item.Ticket.Status;
                        doc.UpdatedDate = UserDateTime.GetUserDate();
                        doc.UpdatedBy = Session["UserId"].ToString();
                        _ticketDocService.Insert(doc);

                        item.TicketDocs.Add(doc);

                        item.Ticket.UpdatedBy = doc.UpdatedBy;
                        item.Ticket.UpdatedDate = doc.UpdatedDate;
                        _service.InsertTicketLog(item.Ticket, "Document (" + doc.DocumentName + ") was uploaded");

                        TempData["SuccessMessage"] = "File has been successfully uploaded.";
                        if (item.Ticket.Status == TicketStatusEnum.Pending)
                            return View(item);
                        else
                            return RedirectToAction(item.BackAction, item.BackController);


                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please upload a valid format. ";
                        return View(item);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Please upload a document. ";
                    return View(item);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Sorry, something went wrong, Record couldn't update, Please contact the IT support. (" + ex.Message.ToString() + ") ";
                return View(item);
            }


        }

        [AccessLogin]
        [HttpPost]
        public ActionResult UploadFiles()
        {
            try
            {
                string dirname = GlobalStaticService.GetFileUploadPath();
                TicketDocService _ticketDocService = new TicketDocService();
                ItemDocService _docService = new ItemDocService();
                string _FileName = "";
                string path = Server.MapPath("~/" + GlobalStaticService.GetFileUploadPath());

                HttpFileCollectionBase files = Request.Files;
                int vaildFilesCount = 0;
                string errorMsg = "";
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    _FileName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(file.FileName);
                    var supportedTypes = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".docx", ".doc", ".xlsx", ".xls" };

                    if (supportedTypes.Contains(Path.GetExtension(file.FileName).ToLower()))
                    {
                        if (file.ContentLength > GlobalStaticService.GetMaximumFileSize()) //3MB
                        {
                            errorMsg += "Please reduce file size (Max. 3MB) of " + file.FileName + "<br>";
                        }
                        else
                        {

                            file.SaveAs(path + _FileName);
                            var doc = new TempDoc();
                            doc.FileName = _FileName;
                            doc.FileUrl = "/" + dirname + _FileName;
                            doc.EmpNo = Session["UserId"].ToString();
                            doc.DocumentName = file.FileName;
                            doc.UpdatedDate = UserDateTime.GetUserDate();
                            doc.UpdatedBy = Session["UserId"].ToString();
                            _docService.InsertTempDoc(doc);

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

        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult AssignAsset(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = new AssignAssetVM();
            item.ItemId = Convert.ToInt64(id);
            item.Ticket = _service.GetItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,RequestedUser,Branch,PendingTeam,Department");

            ItemAssetService _itemAssetService = new ItemAssetService();
            item.AssignedItems = _itemAssetService.GetAssets(ItemTypeEnum.Ticket, id, "Asset,Asset.AssetType,Asset.AssetMake").ToList();

            AssetService _assetService = new AssetService();
            item.LocationItems = _assetService.GetItemsByLocation(0, 0, item.Ticket.BranchId, item.Ticket.DepartmentId, "AssetType,AssetMake").ToList();

            //item.UserItems = _assetService.GetMyAssets(item.Ticket.RequestedBy, "AssetType,AssetMake").ToList();

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult DeleteAsset(long? assetItemId, long? ticketId)
        {
            int errorCode = 0;
            ItemAssetService _itemAssetService = new ItemAssetService();
            var item = _itemAssetService.GetItem(assetItemId, "Asset");
            item.ItemId = Convert.ToInt64(ticketId);
            item.UpdatedDate = UserDateTime.GetUserDate();
            item.UpdatedBy = Session["UserId"].ToString();

            //var ticket = _service.GetItem(item.ItemId, "");
            if (item != null)
            {
                if (_itemAssetService.Delete(item))
                {
                    ItemLogService _logService = new ItemLogService();
                    ItemLog log = new ItemLog();
                    log.ItemType = ItemTypeEnum.Ticket;
                    log.ItemId = item.ItemId;
                    log.Comment = "Asset " + item.Asset.AssetNo + " has been deleted";
                    log.UpdatedBy = item.UpdatedBy;
                    log.UpdatedDate = item.UpdatedDate;
                    _logService.Insert(log);
                    TempData["SuccessMessage"] = "Asset has been successfully deleted.";

                    errorCode = 1;
                }

            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult AssignPending()
        {

            //var items = _service.GetItemsPendingByTeam(Session["UserId"].ToString(), 
            var items = _service.GetItemsNotCompletedByTeam(Session["UserId"].ToString(),
                "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,RequestedUser,Branch,PendingTeam,Department").OrderByDescending(o => o.RequestedDate).ToList();
            ItemDocService _itemDocService = new ItemDocService();
            //foreach (Ticket t in items)
            //{
            //    // t.TicketDocs = _itemDocService.GetTicketDocs(t.TicketId, "").ToList();
            //}

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Assign(long? ticketId, string comment, string userId)
        {
            int errorCode = 0;

            var item = _service.GetItem(ticketId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority");


            if (item != null)
            {
                UserService _userService = new UserService();
                if (_userService.IsUserExist(userId))
                {
                    item.UpdatedBy = Session["UserId"].ToString();
                    item.UpdatedDate = UserDateTime.GetUserDate();

                    item.AssignedBy = item.UpdatedBy;
                    item.AssignedTo = userId;
                    item.Status = TicketStatusEnum.Pending;

                    TicketUpdateService _ticketUpdateService = new TicketUpdateService();
                    var lastTicketUpdate = _ticketUpdateService.GetLastItem(item.TicketId, "");
                    DateTimeService _dateTimeService = new DateTimeService();
                    double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastTicketUpdate.UpdatedDate, item.UpdatedDate);

                    item.SpentTime = item.SpentTimeStatus + spentTimeStatus;
                    item.SpentTimeStatus = item.SpentTime;
                    item.SpentTimeSync = item.UpdatedDate;
                    item.RespondTime = item.SpentTime;

                    var entitySaved = _service.Update(item);
                    if (entitySaved)
                    {
                        var ticketStatus = item;
                        ticketStatus.SpentTime = spentTimeStatus;
                        _service.InsertTicketStatus(ticketStatus, comment);

                        _service.InsertTicketLog(item, "Ticket assigned.");

                        errorCode = 1;

                        #region EMail
                        String emailBody = "";
                        var assigedUser = _userService.GetAsNoTrackingUserByEmpNo(item.AssignedTo, "");
                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();
                        if (!String.IsNullOrEmpty(assigedUser.Email))
                        {
                            ToRecipients.Add(assigedUser.Email);

                            emailBody = emailTemplate.TicketAssigned(assigedUser.FullName, item, item.RequestType);
                            String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New Ticket Assigned | Ticket # " + item.Code,
                                emailBody).Trim();

                        }
                        #endregion

                        TempData["SuccessMessage"] = "Ticket has been successfully assigned.";
                        // return RedirectToAction("Pending");
                    }
                    else
                    {
                        errorCode = 2;
                        TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                    }
                }
                else
                {
                    errorCode = 2;
                    TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                }

            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult AssignMe(long? ticketId, string comment)
        {
            int errorCode = 0;

            var item = _service.GetItem(ticketId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority");

            try
            {
                if (item != null)
                {
                    UserService _userService = new UserService();

                    item.UpdatedBy = Session["UserId"].ToString();
                    item.UpdatedDate = UserDateTime.GetUserDate();

                    item.AssignedBy = item.UpdatedBy;
                    item.AssignedTo = item.UpdatedBy;
                    item.Status = TicketStatusEnum.Pending;

                    TicketUpdateService _ticketUpdateService = new TicketUpdateService();
                    var lastTicketUpdate = _ticketUpdateService.GetLastItem(item.TicketId, "");
                    DateTimeService _dateTimeService = new DateTimeService();
                    double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastTicketUpdate.UpdatedDate, item.UpdatedDate);

                    item.SpentTime = item.SpentTimeStatus + spentTimeStatus;
                    item.SpentTimeStatus = item.SpentTime;
                    item.SpentTimeSync = item.UpdatedDate;
                    item.RespondTime = item.SpentTime;

                    var entitySaved = _service.Update(item);
                    if (entitySaved)
                    {
                        var ticketStatus = item;
                        ticketStatus.SpentTime = spentTimeStatus;
                        _service.InsertTicketStatus(ticketStatus, comment);
                        _service.InsertTicketLog(item, "Ticket was assigned.");

                        errorCode = 1;

                        TempData["SuccessMessage"] = "Ticket has been successfully assigned.";
                        // return RedirectToAction("Pending");

                    }
                    else
                    {
                        errorCode = 2;
                        TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(), ex);

            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        //[HttpGet]
        //[AccessAuthorize]
        //public ActionResult PendingTeam()
        //{

        //    var items = _service.GetItemsPendingByTeam(Session["UserId"].ToString(),
        //        "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,RequestedUser,Branch,PendingTeam").OrderByDescending(o => o.RequestedDate).ToList();
        //    ItemDocService _itemDocService = new ItemDocService();
        //    foreach (Ticket t in items)
        //    {
        //        // t.TicketDocs = _itemDocService.GetTicketDocs(t.TicketId, "").ToList();
        //    }

        //    if (items == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(items);
        //}

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Pending()
        {

            var items = _service.GetItemsByAssignedToPending(Session["UserId"].ToString(), 0,
            "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,RequestedUser,Branch,PendingTeam,Department" +
            ",TicketTasks,TicketTasks.AllocatedTeam,TicketTasks.AssignedToUser" +
            ",TicketUpdates,TicketUpdates.UpdatedUser,TicketUpdates.Team,ItemAssets,ItemAssets.Asset,ItemAssets.Asset.AssetType,ItemAssets.Asset.AssetMake").OrderByDescending(o => o.RequestedDate).ToList();

            //            ",RequestedUser,TicketUpdates,TicketUpdates.UpdatedUser,TicketUpdates.Team,AssignedToUser,AssignedByUser,CreatedUser" +
            //",Branch,TicketLogs,TicketDocs,Level01Team,PendingTeam,TicketTasks,TicketTasks.AssignedByUser,TicketTasks.AssignedToUser,TicketTasks.CreatedUser,TicketTasks.AllocatedTeam" +
            //",ItemAssets,ItemAssets.Asset,ItemAssets.Asset.AssetType,ItemAssets.Asset.AssetMake");

            Session["BackController"] = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            Session["BackAction"] = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult ChangeTicket(long? id)
        {
            TempData["ActiveTicketId"] = id;

            if (id == null)
                return HttpNotFound();
            var item = new TicketChangeVM();
            item.Ticket = _service.GetItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,TicketUpdates,RequestedUser,Branch,Department");
            item.TicketType = item.Ticket.RequestType.TicketType;
            item.TicketTypeId = (Int64)item.TicketType;
            item.TicketId = item.Ticket.TicketId;
            item.CategoryId = item.Ticket.RequestType.CategoryId;
            item.AssetTypeId = item.Ticket.RequestType.AssetTypeId;
            item.SubCategoryId = item.Ticket.RequestType.SubCategoryId;
            item.Subject = item.Ticket.Subject;

            if (item.TicketType == TicketTypeEnum.IN)
            {
                item.Urgency = item.Ticket.Urgency;
                item.Impact = item.Ticket.Impact;
                item.Priority = item.Ticket.Priority;
            }

            ViewBag.CategoryId = item.CategoryId;
            ViewBag.AssetTypeId = item.AssetTypeId;
            ViewBag.SubCategoryId = item.SubCategoryId;

            ViewBag.BackController = Session["BackController"].ToString();
            ViewBag.BackAction = Session["BackAction"].ToString();

            TicketTaskService _taskService = new TicketTaskService();
            if (_taskService.IsTaskNotCompleted(item.TicketId))
            {
                TempData["ErrorMessage"] = "Please complete all tasks";
                return RedirectToAction("Pending");
            }

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult ChangeTicket(TicketChangeVM item)
        {
            ViewBag.BackController = Session["BackController"].ToString();
            ViewBag.BackAction = Session["BackAction"].ToString();
            ViewBag.CategoryId = item.CategoryId;
            ViewBag.AssetTypeId = item.AssetTypeId;
            ViewBag.SubCategoryId = item.SubCategoryId;

            item.TicketType = (TicketTypeEnum)item.TicketTypeId;

            item.Ticket = _service.GetItemAsNoTracking(item.TicketId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,TicketUpdates,RequestedUser,PendingTeam,Branch,Department");

            if (item.CategoryId == 0 || item.AssetTypeId == 0 || item.SubCategoryId == 0 || item.Subject == null || item.Comment == null)
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }
            else if (item.TicketType == TicketTypeEnum.IN && item.TicketPriorityId == 0)
            {
                TempData["ErrorMessage"] = "Please select Urgency and Impact levels. ";
                return View(item);
            }

            var ticket = _service.GetItem(item.TicketId, "");

            RequestTypeService _requestTypeService = new RequestTypeService();
            RequestType requestType = new RequestType();
            requestType = _requestTypeService.GetItemAsNoTracking(item.TicketType, item.CategoryId, item.AssetTypeId, item.SubCategoryId, "Category,SubCategory,AssetType,Team,RequestTypePriorities");
            ticket.RequestTypeId = requestType.RequestTypeId;

            bool isTeamChange = false;
            string teamComment = "";
            bool isRequestChange = false;
            string requestComment = "";
            bool isPriorityChange = false;
            string priorityComment = "";
            bool isSLAChange = false;
            string slaComment = "";

            if (item.Ticket.RequestTypeId != requestType.RequestTypeId)
            {
                isRequestChange = true;
                requestComment = item.Ticket.RequestType.Category.CategoryName + " | " + item.Ticket.RequestType.AssetType.AssetTypeName + " | " +
                    item.Ticket.RequestType.SubCategory.SubCategoryName + " -> " +
                    requestType.Category.CategoryName + " | " + requestType.AssetType.AssetTypeName + " | " + requestType.SubCategory.SubCategoryName;
            }

            if (item.Ticket.PendingTeamId != requestType.TeamId)
            {
                isTeamChange = true;
                ticket.PendingTeamId = requestType.TeamId;
                ticket.AllocatedType = AllocatedTypeEnum.Internal;
                teamComment = item.Ticket.PendingTeam.TeamName + " -> " + requestType.Team.TeamName;
            }

            if (item.TicketType == TicketTypeEnum.IN)
            {
                ticket.Urgency = item.Urgency;
                ticket.Impact = item.Impact;

                TicketPriorityService _ticketPriorityService = new TicketPriorityService();
                TicketPriority priority = new TicketPriority();
                priority = _ticketPriorityService.GetItemAsNoTracking(item.TicketPriorityId, "");
                ticket.Priority = priority.Priority;
                if (item.Ticket.Priority != priority.Priority)
                {
                    isPriorityChange = true;
                    priorityComment = item.Ticket.Priority.EnumDisplayName() + " -> " + priority.Priority.EnumDisplayName();
                }
                var requestPriority = requestType.RequestTypePriorities.Where(w => w.Priority == ticket.Priority).FirstOrDefault();
                ticket.Resolve = requestPriority.Resolve;
                ticket.Respond = requestPriority.Respond;

            }
            else
            {
                ticket.Resolve = Convert.ToDouble(requestType.Sla);
                ticket.Respond = Convert.ToDouble(requestType.Sla);
            }

            if (item.Ticket.Resolve != ticket.Resolve)
            {
                isSLAChange = true;
                slaComment = item.Ticket.Resolve + " -> " + ticket.Resolve;
            }
            ticket.UpdatedBy = Session["UserId"].ToString();
            ticket.UpdatedDate = UserDateTime.GetUserDate();

            if (isTeamChange)
            {
                ticket.Status = TicketStatusEnum.Waiting;
                ticket.AssignedTo = null;
                ticket.AllocatedType = AllocatedTypeEnum.Internal;
            }
            else
            {
                ticket.Status = TicketStatusEnum.Pending;

            }
            TicketUpdateService _ticketUpdateService = new TicketUpdateService();
            var lastTicketUpdate = _ticketUpdateService.GetLastItem(item.TicketId, "");
            DateTimeService _dateTimeService = new DateTimeService();
            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastTicketUpdate.UpdatedDate, ticket.UpdatedDate);

            ticket.SpentTime = ticket.SpentTimeStatus + spentTimeStatus;
            ticket.SpentTimeStatus = ticket.SpentTime;
            ticket.SpentTimeSync = ticket.UpdatedDate;

            var entitySaved = _service.Update(ticket);
            if (entitySaved)
            {
                var ticketStatus = ticket;
                ticketStatus.SpentTime = spentTimeStatus;
                _service.InsertTicketStatus(ticketStatus, item.Comment);

                if (isRequestChange)
                    _service.InsertTicketLog(ticket, "Request Type was changed. ( " + requestComment + " )");
                if (isTeamChange)
                    _service.InsertTicketLog(ticket, "Assigned Team was changed. ( " + teamComment + " )");
                if (isPriorityChange)
                    _service.InsertTicketLog(ticket, "Ticket Priority was changed. ( " + priorityComment + " )");
                if (isSLAChange)
                    _service.InsertTicketLog(ticket, "SLA was changed. ( " + slaComment + " )");



                TempData["SuccessMessage"] = "Ticket has been successfully updated.";
                return RedirectToAction(ViewBag.BackAction, ViewBag.BackController);

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View(item);
            }

        }

        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult TransferTicket(long? id)
        {
            TempData["ActiveTicketId"] = id;

            if (id == null)
                return HttpNotFound();
            var item = new TicketTransferVM();
            item.Ticket = _service.GetItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,TicketUpdates,RequestedUser,Branch,Department");
            item.TicketId = item.Ticket.TicketId;
            item.PendingTeamId = item.Ticket.PendingTeamId;

            ViewBag.PendingTeamId = item.PendingTeamId;

            ViewBag.BackController = Session["BackController"].ToString();
            ViewBag.BackAction = Session["BackAction"].ToString();

            TicketTaskService _taskService = new TicketTaskService();
            if (_taskService.IsTaskNotCompleted(item.TicketId))
            {
                TempData["ErrorMessage"] = "Please complete all tasks";
                return RedirectToAction("Pending");
            }

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult TransferTicket(TicketTransferVM item)
        {
            ViewBag.BackController = Session["BackController"].ToString();
            ViewBag.BackAction = Session["BackAction"].ToString();
            ViewBag.PendingTeamId = item.PendingTeamId;

            item.Ticket = _service.GetItemAsNoTracking(item.TicketId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,TicketUpdates,RequestedUser,PendingTeam,Branch,Department");

            if (item.PendingTeamId == 0 || item.Comment == null)
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }

            var ticket = _service.GetItem(item.TicketId, "");

            bool isTeamChange = false;
            string teamComment = "";
            if (item.Ticket.PendingTeamId != item.PendingTeamId)
            {
                isTeamChange = true;

            }
            if (!isTeamChange)
            {
                TempData["ErrorMessage"] = "Invalid Team.";
                return View(item);
            }

            TeamService _teamService = new TeamService();
            var newTeam = _teamService.GetItemAsNoTracking(item.PendingTeamId, "");
            // var newTeamName = _teamService.GetItemName(item.PendingTeamId, "");

            ticket.PendingTeamId = item.PendingTeamId;
            teamComment = item.Ticket.PendingTeam.TeamName + " -> " + newTeam.TeamName;

            ticket.UpdatedBy = Session["UserId"].ToString();
            ticket.UpdatedDate = UserDateTime.GetUserDate();
            ticket.Status = TicketStatusEnum.Waiting;
            ticket.AssignedTo = null;
            ticket.AllocatedType = AllocatedTypeEnum.Internal;


            TicketUpdateService _ticketUpdateService = new TicketUpdateService();
            var lastTicketUpdate = _ticketUpdateService.GetLastItem(item.TicketId, "");
            DateTimeService _dateTimeService = new DateTimeService();
            double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastTicketUpdate.UpdatedDate, ticket.UpdatedDate);

            ticket.SpentTime = ticket.SpentTimeStatus + spentTimeStatus;
            ticket.SpentTimeStatus = ticket.SpentTime;
            ticket.SpentTimeSync = ticket.UpdatedDate;

            var entitySaved = _service.Update(ticket);
            if (entitySaved)
            {
                var ticketStatus = ticket;
                ticketStatus.SpentTime = spentTimeStatus;
                _service.InsertTicketStatus(ticketStatus, item.Comment);

                if (isTeamChange)
                    _service.InsertTicketLog(ticket, "Transferred to Level 02 support. ( " + teamComment + " )");

                #region EMail
                String emailBody = "";
                //var assigedUser = _userService.GetAsNoTrackingUserByEmpNo(item.AssignedTo, "");
                EmailTemplate emailTemplate = new EmailTemplate();
                EmailClient emailClient = new EmailClient();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                if (!String.IsNullOrEmpty(newTeam.TeamEmail))
                {
                    ToRecipients.Add(newTeam.TeamEmail);

                    emailBody = emailTemplate.TicketTransfer(newTeam.TeamName, item.Ticket, item.Ticket.RequestType);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Transfer | Ticket # " + item.Ticket.Code,
                        emailBody).Trim();

                }
                #endregion

                TempData["SuccessMessage"] = "Ticket has been successfully transferred.";
                return RedirectToAction(ViewBag.BackAction, ViewBag.BackController);

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View(item);
            }

        }

        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult UpdateTicket(long? id)
        {
            ViewBag.BackController = Session["BackController"].ToString();
            ViewBag.BackAction = Session["BackAction"].ToString();
            TempData["ActiveTicketId"] = id;

            if (id == null)
                return HttpNotFound();
            var item = new TicketUpdateVM();
            item.Ticket = _service.GetItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,TicketUpdates,RequestedUser,Branch,Department");
            item.TicketId = item.Ticket.TicketId;
            item.PendingTeamId = item.Ticket.PendingTeamId;
            if (item.Ticket.IsTimeViolated && (item.Ticket.ReasonForSLA == "" || item.Ticket.ReasonForSLA == null))
                item.IsSLAReasonRequired = true;
            //item.Ticket.VendorRefNo = "";
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult UpdateTicket(TicketUpdateVM item)
        {
            ViewBag.BackController = Session["BackController"].ToString();
            ViewBag.BackAction = Session["BackAction"].ToString();
            TicketStatusService _statusService = new TicketStatusService();


            item.Ticket = _service.GetItem(item.TicketId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,TicketUpdates,RequestedUser,Branch,Branch.DeliverySLA,Department");

            //TicketStatusEnum statusCurrent = item.Ticket.Status;

            if (item.Status == 0 || item.Comment == null || item.TicketId == 0)
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }

            if (item.Status == _statusService.GetVendorStatus() && item.VendorId == 0)
            {
                TempData["ErrorMessage"] = "Please select the vendor. ";
                return View(item);
            }

            if (item.IsSLAReasonRequired && (item.ReasonForSLA == null || item.ReasonForSLA == ""))
            {
                TempData["ErrorMessage"] = "Please enter the reason for SLA Violation. ";
                return View(item);
            }

            var ticket = _service.GetItem(item.TicketId, "RequestedUser");
            bool isNewStatus = true;
            if (item.Status == ticket.Status)
                isNewStatus = false;


            ticket.UpdatedBy = Session["UserId"].ToString();
            ticket.UpdatedDate = UserDateTime.GetUserDate();
            ticket.Status = item.Status;

            TicketUpdateService _ticketUpdateService = new TicketUpdateService();
            var lastTicketUpdate = _ticketUpdateService.GetLastItem(item.TicketId, "");
            DateTimeService _dateTimeService = new DateTimeService();
            // double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastTicketUpdate.UpdatedDate, ticket.UpdatedDate);
            double spentTimeStatus = 0;
            if (_statusService.GetTimeCapture(lastTicketUpdate.Status, item.PendingTeamId, ""))
                spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastTicketUpdate.UpdatedDate, ticket.UpdatedDate);

            ticket.SpentTime = ticket.SpentTimeStatus + spentTimeStatus;
            ticket.SpentTimeStatus = ticket.SpentTime;
            ticket.SpentTimeSync = ticket.UpdatedDate;
            if (ticket.Status == _statusService.GetDiviceInTransitStatus() && isNewStatus)
            {
                ticket.SLAVendor += item.Ticket.Branch.DeliverySLA.SLA;
                ticket.Resolve = ticket.Resolve + ticket.SLAVendor;
                ticket.AllocatedType = AllocatedTypeEnum.External;
            }
            else if (_statusService.IsVenderSLAStatus(ticket.Status) && isNewStatus)
            {
                VendorSLAService _vendorSLAService = new VendorSLAService();
                ticket.VendorRefNo = item.VendorRefNo;
                ticket.SLAVendor += _vendorSLAService.GetVendorSLA(item.VendorId, ticket.RequestType.AssetTypeId);
                ticket.VendorId = item.VendorId;
                ticket.Resolve = ticket.Resolve + ticket.SLAVendor;
                ticket.AllocatedType = AllocatedTypeEnum.External;
            }

            if (item.IsSLAReasonRequired)
                ticket.ReasonForSLA = item.ReasonForSLA;


            var entitySaved = _service.Update(ticket);
            if (entitySaved)
            {
                var ticketStatus = ticket;
                ticketStatus.SpentTime = spentTimeStatus;
                _service.InsertTicketStatus(ticketStatus, item.Comment);

                if (ticket.Status == _statusService.GetDiviceInTransitStatus() && isNewStatus)
                {
                    _service.InsertTicketLog(ticket, "SLA extended for transportation by " + ticket.SLAVendor + " hrs.");
                }
                else if (_statusService.IsVenderSLAStatus(ticket.Status) && isNewStatus)
                {
                    _service.InsertTicketLog(ticket, "SLA extended for " + ticket.Status.EnumDisplayName() + " by " + ticket.SLAVendor + " hrs.");
                }

                if (item.IsSLAReasonRequired)
                    _service.InsertTicketLog(ticket, "SLA Violation reason updated");

                _service.InsertTicketLog(ticket, "Ticket status Update -" + item.Status.EnumDisplayName());

                #region Email

                String emailBody = "";
                EmailTemplate emailTemplate = new EmailTemplate();
                EmailClient emailClient = new EmailClient();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                if (ticket.RequestedUser.Email != null && ticket.RequestedUser.Email != "")
                {
                    ToRecipients.Add(ticket.RequestedUser.Email);

                    emailBody = emailTemplate.TicketUpdate(ticket.RequestedUser.FullName
                        , ticket.Code, item.Status.EnumDisplayName(), item.Comment);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Ticket Updates",
                        emailBody).Trim();
                }
                #endregion

                TempData["SuccessMessage"] = "Ticket has been successfully updated.";
                return RedirectToAction(ViewBag.BackAction, ViewBag.BackController);

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View(item);
            }


        }


        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult CompleteTicket(long? id)
        {

            ViewBag.BackController = Session["BackController"].ToString();
            ViewBag.BackAction = Session["BackAction"].ToString();
            TempData["ActiveTicketId"] = id;

            if (id == null)
                return HttpNotFound();
            var item = new TicketCompleteVM();
            item.Ticket = _service.GetItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,TicketUpdates,RequestedUser,Branch,Department");
            //if (item.Ticket.RequestType.TicketType != TicketTypeEnum.IN)
            //    ViewBag.HideDiv = "hidden";
            //else
            //    ViewBag.HideDiv = "";

            if (item.Ticket.IsTimeViolated && (item.Ticket.ReasonForSLA == "" || item.Ticket.ReasonForSLA == null))
                item.IsSLAReasonRequired = true;

            item.TicketId = item.Ticket.TicketId;
            if (item == null)
            {
                return HttpNotFound();
            }

            TicketTaskService _taskService = new TicketTaskService();
            if (_taskService.IsTaskNotCompleted(item.TicketId))
            {
                TempData["ErrorMessage"] = "Please complete all tasks";
                return RedirectToAction("Pending");
            }

            return View(item);

        }

        [HttpPost]
        [AccessAuthorize]
        public ActionResult CompleteTicket(TicketCompleteVM item)
        {
            ViewBag.BackController = Session["BackController"].ToString();
            ViewBag.BackAction = Session["BackAction"].ToString();

            item.Ticket = _service.GetItem(item.TicketId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,TicketUpdates,RequestedUser,Branch,Department");
            //if (item.Ticket.RequestType.TicketType != TicketTypeEnum.IN)
            //    ViewBag.HideDiv = "hidden";
            //else
            //    ViewBag.HideDiv = "";

            if (item.Comment == null)
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }

            if (item.IsSLAReasonRequired && (item.ReasonForSLA == null || item.ReasonForSLA == ""))
            {
                TempData["ErrorMessage"] = "Please enter the reason for SLA Violation. ";
                return View(item);
            }

            //if (item.Ticket.RequestType.TicketType == TicketTypeEnum.IN)
            //{
            //    if (item.RootCause == null || item.RootCause == ""
            //        || item.LessonsLearnt == null || item.LessonsLearnt == ""
            //        || item.CorrectiveAction == null || item.CorrectiveAction == ""
            //        || item.PreventiveAction == null || item.PreventiveAction == "")
            //    {
            //        TempData["ErrorMessage"] = "Please enter required data. ";
            //        return View(item);
            //    }
            //}
            var ticket = _service.GetItem(item.TicketId, "");

            ticket.UpdatedBy = Session["UserId"].ToString();
            ticket.UpdatedDate = UserDateTime.GetUserDate();
            ticket.Status = TicketStatusEnum.Completed;
            //if (item.Ticket.RequestType.TicketType == TicketTypeEnum.IN)
            //{
            //    ticket.RootCause = item.RootCause;
            //    ticket.LessonsLearnt = item.LessonsLearnt;
            //    ticket.CorrectiveAction = item.CorrectiveAction;
            //    ticket.PreventiveAction = item.PreventiveAction;
            //}
            TicketStatusService _statusService = new TicketStatusService();
            TicketUpdateService _ticketUpdateService = new TicketUpdateService();
            var lastTicketUpdate = _ticketUpdateService.GetLastItem(item.TicketId, "");
            DateTimeService _dateTimeService = new DateTimeService();
            // double spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastTicketUpdate.UpdatedDate, ticket.UpdatedDate);
            double spentTimeStatus = 0;
            if (_statusService.GetTimeCapture(lastTicketUpdate.Status, ticket.PendingTeamId, ""))
                spentTimeStatus = _dateTimeService.GetTicketLogTimes(lastTicketUpdate.UpdatedDate, ticket.UpdatedDate);

            ticket.SpentTime = ticket.SpentTimeStatus + spentTimeStatus;
            ticket.SpentTimeStatus = ticket.SpentTime;
            ticket.SpentTimeSync = ticket.UpdatedDate;

            if (item.IsSLAReasonRequired)
                ticket.ReasonForSLA = item.ReasonForSLA;

            var entitySaved = _service.Update(ticket);
            if (entitySaved)
            {
                var ticketStatus = ticket;
                ticketStatus.SpentTime = spentTimeStatus;
                _service.InsertTicketStatus(ticketStatus, item.Comment);

                if (item.IsSLAReasonRequired)
                    _service.InsertTicketLog(ticket, "SLA Violation reason updated");

                _service.InsertTicketLog(ticket, "Ticket was completed");

             

                #region Email
                String emailBody = "";
                EmailTemplate emailTemplate = new EmailTemplate();
                EmailClient emailClient = new EmailClient();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                if (item.Ticket.RequestedUser.Email != null && item.Ticket.RequestedUser.Email != "")
                {
                    ToRecipients.Add(item.Ticket.RequestedUser.Email);
                    //string ticketUrl = "/Login/LoginAuth?auth=" + UrlAuthentication.CreateAuthUrlRate(item.Ticket.TicketId.ToString(), UserDateTime.GetUserDate());

                    //string enSrt =GlobalStaticService.Encrypt(queryString, DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy"));
                    //return RedirectToAction("TicketInfo", "Ticket", new { q = enSrt });

                    emailBody = emailTemplate.TicketComplete(item.Ticket.RequestedUser.FullName, item.Ticket);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Ticket Updates | Ticket # " + item.Ticket.Code,
                        emailBody).Trim();

                }
                #endregion
                TempData["SuccessMessage"] = "Ticket has been successfully updated.";
                return RedirectToAction("Pending");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View(item);
            }


        }

        //[HttpGet]
        //[AccessAuthorize]
        //public ActionResult Transfer(long? companyId, long? categoryId)
        //{

        //    if (companyId == null)
        //        companyId = 0;
        //    if (categoryId == null)
        //        categoryId = 0;
        //    var items = _service.GetItemsPendingNotAssignedToMe(categoryId, companyId, Session["UserId"].ToString(),
        //        "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority").OrderByDescending(o => o.RequestedDate).ToList();

        //    ViewBag.CompanyId = companyId;
        //    ViewBag.CategoryId = categoryId;

        //    if (items == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(items);
        //}

        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult AssignToMe(long? ticketId)
        //{
        //    int errorCode = 0;

        //    var item = _service.GetItem(ticketId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority");


        //    if (item != null)
        //    {

        //        item.UpdatedBy = Session["UserId"].ToString();
        //        item.UpdatedDate = UserDateTime.GetUserDate();

        //        item.AssignedBy = item.UpdatedBy;
        //        item.AssignedTo = item.UpdatedBy;

        //        var entitySaved = _service.Update(item);
        //        if (entitySaved)
        //        {
        //            ItemLogService _logService = new ItemLogService();
        //            ItemLog log = new ItemLog();
        //            log.ItemType = ItemTypeEnum.Ticket;
        //            log.ItemId = item.TicketId;
        //            log.Comment = "Assigned to " + item.AssignedTo + " by " + item.AssignedBy;
        //            log.UpdatedBy = item.UpdatedBy;
        //            log.UpdatedDate = item.UpdatedDate;
        //            _logService.Insert(log);

        //            errorCode = 1;

        //            TempData["SuccessMessage"] = "Ticket has been successfully assigned.";
        //            // return RedirectToAction("Pending");

        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
        //            // return View(item);
        //        }


        //    }

        //    return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        //}


        //[HttpGet]
        //[AccessAuthorize]
        //public ActionResult Update()
        //{

        //    var items = _service.GetItemsByAssignedTo(Session["UserId"].ToString(),
        //       "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser,TicketPriority,AssignedToUser").OrderByDescending(o => o.RequestedDate).ToList();
        //    TicketStatusService _statusService = new TicketStatusService();

        //    Collection<Ticket> tickets = new Collection<Ticket>();
        //    foreach (Ticket t in items)
        //    {
        //        var ticketStatus = _statusService.GetItem(t.Status, t.RequestType.TeamId, "");
        //        if (ticketStatus != null)
        //        {
        //            if (ticketStatus.AllowUpdate)
        //                tickets.Add(t);
        //        }
        //    }

        //    return View(tickets.ToList());
        //}

        [HttpGet]
        [AccessLogin]
        [EncryptedActionParameter]
        public ActionResult Info(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = _service.GetItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team" +
                ",RequestedUser,TicketUpdates,TicketUpdates.UpdatedUser,TicketUpdates.Team,AssignedToUser,AssignedByUser,CreatedUser" +
                ",Branch,Department,TicketLogs,TicketDocs,Level01Team,PendingTeam,TicketTasks,TicketTasks.AssignedByUser,TicketTasks.AssignedToUser,TicketTasks.CreatedUser,TicketTasks.AllocatedTeam" +
                ",ItemAssets,ItemAssets.Asset,ItemAssets.Asset.AssetType,ItemAssets.Asset.AssetMake,Vendor");

            //item.TicketUpdates.OrderBy(o=> o.UpdatedDate);

            //ItemLogService _logService = new ItemLogService();
            //item.ItemLogs = _logService.GetTicketLogs(id, "UpdatedUser").ToList();

            //ItemDocService _itemDocService = new ItemDocService();
            //item.TicketDocs = _itemDocService.GetTicketDocs(item.TicketId, "").ToList();

            //ItemAssetService _itemAssetService = new ItemAssetService();
            //item.ItemAssets = _itemAssetService.GetAssets(ItemTypeEnum.Ticket, item.TicketId, "Asset").ToList();

            //KedbItemService _kedbItemService = new KedbItemService();
            //item.KedbItem = _kedbItemService.GetItemAsNoTrackinByTicketId(item.TicketId, "");

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }


        //[HttpGet]
        //[AccessAuthorize]
        //[EncryptedActionParameter]
        //public ActionResult Assign(long? id)
        //{

        //    if (id == null)
        //        return HttpNotFound();
        //    var item = new Ticket();
        //    item = _service.GetItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,TicketUpdates");
        //    if (item == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(item);

        //}

        //[HttpPost]
        //[AccessAuthorize]
        //public ActionResult Assign(Ticket item, string empNo)
        //{
        //    //if (ModelState.IsValid)
        //    {

        //        if (empNo == null || empNo == "")
        //        {
        //            TempData["ErrorMessage"] = "Please select a user.";
        //            return View(item);
        //        }

        //        item = _service.GetItem(item.TicketId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority");

        //        item.UpdatedBy = Session["UserId"].ToString();
        //        item.UpdatedDate = UserDateTime.GetUserDate();

        //        item.AssignedBy = item.UpdatedBy;
        //        item.AssignedTo = empNo;
        //        item.Status = TicketStatusEnum.Pending;

        //        var entitySaved = _service.Update(item);
        //        if (entitySaved)
        //        {
        //            ItemLogService _logService = new ItemLogService();
        //            ItemLog log = new ItemLog();
        //            log.ItemType = ItemTypeEnum.Ticket;
        //            log.ItemId = item.TicketId;
        //            log.Comment = "Assigned to " + item.AssignedTo + " by " + item.AssignedBy;
        //            log.UpdatedBy = item.UpdatedBy;
        //            log.UpdatedDate = item.UpdatedDate;
        //            _logService.Insert(log);

        //            TempData["SuccessMessage"] = "Ticket has been successfully assigned.";
        //            return RedirectToAction("Waiting");

        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
        //            return View(item);
        //        }
        //    }
        //    //else
        //    //{
        //    //    TempData["ErrorMessage"] = "Sorry, something went wrong. Record couldn't save, Please contact the IT support.";
        //    //    return View(item);
        //    //}
        //}



        //[HttpGet]
        //[AccessAuthorize]
        //public ActionResult RequestChange(long? companyId, long? categoryId)
        //{
        //    if (companyId == null)
        //        companyId = 0;
        //    if (categoryId == null)
        //        categoryId = 0;
        //    var items = _service.GetItemsChangeCat(categoryId, companyId, Session["UserId"].ToString(),
        //       "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser,TicketPriority,AssignedToUser").ToList();
        //    //.OrderByDescending(o => o.RequestedDate);
        //    TicketStatusService _statusService = new TicketStatusService();

        //    //Collection<Ticket> tickets = new Collection<Ticket>();
        //    //foreach (Ticket t in items)
        //    //{
        //    //    var ticketStatus = _statusService.GetItem(t.Status, t.RequestType.TeamId, "");
        //    //    if (ticketStatus != null)
        //    //    {
        //    //        if (ticketStatus.AllowUpdate)
        //    //            tickets.Add(t);
        //    //    }
        //    //}

        //    if (items == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(items);
        //}

        //[HttpGet]
        //[AccessAuthorize]
        //[EncryptedActionParameter]
        //public ActionResult ChangeRequestType(long? id)
        //{

        //    if (id == null)
        //        return HttpNotFound();
        //    var item = new Ticket();
        //    item = _service.GetAsNoTrackingItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,TicketUpdates");
        //    ViewBag.CategoryId = item.RequestType.CategoryId;
        //    ViewBag.SubCategoryId = item.RequestType.SubCategoryId;
        //    ViewBag.AssetTypeId = item.RequestType.AssetTypeId;
        //    if (item == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(item);

        //}

        //[HttpPost]
        //[AccessAuthorize]
        //public ActionResult ChangeRequestType(Ticket item)
        //{
        //    //if (ModelState.IsValid)
        //    {
        //        if (item.RequestType.CategoryId == 0 || item.RequestType.AssetTypeId == 0
        //    || item.RequestType.SubCategoryId == 0 || item.Subject == null)
        //        {
        //            TempData["ErrorMessage"] = "Please enter required data. ";
        //            return View(item);
        //        }

        //        string changeTypeSrt = "";
        //        Ticket newTicket = _service.GetItem(item.TicketId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,TicketPriority");

        //        item.RequestType.TicketType = newTicket.RequestType.TicketType;
        //        long oldTeamId = newTicket.RequestType.TeamId;
        //        changeTypeSrt = newTicket.RequestType.RequestTypeName;
        //        RequestTypeService _requestTypeService = new RequestTypeService();
        //        RequestType requestType = new RequestType();
        //        requestType = _requestTypeService.GetItemAsNoTracking(item.RequestType.TicketType, item.RequestType.CategoryId, item.RequestType.AssetTypeId, item.RequestType.SubCategoryId, "Category,SubCategory,AssetType,Team");

        //        changeTypeSrt += " -> " + requestType.RequestTypeName;

        //        newTicket.RequestTypeId = requestType.RequestTypeId;

        //        newTicket.Subject = item.Subject;

        //        if (item.RequestType.TicketType == TicketTypeEnum.SR)
        //        {
        //            newTicket.Resolve = Convert.ToDouble(requestType.Sla);
        //            newTicket.Respond = Convert.ToDouble(requestType.Sla);
        //        }

        //        if (oldTeamId != requestType.TeamId)
        //        {
        //            newTicket.Status = TicketStatusEnum.Waiting;
        //            newTicket.AssignedBy = null;
        //            newTicket.AssignedTo = null;
        //        }
        //        newTicket.UpdatedBy = Session["UserId"].ToString();
        //        newTicket.UpdatedDate = UserDateTime.GetUserDate();


        //        var entitySaved = _service.Update(newTicket);
        //        if (entitySaved)
        //        {
        //            ItemLogService _logService = new ItemLogService();
        //            ItemLog log = new ItemLog();
        //            log.ItemType = ItemTypeEnum.Ticket;
        //            log.ItemId = newTicket.TicketId;
        //            log.Comment = changeTypeSrt;
        //            log.UpdatedBy = newTicket.UpdatedBy;
        //            log.UpdatedDate = newTicket.UpdatedDate;
        //            _logService.Insert(log);


        //            #region TeamEMail
        //            if (newTicket.Status == TicketStatusEnum.Waiting)
        //            {
        //                EmailTemplate emailTemplate = new EmailTemplate();
        //                EmailClient emailClient = new EmailClient();
        //                List<string> ToRecipients = new List<string>();
        //                List<string> CCRecipients = new List<string>();
        //                string emailBody = "";

        //                if (requestType.Team.TeamEmail != null && requestType.Team.TeamEmail != "")
        //                {
        //                    ToRecipients.Add(requestType.Team.TeamEmail);

        //                    emailBody = emailTemplate.TicketCreateTeamTransfer(requestType.Team.TeamName, newTicket, requestType);
        //                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New Ticket",
        //                        emailBody).Trim();

        //                }
        //                #endregion
        //            }

        //            TempData["SuccessMessage"] = "Ticket has been successfully updated.";
        //            return RedirectToAction("RequestChange");

        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
        //            return View(item);
        //        }
        //    }
        //    //else
        //    //{
        //    //    TempData["ErrorMessage"] = "Sorry, something went wrong. Record couldn't save, Please contact the IT support.";
        //    //    return View(item);
        //    //}
        //}

        [AccessAuthorize]
        public ActionResult MyTickets(long? ticketTypeId, long? categoryId, long? statusId
            , string startDate, string endDate, string checkBoxSLA, string checkBoxOnGoing)
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

            long? teamId = 0;
            long? companyId = 0;
            if (ticketTypeId == null)
                ticketTypeId = 0;
            if (categoryId == null)
                categoryId = 0;
            if (statusId == null)
                statusId = 0;
            var userEmpNo = Session["UserId"].ToString();

            bool sla = false;
            ViewBag.SLA = "";
            if (checkBoxSLA == "on")
            {
                sla = true;
                ViewBag.SLA = "checked";
            }

            bool onGoing = false;
            ViewBag.OnGoing = "";
            if (checkBoxOnGoing == "on")
            {
                onGoing = true;
                ViewBag.OnGoing = "checked";
            }
            //ViewBag.TicketCode = ticketCode;

            var items = _ticketService.GetTikcets(teamId, userEmpNo, ticketTypeId, 0, 0, categoryId, 0, 0,
                statusId, 0, sDate, eDate.AddDays(1), "", sla, onGoing,
             "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser,AssignedToUser,Branch,Department");

            TempData["TeamId"] = teamId;

            ViewBag.TicketTypeId = ticketTypeId;
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryId = categoryId;
            ViewBag.StatusId = statusId;
            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");
            return View(items);

        }


        [HttpGet]
        [AccessAuthorize]
        public ActionResult SLATickets(long? companyId, long? categoryId, long? statusId)
        {
            if (companyId == null)
                companyId = 0;
            if (categoryId == null)
                categoryId = 0;
            if (statusId == null)
                statusId = 0;
            var items = _service.GetItemsByAssignedSLA(categoryId, companyId, statusId, Session["UserId"].ToString(), "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority").OrderByDescending(o => o.RequestedDate).ToList();

            ViewBag.CompanyId = companyId;
            ViewBag.CategoryId = categoryId;
            ViewBag.StatusId = statusId;

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }


        [AccessAuthorize]
        [HttpGet]
        public ActionResult UpdateSLAReason(long? id, string comment)
        {
            int errorCode = 0;
            var ticket = _service.GetItem(id, "");
            ticket.ReasonForSLA = comment;
            ticket.UpdatedDate = UserDateTime.GetUserDate();
            ticket.UpdatedBy = Session["UserId"].ToString();

            var entitySaved = _service.Update(ticket);
            if (entitySaved)
                errorCode = 1;


            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult PendingReview()
        {

            //var items = _service.GetItemsPendingReview("RequestType,RequestType.Category,RequestType.AssetType" +
            //    ",RequestType.SubCategory,RequestType.Team,TicketPriority,RequestedUser,Branch,PendingTeam,Department").OrderByDescending(o => o.RequestedDate).ToList();
            var items = _service.GetItemsPendingReviewSLA(Session["UserId"].ToString(), "RequestType,RequestType.Category,RequestType.AssetType" +
            ",RequestType.SubCategory,RequestType.Team,TicketPriority,RequestedUser,Branch,PendingTeam,Department").OrderByDescending(o => o.RequestedDate).ToList();
            ItemDocService _itemDocService = new ItemDocService();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }


        [AccessAuthorize]
        [HttpGet]
        public ActionResult Review(long? ticketId, string comment)
        {
            int errorCode = 0;

            var item = _service.GetItem(ticketId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority");


            if (item != null)
            {

                item.UpdatedBy = Session["UserId"].ToString();
                item.UpdatedDate = UserDateTime.GetUserDate();
                //item.IsReviewed = true;
                item.ReasonForSLA = comment;
                var entitySaved = _service.Update(item);
                if (entitySaved)
                {
                    TeamService _teamService = new TeamService();

                    //var ticketStatus = item;
                    //ticketStatus.SpentTime = 0;
                    //ticketStatus.PendingTeamId = _teamService.GetTeamIdDefault();
                    //_service.InsertTicketStatus(ticketStatus, comment);

                    _service.InsertTicketLog(item, "Ticket SLA reviewed.");

                    errorCode = 1;

                    TempData["SuccessMessage"] = "Ticket has been successfully reviewed.";
                }
                else
                {
                    errorCode = 2;
                    TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                }


            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetStatusJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();

            Array values = Enum.GetValues(typeof(TicketStatusEnum));

            foreach (TicketStatusEnum val in values)
            {

                Console.WriteLine(String.Format("{0}: {1}", Enum.GetName(typeof(TicketStatusEnum), val), val));

                EnumStatusVM item = new EnumStatusVM();
                //item.Id = val;
                item.Id = Convert.ToInt32(val);
                //item.Name = Enum.GetName(typeof(TicketStatusEnum), val);
                item.Name = val.EnumDisplayName();
                items.Add(item);
            }

            //var items = _service.GetItemsByManager(userId, "");
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetUpdateStatusJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();

            Array values = Enum.GetValues(typeof(TicketStatusEnum));

            foreach (TicketStatusEnum val in values)
            {
                if (val > TicketStatusEnum.Waiting && val < TicketStatusEnum.Completed)
                {
                    Console.WriteLine(String.Format("{0}: {1}", Enum.GetName(typeof(TicketStatusEnum), val), val));

                    EnumStatusVM item = new EnumStatusVM();
                    //item.Id = val;
                    item.Id = Convert.ToInt32(val);
                    //item.Name = Enum.GetName(typeof(TicketStatusEnum), val);
                    item.Name = val.EnumDisplayName();
                    items.Add(item);
                }
            }

            //var items = _service.GetItemsByManager(userId, "");
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetIconJsonResult()
        {

            var items = GlobalStaticService.GetIcons().OrderBy(o => o.Text);
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetColorJsonResult()
        {

            var items = GlobalStaticService.GetColors();
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AccessLogin]
        public JsonResult GetTempDocListJsonResult()
        {
            TicketDocService _ticketDocService = new TicketDocService();
            var items = _ticketDocService.GetTempDocs(Session["UserId"].ToString(), "").ToList();

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [AccessLogin]
        [HttpGet]
        public ActionResult DeleteDoc(long? id)
        {
            int errorCode = 0;
            TicketDocService _docService = new TicketDocService();

            var item = _docService.GetItem(id, "");
            item.UpdatedDate = UserDateTime.GetUserDate();

            long ticketId = item.TicketId;
            string docName = item.DocumentName;
            string fullPath = Server.MapPath("~/" + GlobalStaticService.GetFileUploadPath()) + item.FileName;

            if (_docService.Delete(item))
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                errorCode = 1;
                TicketService ticketService = new TicketService();
                var ticket = ticketService.GetItemAsNoTracking(ticketId, "");
                ticketService.InsertTicketLog(ticket, "Document (" + docName + ") was deleted");

            }

            //}

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [AccessLogin]
        [HttpGet]
        public ActionResult DeleteTempDoc(long? id)
        {
            int errorCode = 0;
            TicketDocService _ticketDocService = new TicketDocService();
            string fullPath = Server.MapPath("~/" + GlobalStaticService.GetFileUploadPath());
            if (_ticketDocService.DeleteTempDoc(id, fullPath))
            {
                errorCode = 1;
            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetAllocatedTypesJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();
            Array values = Enum.GetValues(typeof(AllocatedTypeEnum));

            foreach (AllocatedTypeEnum val in values)
            {

                EnumStatusVM item = new EnumStatusVM();
                item.Id = Convert.ToInt32(val);
                item.Name = GlobalStaticService.EnumDisplayName(val);
                items.Add(item);
            }

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AccessLogin]
        public JsonResult GetTicketByCode(string code)
        {
            TicketMinVM res = new TicketMinVM();
            try
            {
                if (!string.IsNullOrEmpty(code))
                {

                    var ticket = _service.GetItemByTicketCode(code, "RequestType.Category,RequestType.SubCategory,RequestType.AssetType");
                    if (ticket != null)
                    {
                        res.Code = ticket.Code;
                        res.TicketId = ticket.TicketId;
                        res.CategoryName = ticket.RequestType.Category.CategoryName;
                        res.SubCategoryName = ticket.RequestType.SubCategory.SubCategoryName;
                        res.AssetTypeName = ticket.RequestType.AssetType.AssetTypeName;

                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(), ex);

            }
            return Json(JsonConvert.SerializeObject(res), JsonRequestBehavior.AllowGet);

        }

        [AccessLogin]
        public JsonResult GetUserPendtingTicket(string lanId)
        {
            // Example: Fetch an integer value based on the lanId
            int pendingTickets = _service.GetPendingTicketCountByUserId(lanId); // Assume this method gets the count

            return Json(pendingTickets, JsonRequestBehavior.AllowGet);
        }

    }

}
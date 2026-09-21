using Domain;
using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers
{
    public class ChangeController : Controller
    {
        private readonly ChangeService _service = new ChangeService();
        // GET: Change

        [AccessAuthorize]
        public ActionResult Index(long? assetCategoryId, long? assetTypeId, long? changeRequestTypeId, long? statusId, string startDate, string endDate)
        {
            DateTime sDate = UserDateTime.GetUserDateOnly();
            DateTime eDate = UserDateTime.GetUserDateOnly();
            if (startDate != null && startDate != "" && endDate != null && endDate != "")
            {
                try
                {
                    sDate = Convert.ToDateTime(startDate);
                    eDate = Convert.ToDateTime(endDate);
                }
                catch (Exception ex) { }
            }
            if (assetCategoryId == null)
                assetCategoryId = 0;
            if (assetTypeId == null)
                assetTypeId = 0;
            if (changeRequestTypeId == null)
                changeRequestTypeId = 0;
            if (statusId == null)
                statusId = 0;

            var items = _service.GetItems(assetCategoryId, assetTypeId, changeRequestTypeId, statusId, sDate, eDate.AddDays(1), "AssetType,ChangeRequestType");

            ViewBag.AssetTypeId = assetTypeId;
            ViewBag.AssetCategoryId = assetCategoryId;
            ViewBag.ChangeRequestTypeId = changeRequestTypeId;
            ViewBag.StatusId = statusId;
            ViewBag.StartDate = sDate.ToString("MM/dd/yyyy");
            ViewBag.EndDate = eDate.ToString("MM/dd/yyyy");
            return View(items);

        }



        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {
            var item = new ChangeVM();
            item.StartDate = UserDateTime.GetUserDate();
            item.EndDate = UserDateTime.GetUserDate();
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(ChangeVM item, string changeRequestTypeId, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {

                //ViewBag.CategoryId = item.CategoryId;
                //ViewBag.SubCategoryId = item.SubCategoryId;
                ViewBag.AssetTypeId = item.AssetTypeId;
                ViewBag.ChangeRequestTypeId = item.ChangeRequestTypeId;

                if (item.AssetTypeId == 0 || item.ChangeRequestTypeId == 0
                    || item.Subject == null || item.ChangeType == 0 || item.Priority == 0 || item.Risk == 0
                    || item.StartDate == null || item.EndDate == null)
                {
                    TempData["ErrorMessage"] = "Please enter required data. ";
                    return View("Create", item);
                }
                DateTime sDate = UserDateTime.GetUserDate();
                DateTime eDate = UserDateTime.GetUserDate();
                try
                {
                    sDate = Convert.ToDateTime(item.StartDate);
                    eDate = Convert.ToDateTime(item.EndDate);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Please enter valid start and end dates ";
                    return View("Create", item);
                }

                Change newItem = new Change();
                newItem.AssetTypeId = item.AssetTypeId;
                newItem.ChangeRequestTypeId = item.ChangeRequestTypeId;
                newItem.Subject = item.Subject;
                newItem.Description = item.Description;
                newItem.Priority = item.Priority;
                newItem.ChangeType = item.ChangeType;
                newItem.Risk = item.Risk;
                newItem.StartDate = sDate;
                newItem.EndDate = eDate;
                newItem.RequestedBy = Session["UserId"].ToString();
                newItem.UpdatedBy = Session["UserId"].ToString();
                newItem.UpdatedDate = UserDateTime.GetUserDate();
                newItem.RequestedDate = UserDateTime.GetUserDate();

                //RequestTypeService _requestTypeService = new RequestTypeService();
                //RequestType requestType = new RequestType();
                //requestType = _requestTypeService.GetItem(TicketTypeEnum.IN, item.CategoryId, item.AssetTypeId, item.SubCategoryId, "");
                //newItem.RequestTypeId = requestType.RequestTypeId;

                newItem.Status = ChangeStatusEnum.Pending;

                var entitySaved = _service.Insert(newItem);
                if (entitySaved)
                {

                    #region Email

                    var change = _service.GetItem(newItem.ChangeId, "AssetType,ChangeRequestType");
                    string userName = Session["UserName"].ToString();
                    String emailBody = "";
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();
                    ToRecipients.Add("itsm.changes@laugfs.lk");
                    //CCRecipients.Add("kalhari.gamage@laugfs.lk");
                    //CCRecipients.Add("viraga.mahesh@laugfs.lk");
                    emailBody = emailTemplate.ChangeCreate(change, userName);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "ITSM - New Change", emailBody).Trim();

                    #endregion

                    TempData["SuccessMessage"] = "Change has been successfully created.";
                    return RedirectToAction("AssignItems");

                }
                else
                {
                    TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                    return View("Create", item);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Sorry, something went wrong. Record couldn't save, Please contact the IT support.";
                return View("Create", item);
            }

        }


        [HttpGet]
        [AccessLogin]
        [EncryptedActionParameter]
        public ActionResult Info(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = _service.GetItem(id, "AssetType,ChangeRequestType,RequestedUser");
            ItemLogService _logService = new ItemLogService();
            item.ItemLogs = _logService.GetChangeLogs(id, "UpdatedUser").ToList();

            ItemDocService _itemDocService = new ItemDocService();
            item.ItemsDocs = _itemDocService.GetChangeDocs(item.ChangeId, "").ToList();

            //ItemAssetService _itemAssetService = new ItemAssetService();
            //item.ItemAssets = _itemAssetService.GetAssets(ItemTypeEnum.Change, item.ChangeId, "Asset").ToList();

            ChangeTicketService _changeTickerService = new ChangeTicketService();
            var pTickets = _changeTickerService.GetChangeTicketByChangeId(item.ChangeId, "Ticket");
            Collection<Ticket> assignTickets = new Collection<Ticket>();
            foreach (ChangeTicket pt in pTickets)
            {
                assignTickets.Add(pt.Ticket);
            }
            item.Tickets = assignTickets;

            ChangeProblemService _changeProblemService = new ChangeProblemService();
            var problems = _changeProblemService.GetChangeProblemByChangeId(item.ChangeId, "Problem");
            Collection<Problem> assignProblems = new Collection<Problem>();
            foreach (ChangeProblem p in problems)
            {
                assignProblems.Add(p.Problem);
            }
            item.Problems = assignProblems;

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult AssignItems()
        {

            var items = _service.GetItemsForAssign(Session["UserId"].ToString(),
                "AssetType,ChangeRequestType").OrderByDescending(o => o.UpdatedBy).ToList();

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
            ItemDocService _itemDocService = new ItemDocService();
            //item.Change = _service.GetItem(id, "AssetType,ChangeRequestType");
            //item.ItemDocs = _itemDocService.GetChangeDocs(item.Change.ChangeId, "").ToList();

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }

        //[AccessLogin]
        //[HttpPost]
        //public async Task<ActionResult> DocUpload(HttpPostedFileBase itemFile, DocUploadVM item)
        //{
        //    FileUploadService _fileUploadService = new FileUploadService();
        //    ItemDocService _itemDocService = new ItemDocService();
        //   // item.Change = _service.GetItem(item.Change.ChangeId, "AssetType,ChangeRequestType");
        //    //item.ItemDocs = _itemDocService.GetChangeDocs(item.Change.ChangeId, "").ToList();

        //    if (item.FileName == null || item.FileName == "")
        //    {
        //        TempData["ErrorMessage"] = "Please enter file name. ";
        //        return View(item);

        //    }
        //    try
        //    {

        //        if (itemFile != null && itemFile.ContentLength != 0)
        //        {
        //            HttpPostedFileBase _file = itemFile;
        //            //var fileUrl = await _fileUploadService.UploadImageAsync(_file);
        //            ItemDoc doc = new ItemDoc();
        //            doc.FileName = item.FileName;
        //           // doc.FileUrl = fileUrl;
        //            //doc.ItemId = item.Change.ChangeId;
        //            doc.ItemType = ItemTypeEnum.Change;
        //            doc.UpdatedDate = UserDateTime.GetUserDate();
        //            doc.UpdatedBy = Session["UserId"].ToString();
        //            _itemDocService.Insert(doc);

        //            item.ItemDocs.Add(doc);

        //            #region Log
        //            ItemLogService _logService = new ItemLogService();
        //            ItemLog log = new ItemLog();
        //            log.ItemType = ItemTypeEnum.Change;
        //            //log.ItemId = item.Change.ChangeId;
        //            log.Comment = "Document (" + doc.FileName + ") has been Uploaded";
        //            log.UpdatedBy = doc.UpdatedBy;
        //            log.UpdatedDate = doc.UpdatedDate;
        //            _logService.Insert(log);
        //            #endregion
        //            TempData["SuccessMessage"] = "File have been successfully uploaded.";
        //            return RedirectToAction("AssignItems");

        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Please upload a document. ";
        //            return View(item);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Sorry, something went wrong, Record couldn't update, Please contact the IT support. (" + ex.Message.ToString() + ") ";
        //        return View(item);
        //    }


        //}


        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult AssignAsset(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = new AssignAssetVM();
            item.ItemId = Convert.ToInt64(id);
            //item.Ticket = _service.GetItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,TicketPriority,TicketUpdates");

            ItemAssetService _itemAssetService = new ItemAssetService();
            //item.AssignedItems = _itemAssetService.GetAssets(ItemTypeEnum.Change, id, "Asset").ToList();
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }


        //[HttpPost]
        //[AccessAuthorize]
        //public ActionResult AssignAsset(AssignAssetVM item)
        //{
        //    ItemAssetService _itemAssetService = new ItemAssetService();

        //    item.AssignedItems = _itemAssetService.GetAssets(ItemTypeEnum.Change, item.ItemId, "").ToList();

        //    if (item.AssetCode == null || item.AssetCode == "")
        //    {
        //        TempData["ErrorMessage"] = "Please enter the asset code. ";
        //        return View(item);
        //    }
        //    AssetItemDTO _item = new AssetItemDTO();

        //    if (item.AssetCategory == AssetCategoryEnum.ITAsset || item.AssetCategory == AssetCategoryEnum.DataCenter)
        //    {
        //        IMSItemService _imsService = new IMSItemService();
        //        _item = _imsService.GetItem(item.AssetCode);
        //    }
        //    else if (item.AssetCategory == AssetCategoryEnum.Telco)
        //    {
        //        TelcoItemService _telcoService = new TelcoItemService();
        //        _item = _telcoService.GetItem(item.AssetCode);
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Invalid Asset Category.";
        //        return View(item);
        //    }

        //    if (_item.IMSCode == null)
        //    {
        //        TempData["ErrorMessage"] = "Invalid Asset Code.";
        //        return View(item);
        //    }
        //    ItemAsset newItem = new ItemAsset();
        //    newItem.AssetCode = _item.IMSCode;
        //    newItem.AssetName = _item.ItemName;
        //    newItem.Description = _item.Description;
        //    newItem.ItemId = item.ItemId;
        //    newItem.ItemType = ItemTypeEnum.Change;
        //    newItem.ModelName = _item.ModelNo;
        //    newItem.SerialNo = _item.SerialNo;
        //    newItem.UpdatedDate = UserDateTime.GetUserDate();
        //    newItem.UpdatedBy = Session["UserId"].ToString();


        //    if (!_itemAssetService.IsAvailable(newItem))
        //    {
        //        var entitySaved = _itemAssetService.Insert(newItem);
        //        if (entitySaved)
        //        {
        //            ItemLogService _logService = new ItemLogService();
        //            ItemLog log = new ItemLog();
        //            log.ItemType = ItemTypeEnum.Change;
        //            log.ItemId = newItem.ItemId;
        //            log.Comment = "Asset " + newItem.AssetName + "(" + newItem.AssetCode + ")" + " has been assigned";
        //            log.UpdatedBy = newItem.UpdatedBy;
        //            log.UpdatedDate = newItem.UpdatedDate;
        //            _logService.Insert(log);

        //            item.AssignedItems = _itemAssetService.GetAssets(ItemTypeEnum.Change, item.ItemId, "").ToList();

        //            TempData["SuccessMessage"] = "Asset has been successfully assigned.";

        //        }
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Asset already exists.";

        //    }

        //    return View(item);

        //}

        [AccessAuthorize]
        [HttpGet]
        public ActionResult DeleteAsset(long? assetItemId, long? changeId)
        {
            int errorCode = 0;
            ItemAssetService _itemAssetService = new ItemAssetService();
            var item = _itemAssetService.GetItem(assetItemId, "Asset");
            item.ItemId = Convert.ToInt64(changeId);
            item.UpdatedDate = UserDateTime.GetUserDate();
            item.UpdatedBy = Session["UserId"].ToString();

            //var problem = _service.GetItem(item.ItemId, "");
            if (item != null)
            {
                if (_itemAssetService.Delete(item))
                {
                    ItemLogService _logService = new ItemLogService();
                    ItemLog log = new ItemLog();
                    log.ItemType = ItemTypeEnum.Change;
                    log.ItemId = item.ItemId;
                    log.Comment = "Asset " + item.Asset.AssetName + " has been deleted";
                    log.UpdatedBy = item.UpdatedBy;
                    log.UpdatedDate = item.UpdatedDate;
                    _logService.Insert(log);

                    errorCode = 1;
                }

            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult AddTicket(long? id, string startDate, string endDate)
        {

            var item = new AddTicketVM();
            if (id == null)
                return HttpNotFound();

            DateTime sDate = UserDateTime.GetUserDateOnly();
            DateTime eDate = UserDateTime.GetUserDateOnly();
            if (startDate != null && startDate != "" && endDate != null && endDate != "")
            {
                try
                {
                    sDate = Convert.ToDateTime(startDate);
                    eDate = Convert.ToDateTime(endDate);
                }
                catch (Exception ex) { }
            }

            item.Change = _service.GetItem(id, "");
            item.ChangeId = item.Change.ChangeId;

            TicketService _ticketService = new TicketService();
            item.Tickets = _ticketService.GetTikcetsByAssetType(item.Change.AssetTypeId, sDate, eDate.AddDays(1), "RequestedUser").ToList();


            ChangeTicketService _changeTickerService = new ChangeTicketService();
            var pTickets = _changeTickerService.GetChangeTicketByChangeId(item.ChangeId, "Ticket,Ticket.RequestedUser");
            Collection<Ticket> assignTickets = new Collection<Ticket>();
            foreach (ChangeTicket pt in pTickets)
            {
                assignTickets.Add(pt.Ticket);
            }

            item.AssignedTickets = assignTickets;

            ViewBag.StartDate = sDate.ToString("MM/dd/yyyy");
            ViewBag.EndDate = eDate.ToString("MM/dd/yyyy");

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }


        [HttpPost]
        [AccessAuthorize]
        public ActionResult AddTicket(AddTicketVM item, string startDate, string endDate)
        {

            DateTime sDate = UserDateTime.GetUserDateOnly();
            DateTime eDate = UserDateTime.GetUserDateOnly();
            if (startDate != null && startDate != "" && endDate != null && endDate != "")
            {
                try
                {
                    sDate = Convert.ToDateTime(startDate);
                    eDate = Convert.ToDateTime(endDate);
                }
                catch (Exception ex) { }
            }
            item.Change = _service.GetItem(item.ChangeId, "");

            TicketService _ticketService = new TicketService();
            item.Tickets = _ticketService.GetTikcetsByAssetType(item.Change.AssetTypeId, sDate, eDate.AddDays(1), "RequestedUser").ToList();

            ChangeTicketService _changeTickerService = new ChangeTicketService();
            var pTickets = _changeTickerService.GetChangeTicketByChangeId(item.ChangeId, "Ticket,Ticket.RequestedUser");
            Collection<Ticket> assignTickets = new Collection<Ticket>();
            foreach (ChangeTicket pt in pTickets)
            {
                assignTickets.Add(pt.Ticket);
            }

            item.AssignedTickets = assignTickets;

            ViewBag.StartDate = sDate.ToString("MM/dd/yyyy");
            ViewBag.EndDate = eDate.ToString("MM/dd/yyyy");

            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult DeleteTicket(long? ticketId, long? changeId)
        {
            int errorCode = 0;
            ChangeTicketService _changeTicketService = new ChangeTicketService();
            var item = _changeTicketService.GetItem(changeId, ticketId, "");
            item.UpdatedDate = UserDateTime.GetUserDate();
            item.UpdatedBy = Session["UserId"].ToString();
            if (item != null)
            {
                if (_changeTicketService.HasRelationalData(item))
                    errorCode = 2;
                else if (_changeTicketService.Delete(item))
                {
                    ItemLogService _logService = new ItemLogService();
                    ItemLog log = new ItemLog();
                    log.ItemType = ItemTypeEnum.Change;
                    log.ItemId = item.ChangeId;
                    log.Comment = "Ticket " + item.TicketId + " has been deleted";
                    log.UpdatedBy = item.UpdatedBy;
                    log.UpdatedDate = item.UpdatedDate;
                    _logService.Insert(log);

                    errorCode = 1;
                }

            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult AssignTicket(long? ticketId, long? changeId)
        {
            int errorCode = 0;
            ChangeTicketService _changeTicketService = new ChangeTicketService();
            var item = new ChangeTicket();
            if (ticketId != null && changeId != null)
            {
                item.ChangeId = Convert.ToInt64(changeId);
                item.TicketId = Convert.ToInt64(ticketId);
                item.UpdatedDate = UserDateTime.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();

                if (!_changeTicketService.IsAvailable(item))
                {
                    var entitySaved = _changeTicketService.Insert(item);
                    if (entitySaved)
                    {
                        ItemLogService _logService = new ItemLogService();
                        ItemLog log = new ItemLog();
                        log.ItemType = ItemTypeEnum.Change;
                        log.ItemId = item.ChangeId;
                        log.Comment = "Ticket " + item.TicketId + " has been assigned";
                        log.UpdatedBy = item.UpdatedBy;
                        log.UpdatedDate = item.UpdatedDate;
                        _logService.Insert(log);

                        errorCode = 1;

                    }
                }
                else
                    errorCode = 2;


            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult AddProblem(long? id, string startDate, string endDate)
        {

            var item = new AddProblemVM();
            if (id == null)
                return HttpNotFound();

            DateTime sDate = UserDateTime.GetUserDateOnly();
            DateTime eDate = UserDateTime.GetUserDateOnly();
            if (startDate != null && startDate != "" && endDate != null && endDate != "")
            {
                try
                {
                    sDate = Convert.ToDateTime(startDate);
                    eDate = Convert.ToDateTime(endDate);
                }
                catch (Exception ex) { }
            }

            item.Change = _service.GetItem(id, "");
            item.ChangeId = item.Change.ChangeId;

            ProblemService _problemService = new ProblemService();
            item.Problems = _problemService.GetItemsByDR(ProblemStatusEnum.Closed, sDate, eDate.AddDays(1), "RequestedUser").ToList();

            ChangeProblemService _changeProblemService = new ChangeProblemService();
            var cProblems = _changeProblemService.GetChangeProblemByChangeId(item.ChangeId, "Problem,Problem.RequestedUser");
            Collection<Problem> assignProblems = new Collection<Problem>();
            foreach (ChangeProblem p in cProblems)
            {
                assignProblems.Add(p.Problem);
            }

            item.AssignedProblems = assignProblems;

            ViewBag.StartDate = sDate.ToString("MM/dd/yyyy");
            ViewBag.EndDate = eDate.ToString("MM/dd/yyyy");

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }


        [HttpPost]
        [AccessAuthorize]
        public ActionResult AddProblem(AddProblemVM item, string startDate, string endDate)
        {

            DateTime sDate = UserDateTime.GetUserDateOnly();
            DateTime eDate = UserDateTime.GetUserDateOnly();
            if (startDate != null && startDate != "" && endDate != null && endDate != "")
            {
                try
                {
                    sDate = Convert.ToDateTime(startDate);
                    eDate = Convert.ToDateTime(endDate);
                }
                catch (Exception ex) { }
            }
            item.Change = _service.GetItem(item.ChangeId, "");

            ProblemService _problemService = new ProblemService();
            item.Problems = _problemService.GetItemsByDR(ProblemStatusEnum.Closed, sDate, eDate.AddDays(1), "RequestedUser").ToList();

            ChangeProblemService _changeProblemService = new ChangeProblemService();
            var cProblems = _changeProblemService.GetChangeProblemByChangeId(item.ChangeId, "Problem,Problem.RequestedUser");
            Collection<Problem> assignProblems = new Collection<Problem>();
            foreach (ChangeProblem p in cProblems)
            {
                assignProblems.Add(p.Problem);
            }

            item.AssignedProblems = assignProblems;

            ViewBag.StartDate = sDate.ToString("MM/dd/yyyy");
            ViewBag.EndDate = eDate.ToString("MM/dd/yyyy");

            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult DeleteProblem(long? problemId, long? changeId)
        {
            int errorCode = 0;
            ChangeProblemService _changeProblemService = new ChangeProblemService();
            var item = _changeProblemService.GetItem(changeId, problemId, "");
            item.UpdatedDate = UserDateTime.GetUserDate();
            item.UpdatedBy = Session["UserId"].ToString();
            if (item != null)
            {
                if (_changeProblemService.HasRelationalData(item))
                    errorCode = 2;
                else if (_changeProblemService.Delete(item))
                {
                    ItemLogService _logService = new ItemLogService();
                    ItemLog log = new ItemLog();
                    log.ItemType = ItemTypeEnum.Change;
                    log.ItemId = item.ChangeId;
                    log.Comment = "Problem " + item.ProblemId + " has been deleted";
                    log.UpdatedBy = item.UpdatedBy;
                    log.UpdatedDate = item.UpdatedDate;
                    _logService.Insert(log);

                    errorCode = 1;
                }

            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult AssignProblem(long? problemId, long? changeId)
        {
            int errorCode = 0;
            ChangeProblemService _changeProblemService = new ChangeProblemService();
            var item = new ChangeProblem();
            if (problemId != null && changeId != null)
            {
                item.ChangeId = Convert.ToInt64(changeId);
                item.ProblemId = Convert.ToInt64(problemId);
                item.UpdatedDate = UserDateTime.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();

                if (!_changeProblemService.IsAvailable(item))
                {
                    var entitySaved = _changeProblemService.Insert(item);
                    if (entitySaved)
                    {
                        ItemLogService _logService = new ItemLogService();
                        ItemLog log = new ItemLog();
                        log.ItemType = ItemTypeEnum.Change;
                        log.ItemId = item.ChangeId;
                        log.Comment = "Problem " + item.ProblemId + " has been assigned";
                        log.UpdatedBy = item.UpdatedBy;
                        log.UpdatedDate = item.UpdatedDate;
                        _logService.Insert(log);

                        errorCode = 1;

                    }
                }
                else
                    errorCode = 2;


            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Plan()
        {

            var items = _service.GetAllByStatus(ChangeStatusEnum.Pending, "AssetType,ChangeRequestType").OrderByDescending(o => o.UpdatedBy).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult Edit(long? id)
        {
            if (id == null)
                return HttpNotFound();

            var item = _service.GetItem(id, "AssetType,ChangeRequestType");
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Change item)
        {
            if (ModelState.IsValid)
            {
                if (item.Subject == null || item.ChangeType == 0 || item.Priority == 0 || item.Risk == 0
                  || item.StartDate == null || item.EndDate == null)
                {
                    TempData["ErrorMessage"] = "Please enter required data. ";
                    return View("Edit", item);
                }
                DateTime sDate = UserDateTime.GetUserDate();
                DateTime eDate = UserDateTime.GetUserDate();
                try
                {
                    sDate = Convert.ToDateTime(item.StartDate);
                    eDate = Convert.ToDateTime(item.EndDate);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Please enter valid start and end dates ";
                    return View("Create", item);
                }


                item.UpdatedDate = UserDateTime.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();

                Change updateItem = _service.GetItem(item.ChangeId, "");
                updateItem.Subject = item.Subject;
                updateItem.Description = item.Description;
                updateItem.ChangeType = item.ChangeType;
                updateItem.Priority = item.Priority;
                updateItem.Risk = item.Risk;
                updateItem.StartDate = item.StartDate;
                updateItem.EndDate = item.EndDate;

                updateItem.UpdatedDate = UserDateTime.GetUserDate();
                updateItem.UpdatedBy = Session["UserId"].ToString();

                var entityUpdated = _service.Update(updateItem);

                if (entityUpdated)
                {
                    TempData["SuccessMessage"] = "Data updated successfully";
                    return RedirectToAction("Plan");
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
                if (_service.HasRelationalData(item))
                {
                    errorCode = 2;
                    TempData["ErrorMessage"] = "Sorry, something went wrong, Please contact the IT support.";

                }
                else if (_service.Delete(item))
                {
                    errorCode = 1;
                    TempData["SuccessMessage"] = "Change request has been deleted successfully";
                }
            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        [EncryptedActionParameter]
        [AccessAuthorize]
        public ActionResult UpdatePlan(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = _service.GetItem(id, "AssetType,ChangeRequestType");
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }
        [HttpPost]
        [AccessAuthorize]
        public ActionResult UpdatePlan(Change item)
        {
            //if (ModelState.IsValid)
            //{
            var change = _service.GetItemAsNoTracking(item.ChangeId, "AssetType,ChangeRequestType");
            change.Reason = item.Reason;
            change.RollOutPlan = item.RollOutPlan;
            change.BackOutPlan = item.BackOutPlan;
            change.Impact = item.Impact;
            item = change;
            if (item.Reason == null || item.RollOutPlan == null || item.BackOutPlan == null || item.Impact == null)
            {

                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }


            change.UpdatedBy = Session["UserId"].ToString();
            change.UpdatedDate = UserDateTime.GetUserDate();
            change.Status = ChangeStatusEnum.Planned;

            var entitySaved = _service.Update(change);
            if (entitySaved)
            {
                ItemLogService _logService = new ItemLogService();
                ItemLog log = new ItemLog();
                log.ItemType = ItemTypeEnum.Change;
                log.ItemId = change.ChangeId;
                log.Comment = change.Status.EnumDisplayName();
                log.Status = change.Status.ToString();
                log.UpdatedBy = change.UpdatedBy;
                log.UpdatedDate = change.UpdatedDate;
                _logService.Insert(log);

                #region Email
                string userName = Session["UserName"].ToString();
                String emailBody = "";
                EmailTemplate emailTemplate = new EmailTemplate();
                EmailClient emailClient = new EmailClient();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                ToRecipients.Add("itsm.changes@laugfs.lk");
               // CCRecipients.Add("kalhari.gamage@laugfs.lk");
                //CCRecipients.Add("viraga.mahesh@laugfs.lk");
                emailBody = emailTemplate.ChangePlan(change, userName);
                String res = emailClient.SendEmail(ToRecipients, CCRecipients, "ITSM - Change", emailBody).Trim();

                #endregion

                TempData["SuccessMessage"] = "Change has been successfully updated.";
                return RedirectToAction("Plan");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View(item);
            }
            //}
            //else
            //{
            //    TempData["ErrorMessage"] = "Sorry, something went wrong. Record couldn't save, Please contact the IT support.";
            //    return View(item);
            //}
        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Review()
        {

            var items = _service.GetAllByStatus(ChangeStatusEnum.Planned,
                "AssetType,ChangeRequestType").OrderByDescending(o => o.UpdatedBy).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }


        [HttpGet]
        [EncryptedActionParameter]
        [AccessAuthorize]
        public ActionResult UpdateReview(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = _service.GetItem(id, "AssetType,ChangeRequestType");
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }
        [HttpPost]
        [AccessAuthorize]
        public ActionResult UpdateReview(Change item, string textComment, string checkboxApprove)
        {

            bool approve = false;
            if (checkboxApprove == "on")
                approve = true;

            var change = _service.GetItemAsNoTracking(item.ChangeId, "AssetType,ChangeRequestType");
            item = change;
            if (textComment == null || textComment == "")
            {
                TempData["ErrorMessage"] = "Please enter a comment";
                return View(item);
            }

            if (change.Status == ChangeStatusEnum.Planned)
            {
                if (approve)
                    change.Status = ChangeStatusEnum.Reviewed;
                else
                    change.Status = ChangeStatusEnum.Rejected;

                change.UpdatedDate = UserDateTime.GetUserDate();
                change.UpdatedBy = Session["UserId"].ToString();
                var entitySaved = _service.Update(item);
                if (entitySaved)
                {
                    ItemLogService _logService = new ItemLogService();
                    ItemLog log = new ItemLog();
                    log.ItemType = ItemTypeEnum.Change;
                    log.ItemId = change.ChangeId;
                    log.Comment = change.Status.EnumDisplayName() + ": " + textComment;
                    log.Status = change.Status.ToString();
                    log.UpdatedBy = change.UpdatedBy;
                    log.UpdatedDate = change.UpdatedDate;
                    _logService.Insert(log);

                    #region Email
                    string userName = Session["UserName"].ToString();
                    String emailBody = "";
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();
                    ToRecipients.Add("itsm.approval@laugfs.lk");
                    CCRecipients.Add("itsm.changes@laugfs.lk");
                    //CCRecipients.Add("kalhari.gamage@laugfs.lk");
                    //CCRecipients.Add("viraga.mahesh@laugfs.lk");

                    string emailUrl = "/Login/LoginAuth?auth=" + UrlAuthentication.CreateAuthUrlChangeApprove(change.ChangeId.ToString() , UserDateTime.GetUserDate());

                    emailBody = emailTemplate.ChangeReview(change, userName,emailUrl);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "ITSM - Change", emailBody).Trim();

                    #endregion

                    TempData["SuccessMessage"] = "Change has been successfully updated.";
                    return RedirectToAction("Review");
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry, something went wrong. Record couldn't save, Please contact the IT support.";
                    return View(item);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Sorry, something went wrong. Record couldn't save, Please contact the IT support.";
                return View(item);


            }




        }


        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult ReviewApprove(long? id, string comment)
        //{
        //    int errorCode = 0;
        //    if (comment == null || comment == "")
        //        errorCode = 2;
        //    else
        //    {
        //        var item = _service.GetItem(id, "");
        //        if (item != null)
        //        {
        //            if (item.Status == ChangeStatusEnum.Planned)
        //            {
        //                item.Status = ChangeStatusEnum.Reviewed;
        //                item.UpdatedDate = UserDateTime.GetUserDate();
        //                item.UpdatedBy = Session["UserId"].ToString();
        //                var entitySaved = _service.Update(item);
        //                if (entitySaved)
        //                {
        //                    ItemLogService _logService = new ItemLogService();
        //                    ItemLog log = new ItemLog();
        //                    log.ItemType = ItemTypeEnum.Change;
        //                    log.ItemId = item.ChangeId;
        //                    log.Comment = item.Status.EnumDisplayName() + ": " + comment;
        //                    log.Status = item.Status.ToString();
        //                    log.UpdatedBy = item.UpdatedBy;
        //                    log.UpdatedDate = item.UpdatedDate;
        //                    _logService.Insert(log);


        //                    errorCode = 1;
        //                }
        //            }
        //        }
        //    }
        //    return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        //}

        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult ReviewReject(long? id, string comment)
        //{
        //    int errorCode = 0;
        //    if (comment == null || comment == "")
        //        errorCode = 2;
        //    else
        //    {
        //        var item = _service.GetItem(id, "");
        //        if (item != null)
        //        {
        //            if (item.Status == ChangeStatusEnum.Planned)
        //            {
        //                item.Status = ChangeStatusEnum.Rejected;
        //                item.UpdatedDate = UserDateTime.GetUserDate();
        //                item.UpdatedBy = Session["UserId"].ToString();
        //                var entitySaved = _service.Update(item);
        //                if (entitySaved)
        //                {
        //                    ItemLogService _logService = new ItemLogService();
        //                    ItemLog log = new ItemLog();
        //                    log.ItemType = ItemTypeEnum.Change;
        //                    log.ItemId = item.ChangeId;
        //                    log.Comment = item.Status.EnumDisplayName() + ": " + comment;
        //                    log.Status = item.Status.ToString();
        //                    log.UpdatedBy = item.UpdatedBy;
        //                    log.UpdatedDate = item.UpdatedDate;
        //                    _logService.Insert(log);


        //                    errorCode = 1;
        //                }
        //            }
        //        }
        //    }
        //    return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        //}

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Approve()
        {

            var items = _service.GetAllByStatus(ChangeStatusEnum.Reviewed,
                "AssetType,ChangeRequestType").OrderByDescending(o => o.UpdatedBy).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }


        [HttpGet]
        [EncryptedActionParameter]
        [AccessAuthorize]
        public ActionResult UpdateApprove(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = _service.GetItem(id, "AssetType,ChangeRequestType");
            if (item.Status != ChangeStatusEnum.Reviewed)
                return RedirectToAction("Approve");

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }
        [HttpPost]
        [AccessAuthorize]
        public ActionResult UpdateApprove(Change item, string textComment, string checkboxApprove)
        {

            bool approve = false;
            if (checkboxApprove == "on")
                approve = true;

            var change = _service.GetItemAsNoTracking(item.ChangeId, "AssetType,ChangeRequestType");
            item = change;
            if (textComment == null || textComment == "")
            {
                TempData["ErrorMessage"] = "Please enter a comment";
                return View(item);
            }

            if (change.Status == ChangeStatusEnum.Reviewed)
            {
                if (approve)
                    change.Status = ChangeStatusEnum.Approved;
                else
                    change.Status = ChangeStatusEnum.Rejected;

                change.UpdatedDate = UserDateTime.GetUserDate();
                change.UpdatedBy = Session["UserId"].ToString();
                var entitySaved = _service.Update(item);
                if (entitySaved)
                {
                    ItemLogService _logService = new ItemLogService();
                    ItemLog log = new ItemLog();
                    log.ItemType = ItemTypeEnum.Change;
                    log.ItemId = change.ChangeId;
                    log.Comment = change.Status.EnumDisplayName() + ": " + textComment;
                    log.Status = change.Status.ToString();
                    log.UpdatedBy = change.UpdatedBy;
                    log.UpdatedDate = change.UpdatedDate;
                    _logService.Insert(log);

                    #region Email
                    string status = "rejected";
                    if (approve)
                        status = "approved";

                    string userName = Session["UserName"].ToString();
                    String emailBody = "";
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();
                    ToRecipients.Add("itsm.changes@laugfs.lk");
                    //CCRecipients.Add("kalhari.gamage@laugfs.lk");
                    //CCRecipients.Add("viraga.mahesh@laugfs.lk");
                    emailBody = emailTemplate.ChangeApproveReject(change, userName, status);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "ITSM - Change", emailBody).Trim();

                    #endregion

                    //Create Release for Minor and Standard
                    if (change.ChangeType == ChangeTypeEnum.Minor)
                    {
                        #region Release 

                        ReleaseService _releasService = new ReleaseService();
                        Release newItem = new Release();
                        newItem.Subject = "Release - " + change.Subject;
                        newItem.Description = change.Description;
                        newItem.Priority = change.Priority;
                        newItem.AssetTypeId = change.AssetTypeId;
                        newItem.ChangeRequestTypeId = change.ChangeRequestTypeId;
                        newItem.ReleaseType = ReleaseTypeEnum.Minor;
                        newItem.StartDate = UserDateTime.GetUserDateOnly();
                        newItem.EndDate = UserDateTime.GetUserDateOnly();
                        newItem.RequestedBy = change.RequestedBy;
                        newItem.UpdatedBy = Session["UserId"].ToString();
                        newItem.UpdatedDate = UserDateTime.GetUserDate();
                        newItem.RequestedDate = UserDateTime.GetUserDate();
                        newItem.TestPlan = "N/A";
                        newItem.BuildPlan = "N/A";
                        newItem.Status = ReleaseStatusEnum.Planned;

                        var entitySavedRel = _releasService.Insert(newItem);
                        if (entitySavedRel)
                        {

                            var releaseChange = new ReleaseChange();
                            releaseChange.ChangeId = Convert.ToInt64(change.ChangeId);
                            releaseChange.ReleaseId = Convert.ToInt64(newItem.ReleaseId);
                            releaseChange.UpdatedDate = UserDateTime.GetUserDate();
                            releaseChange.UpdatedBy = Session["UserId"].ToString();
                            ReleaseChangeService _relChangeService = new ReleaseChangeService();
                            _relChangeService.Insert(releaseChange);

                        }
                        #endregion
                    }

                    TempData["SuccessMessage"] = "Change has been successfully updated.";
                    return RedirectToAction("Approve");
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry, something went wrong. Record couldn't save, Please contact the IT support.";
                    return View(item);


                }
            }
            else
            {
                TempData["ErrorMessage"] = "Sorry, something went wrong. Record couldn't save, Please contact the IT support.";
                return View(item);


            }




        }


        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult RequestApprove(long? id, string comment)
        //{
        //    int errorCode = 0;

        //    if (comment == null || comment == "")
        //        errorCode = 2;
        //    else
        //    {
        //        var item = _service.GetItem(id, "");
        //        if (item != null)
        //        {
        //            if (item.Status == ChangeStatusEnum.Reviewed)
        //            {
        //                item.Status = ChangeStatusEnum.Approved;
        //                item.UpdatedDate = UserDateTime.GetUserDate();
        //                item.UpdatedBy = Session["UserId"].ToString();
        //                var entitySaved = _service.Update(item);
        //                if (entitySaved)
        //                {
        //                    ItemLogService _logService = new ItemLogService();
        //                    ItemLog log = new ItemLog();
        //                    log.ItemType = ItemTypeEnum.Change;
        //                    log.ItemId = item.ChangeId;
        //                    log.Comment = item.Status.EnumDisplayName() + ": " + comment;
        //                    log.Status = item.Status.ToString();
        //                    log.UpdatedBy = item.UpdatedBy;
        //                    log.UpdatedDate = item.UpdatedDate;
        //                    _logService.Insert(log);


        //                    errorCode = 1;
        //                }
        //            }
        //        }

        //    }
        //    return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        //}

        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult RequestReject(long? id, string comment)
        //{
        //    int errorCode = 0;

        //    if (comment == null || comment == "")
        //        errorCode = 2;
        //    else
        //    {
        //        var item = _service.GetItem(id, "");
        //        if (item != null)
        //        {
        //            if (item.Status == ChangeStatusEnum.Reviewed)
        //            {
        //                item.Status = ChangeStatusEnum.Rejected;
        //                item.UpdatedDate = UserDateTime.GetUserDate();
        //                item.UpdatedBy = Session["UserId"].ToString();
        //                var entitySaved = _service.Update(item);
        //                if (entitySaved)
        //                {
        //                    ItemLogService _logService = new ItemLogService();
        //                    ItemLog log = new ItemLog();
        //                    log.ItemType = ItemTypeEnum.Change;
        //                    log.ItemId = item.ChangeId;
        //                    log.Comment = item.Status.EnumDisplayName() + ": " + comment;
        //                    log.Status = item.Status.ToString();
        //                    log.UpdatedBy = item.UpdatedBy;
        //                    log.UpdatedDate = item.UpdatedDate;
        //                    _logService.Insert(log);


        //                    errorCode = 1;
        //                }
        //            }
        //        }

        //    }
        //    return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        //}


    }
}
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
    public class ProblemController : Controller
    {
        private readonly ProblemService _service = new ProblemService();
        // GET: Problem
        public ActionResult Index(long? teamId, long? categoryId, long? statusId, string startDate, string endDate)
        {
            DateTime sDate = UserDateTime.GetUserDate();
            DateTime eDate = UserDateTime.GetUserDate();
            if (startDate != null && startDate != "" && endDate != null && endDate != "")
            {
                try
                {
                    sDate = Convert.ToDateTime(startDate);
                    eDate = Convert.ToDateTime(endDate);
                }
                catch (Exception ex) { }
            }
            if (teamId == null)
                teamId = 0;
            if (categoryId == null)
                categoryId = 0;
            if (statusId == null)
                statusId = 0;



            var items = _service.GetItems(teamId, categoryId, statusId, sDate, eDate.AddDays(1),
             "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser");

            ViewBag.TeamId = teamId;
            ViewBag.CategoryId = categoryId;
            ViewBag.StatusId = statusId;
            ViewBag.StartDate = sDate.ToString("MM/dd/yyyy");
            ViewBag.EndDate = eDate.ToString("MM/dd/yyyy");
            return View(items);

        }



        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {
            var item = new ProblemVM();

            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(ProblemVM item)
        {
            if (ModelState.IsValid)
            {

                ViewBag.CategoryId = item.CategoryId;
                ViewBag.SubCategoryId = item.SubCategoryId;
                ViewBag.AssetTypeId = item.AssetTypeId;

                if (item.CategoryId == 0 || item.AssetTypeId == 0 || item.SubCategoryId == 0
                    || item.Subject == null || item.Priority == 0)
                {
                    TempData["ErrorMessage"] = "Please enter required data. ";
                    return View("Create", item);
                }

                Problem newItem = new Problem();
                newItem.Subject = item.Subject;
                newItem.Description = item.Description;
                newItem.Priority = item.Priority;
                newItem.RequestedBy = Session["UserId"].ToString();
                newItem.UpdatedBy = Session["UserId"].ToString();
                newItem.UpdatedDate = UserDateTime.GetUserDate();
                newItem.RequestedDate = UserDateTime.GetUserDate();

                RequestTypeService _requestTypeService = new RequestTypeService();
                RequestType requestType = new RequestType();
                requestType = _requestTypeService.GetItemAsNoTracking(TicketTypeEnum.IN, item.CategoryId, item.AssetTypeId, item.SubCategoryId, "Category,SubCategory,AssetType");
                newItem.RequestTypeId = requestType.RequestTypeId;

                newItem.Status = ProblemStatusEnum.Pending;

                var entitySaved = _service.Insert(newItem);
                if (entitySaved)
                {

                    TempData["SuccessMessage"] = "Problem has been successfully created.";

                    #region Email
                    string userName = Session["UserName"].ToString();
                    String emailBody = "";
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();
                    ToRecipients.Add("itsm.problem@laugfs.lk");
                    //CCRecipients.Add("kalhari.gamage@laugfs.lk");
                    //CCRecipients.Add("viraga.mahesh@laugfs.lk");
                    emailBody = emailTemplate.ProblemCreate(newItem, userName, requestType);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "ITSM - New Problem", emailBody).Trim();

                    #endregion


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
            var item = _service.GetItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,RequestedUser");
            ItemLogService _logService = new ItemLogService();
            item.ItemLogs = _logService.GetProblemLogs(id, "UpdatedUser").ToList();

            ItemDocService _itemDocService = new ItemDocService();
            item.ItemsDocs = _itemDocService.GetProblemDocs(item.ProblemId, "").ToList();

            //ItemAssetService _itemAssetService = new ItemAssetService();
            //item.ItemAssets = _itemAssetService.GetAssets(ItemTypeEnum.Problem, item.ProblemId, "Asset").ToList();

            ProblemTicketService _problemTickerService = new ProblemTicketService();
            var pTickets = _problemTickerService.GetProblemTicketByProblemId(item.ProblemId, "Ticket");
            Collection<Ticket> assignTickets = new Collection<Ticket>();
            foreach (ProblemTicket pt in pTickets)
            {
                assignTickets.Add(pt.Ticket);
            }

            item.Tickets = assignTickets;


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

            var items = _service.GetAllByUpdatedBy(Session["UserId"].ToString(),
                "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team").OrderByDescending(o => o.UpdatedBy).ToList();
            //ItemDocService _itemDocService = new ItemDocService();
            //foreach (Ticket t in items)
            //{
            //    t.TicketDocs = _itemDocService.GetTicketDocs(t.TicketId, "").ToList();
            //}

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
            //item.Problem = _service.GetItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team");
            //item.ItemDocs = _itemDocService.GetProblemDocs(item.Problem.ProblemId, "").ToList();

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
        //    //item.Problem = _service.GetItem(item.Problem.ProblemId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team");
        //    //item.ItemDocs = _itemDocService.GetProblemDocs(item.Problem.ProblemId, "").ToList();

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
        //            TicketDoc doc = new TicketDoc();
        //            doc.FileName = item.FileName;
        //            //doc.FileUrl = fileUrl;
        //           // doc.ItemId = item.Problem.ProblemId;
        //            doc.ItemType = ItemTypeEnum.Problem;
        //            doc.UpdatedDate = UserDateTime.GetUserDate();
        //            doc.UpdatedBy = Session["UserId"].ToString();
        //            _itemDocService.Insert(doc);

        //            item.TicketDocs.Add(doc);

        //            #region Log
        //            ItemLogService _logService = new ItemLogService();
        //            ItemLog log = new ItemLog();
        //            log.ItemType = ItemTypeEnum.Problem;
        //            //log.ItemId = item.Problem.ProblemId;
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

            //ItemAssetService _itemAssetService = new ItemAssetService();
            //item.AssignedItems = _itemAssetService.GetAssets(ItemTypeEnum.Problem, id, "Asset").ToList();
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

        //    item.AssignedItems = _itemAssetService.GetAssets(ItemTypeEnum.Problem, item.ItemId, "").ToList();

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
        //    newItem.AssetCategory = item.AssetCategory;
        //    newItem.Description = _item.Description;
        //    newItem.ItemId = item.ItemId;
        //    newItem.ItemType = ItemTypeEnum.Problem;
        //    newItem.ModelName = _item.ModelNo;
        //    newItem.SerialNo = _item.SerialNo;
        //    newItem.Location = _item.Location;
        //    newItem.Company = _item.Company;
        //    newItem.UpdatedDate = UserDateTime.GetUserDate();
        //    newItem.UpdatedBy = Session["UserId"].ToString();


        //    if (!_itemAssetService.IsAvailable(newItem))
        //    {
        //        var entitySaved = _itemAssetService.Insert(newItem);
        //        if (entitySaved)
        //        {
        //            ItemLogService _logService = new ItemLogService();
        //            ItemLog log = new ItemLog();
        //            log.ItemType = ItemTypeEnum.Problem;
        //            log.ItemId = newItem.ItemId;
        //            log.Comment = "Asset " + newItem.AssetName + "(" + newItem.AssetCode + ")" + " has been assigned";
        //            log.UpdatedBy = newItem.UpdatedBy;
        //            log.UpdatedDate = newItem.UpdatedDate;
        //            _logService.Insert(log);

        //            item.AssignedItems = _itemAssetService.GetAssets(ItemTypeEnum.Problem, item.ItemId, "").ToList();

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
        public ActionResult DeleteAsset(long? assetItemId, long? problemId)
        {
            int errorCode = 0;
            ItemAssetService _itemAssetService = new ItemAssetService();
            var item = _itemAssetService.GetItem(assetItemId, "Asset");
            item.ItemId = Convert.ToInt64(problemId);
            item.UpdatedDate = UserDateTime.GetUserDate();
            item.UpdatedBy = Session["UserId"].ToString();

            //var problem = _service.GetItem(item.ItemId, "");
            if (item != null)
            {
                if (_itemAssetService.Delete(item))
                {
                    ItemLogService _logService = new ItemLogService();
                    ItemLog log = new ItemLog();
                    log.ItemType = ItemTypeEnum.Problem;
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

            DateTime sDate = UserDateTime.GetUserDate();
            DateTime eDate = UserDateTime.GetUserDate();
            if (startDate != null && startDate != "" && endDate != null && endDate != "")
            {
                try
                {
                    sDate = Convert.ToDateTime(startDate);
                    eDate = Convert.ToDateTime(endDate);
                }
                catch (Exception ex) { }
            }

            item.Problem = _service.GetItem(id, "");
            item.ProblemId = item.Problem.ProblemId;

            TicketService _ticketService = new TicketService();
            item.Tickets = _ticketService.GetTikcets(item.Problem.RequestTypeId, sDate, eDate.AddDays(1), "RequestedUser").ToList();

            ProblemTicketService _problemTickerService = new ProblemTicketService();
            var pTickets = _problemTickerService.GetProblemTicketByProblemId(item.ProblemId, "Ticket,Ticket.RequestedUser");
            Collection<Ticket> assignTickets = new Collection<Ticket>();
            foreach (ProblemTicket pt in pTickets)
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

            DateTime sDate = UserDateTime.GetUserDate();
            DateTime eDate = UserDateTime.GetUserDate();
            if (startDate != null && startDate != "" && endDate != null && endDate != "")
            {
                try
                {
                    sDate = Convert.ToDateTime(startDate);
                    eDate = Convert.ToDateTime(endDate);
                }
                catch (Exception ex) { }
            }
            item.Problem = _service.GetItem(item.ProblemId, "");

            TicketService _ticketService = new TicketService();
            item.Tickets = _ticketService.GetTikcets(item.Problem.RequestTypeId, sDate, eDate.AddDays(1), "RequestedUser").ToList();

            ProblemTicketService _problemTickerService = new ProblemTicketService();
            var pTickets = _problemTickerService.GetProblemTicketByProblemId(item.ProblemId, "Ticket,Ticket.RequestedUser");
            Collection<Ticket> assignTickets = new Collection<Ticket>();
            foreach (ProblemTicket pt in pTickets)
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
        public ActionResult DeleteTicket(long? ticketId, long? problemId)
        {
            int errorCode = 0;
            ProblemTicketService _problemTicketService = new ProblemTicketService();
            var item = _problemTicketService.GetItem(problemId, ticketId, "");
            item.UpdatedDate = UserDateTime.GetUserDate();
            item.UpdatedBy = Session["UserId"].ToString();
            if (item != null)
            {
                TicketService _ticketService = new TicketService();
                var ticket = _ticketService.GetItem(ticketId, "");

                if (_problemTicketService.HasRelationalData(item))
                    errorCode = 2;
                else if (_problemTicketService.Delete(item))
                {
                    ItemLogService _logService = new ItemLogService();
                    ItemLog log = new ItemLog();
                    log.ItemType = ItemTypeEnum.Problem;
                    log.ItemId = item.ProblemId;
                    log.Comment = "Ticket " + ticket.Code + " has been deleted";
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
        public ActionResult AssignTicket(long? ticketId, long? problemId)
        {
            int errorCode = 0;
            ProblemTicketService _problemTicketService = new ProblemTicketService();
            var item = new ProblemTicket();
            if (ticketId != null && problemId != null)
            {
                item.ProblemId = Convert.ToInt64(problemId);
                item.TicketId = Convert.ToInt64(ticketId);
                item.UpdatedDate = UserDateTime.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();

                TicketService _ticketService = new TicketService();
                var ticket = _ticketService.GetItem(ticketId, "");

                if (!_problemTicketService.IsAvailable(item))
                {
                    var entitySaved = _problemTicketService.Insert(item);
                    if (entitySaved)
                    {
                        ItemLogService _logService = new ItemLogService();
                        ItemLog log = new ItemLog();
                        log.ItemType = ItemTypeEnum.Problem;
                        log.ItemId = item.ProblemId;
                        log.Comment = "Ticket " + ticket.Code + " has been assigned";
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
        public ActionResult Analyze()
        {

            var items = _service.GetAllByStatus(ProblemStatusEnum.Pending,
                "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team").OrderByDescending(o => o.UpdatedBy).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }


        [HttpGet]
        [EncryptedActionParameter]
        [AccessAuthorize]
        public ActionResult UpdateAnalyze(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = _service.GetItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team");
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }
        [HttpPost]
        [AccessAuthorize]
        public ActionResult UpdateAnalyze(Problem item)
        {
            //if (ModelState.IsValid)
            //{
            var problem = _service.GetItemAsNoTracking(item.ProblemId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team");
            problem.RootCause = item.RootCause;
            problem.Symptoms = item.Symptoms;
            problem.Impact = item.Impact;
            item = problem;
            if (item.RootCause == null || item.Symptoms == null || item.Impact == null)
            {

                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }


            problem.UpdatedBy = Session["UserId"].ToString();
            problem.UpdatedDate = UserDateTime.GetUserDate();
            problem.Status = ProblemStatusEnum.Analysed;

            var entitySaved = _service.Update(problem);
            if (entitySaved)
            {
                ItemLogService _logService = new ItemLogService();
                ItemLog log = new ItemLog();
                log.ItemType = ItemTypeEnum.Problem;
                log.ItemId = problem.ProblemId;
                log.Comment = "Problem -" + problem.Status.EnumDisplayName();
                log.Status = problem.Status.ToString();
                log.UpdatedBy = problem.UpdatedBy;
                log.UpdatedDate = problem.UpdatedDate;
                _logService.Insert(log);


                TempData["SuccessMessage"] = "Problem has been successfully updated.";
                return RedirectToAction("Analyze");

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
        public ActionResult Close()
        {

            var items = _service.GetAllByStatus(ProblemStatusEnum.Analysed,
                "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team").OrderByDescending(o => o.UpdatedBy).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [AccessAuthorize]
        public ActionResult UpdateSolution(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = _service.GetItem(id, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team");
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }
        [HttpPost]
        [AccessAuthorize]
        public ActionResult UpdateSolution(Problem item)
        {
            //if (ModelState.IsValid)
            //{
            var problem = _service.GetItemAsNoTracking(item.ProblemId, "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team");
            problem.Solution = item.Solution;
            item = problem;
            if (item.Solution == null)
            {

                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }


            problem.UpdatedBy = Session["UserId"].ToString();
            problem.UpdatedDate = UserDateTime.GetUserDate();
            problem.Status = ProblemStatusEnum.Closed;

            var entitySaved = _service.Update(problem);
            if (entitySaved)
            {
                ItemLogService _logService = new ItemLogService();
                ItemLog log = new ItemLog();
                log.ItemType = ItemTypeEnum.Problem;
                log.ItemId = problem.ProblemId;
                log.Comment = "Problem -" + problem.Status.EnumDisplayName();
                log.Status = problem.Status.ToString();
                log.UpdatedBy = problem.UpdatedBy;
                log.UpdatedDate = problem.UpdatedDate;
                _logService.Insert(log);

                KedbItemService _kedbItemService = new KedbItemService();
                KedbItem kedb = new KedbItem();
                kedb.ProblemId = problem.ProblemId;
                kedb.RequestTypeId = problem.RequestTypeId;
                kedb.Subject = problem.Subject;
                kedb.RootCause = problem.RootCause;
                kedb.Solution = problem.Solution;
                kedb.RequestedBy = problem.UpdatedBy;
                kedb.RequestedDate = problem.UpdatedDate;
                kedb.UpdatedBy = problem.UpdatedBy;
                kedb.UpdatedDate = problem.UpdatedDate;
                kedb.Status = RequestStatusEnum.Pending;
                _kedbItemService.Insert(kedb);

                #region Email
                string userName = Session["UserName"].ToString();
                String emailBody = "";
                EmailTemplate emailTemplate = new EmailTemplate();
                EmailClient emailClient = new EmailClient();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                ToRecipients.Add("itsm.problem@laugfs.lk");
                //CCRecipients.Add("viraga.mahesh@laugfs.lk");
                emailBody = emailTemplate.ProblemClose(problem, userName, problem.RequestType);
                String res = emailClient.SendEmail(ToRecipients, CCRecipients, "ITSM - Problem", emailBody).Trim();
                #endregion

                TempData["SuccessMessage"] = "Problem has been successfully closed.";
                return RedirectToAction("Close");

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
    }
}
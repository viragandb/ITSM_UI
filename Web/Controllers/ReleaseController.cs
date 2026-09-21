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
    public class ReleaseController : Controller
    {
        private readonly ReleaseService _service = new ReleaseService();
        // GET: Release
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
            var item = new ReleaseVM();
            item.StartDate = UserDateTime.GetUserDate();
            item.EndDate = UserDateTime.GetUserDate();
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(ReleaseVM item)
        {
            if (ModelState.IsValid)
            {

                ViewBag.AssetTypeId = item.AssetTypeId;
                ViewBag.ChangeRequestTypeId = item.ChangeRequestTypeId;

                if (item.AssetTypeId == 0 || item.ChangeRequestTypeId == 0
                    || item.Subject == null || item.ReleaseType == 0 || item.Priority == 0
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

                Release newItem = new Release();
                newItem.Subject = item.Subject;
                newItem.Description = item.Description;
                newItem.Priority = item.Priority;
                newItem.AssetTypeId = item.AssetTypeId;
                newItem.ChangeRequestTypeId = item.ChangeRequestTypeId;
                newItem.ReleaseType = item.ReleaseType;
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

                newItem.Status = ReleaseStatusEnum.Pending;

                var entitySaved = _service.Insert(newItem);
                if (entitySaved)
                {
                    #region Email

                    var release = _service.GetItem(newItem.ReleaseId, "AssetType,ChangeRequestType");
                    string userName = Session["UserName"].ToString();
                    String emailBody = "";
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();
                    ToRecipients.Add("itsm.releases@laugfs.lk");
                    //CCRecipients.Add("kalhari.gamage@laugfs.lk");
                    //CCRecipients.Add("viraga.mahesh@laugfs.lk");
                    emailBody = emailTemplate.ReleaseCreate(release, userName);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "ITSM - New Release", emailBody).Trim();

                    #endregion

                    TempData["SuccessMessage"] = "Release has been successfully created.";
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
            item.ItemLogs = _logService.GetReleaseLogs(id, "UpdatedUser").ToList();

            ItemDocService _itemDocService = new ItemDocService();
            item.ItemsDocs = _itemDocService.GetReleaseDocs(item.ReleaseId, "").ToList();

            //ItemAssetService _itemAssetService = new ItemAssetService();
            //item.ItemAssets = _itemAssetService.GetAssets(ItemTypeEnum.Release, item.ReleaseId, "Asset").ToList();

            ReleaseChangeService _releaseChangeService = new ReleaseChangeService();
            var changes = _releaseChangeService.GetReleaseChangeByReleaseId(item.ReleaseId, "Change");
            Collection<Change> assignChanges = new Collection<Change>();
            foreach (ReleaseChange c in changes)
            {
                assignChanges.Add(c.Change);
            }

            item.Changes = assignChanges;


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

            var items = _service.GetItemsForAssign(Session["UserId"].ToString(), "AssetType,ChangeRequestType").OrderByDescending(o => o.UpdatedBy).ToList();

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
            //item.Release = _service.GetItem(id, "AssetType,ChangeRequestType");
            //item.ItemDocs = _itemDocService.GetReleaseDocs(item.Release.ReleaseId, "").ToList();

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
            FileUploadService _fileUploadService = new FileUploadService();
            ItemDocService _itemDocService = new ItemDocService();
            //item.Release = _service.GetItem(item.Release.ReleaseId, "AssetType,ChangeRequestType");
            //item.ItemDocs = _itemDocService.GetReleaseDocs(item.Release.ReleaseId, "").ToList();

            if (item.FileName == null || item.FileName == "")
            {
                TempData["ErrorMessage"] = "Please enter file name. ";
                return View(item);

            }
            try
            {

                if (itemFile != null && itemFile.ContentLength != 0)
                {
                    //HttpPostedFileBase _file = itemFile;
                    //var fileUrl = await _fileUploadService.UploadImageAsync(_file);
                    ItemDoc doc = new ItemDoc();
                    //doc.FileName = item.FileName;
                    //doc.FileUrl = fileUrl;
                    //doc.ItemId = item.Release.ReleaseId;
                    //doc.ItemType = ItemTypeEnum.Release;
                    //doc.UpdatedDate = UserDateTime.GetUserDate();
                    //doc.UpdatedBy = Session["UserId"].ToString();
                    //_itemDocService.Insert(doc);

                    //item.ItemDocs.Add(doc);

                    #region Log
                    ItemLogService _logService = new ItemLogService();
                    ItemLog log = new ItemLog();
                    log.ItemType = ItemTypeEnum.Release;
                   // log.ItemId = item.Release.ReleaseId;
                    log.Comment = "Document (" + doc.FileName + ") has been Uploaded";
                    log.UpdatedBy = doc.UpdatedBy;
                    log.UpdatedDate = doc.UpdatedDate;
                    _logService.Insert(log);
                    #endregion
                    TempData["SuccessMessage"] = "File have been successfully uploaded.";
                    return RedirectToAction("AssignItems");

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

        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult AssignAsset(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = new AssignAssetVM();
            item.ItemId = Convert.ToInt64(id);

            ItemAssetService _itemAssetService = new ItemAssetService();
          //  item.AssignedItems = _itemAssetService.GetAssets(ItemTypeEnum.Release, id, "Asset").ToList();
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

        //    item.AssignedItems = _itemAssetService.GetAssets(ItemTypeEnum.Release, item.ItemId, "").ToList();

        //    if (item.AssetCode == null || item.AssetCode == "")
        //    {
        //        TempData["ErrorMessage"] = "Please enter the asset code. ";
        //        return View(item);
        //    }
        //    //if (item.AssetTypeId == 0)
        //    //{
        //    //    TempData["ErrorMessage"] = "Please select the asset type. ";
        //    //    return View(item);
        //    //}
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
        //    //newItem.AssetTypeId = item.AssetTypeId;
        //    newItem.Description = _item.Description;
        //    newItem.ItemId = item.ItemId;
        //    newItem.ItemType = ItemTypeEnum.Release;
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
        //            log.ItemType = ItemTypeEnum.Release;
        //            log.ItemId = newItem.ItemId;
        //            log.Comment = "Asset " + newItem.AssetName + "(" + newItem.AssetCode + ")" + " has been assigned";
        //            log.UpdatedBy = newItem.UpdatedBy;
        //            log.UpdatedDate = newItem.UpdatedDate;
        //            _logService.Insert(log);

        //            item.AssignedItems = _itemAssetService.GetAssets(ItemTypeEnum.Release, item.ItemId, "").ToList();

        //            TempData["SuccessMessage"] = "Asset has been successfully assigned.";

        //        }
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Asset already exists.";

        //    }

        //    return View(item);
        //    // return RedirectToAction("AssignAsset", new { id = item.ItemId });

        //}

        [AccessAuthorize]
        [HttpGet]
        public ActionResult DeleteAsset(long? assetItemId, long? releaseId)
        {
            int errorCode = 0;
            ItemAssetService _itemAssetService = new ItemAssetService();
            var item = _itemAssetService.GetItem(assetItemId, "Asset");
            item.ItemId = Convert.ToInt64(releaseId);
            item.UpdatedDate = UserDateTime.GetUserDate();
            item.UpdatedBy = Session["UserId"].ToString();

            //var problem = _service.GetItem(item.ItemId, "");
            if (item != null)
            {
                if (_itemAssetService.Delete(item))
                {
                    ItemLogService _logService = new ItemLogService();
                    ItemLog log = new ItemLog();
                    log.ItemType = ItemTypeEnum.Release;
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
        public ActionResult AddChange(long? id, string startDate, string endDate)
        {

            var item = new AddChangeVM();
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

            item.Release = _service.GetItem(id, "");
            item.ReleaseId = item.Release.ReleaseId;

            ChangeService _changeService = new ChangeService();
            item.Changes = _changeService.GetItemsToAssign(item.Release.ChangeRequestTypeId, sDate, eDate.AddDays(1), "RequestedUser").ToList();

            ReleaseChangeService _releaseChangeService = new ReleaseChangeService();
            var changes = _releaseChangeService.GetReleaseChangeByReleaseId(item.ReleaseId, "Change,Change.RequestedUser");
            Collection<Change> assignChanges = new Collection<Change>();
            foreach (ReleaseChange c in changes)
            {
                assignChanges.Add(c.Change);
            }
            item.AssignedChanges = assignChanges;

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
        public ActionResult AddChange(AddChangeVM item, string startDate, string endDate)
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
            item.Release = _service.GetItem(item.ReleaseId, "");

            ChangeService _changeService = new ChangeService();
            item.Changes = _changeService.GetItemsToAssign(item.Release.ChangeRequestTypeId, sDate, eDate.AddDays(1) , "RequestedUser").ToList();

            ReleaseChangeService _releaseChangeService = new ReleaseChangeService();
            var changes = _releaseChangeService.GetReleaseChangeByReleaseId(item.ReleaseId, "Change,Change.RequestedUser");
            Collection<Change> assignChanges = new Collection<Change>();
            foreach (ReleaseChange c in changes)
            {
                assignChanges.Add(c.Change);
            }
            item.AssignedChanges = assignChanges;

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
        public ActionResult DeleteChange(long? releaseId, long? changeId)
        {
            int errorCode = 0;
            ReleaseChangeService _releaseChangeService = new ReleaseChangeService();
            var item = _releaseChangeService.GetItem(releaseId, changeId, "");
            item.UpdatedDate = UserDateTime.GetUserDate();
            item.UpdatedBy = Session["UserId"].ToString();
            if (item != null)
            {
                if (_releaseChangeService.HasRelationalData(item))
                    errorCode = 2;
                else if (_releaseChangeService.Delete(item))
                {
                    ItemLogService _logService = new ItemLogService();
                    ItemLog log = new ItemLog();
                    log.ItemType = ItemTypeEnum.Release;
                    log.ItemId = item.ReleaseId;
                    log.Comment = "Change " + item.ChangeId + " has been deleted";
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
        public ActionResult AssignChange(long? releaseId, long? changeId)
        {
            int errorCode = 0;
            ReleaseChangeService _releaseChangeService = new ReleaseChangeService();
            var item = new ReleaseChange();
            if (releaseId != null && changeId != null)
            {
                item.ChangeId = Convert.ToInt64(changeId);
                item.ReleaseId = Convert.ToInt64(releaseId);
                item.UpdatedDate = UserDateTime.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();

                if (!_releaseChangeService.IsAvailable(item))
                {
                    var entitySaved = _releaseChangeService.Insert(item);
                    if (entitySaved)
                    {
                        ItemLogService _logService = new ItemLogService();
                        ItemLog log = new ItemLog();
                        log.ItemType = ItemTypeEnum.Release;
                        log.ItemId = item.ReleaseId;
                        log.Comment = "Change " + item.ChangeId + " has been assigned";
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

            var items = _service.GetAllByStatus(ReleaseStatusEnum.Pending, "AssetType,ChangeRequestType").OrderByDescending(o => o.UpdatedBy).ToList();

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
        public ActionResult Edit(Release item)
        {
            if (ModelState.IsValid)
            {
                if (item.Subject == null || item.ReleaseType == 0 || item.Priority == 0 
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

                Release updateItem = _service.GetItem(item.ReleaseId, "");
                updateItem.Subject = item.Subject;
                updateItem.Description = item.Description;
                updateItem.ReleaseType = item.ReleaseType;
                updateItem.Priority = item.Priority;
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
                    TempData["SuccessMessage"] = "Release request has been deleted successfully";
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
        public ActionResult UpdatePlan(Release item)
        {
            //if (ModelState.IsValid)
            //{
            var updateItem = _service.GetItemAsNoTracking(item.ReleaseId, "AssetType,ChangeRequestType");
            updateItem.TestPlan = item.TestPlan;
            updateItem.BuildPlan = item.BuildPlan;
            item = updateItem;
            if (item.BuildPlan == null || item.TestPlan == null
                || item.BuildPlan == "" || item.TestPlan == "")
            {

                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }


            updateItem.UpdatedBy = Session["UserId"].ToString();
            updateItem.UpdatedDate = UserDateTime.GetUserDate();
            updateItem.Status = ReleaseStatusEnum.Planned;

            var entitySaved = _service.Update(updateItem);
            if (entitySaved)
            {
                ItemLogService _logService = new ItemLogService();
                ItemLog log = new ItemLog();
                log.ItemType = ItemTypeEnum.Release;
                log.ItemId = updateItem.ReleaseId;
                log.Comment = updateItem.Status.EnumDisplayName();
                log.Status = updateItem.Status.ToString();
                log.UpdatedBy = updateItem.UpdatedBy;
                log.UpdatedDate = updateItem.UpdatedDate;
                _logService.Insert(log);

                #region Email

                string userName = Session["UserName"].ToString();
                String emailBody = "";
                EmailTemplate emailTemplate = new EmailTemplate();
                EmailClient emailClient = new EmailClient();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                ToRecipients.Add("itsm.releases@laugfs.lk");
                //CCRecipients.Add("kalhari.gamage@laugfs.lk");
                //CCRecipients.Add("viraga.mahesh@laugfs.lk");
                emailBody = emailTemplate.ReleasePlan(updateItem, userName);
                String res = emailClient.SendEmail(ToRecipients, CCRecipients, "ITSM - Release", emailBody).Trim();

                #endregion

                TempData["SuccessMessage"] = "Release has been successfully updated.";
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
        public ActionResult Complete()
        {

            var items = _service.GetAllByStatus(ReleaseStatusEnum.Planned, "AssetType,ChangeRequestType").OrderByDescending(o => o.UpdatedBy).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [HttpGet]
        [EncryptedActionParameter]
        [AccessAuthorize]
        public ActionResult UpdateComplete(long? id)
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
        public ActionResult UpdateComplete(Release item, string textComment)
        {
            //if (ModelState.IsValid)
            //{
            var updateItem = _service.GetItemAsNoTracking(item.ReleaseId, "AssetType,ChangeRequestType");

            item = updateItem;

            if (textComment == null || textComment == "")
            {
                TempData["ErrorMessage"] = "Please enter a comment";
                return View(item);
            }


            if (updateItem.Status == ReleaseStatusEnum.Planned)
            {
                updateItem.Status = ReleaseStatusEnum.Completed;
                updateItem.UpdatedDate = UserDateTime.GetUserDate();
                updateItem.UpdatedBy = Session["UserId"].ToString();
                var entitySaved = _service.Update(item);
                if (entitySaved)
                {
                    ItemLogService _logService = new ItemLogService();
                    ItemLog log = new ItemLog();
                    log.ItemType = ItemTypeEnum.Release;
                    log.ItemId = updateItem.ReleaseId;
                    log.Comment = updateItem.Status.EnumDisplayName() + ": " + textComment;
                    log.Status = updateItem.Status.ToString();
                    log.UpdatedBy = updateItem.UpdatedBy;
                    log.UpdatedDate = updateItem.UpdatedDate;
                    _logService.Insert(log);

                    #region Email

                    string userName = Session["UserName"].ToString();
                    String emailBody = "";
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();
                    ToRecipients.Add("itsm.releases@laugfs.lk");
                    //CCRecipients.Add("kalhari.gamage@laugfs.lk");
                    //CCRecipients.Add("viraga.mahesh@laugfs.lk");
                    emailBody = emailTemplate.ReleaseComplete(updateItem, userName);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "ITSM - Release", emailBody).Trim();

                    #endregion

                    TempData["SuccessMessage"] = "Release has been successfully completed.";
                    return RedirectToAction("Complete");
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry, something went wrong. Record couldn't save, Please contact the IT support.";
                    return View(item);

                }
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

        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult RequestComplete(long? id, string comment)
        //{
        //    int errorCode = 0;

        //    if (comment == null || comment == "")
        //        errorCode = 2;
        //    else
        //    {
        //        var item = _service.GetItem(id, "");
        //        if (item != null)
        //        {
        //            if (item.Status == ReleaseStatusEnum.Planned)
        //            {
        //                item.Status = ReleaseStatusEnum.Completed;
        //                item.UpdatedDate = UserDateTime.GetUserDate();
        //                item.UpdatedBy = Session["UserId"].ToString();
        //                var entitySaved = _service.Update(item);
        //                if (entitySaved)
        //                {
        //                    ItemLogService _logService = new ItemLogService();
        //                    ItemLog log = new ItemLog();
        //                    log.ItemType = ItemTypeEnum.Release;
        //                    log.ItemId = item.ReleaseId;
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
        public ActionResult Review()
        {

            var items = _service.GetAllByStatus(ReleaseStatusEnum.Completed, "AssetType,ChangeRequestType").OrderByDescending(o => o.UpdatedBy).ToList();

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
        public ActionResult UpdateReview(Release item, string textComment, string checkboxApprove)
        {

            bool approve = false;
            if (checkboxApprove == "on")
                approve = true;

            var updateItem = _service.GetItemAsNoTracking(item.ReleaseId, "AssetType,ChangeRequestType");
            item = updateItem;
            if (textComment == null || textComment == "")
            {
                TempData["ErrorMessage"] = "Please enter a comment";
                return View(item);
            }

            if (updateItem.Status == ReleaseStatusEnum.Completed)
            {
                if (approve)
                    updateItem.Status = ReleaseStatusEnum.Reviewed;
                else
                    updateItem.Status = ReleaseStatusEnum.Rejected;

                updateItem.UpdatedDate = UserDateTime.GetUserDate();
                updateItem.UpdatedBy = Session["UserId"].ToString();
                var entitySaved = _service.Update(item);
                if (entitySaved)
                {
                    ItemLogService _logService = new ItemLogService();
                    ItemLog log = new ItemLog();
                    log.ItemType = ItemTypeEnum.Release;
                    log.ItemId = updateItem.ReleaseId;
                    log.Comment = updateItem.Status.EnumDisplayName() + ": " + textComment;
                    log.Status = updateItem.Status.ToString();
                    log.UpdatedBy = updateItem.UpdatedBy;
                    log.UpdatedDate = updateItem.UpdatedDate;
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
                    ToRecipients.Add("itsm.releases@laugfs.lk");
                    //CCRecipients.Add("kalhari.gamage@laugfs.lk");
                    //CCRecipients.Add("viraga.mahesh@laugfs.lk");
                    emailBody = emailTemplate.ReleaseApproveReject(updateItem, userName, status);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "ITSM - Release", emailBody).Trim();

                    #endregion


                    TempData["SuccessMessage"] = "Release has been successfully updated.";
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
        //            if (item.Status == ReleaseStatusEnum.Completed)
        //            {
        //                item.Status = ReleaseStatusEnum.Reviewed;
        //                item.UpdatedDate = UserDateTime.GetUserDate();
        //                item.UpdatedBy = Session["UserId"].ToString();
        //                var entitySaved = _service.Update(item);
        //                if (entitySaved)
        //                {
        //                    ItemLogService _logService = new ItemLogService();
        //                    ItemLog log = new ItemLog();
        //                    log.ItemType = ItemTypeEnum.Release;
        //                    log.ItemId = item.ReleaseId;
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
        //            if (item.Status == ReleaseStatusEnum.Completed)
        //            {
        //                item.Status = ReleaseStatusEnum.Rejected;
        //                item.UpdatedDate = UserDateTime.GetUserDate();
        //                item.UpdatedBy = Session["UserId"].ToString();
        //                var entitySaved = _service.Update(item);
        //                if (entitySaved)
        //                {
        //                    ItemLogService _logService = new ItemLogService();
        //                    ItemLog log = new ItemLog();
        //                    log.ItemType = ItemTypeEnum.Release;
        //                    log.ItemId = item.ReleaseId;
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
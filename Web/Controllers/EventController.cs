using Domain;
using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers
{
    public class EventController : Controller
    {
        private readonly EventService _service = new EventService();
        // GET: Event
        [AccessAuthorize]
        public ActionResult Index(long? eventTypeId, long? statusId, string startDate, string endDate)
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
            if (eventTypeId == null)
                eventTypeId = 0;
            if (statusId == null)
                statusId = 0;

            var items = _service.GetItems(eventTypeId, statusId, sDate, eDate.AddDays(1), "EventType,RequestedUser");

            ViewBag.EventTypeId = eventTypeId;
            ViewBag.StatusId = statusId;
            ViewBag.StartDate = sDate.ToString("MM/dd/yyyy");
            ViewBag.EndDate = eDate.ToString("MM/dd/yyyy");
            return View(items);

        }




        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {
            var item = new Event();
            item.EventDate = UserDateTime.GetUserDate();
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public async Task<ActionResult> Create(Event item, HttpPostedFileBase image1, HttpPostedFileBase image2, HttpPostedFileBase image3)
        {
            if (ModelState.IsValid)
            {

                ViewBag.EventTypeId = item.EventTypeId;
                if (item.EventTypeId == 0 || item.EventDate == null ||
                    String.IsNullOrWhiteSpace(item.Subject) || String.IsNullOrWhiteSpace(item.EventTime))
                {
                    TempData["ErrorMessage"] = "Please enter required data. ";
                    return View("Create", item);
                }
                DateTime eventDate = UserDateTime.GetUserDate();
                try
                {
                    eventDate = Convert.ToDateTime(item.EventDate.ToString("MM/dd/yyyy") + " " + item.EventTime);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Please enter valid start and end dates ";
                    return View("Create", item);
                }

                if (image1 != null && image1.ContentLength != 0)
                {
                    if (image1.ContentLength > 2097152) //2MB
                    {
                        TempData["ErrorMessage"] = "Please reduce file size (Max. 2MB). ";
                        return View("Create", item);
                    }

                    if (image1.ContentType != "image/jpg" && image1.ContentType != "image/jpeg" && image1.ContentType != "image/png")
                    {
                        TempData["ErrorMessage"] = "Please upload a image. ";
                        return View("Create", item);
                    }
                }

                // if (file.ContentType.Contains("image"))
                FileUploadService _fileUploadService = new FileUploadService();
                ItemDocService _itemDocService = new ItemDocService();

                Event newItem = new Event();
                newItem.EventTypeId = item.EventTypeId;
                newItem.Subject = item.Subject;
                newItem.Description = item.Description;
                newItem.EventDate = eventDate;
                newItem.RequestedBy = Session["UserId"].ToString();
                newItem.UpdatedBy = Session["UserId"].ToString();
                newItem.UpdatedDate = UserDateTime.GetUserDate();
                newItem.RequestedDate = UserDateTime.GetUserDate();
                newItem.Status = EventStatusEnum.Opened;

                var entitySaved = _service.Insert(newItem);
                if (entitySaved)
                {
                    if (image1 != null && image1.ContentLength != 0)
                    {
                        if (image1.ContentLength <= 2097152) //2MB
                        {

                            if (image1.ContentType == "image/jpg" || image1.ContentType == "image/jpeg" || image1.ContentType == "image/png")
                            {
                                HttpPostedFileBase _file = image1;
                                //var fileUrl = await _fileUploadService.UploadImageAsync(_file);
                                ItemDoc doc = new ItemDoc();
                                doc.FileName = image1.FileName;
                               // doc.FileUrl = fileUrl;
                                doc.ItemId = newItem.EventId;
                                doc.ItemType = ItemTypeEnum.Event;
                                doc.UpdatedDate = UserDateTime.GetUserDate();
                                doc.UpdatedBy = Session["UserId"].ToString();
                                _itemDocService.Insert(doc);

                                //item.ItemDocs.Add(doc);
                            }
                        }
                    }

                    if (image2 != null && image2.ContentLength != 0)
                    {
                        if (image2.ContentLength <= 2097152) //2MB
                        {

                            if (image2.ContentType == "image/jpg" || image2.ContentType == "image/jpeg" || image2.ContentType == "image/png")
                            {
                                HttpPostedFileBase _file = image2;
                                //var fileUrl = await _fileUploadService.UploadImageAsync(_file);
                                ItemDoc doc = new ItemDoc();
                                doc.FileName = image2.FileName;
                                //doc.FileUrl = fileUrl;
                                doc.ItemId = newItem.EventId;
                                doc.ItemType = ItemTypeEnum.Event;
                                doc.UpdatedDate = UserDateTime.GetUserDate();
                                doc.UpdatedBy = Session["UserId"].ToString();
                                _itemDocService.Insert(doc);
                            }
                        }
                    }

                    if (image3 != null && image3.ContentLength != 0)
                    {
                        if (image3.ContentLength <= 2097152) //2MB
                        {

                            if (image3.ContentType == "image/jpg" || image3.ContentType == "image/jpeg" || image3.ContentType == "image/png")
                            {
                                HttpPostedFileBase _file = image3;
                                //var fileUrl = await _fileUploadService.UploadImageAsync(_file);
                                ItemDoc doc = new ItemDoc();
                                doc.FileName = image3.FileName;
                                //doc.FileUrl = fileUrl;
                                doc.ItemId = newItem.EventId;
                                doc.ItemType = ItemTypeEnum.Event;
                                doc.UpdatedDate = UserDateTime.GetUserDate();
                                doc.UpdatedBy = Session["UserId"].ToString();
                                _itemDocService.Insert(doc);

                                //item.ItemDocs.Add(doc);
                            }
                        }
                    }

                    #region Email

                    var eventItem = _service.GetItem(newItem.EventId, "EventType");
                    string userName = Session["UserName"].ToString();
                    String emailBody = "";
                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();
                    ToRecipients.Add("itsm.events@laugfs.lk");
                    //CCRecipients.Add("viraga.mahesh@laugfs.lk");
                    emailBody = emailTemplate.EventOpen(eventItem, userName);
                    String res = emailClient.SendEmail(ToRecipients, CCRecipients, "ITSM - New Event", emailBody).Trim();

                    #endregion

                    TempData["SuccessMessage"] = "Event has been successfully created.";
                    return RedirectToAction("Create");

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
        [AllowAnonymous]
        public JsonResult GetEventTypeListJsonResult()
        {

            var items = _service.GetEventTypes("");
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Pending()
        {

            var items = _service.GetAllByStatus(EventStatusEnum.Opened, "EventType").OrderByDescending(o => o.UpdatedDate).ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Close(long? id, string comment)
        {
            int errorCode = 0;

            var update = _service.GetItemAsNoTracking(id, "EventType");
            update.ActionTaken = comment;
            update.Status = EventStatusEnum.Closed;
            update.UpdatedDate = UserDateTime.GetUserDate();
            update.UpdatedBy = Session["UserId"].ToString();

            var entitySaved = _service.Update(update);
            if (entitySaved)
            {
                errorCode = 1;
                TempData["SuccessMessage"] = "Event has been successfully closed.";

                #region Email

                string userName = Session["UserName"].ToString();
                String emailBody = "";
                EmailTemplate emailTemplate = new EmailTemplate();
                EmailClient emailClient = new EmailClient();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                ToRecipients.Add("itsm.events@laugfs.lk");
                //CCRecipients.Add("viraga.mahesh@laugfs.lk");
                emailBody = emailTemplate.EventClosed(update, userName);
                String res = emailClient.SendEmail(ToRecipients, CCRecipients, "ITSM - Event Closed", emailBody).Trim();

                #endregion

            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }




        [HttpGet]
        [AccessLogin]
        [EncryptedActionParameter]
        public ActionResult Info(long? id)
        {

            if (id == null)
                return HttpNotFound();
            var item = _service.GetItem(id, "EventType,RequestedUser");

            ItemDocService _itemDocService = new ItemDocService();
            item.ItemsDocs = _itemDocService.GetEventDocs(item.EventId, "").ToList();

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }

    }
}
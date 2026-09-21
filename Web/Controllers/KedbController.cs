using Domain;
using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Utility;

namespace Web.Controllers
{
    public class KedbController : Controller
    {
        private KedbItemService _service = new KedbItemService();
        // GET: Kedb

        [AccessAuthorize]
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


        [HttpGet]
        [AccessAuthorize]
        public ActionResult Pending()
        {
            var items = _service.GetByStatus(RequestStatusEnum.Pending,
            "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory,RequestType.Team,Ticket,RequestedUser").ToList();

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }


        [AccessAuthorize]
        [HttpGet]
        public ActionResult Approve(long? id)
        {
            int errorCode = 0;

            var item = _service.GetItem(id, "");
            if (item != null)
            {
                if (item.Status == RequestStatusEnum.Pending)
                {
                    item.Status = RequestStatusEnum.Approved;
                    item.UpdatedDate = UserDateTime.GetUserDate();
                    item.UpdatedBy = Session["UserId"].ToString();
                    _service.Update(item);
                    errorCode = 1;
                }
            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Reject(long? id)
        {
            int errorCode = 0;

            var item = _service.GetItem(id, "");
            if (item != null)
            {
                if (item.Status == RequestStatusEnum.Pending)
                {
                    item.Status = RequestStatusEnum.Rejected;
                    item.UpdatedDate = UserDateTime.GetUserDate();
                    item.UpdatedBy = Session["UserId"].ToString();
                    _service.Update(item);
                    errorCode = 1;
                }
            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }



        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetKEDBItemstJsonResult(long? categoryId,long? assetTypeId,long? subcategoryId)
        {

            var items = _service.GetItems(categoryId, assetTypeId, subcategoryId,"");
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }
    }
}
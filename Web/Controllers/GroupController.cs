using Domain;
using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Utility;

namespace PMSWeb.Controllers
{
    [AccessAuthorize]
    public class GroupController : Controller
    {
        private readonly GroupService _service = new GroupService();
        // GET: GeneralModule/Group

        public ActionResult Index()
        {
            var groups = _service.GetGroups("");
            return View(groups);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "GroupName")] Group item)
        {
            if (ModelState.IsValid)
            {
                item.UpdatedDate = DateTimeService.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();

                if (!_service.IsExistingGroup(item.GroupName))
                {
                    var entitySaved = _service.InsertGroup(item);
                    if (entitySaved)
                    {
                        TempData["SuccessMessage"] = "Group has been sucessfully saved";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Record couldn't update, Please contact the IT support.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Group  is all ready exist, try a different name";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Sorry, something went wrong. Record details couldn't update, Please contact the IT support.";
            }
            return View(item);
        }

        [HttpGet]
        public ActionResult Edit(long? id)
        {
            var group = _service.GetGroup(id, "");
            return View(group);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "GroupId,GroupName")] Group item)
        {
            if (ModelState.IsValid)
            {
                item.UpdatedDate = DateTimeService.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();
                var entityUpdated = _service.UpdateGroup(item);
                if (entityUpdated)
                {
                    TempData["SuccessMessage"] = "Group has been sucessfully updated";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Record couldn't update, Please contact the IT support.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Sorry, something went wrong. Record details couldn't update, Please contact the IT support.";
            }

            return View(item);
        }

        [HttpGet]
        public ActionResult Delete(long id)
        {
            //var isDeleted = false;
            //var group = _service.GetGroup(id, "");
            //if (group != null)
            //{
            //    isDeleted = _service.DeleteGroup(group);
            //}
            //return Json(JsonConvert.SerializeObject(isDeleted), JsonRequestBehavior.AllowGet);

            int errorCode = 0;

            var item = _service.GetGroup(id, "");
            item.UpdatedDate = UserDateTime.GetUserDate();
            item.UpdatedBy = Session["UserId"].ToString();
            if (item != null)
            {
                if (_service.DeleteGroup(item))
                    errorCode = 1;
            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);


        }


        //[HttpGet]
        //public JsonResult GetCompanyListJsonResult()
        //{
        //    var tenantId = Convert.ToInt64(Session["TenantId"]);
        //    var employeeId = Convert.ToInt64(Session["EmployeeId"]);

        //    var accounts = _accountService.GetAccounts(tenantId, employeeId, "");
        //    return Json(JsonConvert.SerializeObject(accounts.ToList(), Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public JsonResult GetAccountListJsonResult()
        //{
        //    var tenantId = Convert.ToInt64(Session["TenantId"]);
        //    var employeeId = Convert.ToInt64(Session["EmployeeId"]);

        //    var accounts = _accountService.GetAccounts(tenantId, employeeId, "");
        //    //return Json(JsonConvert.SerializeObject(accounts.ToList()), JsonRequestBehavior.AllowGet);
        //    return Json(JsonConvert.SerializeObject(accounts.ToList(), Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        //}
    }
}

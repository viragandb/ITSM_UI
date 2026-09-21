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
    public class BranchController : Controller
    {
        private readonly BranchService _service = new BranchService();
        private readonly BranchDepartmentService _branchDeptService = new BranchDepartmentService();
        // GET: Branch
        [AccessAuthorize]
        public ActionResult Index()
        {
            var items = _service.GetAll("Region,DeliverySLA");
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Branch item)
        {
            ViewBag.RegionId = item.RegionId;
            ViewBag.DeliverySLAId = item.DeliverySLAId;
            if (ModelState.IsValid)
            {
                if (item.RegionId == 0 || item.DeliverySLAId == 0)
                {
                    TempData["ErrorMessage"] = "Please enter required data. ";
                    return View("Create", item);
                }
   

                item.UpdatedDate = UserDateTime.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();
                var entitySaved = _service.Insert(item);
                if (entitySaved)
                {
                    TempData["SuccessMessage"] = "Data Saved successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry, something went wrong, Record couldn't save, Please contact the IT support.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Sorry, something went wrong, Record couldn't save, Please contact the IT support.";
            }

            return View(item);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Edit(long? id)
        {
            if (id == null)
                return HttpNotFound();

            var item = _service.GetItem(id, "Region,DeliverySLA");

            if (item == null)
            {
                return HttpNotFound();
            }

            ViewBag.RegionId = item.RegionId;
            ViewBag.DeliverySLAId = item.DeliverySLAId;
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Branch item)
        {
            ViewBag.RegionId = item.RegionId;
            ViewBag.DeliverySLAId = item.DeliverySLAId;

            if (ModelState.IsValid)
            {
                if (item.RegionId == 0)
                {
                    TempData["ErrorMessage"] = "Please enter required data. ";
                    return View("Create", item);
                }
                item.UpdatedDate = UserDateTime.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();
                var entityUpdated = _service.Update(item);
                if (entityUpdated)
                {
                    TempData["SuccessMessage"] = "Data updated successfully";
                    return RedirectToAction("Index");
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
            item.UpdatedDate = UserDateTime.GetUserDate();
            item.UpdatedBy = Session["UserId"].ToString();
            if (item != null)
            {
                if (_service.HasRelationalData(item))
                    errorCode = 2;
                else if (_service.Delete(item))
                    errorCode = 1;
            }
            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }



        [HttpGet]
        [AccessAuthorize]
        public ActionResult BranchDept(long? branchId)
        {
            var items = _branchDeptService.GetItemsByBranchId(branchId, "Branch,Department");
            ViewBag.BranchId = branchId;

            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult AssignDept()
        {
            return View();
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignDept(BranchDepartment item)
        {
            ViewBag.DepartmentId = item.DepartmentId;
            ViewBag.BranchId = item.BranchId;
            if (ModelState.IsValid)
            {
                if (item.DepartmentId == 0 || item.BranchId == 0)
                {
                    TempData["ErrorMessage"] = "Please select the department and branch.";
                    return View(item);

                }
                if (_branchDeptService.IsUpdated(item.DepartmentId, item.BranchId))
                {
                    TempData["ErrorMessage"] = "Department has been alreday assigned.";
                    return View(item);
                }
                item.UpdatedDate = UserDateTime.GetUserDate();
                item.UpdatedBy = Session["UserId"].ToString();
                var entitySaved = _branchDeptService.Insert(item);
                if (entitySaved)
                {
                    TempData["SuccessMessage"] = "Data Saved successfully";
                    return RedirectToAction("BranchDept", new { branchId = item.BranchId });
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry, something went wrong, Record couldn't save, Please contact the IT support.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Sorry, something went wrong, Record couldn't save, Please contact the IT support.";
            }

            return View(item);
        }


        [AccessAuthorize]
        [HttpGet]
        public ActionResult DeleteBranchDept(long? id)
        {
            int errorCode = 0;

            if (_branchDeptService.Delete(id))
                errorCode = 1;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetItemListJsonResult()
        {

            var items = _service.GetAll("").OrderBy(o=>o.Name);
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetItemListUserJsonResult()
        {

            var items = _service.GetItemsByUser(Session["UserId"].ToString(),"").OrderBy(o => o.Name);
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);


        }
    }
}
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
    public class TaskCategoryController : Controller
    {
        private readonly TaskCategoryService _service = new TaskCategoryService();
        // GET: TaskCategory

        [AccessAuthorize]
        public ActionResult Index(long? teamId)
        {
            var items = _service.GetItemsByTeamId(teamId,"");
            ViewBag.TeamId = teamId;
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
        public ActionResult Create([Bind(Include = "TeamId,TaskCategoryName,Weightage")] TaskCategory item)
        {

            if (ModelState.IsValid)
            {
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

            var item = _service.GetItem(id, "Team");
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TaskCategoryId,Team.TeamName,TeamId,TaskCategoryName,Weightage")] TaskCategory item)
        {
            if (ModelState.IsValid)
            {
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
        [AllowAnonymous]
        public JsonResult GetTaskCategoriesListJsonResult(long? teamId)
        {

            var items = _service.GetItemsByTeamId(teamId, "");
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }



    }
}
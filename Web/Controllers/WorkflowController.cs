using Domain;
using log4net;
using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers
{
    public class WorkflowController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AssetController));
        private readonly WorkflowService _service = new WorkflowService();
        // GET: Workflow
        [AccessAuthorize]
        public ActionResult Index()
        {
            var items = _service.GetAll("WorkflowLevels,WorkflowLevels.Team");
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {
            var item = new WorkflowVM();
            Collection<WorkflowLevel> items = new Collection<WorkflowLevel>();
            item.WorkflowLevels = items.ToList();
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(WorkflowVM item)
        {

            //ViewBag.DepartmentId = item.DepartmentId;
            if (item.WorkflowType == 0 | item.Name == null || item.Name == "")
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }

            if (item.WorkflowLevels == null)
            {
                TempData["ErrorMessage"] = "Please add workflow levels. ";
                return View(item);
            }

            Workflow newItem = new Workflow();
            newItem.WorkflowType = item.WorkflowType;
            newItem.Name = item.Name;
            newItem.UpdatedBy = Session["UserId"].ToString();
            newItem.UpdatedDate = UserDateTime.GetUserDate();
            var items = new Collection<WorkflowLevel>();
            foreach (var level in item.WorkflowLevels)
            {
                var _level = new WorkflowLevel();
                _level.LevelNo = level.LevelNo;
                _level.TeamId = level.TeamId;
                _level.WorkflowLevelType = level.WorkflowLevelType;
                _level.UpdatedBy = Session["UserId"].ToString();
                _level.UpdatedDate = UserDateTime.GetUserDate();
                items.Add(_level);
            }
            newItem.WorkflowLevels = items;
            var entitySaved = _service.Insert(newItem);
            if (entitySaved)
            {

                TempData["SuccessMessage"] = "Workflow has been successfully created.";
                return RedirectToAction("Index");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString());
                return View(item);
            }

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Edit(long? id)
        {
            if (id == null)
                return HttpNotFound();

            var workflow = _service.GetAsNoTrackingItem(id, "WorkflowLevels,WorkflowLevels.Team");
            var item = new WorkflowVM();
            item.WorkflowId = workflow.WorkflowId;
            item.Name = workflow.Name;
            item.WorkflowType = workflow.WorkflowType;
            item.WorkflowLevels = workflow.WorkflowLevels.ToList();

            //Collection<WorkflowLevel> items = new Collection<WorkflowLevel>();
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WorkflowVM item)
        {

            //ViewBag.DepartmentId = item.DepartmentId;
            if (item.WorkflowType == 0 | item.Name == null || item.Name == "")
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View(item);
            }

            if (item.WorkflowLevels == null)
            {
                TempData["ErrorMessage"] = "Please add workflow levels. ";
                return View(item);
            }

            Workflow newItem = new Workflow();
            var workflow = _service.GetItem(item.WorkflowId, "");
            workflow.Name = item.Name;
            workflow.UpdatedBy = Session["UserId"].ToString();
            workflow.UpdatedDate = UserDateTime.GetUserDate();
           //workflow.WorkflowLevels.Clear();
            var entitySaved = _service.Update(workflow);
            if (entitySaved)
            {
                WorkflowLevelLevelService _workflowLevelService = new WorkflowLevelLevelService();
                var levels = _workflowLevelService.GetItems(workflow.WorkflowId,"");
                foreach (var level in levels)
                {
                    _workflowLevelService.Delete(level.WorkflowLevelId);
                }
                foreach (var level in item.WorkflowLevels.OrderBy(o => o.LevelNo))
                {
                    var _level = new WorkflowLevel();
                    //var _level = _workflowLevelService.GetItem(workflow.WorkflowId, level.LevelNo, "");
                    //if (_level != null)
                    {
                        _level.WorkflowId = workflow.WorkflowId;
                        _level.LevelNo = level.LevelNo;
                        _level.TeamId = level.TeamId;
                        _level.WorkflowLevelType = level.WorkflowLevelType;
                        _level.UpdatedBy = Session["UserId"].ToString();
                        _level.UpdatedDate = UserDateTime.GetUserDate();
                        _workflowLevelService.Insert(_level);
                    }
                }

                TempData["SuccessMessage"] = "Workflow has been successfully updated.";
                return RedirectToAction("Index");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString());
                return View(item);
            }

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
        public JsonResult GetLevelTypesJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();
            Array values = Enum.GetValues(typeof(WorkflowLevelTypeEnum));

            foreach (WorkflowLevelTypeEnum val in values)
            {

                Console.WriteLine(String.Format("{0}: {1}", Enum.GetName(typeof(WorkflowLevelTypeEnum), val), val));
                EnumStatusVM item = new EnumStatusVM();
                item.Id = Convert.ToInt32(val);
                item.Name = val.EnumDisplayName();
                items.Add(item);
            }

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetItemsAccessRequestJsonResult()
        {

            var items = _service.GetItemsByType(WorkflowTypeEnum.AccessRequest,"").OrderBy(o => o.Name);
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }
    }
}
using Domain;
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
    public class RequestTypeController : Controller
    {
        private readonly RequestTypeService _service = new RequestTypeService();

        [AccessAuthorize]
        public ActionResult Index(long? teamId, long? categoryId, string ticketTypeId)
        {
            int ticketType = 1;
            try
            {
                ticketType = Convert.ToInt32(ticketTypeId);

            }
            catch (Exception ex)
            {
            }
            if (ticketType == 0)
                ticketType = 1;

            if (categoryId == null)
                categoryId = 0;
            ViewBag.TeamId = teamId;
            ViewBag.TicketTypeId = ticketType;
            ViewBag.CategoryId = categoryId;

            var items = _service.GetAll(teamId, categoryId, (TicketTypeEnum)ticketType, "Team,Category,SubCategory,AssetType,RequestTypePriorities");
            return View(items);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {
            var item = new RequestTypeVM();
            Collection<RequestTypePriority> slas = new Collection<RequestTypePriority>();
            Array values = Enum.GetValues(typeof(TicketPriorityEnum));
            foreach (TicketPriorityEnum val in values)
            {
                RequestTypePriority _sla = new RequestTypePriority();
                _sla.Priority = val;
                _sla.Respond = 0;
                _sla.Resolve = 0;
                slas.Add(_sla);
            }
            item.RequestTypePriorities = slas.ToList();

            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RequestTypeVM item)
        {

            if (ModelState.IsValid)
            {
                ViewBag.TeamId = item.TeamId;
                ViewBag.CategoryId = item.CategoryId;
                ViewBag.SubCategoryId = item.SubCategoryId;
                ViewBag.AssetTypeId = item.AssetTypeId;


                if (item.TicketType == 0 || item.TeamId == 0 || item.CategoryId == 0 || item.SubCategoryId == 0
                     || item.AssetTypeId == 0)
                {
                    TempData["ErrorMessage"] = "Please enter required data. ";
                    return View("Create", item);
                }

                int sla = 0;
                if (item.TicketType == TicketTypeEnum.IN)
                {
                    item.Sla = 0;

                    foreach (var priority in item.RequestTypePriorities)
                    {
                        try
                        {
                            sla = Convert.ToInt32(priority.Respond);
                            sla = Convert.ToInt32(priority.Resolve);
                        }
                        catch (Exception ex)
                        {
                            TempData["ErrorMessage"] = "Invalid Respond/Resolve value.";
                            return View(item);

                        }

                        if (priority.Respond <= 0 || priority.Resolve <= 0)
                        {
                            TempData["ErrorMessage"] = "Invalid Respond/Resolve value.";
                            return View(item);
                        }
                    }
                }
                else if (item.TicketType == TicketTypeEnum.SR)
                {
                    if (item.Sla == 0)
                    {
                        TempData["ErrorMessage"] = "Invalid SLA value.";
                        return View(item);
                    }
                    try
                    {
                        sla = Convert.ToInt32(item.Sla);
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Invalid SLA value.";
                        return View(item);

                    }
                }

                AssetTypeService _assetTypeService = new AssetTypeService();
                SubCategoryService _subCategoryService = new SubCategoryService();

                if (_service.ItemAvilable(item.CategoryId, item.AssetTypeId, item.SubCategoryId, ""))
                {
                    TempData["ErrorMessage"] = "Invalid, Request Type already exists.";
                    return View(item);

                }

                var newItem = new RequestType();
                newItem.TicketType = item.TicketType;
                newItem.CategoryId = item.CategoryId;
                newItem.AssetTypeId = item.AssetTypeId;
                newItem.SubCategoryId = item.SubCategoryId;
                newItem.TeamId = item.TeamId;
                newItem.Sla = item.Sla;
                newItem.RequestTypeName = _assetTypeService.GetItem(item.AssetTypeId, "").AssetTypeName + " - " + _subCategoryService.GetItem(item.SubCategoryId, "").SubCategoryName;
                newItem.UpdatedDate = UserDateTime.GetUserDate();
                newItem.UpdatedBy = Session["UserId"].ToString();
                newItem.IsActive = true;
                Collection<RequestTypePriority> requestSLAs = new Collection<RequestTypePriority>();
                if (item.RequestTypePriorities != null)
                {
                    foreach (var priority in item.RequestTypePriorities)
                    {
                        RequestTypePriority requestSLA = new RequestTypePriority();
                        requestSLA.Priority = priority.Priority;
                        requestSLA.Respond = priority.Respond;
                        requestSLA.Resolve = priority.Resolve;
                        requestSLA.UpdatedBy = newItem.UpdatedBy;
                        requestSLA.UpdatedDate = newItem.UpdatedDate;
                        requestSLAs.Add(requestSLA);
                    }
                    newItem.RequestTypePriorities = requestSLAs;
                }
                var entitySaved = _service.Insert(newItem);
                if (entitySaved)
                {
                    TempData["SuccessMessage"] = "Data Saved successfully";
                    return RedirectToAction("Index", new
                    {
                        teamId = item.TeamId,
                        categoryId = item.CategoryId,
                        ticketTypeId = (int)item.TicketType
                    });
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry, something went wrong, Record couldn't save, Please contact the IT support.";
                }
            }
            //else
            //{
            //    TempData["ErrorMessage"] = "Sorry, something went wrong, Record couldn't save, Please contact the IT support.";
            //}

            return View(item);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Edit(long? id)
        {
            if (id == null)
                return HttpNotFound();

            var request = _service.GetItem(id, "Team,Category,SubCategory,AssetType,RequestTypePriorities");
            var item = new RequestTypeVM();
            item.RequestTypeId = request.RequestTypeId;
            item.TicketType = request.TicketType;
            item.CategoryId = request.CategoryId;
            item.Category = request.Category;
            item.SubCategoryId = request.SubCategoryId;
            item.SubCategory = request.SubCategory;
            item.TeamId = request.TeamId;
            item.Team = request.Team;
            item.AssetType = request.AssetType;
            item.AssetTypeId = request.AssetTypeId;
            item.RequestTypeName = request.RequestTypeName;
            item.RequestTypePriorities = request.RequestTypePriorities.ToList();

            item.Sla = request.Sla;

            ViewBag.TeamId = item.TeamId;

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RequestTypeVM item)
        {
            if (ModelState.IsValid)
            {
                ViewBag.TeamId = item.TeamId;

                if (item.TeamId == 0)
                {
                    TempData["ErrorMessage"] = "Please select required data. ";
                    return View("Edit", item);
                }

                int sla = 0;
                if (item.TicketType == TicketTypeEnum.IN)
                {
                    item.Sla = 0;
                    foreach (var priority in item.RequestTypePriorities)
                    {
                        try
                        {
                            sla = Convert.ToInt32(priority.Respond);
                            sla = Convert.ToInt32(priority.Resolve);
                        }
                        catch (Exception ex)
                        {
                            TempData["ErrorMessage"] = "Invalid Respond/Resolve value.";
                            return View(item);

                        }

                        if (priority.Respond <= 0 || priority.Resolve <= 0)
                        {
                            TempData["ErrorMessage"] = "Invalid Respond/Resolve value.";
                            return View(item);
                        }
                    }
                }
                else if (item.TicketType == TicketTypeEnum.SR)
                {
                    if (item.Sla == 0)
                    {
                        TempData["ErrorMessage"] = "Invalid SLA value.";
                        return View(item);
                    }
                    try
                    {
                        sla = Convert.ToInt32(item.Sla);
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Invalid SLA value.";
                        return View(item);

                    }
                }

                var requestType = _service.GetItem(item.RequestTypeId, "Category,SubCategory,AssetType,RequestTypePriorities");
                requestType.Sla = sla;
                requestType.TeamId = item.TeamId;
                requestType.UpdatedDate = UserDateTime.GetUserDate();
                requestType.UpdatedBy = Session["UserId"].ToString();

                if (item.TicketType == TicketTypeEnum.IN)
                {
                    foreach (var p in requestType.RequestTypePriorities)
                    {
                        foreach (var priority in item.RequestTypePriorities)
                        {
                            if (p.Priority == priority.Priority)
                            {
                                p.Respond = priority.Respond;
                                p.Resolve = priority.Resolve;
                                p.UpdatedBy = requestType.UpdatedBy;
                                p.UpdatedDate = requestType.UpdatedDate;
                            }

                        }
                    }
                }
                var entityUpdated = _service.Update(requestType);
                if (entityUpdated)
                {
                    TempData["SuccessMessage"] = "Data updated successfully";
                    return RedirectToAction("Index", new
                    {
                        teamId = item.TeamId,
                        categoryId = item.CategoryId,
                        ticketTypeId = (int)item.TicketType
                    });
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
        public JsonResult GetRequestTypeListJsonResult()
        {

            var items = _service.GetAll("");
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetTicketTypesJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();

            Array values = Enum.GetValues(typeof(TicketTypeEnum));

            foreach (TicketTypeEnum val in values)
            {

                //Console.WriteLine(String.Format("{0}: {1}", Enum.GetName(typeof(TicketTypeEnum), val), val));

                EnumStatusVM item = new EnumStatusVM();
                //item.Id = val;
                item.Id = Convert.ToInt32(val);
                item.Name = GlobalStaticService.EnumDisplayName(val);
                // Enum.GetName(typeof(TicketTypeEnum), val);
                items.Add(item);
            }

            //var items = _service.GetItemsByManager(userId, "");
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetTicketMediumJsonResult()
        {
            Collection<EnumStatusVM> items = new Collection<EnumStatusVM>();
            Array values = Enum.GetValues(typeof(TicketMediumEnum));
            foreach (TicketMediumEnum val in values)
            {
                EnumStatusVM item = new EnumStatusVM();
                item.Id = Convert.ToInt32(val);
                item.Name = GlobalStaticService.EnumDisplayName(val);
                items.Add(item);
            }

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }
    }
}
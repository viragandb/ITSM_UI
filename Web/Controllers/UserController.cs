using Domain;
using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Utility;

namespace IMS.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _service = new UserService();

        [AccessAuthorize]
        public ActionResult Index()
        {

            var items = _service.GetAsNoTrackingAll("Branch");

            return View(items);
        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Create(string empNo)
        {

            User user = new User();
            user.ProImageName = _service.GetProfileImageDefault();

            if (empNo != "" && empNo != null)
            {
                user = _service.GetADUser(empNo);
                if (user.EmpNo != null)
                    return View(user);
                else
                {
                    TempData["ErrorMessage"] = "Invalid Emp.No. ";
                    return View(user);
                }
            }

            return View(user);
        }

        [AccessLogin]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "EmpNoSearch")]
        public ActionResult EmpNoSearch(User item)
        {
            return RedirectToAction("Create", new { empNo = item.EmpNo });
        }
        [AccessLogin]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Save")]
        public ActionResult Save(User item)
        {
            try
            {
                bool isUpdated = false;
                string updatedBy = Session["UserId"].ToString();
                DateTime updatedDate = UserDateTime.GetUserDate();
                ViewBag.BranchId = item.BranchId;
                ViewBag.DepartmentId = item.DepartmentId;

                User user = new User();
                user = _service.GetUserByEmpNo(item.EmpNo, "");
                bool entitySaved = false;
                if (user != null)
                {
                    //if (user.UserStatus ==UserStatusEnum.Inactive)
                    //{
                    TempData["ErrorMessage"] = "User already exists in the system. ";
                    return RedirectToAction("Index");
                    //}
                    //else
                    //{
                    //    user.DesignationName = item.DesignationName;
                    //    user.Email = item.Email;
                    //    user.ContactNo = item.ContactNo;
                    //    user.IsDeleted = false;
                    //    user.UpdatedBy = updatedBy;
                    //    user.UpdatedDate = updatedDate;
                    //    user.UserStatus = UserStatusEnum.Active;
                    //    entitySaved = _service.Update(user);
                    //    isUpdated = true;
                    //}

                }


                if (!isUpdated)
                {
                    user = _service.GetADUser(item.EmpNo);

                    item.FullName = user.FullName;
                    item.ContactNo = user.ContactNo;
                    item.DepartmentName = user.DepartmentName;
                    item.DesignationName = user.DesignationName;
                    item.Email = user.Email;
                    item.UpdatedBy = updatedBy;
                    item.UpdatedDate = updatedDate;
                    item.LastActiveDate = updatedDate;
                    //item.UserId = Convert.ToInt64(item.EmpNo);
                    item.UserStatus = UserStatusEnum.Active;
                    item.ProImageName = _service.GetProfileImageDefault();
                    if (item.BranchId == 0)
                        item.BranchId = null;

                    entitySaved = _service.Insert(item);
                }
                if (entitySaved)
                {
                    TempData["SuccessMessage"] = "User has been successfully created.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                }


            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
            }

            return RedirectToAction("Create", new { empNo = item.EmpNo });


        }

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Edit(string empNo)
        {
            if (empNo == null)
                return HttpNotFound();

            var item = _service.GetUserByEmpNo(empNo, "");
            //ViewBag.UserGroupId = item.UserGroupId;

            ViewBag.BranchId = item.BranchId;
            ViewBag.DepartmentId = item.DepartmentId;
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User item)
        {
            try
            {
                ViewBag.BranchId = item.BranchId;
                ViewBag.DepartmentId = item.DepartmentId;

                if (item.BranchId == 0 || item.DepartmentId == 0 )
                {
                    TempData["ErrorMessage"] = "Please enter required data. ";
                    return View(item);
                }

                string updatedBy = Session["UserId"].ToString();
                DateTime updatedDate = UserDateTime.GetUserDate();

                User user = new Domain.User();
                user = _service.GetUserByEmpNo(item.EmpNo, "");
                user.BranchId = item.BranchId;
                user.DepartmentId = item.DepartmentId;
                user.UpdatedDate = updatedDate;
                user.UpdatedBy = updatedBy;
                // user.UserGroupId = item.UserGroupId;
                user.UserStatus = item.UserStatus;

                var entityUpdated = _service.Update(user);
                if (entityUpdated)
                {
                    TempData["SuccessMessage"] = "User has been sucessfully updated";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Record couldn't update, Please contact the IT support.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Sorry, something went wrong. " + ex.Message.ToString();
            }

            return View(item);
        }



        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetSearchUserByEmailJsonResult(string term)
        {

            Collection<User> items = new Collection<User>();

            ADService adService = new ADService();
            items = adService.GetUsersByEmailSearch(term);


            return Json(items, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetSearchUserByNameJsonResult(string term)
        {

            Collection<User> items = new Collection<User>();

            ADService adService = new ADService();
            items = adService.GetUsersByNameSearch(term);


            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetSearchUserByLanIdJsonResult(string term)
        {

            Collection<User> items = new Collection<User>();

            ADService adService = new ADService();
            items = adService.GetUsersByLanId(term);


            return Json(items, JsonRequestBehavior.AllowGet);
        }

        //[AccessAuthorize]
        //[HttpGet]
        //public ActionResult Delete(long? id)
        //{
        //    int errorCode = 0;
        //    string updatedBy = Session["UserId"].ToString();
        //    DateTime updatedDate = UserDateTime.GetUserDate();

        //    var item = _service.GetItem(id, "");

        //    if (item != null)
        //    {
        //        item.UpdatedBy = updatedBy;
        //        item.UpdatedDate = updatedDate;
        //        item.LastActiveDate = updatedDate;
        //        item.UserStatus = UserStatusEnum.Inactive;

        //        if (_service.HasRelationalData(item))
        //            errorCode = 2;
        //        else if (_service.Delete(item))
        //            errorCode = 1;
        //    }

        //    return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public JsonResult GetUsersAllListJsonResult()
        //{

        //    var items = _service.GetAll("");
        //    return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult GetUsersAllListJsonResult()
        //{
        //    var items = _service.GetAll("");

        //    Collection<User> users = new Collection<User>();
        //    foreach(User u in items)
        //    {
        //        User user = new User();
        //        user.EmpNo = u.EmpNo;
        //        user.FullName = u.FullName;
        //        users.Add(user);
        //    }
        //    return Json(JsonConvert.SerializeObject(users), JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetUsersAllListJsonResult()
        {
            var items = _service.GetAll("").Where(w => w.UserStatus == UserStatusEnum.Active);
            return new LargeJsonActionResult(items);
        }



        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetUsersByTeamIdJsonResult(long? teamId)
        {

            var items = _service.GetUserByTeamId(teamId, "");
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetUsersAppSupportTeamJsonResult()
        {
            TeamService _teamService = new TeamService();
            var items = _service.GetUserByTeamId(_teamService.GetTeamIdAppSupport(), "").ToList();
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetUsersITOppsTeamJsonResult()
        {
            TeamService _teamService = new TeamService();
            var items = _service.GetUserByTeamId(_teamService.GetTeamIdITOperation(), "").ToList();
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetUsersNetworkTeamJsonResult()
        {
            TeamService _teamService = new TeamService();
            var items = _service.GetUserByTeamId(_teamService.GetTeamIdNetwork(), "").ToList();
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetUsersDBAdminTeamJsonResult()
        {
            TeamService _teamService = new TeamService();
            var items = _service.GetUserByTeamId(_teamService.GetTeamIdDBAdmin(), "").ToList();
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetUsersAVPJsonResult()
        {
            TeamService _teamService = new TeamService();
            var items = _service.GetUserByTeamId(_teamService.GetTeamIdAVP(), "").ToList();
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetUsersListbyName(string term)
        {

            var items = _service.GetUsersByName(term, "");
            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [AccessLogin]
        public JsonResult GetADUserJsonResult(string lanId)
        {

            User user = new User();
            user = _service.GetADUser(lanId);
            return Json(JsonConvert.SerializeObject(user), JsonRequestBehavior.AllowGet);

            //return Json(user, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AccessLogin]
        public JsonResult GetSysUserJsonResult(string lanId)
        {

            User user = new User();
            user = _service.GetAsNoTrackingUserByEmpNo(lanId, "");
            //if (user == null)
            //    user = _service.GetADUser(lanId);
            return Json(JsonConvert.SerializeObject(user), JsonRequestBehavior.AllowGet);

            //return Json(user, JsonRequestBehavior.AllowGet);

        }



    }
}
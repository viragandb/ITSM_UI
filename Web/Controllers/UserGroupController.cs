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
    public class UserGroupController : Controller
    {
        private readonly UserGroupService _service = new UserGroupService();

        //public ActionResult Index()
        //{
        //    var items = _service.GetAll("");
        //    return View(items);
        //}

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Create()
        {
            return View();
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(List<UserGroup> userGroups, string empNo, long? companyId)
        {

            if (empNo == null || empNo == "0")
            {
                TempData["ErrorMessage"] = "Please select a User.";
                return View();
            }

            string updatedBy = Session["UserId"].ToString();
            DateTime updatedDate = DateTimeService.GetUserDate();
            try
            {
                foreach (var userGroup in userGroups)
                {

                    userGroup.EmpNo = empNo;
                    if (userGroup.UserGroupId != 0)
                    {

                        if (!userGroup.IsSelected)
                        {
                            userGroup.UpdatedBy = updatedBy;
                            userGroup.UpdatedDate = updatedDate;
                            // userGroup.IsDeleted = true;
                            _service.Delete(userGroup);

                        }
                    }
                    else
                    {
                        if (userGroup.IsSelected)
                        {

                            userGroup.UpdatedBy = updatedBy;
                            userGroup.UpdatedDate = updatedDate;
                            _service.Insert(userGroup);

                        }
                    }
                }

                ViewBag.CompanyId = companyId;
                ViewBag.EmpNo = empNo;

                TempData["SuccessMessage"] = "User Groups have been successfully updated.";

                return RedirectToAction("Create");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
            }
            return View();
        }


        [AccessAuthorize]
        [HttpGet]
        public ActionResult UserAccess( long? branchId, long? departmentId, long? groupId)
        {
          
            if (groupId == null)
                groupId = 0;
            if (branchId == null)
                branchId = 0;
            if (departmentId == null)
                departmentId = 0;

            ViewBag.BranchId = branchId;
            ViewBag.DepartmentId = departmentId;
            ViewBag.GroupId = groupId;

            //AssetTransferRequestService _service = new AssetTransferRequestService();
            var items = _service.GetUserAccess(branchId, departmentId, groupId
                    , "Group,User,User.Branch,User.Department,User.UserBranches,User.UserDepartments,User.UserBranches.Branch,User.UserDepartments.Department");

            return View(items);
           // return View();

        }



        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetEmployeeListJsonResult()
        {
            UserService _userService = new UserService();
            var employees = _userService.GetAll("");
            return Json(JsonConvert.SerializeObject(employees.ToList(), Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetUserGroups(string empNo)
        {
            var userGroups = _service.GetUserGroupsViewModels(empNo);

            return View(userGroups);
        }

    }
}
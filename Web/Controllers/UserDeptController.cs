using Domain;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Utility;

namespace Web.Controllers
{
    public class UserDeptController : Controller
    {
        private readonly UserDepartmentService _service = new UserDepartmentService();
        // GET: UserBranch
      

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Create()
        {
            return View();
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(List<UserDepartment> userDepts, string empNo, long? companyId)
        {

            if (empNo == null || empNo == "0")
            {
                TempData["ErrorMessage"] = "Please select a User.";
                return View();
            }

            string updatedBy = Session["UserId"].ToString();
            DateTime updatedDate = UserDateTime.GetUserDate();
            try
            {
                foreach (var userDept in userDepts)
                {

                    userDept.EmpNo = empNo;
                    if (userDept.UserDepartmentId != 0)
                    {

                        if (!userDept.IsSelected)
                        {
                            userDept.UpdatedBy = updatedBy;
                            userDept.UpdatedDate = updatedDate;
                            // userTeam.IsDeleted = true;
                            _service.Delete(userDept);

                        }
                    }
                    else
                    {
                        if (userDept.IsSelected)
                        {

                            userDept.UpdatedBy = updatedBy;
                            userDept.UpdatedDate = updatedDate;
                            _service.Insert(userDept);

                        }
                    }
                }

                ViewBag.EmpNo = empNo;

                TempData["SuccessMessage"] = "User Departments have been successfully updated.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
            }
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetUserDepts(string empNo)
        {
            var items = _service.GetUserDepartmentsViewModels(empNo);

            return View(items);
        }

    }
}
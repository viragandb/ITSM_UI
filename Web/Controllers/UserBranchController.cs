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
    public class UserBranchController : Controller
    {
        private readonly UserBranchService _service = new UserBranchService();
        // GET: UserBranch
       

        [HttpGet]
        [AccessAuthorize]
        public ActionResult Create()
        {
            return View();
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(List<UserBranch> userBranches, string empNo, long? companyId)
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
                foreach (var userBranch in userBranches)
                {

                    userBranch.EmpNo = empNo;
                    if (userBranch.UserBranchId != 0)
                    {

                        if (!userBranch.IsSelected)
                        {
                            userBranch.UpdatedBy = updatedBy;
                            userBranch.UpdatedDate = updatedDate;
                            // userTeam.IsDeleted = true;
                            _service.Delete(userBranch);

                        }
                    }
                    else
                    {
                        if (userBranch.IsSelected)
                        {

                            userBranch.UpdatedBy = updatedBy;
                            userBranch.UpdatedDate = updatedDate;
                            _service.Insert(userBranch);

                        }
                    }
                }

                ViewBag.CompanyId = companyId;
                ViewBag.EmpNo = empNo;

                TempData["SuccessMessage"] = "User Branches have been successfully updated.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
            }
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetUserBranches(string empNo)
        {
            var userTeams = _service.GetUserBranchesViewModels(empNo);

            return View(userTeams);
        }

    }
}
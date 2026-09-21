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
    public class UserTeamController : Controller
    {
        // GET: UserTeam
        private readonly UserTeamService _service = new UserTeamService();

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
        public ActionResult Create(List<UserTeam> userTeams, string empNo, long? companyId)
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
                foreach (var userTeam in userTeams)
                {

                    userTeam.EmpNo = empNo;
                    if (userTeam.UserTeamId != 0)
                    {

                        if (!userTeam.IsSelected)
                        {
                            userTeam.UpdatedBy = updatedBy;
                            userTeam.UpdatedDate = updatedDate;
                            // userTeam.IsDeleted = true;
                            _service.Delete(userTeam);

                        }
                    }
                    else
                    {
                        if (userTeam.IsSelected)
                        {

                            userTeam.UpdatedBy = updatedBy;
                            userTeam.UpdatedDate = updatedDate;
                            _service.Insert(userTeam);

                        }
                    }
                }

                ViewBag.CompanyId = companyId;
                ViewBag.EmpNo = empNo;

                TempData["SuccessMessage"] = "User Teams have been successfully updated.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
            }
            return View();
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
        public ActionResult GetUserTeams(string empNo)
        {
            var userTeams = _service.GetUserTeamsViewModels(empNo);

            return View(userTeams);
        }
    }
}
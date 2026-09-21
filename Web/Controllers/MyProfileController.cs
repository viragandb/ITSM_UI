using Domain;
using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Utility;

namespace Web.Controllers
{
    public class MyProfileController : Controller
    {
        private readonly UserService _service = new UserService();
        // GET: MyProfile
        [AccessLogin]
        public ActionResult Index()
        {
            var item = _service.GetAsNoTrackingUserByEmpNo(Session["UserId"].ToString(), "");
            ViewBag.BranchId = item.BranchId;
            ViewBag.DepartmentId = item.DepartmentId;
            return View(item);
        }

        [HttpGet]
        [AccessLogin]
        public ActionResult UploadProfile()
        {
            var item = _service.GetAsNoTrackingUserByEmpNo(Session["UserId"].ToString(), "");

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);

        }

        [AccessLogin]
        [HttpPost]
        public async Task<ActionResult> UploadProfile(HttpPostedFileBase itemFile)
        {

            var item = _service.GetUserByEmpNo(Session["UserId"].ToString(), "");
            try
            {

                if (itemFile != null && itemFile.ContentLength != 0)
                {
                    if (itemFile.ContentLength > 2097152) //2MB
                    {
                        TempData["ErrorMessage"] = "Please reduce file size (Max. 2MB). ";
                        return View(item);
                    }
                    if (itemFile.ContentType.ToLower() == "image/jpeg" || itemFile.ContentType.ToLower() == "image/png")
                    {
                        string dirname = "UploadedFiles/ProImg";
                        string _FileName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(itemFile.FileName);
                        string _path = Path.Combine(Server.MapPath("~/" + dirname), _FileName);
                        itemFile.SaveAs(_path);

                        item.ProImageName = "/" + dirname + "/" + _FileName;

                        item.UpdatedBy = Session["UserId"].ToString();
                        item.UpdatedDate = UserDateTime.GetUserDate();
                        _service.Update(item);

                        Session["ProfileImage"] = item.ProImageName;

                        TempData["SuccessMessage"] = "Image has been successfully uploaded.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please upload a image. ";
                        return View(item);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Please upload a document. ";
                    return View(item);
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Sorry, something went wrong, Record couldn't update, Please contact the IT. (" + ex.Message.ToString() + ") ";
                return View(item);
            }


        }


       
    }
}
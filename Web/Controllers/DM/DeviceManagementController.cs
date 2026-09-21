using Domain;
using Domain.DM;
using Service;
using Service.DM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers.DM
{
    public class DeviceManagementController : Controller
    {
        private readonly DeviceManagementRequestService _service = new DeviceManagementRequestService();
        public ActionResult Index()
        {
            return View();
        }


        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create()
        {

            var item = new NewDeviceManagementVM();
            item.RequestedUser = new User();
            item.ExpireDate = UserDateTime.GetUserDateOnly().ToString("dd/MM/yyyy");
            ViewBag.ExpireDate = item.ExpireDate;

            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(NewDeviceManagementVM item)
        {
            var dateExpire = UserDateTime.GetUserDateOnly();


            if (item.Subject == null || item.Description == null
                || item.DurationType == 0 || (item.RequestedBy == null ) || item.ApprovalBy == null)
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View("Create", item);
            }

           
            if (item.DurationType == AssignedTypeEnum.Temporary)
            {
                try
                {
                    dateExpire = UserDateTime.ConvertToAppDateTime(item.ExpireDate);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Please enter valid expire date. ";
                    return View(item);
                }

                if (dateExpire > UserDateTime.GetUserDate().AddMonths(6))
                {
                    TempData["ErrorMessage"] = "Please enter valid expire date. ";
                    return View(item);
                }
            }


            UserService _userService = new UserService();
            User requestedUser = new User();
            if (item.RequestedBy != "" && item.RequestedBy != null)
            {
                requestedUser = _userService.GetUserByEmpNo_UpdateByAD(item.RequestedBy, Session["UserId"].ToString(), UserDateTime.GetUserDate(), "");
                if (requestedUser.EmpNo == null)
                {
                    TempData["ErrorMessage"] = "Invalid User. ";
                    return View("Create", item);
                }

            }

            User approvalUser = new User();
            if (item.ApprovalBy != "" && item.ApprovalBy != null)
            {
                approvalUser = _userService.GetUserByEmpNo_UpdateByAD(item.ApprovalBy, Session["UserId"].ToString(), UserDateTime.GetUserDate(), "");
                //if (approvalUser.EmpNo == null || item.ApprovalBy == Session["UserId"].ToString())
                //{
                //    TempData["ErrorMessage"] = "Invalid Approval User. ";
                //    return View("Create", item);
                //}

            }

            DeviceManagementRequest newItem = new DeviceManagementRequest();
            newItem.UpdatedBy = Session["UserId"].ToString();
            newItem.UpdatedDate = UserDateTime.GetUserDate();
            newItem.RequestedBy = newItem.UpdatedBy;
            newItem.RequestedDate = newItem.UpdatedDate;
            newItem.DeviceManagementType = item.DeviceManagementType;
            newItem.ApprovalBy = item.ApprovalBy;
            newItem.RequestedFor = item.RequestedBy;
            newItem.Subject = item.Subject;
            newItem.Description = item.Description;
            newItem.DurationType = item.DurationType;
            newItem.ExpireDate = dateExpire;

            newItem.Status = DeviceManagementStatusEnum.Pending;
            newItem.SpentTimeSync = newItem.RequestedDate;
            newItem.SpentTime = 0;

            if (item.AssetId > 0)
             newItem.AssetId = item.AssetId;

            var entitySaved = _service.Insert(newItem);
            if (entitySaved)
            {

                _service.UpdateStatus(newItem);
                _service.InsertLog(newItem, "New request initiated");

                //#region Email
                //try
                //{
                //    var emailRequest = _service.GetItemAsNoTracking(newTicket.AccessRequestId, "ApprovalByUser,CreatedUser,AccessRequestType,RequestedUser");
                //    EmailTemplate emailTemplate = new EmailTemplate();
                //    EmailClient emailClient = new EmailClient();
                //    List<string> ToRecipients = new List<string>();
                //    List<string> CCRecipients = new List<string>();
                //    if (!string.IsNullOrEmpty(emailRequest.ApprovalByUser.Email))
                //    {
                //        ToRecipients.Add(emailRequest.ApprovalByUser.Email);
                //        if (!string.IsNullOrEmpty(emailRequest.CreatedUser.Email))
                //            CCRecipients.Add(emailRequest.CreatedUser.Email);

                //        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "New " + emailRequest.AccessRequestType.Name + " request pending approval | Request # " + emailRequest.AccessRequestId,
                //                    emailTemplate.AccessRequestCreate(emailRequest)).Trim();
                //    }
                //}
                //catch (Exception ex) { }
                //#endregion

                TempData["SuccessMessage"] = "Request has been successfully created.";
                return RedirectToAction("MyRequest");

            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View("Create", item);
            }



        }


    }
}
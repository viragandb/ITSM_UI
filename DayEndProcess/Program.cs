using Data;
using Domain;
using Domain.AR;
using Newtonsoft.Json;
using Service;
using Service.RA;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Web.Utility;

namespace DayEndProcess
{
    class Program
    {
        private static UnitOfWork _contextUow = new UnitOfWork();

        // public static DateTime nowDate = UserDateTime.GetUserDateOnly();
        public static DateTime nowDate = UserDateTime.GetUserDate();
        public static DateTime nowDateOnly = UserDateTime.GetUserDateOnly();
        // public static DateTime alertDate = nowDate.AddDays(-1);

        public static string styleRowHeader = "background:#182643;padding:8px;color:white; border: 1px solid #ddd;";
        public static string styleRowRed = "background:#F596B1;padding:8px;color:white; border: 1px solid #ddd;";
        public static string styleRow = "padding:8px; vertical-align: top; border: 1px solid #ddd;";
        public static string styleRowRight = "padding:8px; vertical-align: top;text-align:right; border: 1px solid #ddd;";
        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("ITSM Day End Process " + System.DateTime.Today + "");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("**************************************************");
            Console.WriteLine("**************** DO NOT CLOSE...! ****************");
            Console.WriteLine("**************************************************");

            Console.WriteLine(nowDate);


            #region Live

            // AutoClosedTickets(); //Requested by gange 01/31/2025
            UpdateTicketSLA();
            PendingClosedReminderAlert();
            DailyStatus();
            DailyPending();

            #region Asset Transaction

            DateTime sDate = nowDateOnly;
            DateTime eDate = nowDateOnly;
            string datePeriod = "";
            string emailName = "";
            if (nowDateOnly.Day == 1)
            {
                sDate = nowDateOnly.AddMonths(-1).AddDays(15);
                eDate = nowDateOnly;

                datePeriod = sDate.ToString("dd/MM/yyyy") + " - " + eDate.AddDays(-1).ToString("dd/MM/yyyy");
                emailName = "Bi-Weekly Asset Transaction";
                AssetTransactionEmail(sDate, eDate, datePeriod, emailName);

                AssetSummaryATM_CRMEmail();

            }
            else if (nowDateOnly.Day == 15) 
            {
                sDate = nowDateOnly.AddDays(-14);
                eDate = nowDateOnly.AddDays(1);

                datePeriod = sDate.ToString("dd/MM/yyyy") + " - " + eDate.AddDays(-1).ToString("dd/MM/yyyy");
                emailName = "Bi-Weekly Asset Transaction";
                AssetTransactionEmail(sDate, eDate, datePeriod, emailName);
            }

            #endregion

            #region Asset Acceptance
            InitialAssetAcceptanceReminder();
            FinalAssetAcceptanceReminder();
            #endregion

            #region Remote Access
            ProcessOverdueRevokeConfirmationPending();
            RemoteAccessRevokationReminderTwoWeeksBefore();
            FinalRemoteAccessRevokeReminder();
           
            #endregion

            #endregion

            //TestHolidayAPI();
        }


        //public static void CloseNotRatedTickets()
        //{
        //    TicketService _ticketService = new TicketService();
        //    TicketRateService _rateService = new TicketRateService();
        //    var _tickets = _ticketService.GetNotRatedTickets(nowDate.Date.AddDays(-5), "").ToList();

        //    foreach (Ticket _ticket in _tickets)
        //    {
        //        //Ticket ticket = new Ticket();
        //        //ticket = _ticketService.GetItem(_ticket.Gget)
        //        //ticket.TicketId = log.ItemId;

        //        _ticket.Rate = RateTypeEnum.Rate3;

        //        _ticket.Status = TicketStatusEnum.Rated; ;
        //        _ticket.SpentTime = 0;
        //        _ticket.UpdatedBy = "sysuser";
        //        _ticket.UpdatedDate = nowDate;
        //        _ticketService.UpdateStatus(_ticket);


        //        TicketRate rate = new TicketRate();
        //        rate.Rate = _ticket.Rate;
        //        rate.RateValue = (int)_ticket.Rate;
        //        rate.TicketId = _ticket.TicketId;
        //        rate.UpdatedBy = _ticket.UpdatedBy;
        //        rate.UpdatedDate = _ticket.UpdatedDate;
        //        _rateService.Insert(rate);

        //        Console.WriteLine("*** " + _ticket.Code + " Rated ");

        //    }


        //}

        private static void UpdateTicketSLA()
        {
            TicketStatusService _statusService = new TicketStatusService();
            TicketService _ticketService = new TicketService();
            DateTimeService _dateTimeService = new DateTimeService();
            var slaTickets = _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status < TicketStatusEnum.Completed
            //&& e.SpentTimeSync.Date < nowDate.Date
            , includeProperties: "");
            double spentTime = 0;
            foreach (var ticket in slaTickets)
            {
                spentTime = 0;
                //if (_ticketService.IsTimeCaptureStatus(ticket.Status))
                if (_statusService.GetTimeCapture(ticket.Status, ticket.PendingTeamId, ""))
                {
                    spentTime = _dateTimeService.GetTicketLogTimes(ticket.SpentTimeSync, nowDate);
                    var updateTicket = _ticketService.GetItem(ticket.TicketId, "");

                    updateTicket.SpentTime = updateTicket.SpentTime + spentTime;
                    updateTicket.SpentTimeSync = nowDate;

                    _ticketService.Update(updateTicket);
                    Console.WriteLine(" ****** " + updateTicket.Code + " SLA Updated **** ");
                }
                else
                {
                    Console.WriteLine(" ****** " + ticket.Code + " SLA Not Updated **** ");

                }
            }
        }

        private static void AutoClosedTickets()
        {
            DateTime checkDate = nowDate.AddDays(-5).Date;
            TicketUpdateService _ticketUpdateService = new TicketUpdateService();
            TicketService _ticketService = new TicketService();
            DateTimeService _dateTimeService = new DateTimeService();
            var tickets = _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == TicketStatusEnum.Completed
            && e.UpdatedDate < checkDate
            //&& e.SpentTimeSync.Date < nowDate.Date
            , includeProperties: "");

            foreach (var ticket in tickets)
            {
                var updateTicket = _ticketService.GetItem(ticket.TicketId, "");
                updateTicket.Rate = RateTypeEnum.Rate3;
                updateTicket.Status = TicketStatusEnum.Rated;

                var entitySaved = _ticketService.Update(updateTicket);
                if (entitySaved)
                {
                    updateTicket.UpdatedBy = null;
                    _ticketService.InsertTicketLog(updateTicket, "Ticket - Closed by the system");
                    Console.WriteLine(" ****** " + updateTicket.Code + " Closed by the system ****");
                }

            }

        }

        private static void PendingClosedReminderAlert()
        {
            TicketUpdateService _ticketUpdateService = new TicketUpdateService();
            UserService _userService = new UserService();
            TicketService _ticketService = new TicketService();
            EmailService _emailService = new EmailService();
            DateTimeService _dateTimeService = new DateTimeService();
            var tickets = _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status == TicketStatusEnum.Completed
            //&& e.RequestedBy=="viraga_9584"
            , includeProperties: "RequestType,RequestType.Category,RequestType.AssetType,RequestType.SubCategory");

            foreach (var ticket in tickets)
            {
                var user = _userService.GetAsNoTrackingUserByEmpNo(ticket.RequestedBy, "");
                // var approver = _userService.GetAsNoTrackingUserByEmpNo(ticket.ApprovalBy, "");
                if (!String.IsNullOrEmpty(user.Email))
                {

                    StringBuilder Body = new StringBuilder("");
                    Body.Append("<br><b>Dear " + user.FullName + ",</b><br><br>");
                    Body.Append("The below ticket has been resolved. Please check & close the ticket. We hope that the ticket was resolved to your satisfaction. ");
                    //Body.Append("This will be closed automatically if no action is taken with in 05 days from the date this has been requested to be closed.");

                    Body.Append("<br>");
                    Body.Append("<br>");
                    Body.Append("<b>Ticket # </b>: " + ticket.Code);
                    Body.Append("<br>");
                    Body.Append("<b>Subject </b>: " + ticket.Subject);
                    Body.Append("<br>");
                    Body.Append("<b>Category </b>: " + ticket.RequestType.Category.CategoryName);
                    Body.Append("<br>");
                    Body.Append("<b>Asset Type </b>: " + ticket.RequestType.AssetType.AssetTypeName);
                    Body.Append("<br>");
                    Body.Append("<b>Subcategory </b>: " + ticket.RequestType.SubCategory.SubCategoryName);

                    string actionUrl = GlobalStaticService.GetAppUrl() + "/Login/Auth?nav=" + (int)NavAuthTypeEnum.CompleteRequestedUser + "&key=";

                    Body.Append("<br><br><div style='padding-bottom:20px'><a href='" + actionUrl + "' ");
                    Body.Append(" target='_blank' style='text-decoration:none;padding:6px;border-radius:0.35rem;color:#ffffff;background-color:#009ef7;font-weight:600!important' >");
                    Body.Append(" &nbsp; Rate and Close &nbsp; </a> </div>");

                    List<string> ToRecipients = new List<string>();
                    List<string> CCRecipients = new List<string>();
                    List<string> BCCRecipients = new List<string>();

                    ToRecipients.Add(user.Email);

                    string body = Body.ToString();
                    //BCCRecipients.Add("viraga.hiripitiya@12345.com");

                    string subject = "Reminder To Close | Ticket # " + ticket.Code;

                    string result = _emailService.SendEmailScheduled(ToRecipients, CCRecipients, BCCRecipients, subject, Body.ToString());
                    Console.WriteLine("Response : " + result);
                    Console.WriteLine(" ****** " + subject + " Request Close Email Sent! ****       ");

                }

            }

        }

        public static void DailyStatus()
        {

            bool isSend = false;
            string srtEmail = "";
            TicketService _ticketService = new TicketService();

            srtEmail += "<p style='font-weight:bold!important'>Dear All,</p>";
            //srtEmail += "<p style='text-align:justify'>";
            //srtEmail += "Please find tickets without status updates";
            //srtEmail += "</p>";

            srtEmail += "<p style='text-align:justify'><b>Daily Ticket status summary - " + nowDateOnly.ToShortDateString() + "</b> </p>";

            #region Status wise SUmmary 
            var _ticketsSatus = _contextUow.TicketRepository.GetAsNoTracking(e => e.IsDeleted == false && e.RequestedDate >= nowDateOnly, includeProperties: "Branch,RequestType,RequestType.SubCategory,RequestType.AssetType,RequestedUser,AssignedToUser,ItemAssets.Asset").OrderBy(o => o.Status);

            var statusWise = _ticketsSatus.GroupBy(g => new { g.Status })
              .Select(r => new
              {
                  t_item = r.Key,
                  count = r.Count()
              });

            //srtEmail += "<p style='text-align:justify'><b>Today Ticket status summary </b> </p>";
            srtEmail += " <table width='70%' style='border: 2px solid #182643; border-spacing: 0; border-collapse: collapse;font-family:arial, verdana, sans; font-weight:400;'>";
            srtEmail += " <tr >";
            srtEmail += " <td style='" + styleRowHeader + "' >Ticket Status</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' width='100px' >Count</td> ";
            srtEmail += " </tr>";
            foreach (var r in statusWise)
            {
                srtEmail += " <tr>";
                srtEmail += " <td style='" + styleRow + "'>" + r.t_item.Status.EnumDisplayName() + "</td>";
                srtEmail += " <td style='" + styleRow + "' align='right'>" + r.count + "</td>";
                srtEmail += " </tr>";

            }
            srtEmail += " <tr >";
            srtEmail += " <td style='" + styleRowHeader + "' >Total</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' align='right'>" + _ticketsSatus.Count() + "</td> ";
            srtEmail += " </tr>";
            srtEmail += " <tr >";
            srtEmail += " <td style='" + styleRowHeader + "' >SLA violated</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' align='right'>" + _ticketsSatus.Where(w => w.IsTimeViolated == true).Count() + "</td> ";
            srtEmail += " </tr>";
            srtEmail += "</table>";
            #endregion


            #region Details

            srtEmail += "<p style='text-lign:justify'><strong>Daily Ticket Details</strong> </p>";

            srtEmail += " <table width='100%' style='border: 2px solid #182643; border-spacing: 0; border-collapse: collapse;font-family:arial, verdana, sans; font-weight:400;'>";
            srtEmail += " <tr style='background:#182643;color:#ffffff'>";
            srtEmail += " <td style='" + styleRowHeader + "' width='100px' >Ticket #</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Attended Person</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Location</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Asset Type</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Sub Category</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Description</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Requested By</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Reported Date</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Updated Date</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Spent Hrs</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Asset/s</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>SLA Violation</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Status</td> ";
            srtEmail += " </tr>";

            foreach (Domain.Ticket _ticket in _ticketsSatus)
            {
                isSend = true;
                srtEmail += " <tr>";
                string assignedUserName = "N/A";
                try
                {
                    assignedUserName = _ticket.AssignedToUser.FullName;
                }
                catch (Exception ex) { }

                srtEmail += " <td style='" + styleRow + "'>" + _ticket.Code + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + assignedUserName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.Branch.Name + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.RequestType.AssetType.AssetTypeName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.RequestType.SubCategory.SubCategoryName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.Description + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.RequestedUser.FullName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.RequestedDate + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.UpdatedDate + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.SpentTime + "</td>";


                string assetList = "";
                foreach (var asset in _ticket.ItemAssets)
                {
                    if (assetList != "")
                        assetList += " <br> " + asset.Asset.AssetNo + " " + asset.Asset.AssetName;
                    else
                        assetList = asset.Asset.AssetNo + " " + asset.Asset.AssetName;

                }
                srtEmail += " <td style='" + styleRow + "'>" + assetList + "</td>";

                if (_ticket.IsTimeViolated)
                    srtEmail += " <td style='" + styleRowRed + "'>" + _ticket.IsTimeViolated + "</td>";
                else
                    srtEmail += " <td style='" + styleRow + "'>" + _ticket.IsTimeViolated + "</td>";

                srtEmail += " <td style='" + styleRow + "'>" + _ticket.Status.EnumDisplayName() + "</td>";
                srtEmail += " </tr>";



            }
            srtEmail += "</table>";
            #endregion


            srtEmail += "<br><br>";

            if (isSend)
            {
                EmailService _emailService = new EmailService();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                List<string> BCCRecipients = new List<string>();

                ToRecipients.Add("ITServiceDeskOperations@12345.com");
                BCCRecipients.Add("viraga.hiripitiya@12345.com");
                string subject = "Daily Tickets Status - " + nowDateOnly.ToShortDateString();

                string result = _emailService.SendEmailScheduled(ToRecipients, CCRecipients, BCCRecipients, subject, srtEmail);
                Console.WriteLine("Response : " + result);
                Console.WriteLine(" ****** " + subject + " Email Sent! ****");
            }

        }
        public static void DailyPending()
        {
            bool isSend = false;


            string srtEmail = "";
            TicketService _ticketService = new TicketService();

            srtEmail += "<p style='font-weight:bold!important'>Dear All,</p>";
            //srtEmail += "<p style='text-align:justify'>";
            //srtEmail += "Please find tickets without status updates";
            //srtEmail += "</p>";
            srtEmail += "<p style='text-lign:justify'><strong>Pending Tickets - " + nowDateOnly.ToShortDateString() + "</strong> </p>";

            var _ticketsPending = _ticketService.GetItemsPending("Branch,RequestType,RequestType.SubCategory,RequestType.AssetType,RequestedUser,AssignedToUser,TicketUpdates,ItemAssets.Asset").OrderBy(o => o.AssignedTo);

            #region Status wise SUmmary 

            //srtEmail += "<p style='text-align:justify'><b>Today Ticket status summary </b> </p>";
            srtEmail += " <table width='70%' style='border: 2px solid #182643; border-spacing: 0; border-collapse: collapse;font-family:arial, verdana, sans; font-weight:400;'>";

            srtEmail += " <tr >";
            srtEmail += " <td style='" + styleRowHeader + "' >Total Pending</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' align='right'>" + _ticketsPending.Count() + "</td> ";
            srtEmail += " </tr>";
            srtEmail += " <tr >";
            srtEmail += " <td style='" + styleRowHeader + "' >SLA violated</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' align='right'>" + _ticketsPending.Where(w => w.IsTimeViolated == true).Count() + "</td> ";
            srtEmail += " </tr>";
            srtEmail += " <tr >";
            srtEmail += " <td style='" + styleRowHeader + "' >SLA Not violated</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' align='right'>" + _ticketsPending.Where(w => w.IsTimeViolated == false).Count() + "</td> ";
            srtEmail += " </tr>";
            srtEmail += "</table>";
            #endregion

            #region Pending

            srtEmail += "<p style='text-lign:justify'><strong>Tickets Details</strong> </p>";

            srtEmail += " <table width='100%' style='border: 2px solid #182643; border-spacing: 0; border-collapse: collapse;font-family:arial, verdana, sans; font-weight:400;'>";
            srtEmail += " <tr style='background:#182643;color:#ffffff'>";
            srtEmail += " <td style='" + styleRowHeader + "' width='100px' >Ticket #</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Attended Person</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Location</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Asset Type</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Sub Category</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Description</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Requested By</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Reported Date</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Updated Date</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Spent Hrs</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Asset/s</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>SLA Violation</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Status</td> ";
            srtEmail += " </tr>";

            foreach (Domain.Ticket _ticket in _ticketsPending)
            {
                isSend = true;
                string assignedUserName = "N/A";
                try
                {
                    assignedUserName = _ticket.AssignedToUser.FullName;
                }
                catch (Exception ex) { }
                srtEmail += " <tr>";
                srtEmail += " <td rowspan='2' style='" + styleRow + "'>" + _ticket.Code + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + assignedUserName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.Branch.Name + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.RequestType.AssetType.AssetTypeName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.RequestType.SubCategory.SubCategoryName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.Description + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.RequestedUser.FullName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.RequestedDate + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.UpdatedDate + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _ticket.SpentTime + "</td>";

                string assetList = "";
                foreach (var asset in _ticket.ItemAssets)
                {
                    if (assetList != "")
                        assetList += " <br> " + asset.Asset.AssetNo + " " + asset.Asset.AssetName;
                    else
                        assetList = asset.Asset.AssetNo + " " + asset.Asset.AssetName;

                }
                srtEmail += " <td style='" + styleRow + "'>" + assetList + "</td>";

                if (_ticket.IsTimeViolated)
                    srtEmail += " <td style='" + styleRowRed + "'>" + _ticket.IsTimeViolated + "</td>";
                else
                    srtEmail += " <td style='" + styleRow + "'>" + _ticket.IsTimeViolated + "</td>";

                srtEmail += " <td style='" + styleRow + "'>" + _ticket.Status.EnumDisplayName() + "</td>";
                srtEmail += " </tr>";

                var lastUpdate = _ticket.TicketUpdates.OrderByDescending(o => o.UpdatedDate).FirstOrDefault();
                srtEmail += " <tr>";
                srtEmail += " <td style='" + styleRow + "' colspan='12'>" + lastUpdate.UpdatedBy + " - " + lastUpdate.UpdatedDate.ToString() + "<br>" + lastUpdate.Comment + "</td>";
                srtEmail += " </tr>";
            }

            srtEmail += "</table>";
            #endregion


            srtEmail += "<br><br>";

            if (isSend)
            {
                EmailService _emailService = new EmailService();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                List<string> BCCRecipients = new List<string>();

                ToRecipients.Add("ITServiceDeskOperations@12345.com");
                BCCRecipients.Add("viraga.hiripitiya@12345.com");
                string subject = "Pending Tickets Status - " + nowDateOnly.ToShortDateString();

                string result = _emailService.SendEmailScheduled(ToRecipients, CCRecipients, BCCRecipients, subject, srtEmail);
                Console.WriteLine("Response : " + result);
                Console.WriteLine(" ****** " + subject + " Email Sent! ****");
            }

        }


        public static void AssetTransactionEmail(DateTime sDate, DateTime eDate, string datePeriod, string emailName)
        {
            bool isSend = false;


            string srtEmail = "";

            var assetTrans = _contextUow.TransferItemRepository.GetAsNoTracking(e =>
            //(e.AssetTransferRequest.TransactionType != AssetTransactionTypeEnum.ToBeDisposed)
            (e.AssetTransferRequest.TransactionType == AssetTransactionTypeEnum.Transfer)
            && (e.AssetTransferRequest.AssignedType == AssignedTypeEnum.Permanent)
            && (e.AssetTransferRequest.Status == AssetTransStatusEnum.Completed)
            && e.AssetTransferRequest.InitiatedDate >= sDate && e.AssetTransferRequest.InitiatedDate < eDate
                , includeProperties: "AssetTransferRequest,AssetTransferRequest.InitiatedUser,AssetTransferRequest.BranchFrom,AssetTransferRequest.DepartmentFrom" +
                ",AssetTransferRequest.Branch,AssetTransferRequest.Department,AssignedToUser,Asset,Asset.AssetType");

            srtEmail += "<p style='font-weight:bold!important'>Dear All,</p>";
            srtEmail += "<p style='text-lign:justify'><strong>Please find the " + emailName + " for the period " + datePeriod + "</strong> </p>";

            #region Summary

            //srtEmail += "<p style='text-align:justify'><b>Today Ticket status summary </b> </p>";
            //srtEmail += " <table width='70%' style='border: 2px solid #182643; border-spacing: 0; border-collapse: collapse;font-family:arial, verdana, sans; font-weight:400;'>";

            //srtEmail += " <tr >";
            //srtEmail += " <td style='" + styleRowHeader + "' >Total Pending</td> ";
            //srtEmail += " <td style='" + styleRowHeader + "' align='right'>" + _ticketsPending.Count() + "</td> ";
            //srtEmail += " </tr>";
            //srtEmail += " <tr >";
            //srtEmail += " <td style='" + styleRowHeader + "' >SLA violated</td> ";
            //srtEmail += " <td style='" + styleRowHeader + "' align='right'>" + _ticketsPending.Where(w => w.IsTimeViolated == true).Count() + "</td> ";
            //srtEmail += " </tr>";
            //srtEmail += " <tr >";
            //srtEmail += " <td style='" + styleRowHeader + "' >SLA Not violated</td> ";
            //srtEmail += " <td style='" + styleRowHeader + "' align='right'>" + _ticketsPending.Where(w => w.IsTimeViolated == false).Count() + "</td> ";
            //srtEmail += " </tr>";
            //srtEmail += "</table>";
            #endregion

            #region Items

            //srtEmail += "<p style='text-lign:justify'><strong>Tickets Details</strong> </p>";

            srtEmail += " <table width='100%' style='border: 2px solid #182643; border-spacing: 0; border-collapse: collapse;font-family:arial, verdana, sans; font-weight:400;'>";
            srtEmail += " <tr style='background:#182643;color:#ffffff'>";
            srtEmail += " <td style='" + styleRowHeader + "' width='100px' > #</td> ";
            //srtEmail += " <td style='" + styleRowHeader + "'>Transaction Type</td> ";
            //srtEmail += " <td style='" + styleRowHeader + "' >Asset Type</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Date Transferred</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Fixed Assets Number</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Description</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Serial No</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Unit (From)</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>DAO of the Unit (From)</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Unit (To) </td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>DAO of the Unit (To)</td> ";
            srtEmail += " <td style='" + styleRowHeader + "'>Remarks</td> ";
            srtEmail += " </tr>";

            string DAOCodeFrom = "";
            string DAOCode = "";

            foreach (Domain.TransferItem _item in assetTrans)
            {
                DAOCodeFrom = "";
                DAOCode = "";
                isSend = true;

                if (_item.AssetTransferRequest.DepartmentIdFrom == DepartmentService.GetDeptIdBranch())
                    DAOCodeFrom = _item.AssetTransferRequest.BranchFrom.DAOCode;
                else
                    DAOCodeFrom = _item.AssetTransferRequest.DepartmentFrom.DAOCode;

                if (_item.AssetTransferRequest.DepartmentId == DepartmentService.GetDeptIdBranch())
                    DAOCode = _item.AssetTransferRequest.Branch.DAOCode;
                else
                    DAOCode = _item.AssetTransferRequest.Department.DAOCode;


                srtEmail += " <tr>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.AssetTransferRequest.AssetTransferRequestId + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.AssetTransferRequest.InitiatedDate.ToString("dd/MM/yyyy") + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.Asset.AssetNo + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.Asset.AssetName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.Asset.SerialNo + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.AssetTransferRequest.BranchFrom.Name + " / " + _item.AssetTransferRequest.DepartmentFrom.DepartmentName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + DAOCodeFrom + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.AssetTransferRequest.Branch.Name + " / " + _item.AssetTransferRequest.Department.DepartmentName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + DAOCode + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.AssetTransferRequest.TransactionType.EnumDisplayName() + " - " + _item.Asset.AssetType.AssetTypeName + "</td>";

                //string assetList = "";
                //foreach (var asset in _ticket.ItemAssets)
                //{
                //    if (assetList != "")
                //        assetList += " <br> " + asset.Asset.AssetNo + " " + asset.Asset.AssetName;
                //    else
                //        assetList = asset.Asset.AssetNo + " " + asset.Asset.AssetName;

                //}
                srtEmail += " </tr>";
            }

            srtEmail += "</table>";
            #endregion


            srtEmail += "<br><br>";

            if (isSend)
            {
                EmailService _emailService = new EmailService();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                List<string> BCCRecipients = new List<string>();

                ToRecipients.Add("FinanceFixedAssetsUnit@12345.com");
                CCRecipients.Add("ITAsset.Transactions@12345.com");
                BCCRecipients.Add("viraga.hiripitiya@12345.com");
                string subject = emailName + " | " + datePeriod;

                string result = _emailService.SendEmailScheduled(ToRecipients, CCRecipients, BCCRecipients, subject, srtEmail);
                Console.WriteLine("Response : " + result);
                Console.WriteLine(" ****** " + subject + " Email Sent! ****");
            }

        }

        public static void AssetSummaryATM_CRMEmail()
        {
            bool isSend = false;


            string srtEmail = "";

            var assetTrans = _contextUow.AssetRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.Status != AssetStatusEnum.ToBeDisposed
             && (e.AssetTypeId == 173 || e.AssetTypeId == 174)
                , includeProperties: "AssetType,AssetMake,Branch,Department");

            srtEmail += "<p style='font-weight:bold!important'>Dear All,</p>";
            srtEmail += "<p style='text-lign:justify'><strong>Please find the current ATM/CRM asset list as @ " + nowDateOnly.ToString("dd/MM/yyyy") + "</strong> </p>";

            #region Summary

            srtEmail += "<p style='text-align:justify'><b>Asset Type Summary </b> </p>";
            srtEmail += " <table width='70%' style='border: 2px solid #182643; border-spacing: 0; border-collapse: collapse;font-family:arial, verdana, sans; font-weight:400;'>";
            srtEmail += " <tr >";
            srtEmail += " <td style='" + styleRowHeader + "' >Total</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' align='right'>" + assetTrans.Count() + "</td> ";
            srtEmail += " </tr>";
            srtEmail += " <tr >";
            srtEmail += " <td style='" + styleRowHeader + "' >ATM</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' align='right'>" + assetTrans.Where(w => w.AssetTypeId == 173).Count() + "</td> ";
            srtEmail += " </tr>";
            srtEmail += " <tr >";
            srtEmail += " <td style='" + styleRowHeader + "' >CRM</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' align='right'>" + assetTrans.Where(w => w.AssetTypeId == 174).Count() + "</td> ";
            srtEmail += " </tr>";
            srtEmail += "</table>";
            #endregion

            #region Items

            srtEmail += "<p style='text-lign:justify'><strong>Asset List</strong> </p>";

            srtEmail += " <table width='100%' style='border: 2px solid #182643; border-spacing: 0; border-collapse: collapse;font-family:arial, verdana, sans; font-weight:400;'>";
            srtEmail += " <tr style='background:#182643;color:#ffffff'>";
            srtEmail += " <td style='" + styleRowHeader + "' >Assets #</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Asset Type</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Serial No</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Barcode</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Make</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Model</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Location</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Department</td> ";
            srtEmail += " <td style='" + styleRowHeader + "' >Status</td> ";
            srtEmail += " </tr>";

            foreach (Domain.Asset _item in assetTrans)
            {
                isSend = true;

                srtEmail += " <tr>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.AssetNo + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.AssetType.AssetTypeName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.SerialNo + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.Barcode + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.AssetMake.Name + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.ModelName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.Branch.Name + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.Department.DepartmentName + "</td>";
                srtEmail += " <td style='" + styleRow + "'>" + _item.Status.EnumDisplayName() + "</td>";

                srtEmail += " </tr>";
            }

            srtEmail += "</table>";
            #endregion


            srtEmail += "<br><br>";

            if (isSend)
            {
                EmailService _emailService = new EmailService();
                List<string> ToRecipients = new List<string>();
                List<string> CCRecipients = new List<string>();
                List<string> BCCRecipients = new List<string>();

                ToRecipients.Add("FinanceFixedAssetsUnit@12345.com");
                CCRecipients.Add("ITAsset.Transactions@12345.com");
                BCCRecipients.Add("viraga.hiripitiya@12345.com");
                string subject = "Monthly ATM/CRM Asset Summary as @ " + nowDateOnly.ToString("dd/MM/yyyy");

                string result = _emailService.SendEmailScheduled(ToRecipients, CCRecipients, BCCRecipients, subject, srtEmail);
                Console.WriteLine("Response : " + result);
                Console.WriteLine(" ****** " + subject + " Email Sent! ****");
            }

        }

        public static void TestHolidayAPI()
        {
            Console.WriteLine("TestHolidayAPI");
            HolidayService _holidayService = new HolidayService();

            var date = UserDateTime.GetUserDateOnly().AddDays(-6);
            Console.WriteLine(date.ToShortDateString());
                Console.WriteLine(_holidayService.IsHoliday(date.Date));
                Console.WriteLine(_holidayService.IsHoliday(date.Date.AddDays(1)));
                Console.WriteLine(_holidayService.IsHoliday(date.Date.AddDays(2)));
            Console.WriteLine("");
            Console.ReadLine();
        }

        //public static double GetTicketLogTimes2(DateTime startDateTime, DateTime endDateTime)
        //{
        //    string startTime = "08:30:00";
        //    string endTime = "17:00:00";
        //    double totMint = 0;
        //    string erroLog = "";
        //    erroLog += "*****";
        //    erroLog += "" + startDateTime.ToString() + " - " + endDateTime.ToString();
        //    TimeSpan s = endDateTime - startDateTime;

        //    try
        //    {

        //        DateTime taskStarted = Convert.ToDateTime(startDateTime);
        //        DateTime taskCompleted = Convert.ToDateTime(endDateTime);

        //        DateTime date = taskStarted.Date;
        //        DateTime datePro = taskStarted.Date;

        //        for (int i = 0; date.AddDays(i) <= taskCompleted.Date; i++)
        //        {
        //            Console.WriteLine("** " +datePro.ToString());

        //            datePro = date.AddDays(i);
        //            erroLog += " datePro " + datePro.ToString();
        //            if (!IsNotWorkingday(datePro))
        //            {
        //                Console.WriteLine("WD");

        //                DateTime start = Convert.ToDateTime(datePro.ToString("dd/MM/yyyy") + " " + startTime, new CultureInfo("en-GB"));
        //                DateTime end = Convert.ToDateTime(datePro.ToString("dd/MM/yyyy") + " " + endTime, new CultureInfo("en-GB"));
        //                erroLog += " start " + start.ToString();
        //                erroLog += " end " + end.ToString();

        //                if (i == 0)
        //                {
        //                    if (taskStarted.Date == taskCompleted.Date)
        //                    {
        //                        if (taskCompleted <= end)
        //                            totMint += GetDifMinutes(taskCompleted, taskStarted);
        //                        else
        //                            totMint += GetDifMinutes(end, taskStarted);
        //                    }
        //                    else
        //                        totMint += GetDifMinutes(end, taskStarted);
        //                }
        //                else
        //                {
        //                    if (datePro == taskCompleted.Date)
        //                        totMint += GetDifMinutes(taskCompleted, start);
        //                    else
        //                    {
        //                        totMint += GetDifMinutes(end, start);
        //                    }

        //                }

        //            }

        //            Console.WriteLine("Mint " + totMint);

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        //Log.Error("GetTicketLogTimes | " + startDateTime.ToString() + " | " + endDateTime.ToString(), ex);

        //    }


        //    if (totMint > 0)
        //    {
        //        totMint = totMint / 60;
        //        totMint = Math.Round(totMint, 2);

        //    }
        //    Console.WriteLine("Tot Mint " + totMint);
        //    return totMint;
        //}

        //public static bool IsNotWorkingday(DateTime date)
        //{
        //    bool res = false;

        //    if (date.DayOfWeek == DayOfWeek.Saturday)
        //        return true;
        //    else if (date.DayOfWeek == DayOfWeek.Sunday)
        //        return true;
        //    else
        //    {
        //        Console.WriteLine(date.ToString());

        //        try
        //        {
        //            HolidaySQLService _holidatService = new HolidaySQLService();
        //            int year = date.Year;
        //            int month = date.Month;
        //            int day = date.Day;
        //            if (_holidatService.IsHoliday(year,month,day))
        //            {
        //                return true;
        //            }
        //            ////using (HttpClient client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
        //            ////using (HttpClientHandler handler = new HttpClientHandler { Credentials = CredentialCache.DefaultNetworkCredentials })
        //            ////using (HttpClientHandler handler = new HttpClientHandler())
        //            //{
        //            //    // handler.Credentials = new NetworkCredential("AP-BCAPIUser", "!@aipBC2023QA%#LKR", null); // No domain

        //            //    //handler.Credentials = new NetworkCredential("your-username", "your-password", "your-domain");
        //            //    //handler.UseDefaultCredentials = true; //

        //            //    var credentials = new NetworkCredential("AP-BCAPIUser", "!@aipBC2023QA %#LKR");
        //            //    //using (HttpClient client = new HttpClient(handler))
        //            //    using (var client = new HttpClient(new HttpClientHandler() { Credentials = credentials }))
        //            //    {
        //            //        Console.WriteLine("2");

        //            //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;

        //            //        client.DefaultRequestHeaders.Accept.Clear();
        //            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            //        client.DefaultRequestHeaders.Add("AccessToken", "CSRM-2023-BC-5482");

        //            //        int year = date.Year;
        //            //        int month = date.Month;
        //            //        int day = date.Day;

        //            //        string url = $"https://csrm.ndblk.int/api/holiday/IsHoliday?year={year}&month={month}&day={day}";
        //            //        Console.WriteLine("3");

        //            //        HttpResponseMessage response = client.GetAsync(url).Result;
        //            //        Console.WriteLine(response.StatusCode);

        //            //        if (response.IsSuccessStatusCode)
        //            //        {
        //            //            Console.WriteLine("4");

        //            //            string responseBody = response.Content.ReadAsStringAsync().Result;
        //            //            var result = JsonConvert.DeserializeObject<RequestResult>(responseBody);
        //            //            // Access the values of Code and Message
        //            //            Console.WriteLine(result.Code);

        //            //            if (result.Code == 1)
        //            //                return true;
        //            //            else
        //            //                return false;

        //            //            //int code = result.Code;
        //            //            //string message = result.Message;


        //            //        }
        //            //        else
        //            //        {
        //            //            Console.WriteLine("5");

        //            //        }
        //            //    }
        //            //}
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message.ToString());
        //        }
        //        //HolidayService _holidayService = new HolidayService();
        //        //string dd = _holidayService.IsHoliday2(date.Date);
        //        //Console.WriteLine(dd);

        //        //if (dd == "1")
        //        //    return true;
        //        //else if (dd == "0")
        //        //    return false;
        //        //else
        //        //{
        //        //    Console.WriteLine(dd);
        //        //    return false;
        //        //}


        //    }

        //    return res;
        //}

        //public static double GetDifMinutes(DateTime a, DateTime b)
        //{
        //    double res = 0;
        //    TimeSpan dif = new TimeSpan();
        //    dif = a - b;
        //    res = dif.TotalMinutes;
        //    if (res < 0)
        //        res = 0;
        //    return res;
        //}

        //class RequestResult
        //{
        //    public int Code { get; set; }
        //    public string Message { get; set; }
        //}


        #region Remote access reminder emails
        // process for sending reminder email 2 weeks before end date / expiry date of remote access request
        public static void RemoteAccessRevokationReminderTwoWeeksBefore()
        {
            DateTime targetDate = DateTimeService.GetUserDate().AddDays(14);
            DateTime startTime = nowDate;
            int count = 0;
            int failedCount = 0;
            Console.WriteLine(string.Format("[{0:HH:mm:ss}] Initiating Remote Access 2 weeks before Expiry Alert process...", startTime));

            RemoteAccessRequestService remoteAccessRequestService = new RemoteAccessRequestService();

            var includes = "RequestedUser,ApprovedAVPOrVPUser,ApprovedSupervisorUser";
            var statusFilter = new[] {
                AccessRequestStatusEnum.Completed,
                AccessRequestStatusEnum.RevokeConfirmedContinue
            };
            var ExpiringRequests = remoteAccessRequestService.GetExpiringRequests(targetDate, includes, statusFilter);

            string pattern = @"\b(assistant\s*vice\s*president|asst\.?\s*vp|avp|vp|vice\s*president)\b";
            string networkEmail = remoteAccessRequestService.GetNetworkImplementerTeamEmail();
            string securityEmail = remoteAccessRequestService.GetSecurityImplementerTeamEmail();

            foreach (var request in ExpiringRequests)
            {
                

                try
                {
                    var RequesterDesignation = (request.RequestedUser.DesignationName ?? "").ToLower().Trim();
                    bool IsRequesterAvpVp = Regex.IsMatch(RequesterDesignation, pattern, RegexOptions.IgnoreCase);

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string> { request.RequestedUser.Email };
                    List<string> CCRecipients = new List<string> { networkEmail, securityEmail };

                    // Determine supervisor email cleanly without copying the whole block
                    if (IsRequesterAvpVp && request.ApprovedAVPOrVPUser != null)
                    {
                        ToRecipients.Add(request.ApprovedAVPOrVPUser.Email);
                    }
                    else if (request.ApprovedSupervisorUser != null)
                    {
                        ToRecipients.Add(request.ApprovedSupervisorUser.Email);
                    }

                    emailClient.SendEmail(ToRecipients, CCRecipients, "Reminder Remote Work Access Revocation Notice - " + request.RemoteAccessRequestId,
                                emailTemplate.RemoteRevokationTwoWeeksBefore(request));

                    count++;
                }
                catch (Exception ex)
                {
                    failedCount++;
                    Console.WriteLine($"[ERROR] Failed reminder for Request {request.RemoteAccessRequestId}: {ex.Message}");
                }
            }

            DateTime endTime = nowDate;
            double totalSeconds = (endTime - startTime).TotalSeconds;
            Console.WriteLine(string.Format("[{0:HH:mm:ss}] Process completed in {1:F2} seconds. Total notifications sent: {2}. Total Failed: {3}.", endTime, totalSeconds, count, failedCount));
        }


        // process for sending final reminder email for remote access expiry
        public static void FinalRemoteAccessRevokeReminder()
        {
            DateTime targetDate = DateTimeService.GetUserDate();
            DateTime startTime = nowDate;
            int count = 0;
            int failedCount = 0;
            Console.WriteLine(string.Format("[{0:HH:mm:ss}] Initiating Remote Access Final Expiry Alert process...", startTime));

            // get request expired on today
            RemoteAccessRequestService remoteAccessRequestService = new RemoteAccessRequestService();
            var dateTimeService = new DateTimeService();

            var includes = "RequestedUser,ApprovedAVPOrVPUser,ApprovedSupervisorUser";
            var statusFilter = new[] {
                AccessRequestStatusEnum.Completed,
                AccessRequestStatusEnum.ToBeRevoked,
                AccessRequestStatusEnum.RevokeConfirmedContinue
            };
            var ExpiredRequests = remoteAccessRequestService.GetExpiringRequests(targetDate, includes, statusFilter); ;
            string pattern = @"\b(assistant\s*vice\s*president|asst\.?\s*vp|avp|vp|vice\s*president)\b";
            string networkEmail = remoteAccessRequestService.GetNetworkImplementerTeamEmail();
            string securityEmail = remoteAccessRequestService.GetSecurityImplementerTeamEmail();
            string systemUser = "sysuser";
            foreach (var request in ExpiredRequests)
            {
                try
                {

                    var lastUpdated = request.UpdatedDate;
                    var currentDateTime = UserDateTime.GetUserDate();

                    double spentTimeStatus = dateTimeService.GetTicketLogTimes(lastUpdated, currentDateTime);

                    // 1. Update status flag safely
                    request.Status = Domain.AR.AccessRequestStatusEnum.Expired;
                    request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                    request.SpentTimeStatus = request.SpentTime;
                    request.SpentTimeSync = currentDateTime;
                    request.UpdatedDate = currentDateTime;
                    request.UpdatedBy = systemUser;
                    request.ApprovalStage = Domain.RA.RemoteAccessApprovalStageEnum.ExpiredStage;

                    // 1. Insert Update Status
                    bool isStatusInserted = remoteAccessRequestService.InsertUpdateStatus(
                        request.RemoteAccessRequestId,
                        null,
                        Domain.AR.AccessRequestStatusEnum.Expired,
                        $"Request #{request.RemoteAccessRequestId} marked as expired by system.", 
                        request.SpentTime,
                        false,
                        systemUser,
                        currentDateTime
                    );

                    if (!isStatusInserted)
                    {
                        throw new Exception("Failed to insert update status for expired request.");
                    }

                    // 2. Update Main Request Record
                    bool isRequestUpdated = remoteAccessRequestService.Update(request);
                    if (!isRequestUpdated)
                    {
                        throw new Exception("Failed to update main remote access request for expired request.");
                    }

                    // 3. Insert Log
                    bool isLogInserted = remoteAccessRequestService.InsertLog(
                        request.RemoteAccessRequestId,
                        Domain.AR.AccessRequestStatusEnum.Expired,
                        $"Request #{request.RemoteAccessRequestId} marked as expired by system.",
                        false,
                        systemUser,
                        currentDateTime
                    );

                    if (!isLogInserted)
                    {
                        throw new Exception($"Failed to insert the request log for expired request.");
                    }

                    var RequesterDesignation = (request.RequestedUser.DesignationName ?? "").ToLower().Trim();
                    bool IsRequesterAvpVp = Regex.IsMatch(RequesterDesignation, pattern, RegexOptions.IgnoreCase);

                    EmailTemplate emailTemplate = new EmailTemplate();
                    EmailClient emailClient = new EmailClient();
                    List<string> ToRecipients = new List<string> { request.RequestedUser.Email };
                    List<string> CCRecipients = new List<string> { networkEmail, securityEmail };

                    if (IsRequesterAvpVp && request.ApprovedAVPOrVPUser != null)
                    {
                        ToRecipients.Add(request.ApprovedAVPOrVPUser.Email);
                    }
                    else if (request.ApprovedSupervisorUser != null)
                    {
                        ToRecipients.Add(request.ApprovedSupervisorUser.Email);
                    }

                    emailClient.SendEmail(ToRecipients, CCRecipients, "Final Remote Work Access Revocation Notice - " + request.RemoteAccessRequestId,
                                emailTemplate.RemoteRevokationFinal(request));

                    count++;
                    Console.WriteLine($"Successfully processed Request ID: {request.RemoteAccessRequestId}");

                }
                catch (Exception ex)
                {
                    failedCount++;
                    Console.WriteLine($"[ERROR] Failed expiring Request {request.RemoteAccessRequestId}: {ex.Message}");
                }

            }

            DateTime endTime = nowDate;
            double totalSeconds = (endTime - startTime).TotalSeconds;

            Console.WriteLine(string.Format("[{0:HH:mm:ss}] Process completed in {1:F2} seconds. Total notifications updated & sent: {2}. Total Failed: {3}", endTime, totalSeconds, count, failedCount));
        }


        // marks revoke confirmation pending request as to be revoked if no reply
        public static void ProcessOverdueRevokeConfirmationPending()
        {
            var remoteAccessRequestService = new RemoteAccessRequestService();
            var dateTimeService = new DateTimeService();

            var results = remoteAccessRequestService.GetItemsPendingRevokeConfirmationPastTwoWeeks("");

            int successCount = 0;
            int failureCount = 0;

            
            string systemUser = "sysuser";
            Console.WriteLine(string.Format("[{0:HH:mm:ss}] Initiating Remote Access process to mark request as to be revoked...", nowDate));

            foreach (var request in results)
            {
                try
                {
                    var lastUpdated = request.UpdatedDate;
                    var currentDateTime = UserDateTime.GetUserDate();

                    double spentTimeStatus = dateTimeService.GetTicketLogTimes(lastUpdated, currentDateTime);


                    request.Status = Domain.AR.AccessRequestStatusEnum.ToBeRevoked;
                    request.SpentTime = request.SpentTimeStatus + spentTimeStatus;
                    request.SpentTimeStatus = request.SpentTime;
                    request.SpentTimeSync = currentDateTime;
                    request.UpdatedDate = currentDateTime;
                    request.UpdatedBy = systemUser;

                    // 1. Insert Update Status
                    bool isStatusInserted = remoteAccessRequestService.InsertUpdateStatus(
                        request.RemoteAccessRequestId,
                        null,
                        Domain.AR.AccessRequestStatusEnum.ToBeRevoked,
                        $"Request #{request.RemoteAccessRequestId} marked as to be revoked by system.", request.SpentTime,
                        false,
                        systemUser,
                        currentDateTime
                    );

                    if (!isStatusInserted)
                    {
                        throw new Exception("Failed to insert update status for to be revoked request.");
                    }

                    // 2. Update Main Request Record
                    bool isRequestUpdated = remoteAccessRequestService.Update(request);
                    if (!isRequestUpdated)
                    {
                        throw new Exception("Failed to update main remote access request for to be revoked request.");
                    }

                    // 3. Insert Log
                    bool isLogInserted = remoteAccessRequestService.InsertLog(
                        request.RemoteAccessRequestId,
                        Domain.AR.AccessRequestStatusEnum.ToBeRevoked,
                        $"Request #{request.RemoteAccessRequestId} marked as to be revoked by system.",
                        false,
                        systemUser,
                        currentDateTime
                    );

                    if (!isLogInserted)
                    {
                        throw new Exception($"Failed to insert the request log for to be revoked request.");
                    }

                    successCount++;
                    Console.WriteLine($"Successfully processed Request ID: {request.RemoteAccessRequestId}");
                }
                catch (Exception ex)
                {
                    failureCount++;
                   
                    Console.WriteLine($"Error processing Request ID {request.RemoteAccessRequestId}: {ex.Message}");
                }
            }

            // Final execution summary
            Console.WriteLine($"\nProcess Completed. Success: {successCount}, Failures: {failureCount}");
        }


        #endregion











        #region Asset Acceptance Reminder Emails

        // function to send initial asset acceptance reminder 2 days after assigning from intiated date
        public static void InitialAssetAcceptanceReminder()
        {

            AssetTransferRequestService _assetTransferService = new AssetTransferRequestService();

            DateTime targetDate = DateTime.Now.AddDays(-2);

            // get all transfer requests that are assigned
            var AssetTransferRequests = _assetTransferService.GetNotifyingAssignedTransferRequests(targetDate, "InitiatedUser,AuthorizedUser,CompletedUser" +
                    ",BranchFrom,DepartmentFrom,Branch,Department,AssignedToUser,TransferItems,TransferItems.AssignedToUser,TransferItems.Asset" +
                    ",TransferItems.Asset.AssetType,TransferItems.Asset.AssetMake").ToList();


            foreach (var request in AssetTransferRequests)
            {                

                foreach (var item in request.TransferItems)
                {
                    try
                    {

                       

                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();

                        // TO: assigned user
                        ToRecipients.Add(request.AssignedToUser.Email);
                        

                        // CC: helpdesk
                        CCRecipients.Add("it.helpdesk@12345.com");
                        

                        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Reminder: Pending IT Asset Acceptance – Action Required #" + request.AssetTransferRequestId,
                                    emailTemplate.InitialAssetAssignedReminder(request.InitiatedDate, request.AssignedToUser.FullName, item.Asset )).Trim();
                    }
                    catch (Exception ex) { }


                }
            }


        }

        // function to send final asset acceptance reminder 
        public static void FinalAssetAcceptanceReminder()
        {

            AssetTransferRequestService _assetTransferService = new AssetTransferRequestService();
            AssetService assetService = new AssetService();
            AssetLogService assetLogService = new AssetLogService();

            DateTime targetDate = DateTime.Now.AddDays(-4);

            // get all transfer requests that are assigned
            var AssetTransferRequests = _assetTransferService.GetNotifyingAssignedTransferRequests(targetDate, "InitiatedUser,AuthorizedUser,CompletedUser" +
                    ",BranchFrom,DepartmentFrom,Branch,Department,AssignedToUser,TransferItems,TransferItems.AssignedToUser,TransferItems.Asset" +
                    ",TransferItems.Asset.AssetType,TransferItems.Asset.AssetMake").ToList();


            foreach (var request in AssetTransferRequests)
            {                

                foreach (var item in request.TransferItems)
                {
                    try
                    {

                        var asset = assetService.GetItem(item.Asset.AssetId,"");

                        if (asset != null)
                        {

                            asset.Status = AssetStatusEnum.Assigned;
                            asset.UpdatedBy = "sysuser";
                            asset.UpdatedDate = UserDateTime.GetUserDate();

                            if (!assetService.Update(asset))
                            {
                                throw new Exception("Failed to update asset status to assigned.");
                            }

                            var assetLog = new AssetLog
                            {
                                AssetId = asset.AssetId,
                                Status = AssetStatusEnum.Assigned,
                                Comment = "Asset automatically assigned to user by system.",
                                IsDeleted = false,
                                UpdatedBy = "sysuser",
                                UpdatedDate = UserDateTime.GetUserDate()
                            };

                            if (!assetLogService.Insert(assetLog))
                            {
                                throw new Exception("Failed to insert asset log for assigned asset.");
                            }

                        }

                        EmailTemplate emailTemplate = new EmailTemplate();
                        EmailClient emailClient = new EmailClient();
                        List<string> ToRecipients = new List<string>();
                        List<string> CCRecipients = new List<string>();

                        // TO: assigned user
                        ToRecipients.Add(request.AssignedToUser.Email);
                        

                        // CC: email group
                        CCRecipients.Add("it.helpdesk@12345.com");
                        

                        String res = emailClient.SendEmail(ToRecipients, CCRecipients, "Final Notice: IT Asset Auto-Assigned - " + request.AssetTransferRequestId,
                                    emailTemplate.FinalAssetAssignedReminder(request.InitiatedDate, request.AssignedToUser.FullName, item.Asset )).Trim();
                    }
                    catch (Exception ex) { }


                }
            }


        }
        #endregion



    }

}

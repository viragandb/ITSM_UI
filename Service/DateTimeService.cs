using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Service
{
    public class DateTimeService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Controller));
        private string startTime = "08:30:00";
        private string endTime = "17:00:00";
        //private string endTimeSatDay = "14:00:00";
        public static DateTime GetUserDate()
        {
            //return DateTime.UtcNow;
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Sri Lanka Standard Time");
        }

        public static DateTime GetUserDateOnly()
        {
            //return DateTime.UtcNow;
            //return DateTime.Today;
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Today, "Sri Lanka Standard Time");
        }

        public static void ConvertDateTimeFormat()
        {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = culture;
        }
      
        public static DateTime ConvertStringToDateTime(string dobSrt)
        {
            ConvertDateTimeFormat();
            var dob = Convert.ToDateTime(dobSrt);
            return dob;
        }
        public static DateTime ConvertCustomerDOBToDate(string dobSrt)
        {
            ConvertDateTimeFormat();
            var dob = Convert.ToDateTime(dobSrt.Substring(6, 2) + "/" + dobSrt.Substring(4, 2) + "/" + dobSrt.Substring(0, 4));
            return dob;
        }

        public static string ConvertCustomerDOBToString(DateTime dob)
        {
            var dobSrt = dob.ToString("yyyyMMdd");
            return dobSrt;
        }
        public static int GetTicketOpenTime()
        {
            return 30;
        }

        public static DateTime GetTicketOpenCutoffDate()
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Sri Lanka Standard Time").AddMinutes(GetTicketOpenTime());
        }
        public double GetTicketLogTimes(DateTime startDateTime, DateTime endDateTime)
        {
            //Date Time Format to (dd/MM/yyyy)
            ConvertDateTimeFormat();

            double totMint = 0;
            string erroLog = "";
            erroLog += "*****";
            erroLog += "" + startDateTime.ToString() + " - " + endDateTime.ToString();
            TimeSpan s = endDateTime - startDateTime;

            try
            {

                DateTime taskStarted = Convert.ToDateTime(startDateTime);
                DateTime taskCompleted = Convert.ToDateTime(endDateTime);

                DateTime date = taskStarted.Date;
                DateTime datePro = taskStarted.Date;

                for (int i = 0; date.AddDays(i) <= taskCompleted.Date; i++)
                {
                    datePro = date.AddDays(i);
                    erroLog += " datePro " + datePro.ToString();
                    if (!IsNotWorkingday(datePro))
                    {
                        DateTime start = Convert.ToDateTime(datePro.ToString("dd/MM/yyyy") + " " + startTime, new CultureInfo("en-GB"));
                        DateTime end = Convert.ToDateTime(datePro.ToString("dd/MM/yyyy") + " " + endTime, new CultureInfo("en-GB"));
                        erroLog += " start " + start.ToString();
                        erroLog += " end " + end.ToString();

                        if (i == 0)
                        {
                            if (taskStarted.Date == taskCompleted.Date)
                            {
                                if (taskCompleted <= end)
                                    totMint += GetDifMinutes(taskCompleted, taskStarted);
                                else
                                    totMint += GetDifMinutes(end, taskStarted);
                            }
                            else
                                totMint += GetDifMinutes(end, taskStarted);
                        }
                        else
                        {
                            if (datePro == taskCompleted.Date)
                                totMint += GetDifMinutes(taskCompleted, start);
                            else
                            {
                                totMint += GetDifMinutes(end, start);
                            }

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                //erroLog += " error " + ex.Message.ToString();
                //erroLog += " StackTrace " + ex.StackTrace.ToString();

                //File.AppendAllText(@"D:\Error\error.txt", erroLog + Environment.NewLine);
                Log.Error("GetTicketLogTimes | " + startDateTime.ToString() + " | " + endDateTime.ToString() , ex);

            }

            //File.AppendAllText(@"D:\Error\error.txt", erroLog + Environment.NewLine);

            if (totMint > 0)
            {
                totMint = totMint / 60;
                totMint = Math.Round(totMint, 2);

            }
            return totMint;
        }
      
        public double GetTicketLogTimesDays(DateTime startDateTime, DateTime endDateTime)
        {
            //Date Time Format to (dd/MM/yyyy)
            // ConvertDateTimeFormat();

            double totMint = 0;
            string erroLog = "";
            erroLog += "*****";
            erroLog += "" + startDateTime.ToString() + " - " + endDateTime.ToString();
            // TimeSpan s = endDateTime - startDateTime;
            double spentDays = 0;
            try
            {

                DateTime taskStarted = startDateTime.Date;
                DateTime taskCompleted = endDateTime.Date;

                DateTime date = taskStarted.Date;
                DateTime datePro = taskStarted.Date;

                for (int i = 0; date.AddDays(i) < taskCompleted.Date; i++)
                {
                    datePro = date.AddDays(i);
                    erroLog += " datePro " + datePro.ToString();
                    if (!IsNotWorkingday(datePro))
                    {
                        spentDays++;

                    }
                }

            }
            catch (Exception ex)
            {
                erroLog += " error " + ex.Message.ToString();
                erroLog += " StackTrace " + ex.StackTrace.ToString();

                File.AppendAllText(@"D:\Error\CSRM.txt", erroLog + Environment.NewLine);

            }


            return spentDays;
        }
        public double GetTaskSpentTime(DateTime startDateTime, DateTime endDateTime)
        {
            double totMint = 0;
            try
            {

                if (startDateTime < endDateTime)
                    totMint += GetDifMinutes(endDateTime, startDateTime);
                if (totMint > 0)
                {
                    totMint = totMint / 60;
                    totMint = Math.Round(totMint, 2);
                }
            }
            catch (Exception ex)
            {

            }

            return totMint;
        }

        public double GetDifMinutes(DateTime a, DateTime b)
        {
            double res = 0;
            TimeSpan dif = new TimeSpan();
            dif = a - b;
            res = dif.TotalMinutes;
            if (res < 0)
                res = 0;
            return res;
        }

        public bool IsNotWorkingday(DateTime date)
        {
            bool res = false;

            if (date.DayOfWeek == DayOfWeek.Saturday)
                return true;
            else if (date.DayOfWeek == DayOfWeek.Sunday)
                return true;
            else
            {
                //HolidaySQLService _holidatService = new HolidaySQLService();
                //int year = date.Year;
                //int month = date.Month;
                //int day = date.Day;
                //if (_holidatService.IsHoliday(year, month, day))
                //{
                //    return true;
                //}

                //HolidayService _holidayService = new HolidayService();
                //if (_holidayService.IsHoliday(date.Date))
                //    return true;

                    return false;

            }

            return res;
        }

        public double GetWorkingDays(DateTime startDateTime, DateTime endDateTime)
        {
            //Date Time Format to (dd/MM/yyyy)
            ConvertDateTimeFormat();

            double workingDays = 0;

            try
            {

                DateTime taskStarted = Convert.ToDateTime(startDateTime);
                DateTime taskCompleted = Convert.ToDateTime(endDateTime);

                DateTime date = taskStarted.Date;
                DateTime datePro = taskStarted.Date;

                for (int i = 0; date.AddDays(i) <= taskCompleted.Date; i++)
                {
                    datePro = date.AddDays(i);
                    if (!IsNotWorkingday(datePro))
                    {
                        workingDays++;
                    }
                }

            }
            catch (Exception ex)
            {
            }

            return workingDays;
        }

        public double GetWorkingHours(DateTime startDateTime, DateTime endDateTime)
        {
            //Date Time Format to (dd/MM/yyyy)
            ConvertDateTimeFormat();

            double workingDays = GetWorkingDays(startDateTime, endDateTime);


            try
            {

                return workingDays * 8.5;

            }
            catch (Exception ex)
            {
            }


            return 0;
        }


    }
}

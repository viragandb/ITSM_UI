using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace Web.Utility
{
    public class UserDateTime
    {
        public static DateTime GetUserDate()
        {
            //return DateTime.UtcNow;

         

            //return DateTime.Now;
           return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Sri Lanka Standard Time");

        }

        public static DateTime ConvertToAppDateTime(string dateTime)
        {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = culture;

            return Convert.ToDateTime(dateTime);

        }

        //public static void ConvertDateTimeFormat()
        //{
        //    CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        //    culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        //    Thread.CurrentThread.CurrentCulture = culture;

        //}
        //public static DateTime ConvertToAppDateTime(DateTime dateTime)
        //{
        //    CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        //    culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        //    Thread.CurrentThread.CurrentCulture = culture;

        //    return Convert.ToDateTime(dateTime);

        //}

        public static DateTime GetUserDateOnly()
        {
          
            //return DateTime.UtcNow;
            //return DateTime.Today;
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Today, "Sri Lanka Standard Time");

        }



        //public static Collection<DayOfWeekVM> GetDaysOfWeek()
        //{
        //    Collection<DayOfWeekVM> _days = new Collection<DayOfWeekVM>();

        //    for (int i = 0; i < 7; i++)
        //    {
        //        DayOfWeekVM m1 = new DayOfWeekVM();
        //        m1.Day = Enum.GetName(typeof(DayOfWeek), i).ToString();
        //        m1.DayNo = i;
        //        _days.Add(m1);
        //    }

        //    return _days;
        //}


        //public static Collection<DayOfMonthVM> GetDaysOfMonth()
        //{
        //    Collection<DayOfMonthVM> _months = new Collection<DayOfMonthVM>();

        //    for (int i = 1; i < 29; i++)
        //    {
        //        DayOfMonthVM m1 = new DayOfMonthVM();
        //        m1.Day = i.ToString();
        //        m1.DayNo = i;
        //        _months.Add(m1);
        //    }



        //    return _months;
        //}
    }
}
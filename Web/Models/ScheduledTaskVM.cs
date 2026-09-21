using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ScheduledTaskVM : ScheduledTask
    {


        public virtual IList<DayOfWeekVM> WeekDays { get; set; }
        public virtual IList<DayOfMonthVM> MonthDays { get; set; }
        public virtual ICollection<ScheduledDate> Dates { get; set; }


    }
}
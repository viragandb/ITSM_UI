using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RA
{
    public enum PlannedSheduleEnum : int
    {

        [Display(Name = "Weekends")]
        Weekends = 1,

        [Display(Name = "After Hours")]
        AfterHours = 2,

        [Display(Name = "On Call")]
        OnCall = 3,

        [Display(Name = "Month End")]
        MonthEnd = 4,

        [Display(Name = "Customer Visits")]
        CustomerVisits = 5,

        [Display(Name = "Medical")]
        Medical = 6,

        [Display(Name = "Personal Reasons")]
        PersonalReasons = 7,
    }
}

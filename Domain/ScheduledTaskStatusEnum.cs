using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum ScheduledTaskStatusEnum : int
    {


        [Display(Name = "Pending")]
        Pending = 1,

        [Display(Name = "Pending Approval")]
        Completed = 2,

        [Display(Name = "Completed")]
        Approved = 3,

        //[Display(Name = "Delayed")]
        //Delayed = 6,

        [Display(Name = "Rejected")]
        Rejected = 8,
    }
}

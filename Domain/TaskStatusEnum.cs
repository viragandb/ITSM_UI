using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum TaskStatusEnum : int
    {


        [Display(Name = "Pending")]
        Pending = 1,

        [Display(Name = "Approved")]
        Approved = 2,

        [Display(Name = "Assigned")]
        Assigned = 3,

        [Display(Name = "Completed")]
        Completed = 8,

        [Display(Name = "Rejected")]
        Rejected = 10

    }
}

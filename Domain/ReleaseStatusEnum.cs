using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum ReleaseStatusEnum : int
    {
        [Display(Name = "Pending")]
        Pending = 1,

        [Display(Name = "Planned")]
        Planned = 2,

        [Display(Name = "Completed")]
        Completed = 5,

        [Display(Name = "Reviewed")]
        Reviewed = 7,
        [Display(Name = "Rejected")]
        Rejected = 9
    }
}

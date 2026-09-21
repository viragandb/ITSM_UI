using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum ChangeStatusEnum : int
    {
        [Display(Name = "Pending")]
        Pending = 1,

        [Display(Name = "Planned")]
        Planned = 2,

        [Display(Name = "Reviewed")]
        Reviewed = 5,

        [Display(Name = "Approved")]
        Approved = 6,
        [Display(Name = "Rejected")]
        Rejected = 9
    }
}

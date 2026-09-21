using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum AssetTransStatusEnum : int
    {
        [Display(Name = "Initiated")]
        Initiated = 1,

        [Display(Name = "In Transit")]
        InTransit = 2,

        [Display(Name = "Sent To Vendor")]
        Vendor = 5,

        [Display(Name = "Completed")]
        Completed = 10,

        [Display(Name = "Rejected")]
        Rejected = 15,


    }
}

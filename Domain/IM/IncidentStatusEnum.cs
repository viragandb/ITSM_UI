using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IM
{
    public enum IncidentStatusEnum : int
    {
       

        [Display(Name = "Pending")]
        Pending = 1,
        //********** In Progress


        [Display(Name = "In Progress")]
        InProgress = 10,


        //*********** Vender

        [Display(Name = "Escalated To Vendor")]
        Vendor = 20,


        //******
        [Display(Name = "Completed")]
        Completed = 25,

        [Display(Name = "Reviewed")]
        Reviewed = 28,

        [Display(Name = "Closed")]
        Closed = 30,

        [Display(Name = "Rejected")]
        Rejected = 40
    }
}

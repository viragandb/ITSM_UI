using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum TicketFlowTypeEnum : int
    {
        [Display(Name = "Initiate")]
        Initiate = 1,

        [Display(Name = "Level 01")]
        Level01 = 5,

        [Display(Name = "Vendor")]
        Vendor = 20,

        //[Display(Name = "Vendor")]
        //Vendor = 20,

        //[Display(Name = "Audit")]


        [Display(Name = "Closed")]
        Closed = 50,

    }
}

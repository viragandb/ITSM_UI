using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum EventStatusEnum : int
    {
        [Display(Name = "Opened")]
        Opened = 1,

        [Display(Name = "Closed")]
        Closed = 2,
       
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum DowntimeTypeEnum : int
    {
        [Display(Name = "Planned")]
        Planned = 1,

        [Display(Name = "Unplanned")]
        Unplanned = 2

      
    }
}

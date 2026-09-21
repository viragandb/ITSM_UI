using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{

    public enum RepeatTypeEnum : int
    {
        [Display(Name = "Never")]
        Never = 1,

        //[Display(Name = "Day")]
        //Day = 2,

        [Display(Name = "Week")]
        Week = 3,

        [Display(Name = "Month")]
        Month = 4,
        [Display(Name = "Year")]
        Year = 5
    }
}

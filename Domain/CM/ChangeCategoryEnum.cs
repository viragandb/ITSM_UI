using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CM
{
    public enum ChangeCategoryEnum : int
    {

        [Display(Name = "Minor")]
        Minor = 1,

        [Display(Name = "Significant")]
        Significant = 2,

        [Display(Name = "Major")]
        Major = 3,

        [Display(Name = "Emergency")]
        Emergency = 4

    }
}

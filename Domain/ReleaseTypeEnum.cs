using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum ReleaseTypeEnum : int
    {
        [Display(Name = "Minor")]
        Minor = 1,

        [Display(Name = "Standard")]
        Standard = 2,

        [Display(Name = "Major")]
        Major = 3,

        [Display(Name = "Emergency")]
        Emergency = 4
    }
}

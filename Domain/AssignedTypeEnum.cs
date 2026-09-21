using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum AssignedTypeEnum : int
    {

        [Display(Name = "Permanent")]
        Permanent = 1,

        [Display(Name = "Temporary")]
        Temporary = 2
    }
}

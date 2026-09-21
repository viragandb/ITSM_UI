using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum BackupTypeEnum : int
    {

        [Display(Name = "Daily")]
        Daily = 1,

        //[Display(Name = "Weekly")]
        //Weekly = 2,

        [Display(Name = "Monthly")]
        Monthly = 3,
        [Display(Name = "Yearly")]
        Yearly = 4
    }
}

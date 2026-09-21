using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum MaintenanceFrequencyEnum : int
    {
        [Display(Name = "N/A")]
        N_A = 0,

        [Display(Name = "Weekly")]
        Weekly = 1,

        [Display(Name = "Monthly")]
        Monthly = 2,

        [Display(Name = "Annually")]
        Annually = 3

    }
}

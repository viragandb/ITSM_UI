using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
     public enum RateTypeEnum : int
    {
        //[Display(Name = "Extremely Satisfied")]
        //Rate5 = 5,
        //[Display(Name = "Satisfied")]
        //Rate4 = 4,
        //[Display(Name = "Neutral")]
        //Rate3 = 3,
        //[Display(Name = "Unsatisfied")]
        //Rate2 = 2,
        //[Display(Name = "Extremely Unsatisfied")]
        //Rate1 = 1

        [Display(Name = "Excellent")]
        Rate5 = 5,
        [Display(Name = "Good")]
        Rate4 = 4,
        [Display(Name = "Average")]
        Rate3 = 3,
        [Display(Name = "Needs Improvement")]
        Rate2 = 2,
        [Display(Name = "Poor")]
        Rate1 = 1


    }
}

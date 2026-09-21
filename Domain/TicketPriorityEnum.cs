using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum TicketPriorityEnum:int
    {
        [Display(Name = "Very Low")]
        VeryLow = 1,

        [Display(Name = "Low")]
        Low = 2,
        [Display(Name = "Medium")]
        Medium = 3,

        [Display(Name = "High")]
        High = 4,

        [Display(Name = "Very High")]
        VeryHigh = 5
    }
}

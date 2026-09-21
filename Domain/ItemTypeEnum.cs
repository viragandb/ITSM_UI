using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum ItemTypeEnum : int
    {
        [Display(Name = "Ticket")]
        Ticket = 1,

        [Display(Name = "Problem")]
        Problem = 2,

        [Display(Name = "Change")]
        Change = 3,

        [Display(Name = "Release")]
        Release = 4,

        [Display(Name = "Work Task")]
        Task = 5,

        [Display(Name = "Event")]
        Event = 6,

        [Display(Name = "GRN")]
        GRN = 10
    }
}

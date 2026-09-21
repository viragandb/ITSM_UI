using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RA
{
    public enum ClarificationType:int
    {
        [Display(Name = "Requester")]
        Requester = 1,

        [Display(Name = "Supervisor")]
        Supervisor = 2,

        [Display(Name = "AVPOrVP")]
        AVPOrVP = 3,

        [Display(Name = "Team")]
        Team = 4
    }
}

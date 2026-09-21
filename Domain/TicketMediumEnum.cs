using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum TicketMediumEnum : int
    {
        [Display(Name = "User")]
        User = 1,

        [Display(Name = "Call")]
        Call = 2,

        [Display(Name = "Email")]
        Email = 3,

        [Display(Name = "Bot")]
        Bot = 4
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum TicketTypeEnum : int
    {
        [Display(Name = "Service Request")]
        SR = 1,

        [Display(Name = "Interruption")]
        IN = 2
    }
}

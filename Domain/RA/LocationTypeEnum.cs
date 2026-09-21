using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RA
{
    public enum LocationTypeEnum : int
    {
        [Display(Name = "Home")]
        Home = 1,

        [Display(Name = "Co-working space")]
        Coworkingspace = 2,

        [Display(Name = "Boarding Place")]
        BoardingPlace = 3,

        [Display(Name = "Rented")]
        Rented = 4,

        [Display(Name = "Accommodation")]
        Accommodation = 5,

        [Display(Name = "Other")]
        Other = 6,

        
    }
}

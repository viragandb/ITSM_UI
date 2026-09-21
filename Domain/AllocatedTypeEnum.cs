using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum AllocatedTypeEnum : int
    {
        [Display(Name = "Internal")]
        Internal = 1,
        [Display(Name = "External")]
        External = 2

       
    }
}

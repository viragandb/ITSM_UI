using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum SystemEnvironmentTypeEnum : int
    {
        //[Display(Name = "Live")]
        //Live = 1,
        [Display(Name = "Support")]
        Support = 2,
        [Display(Name = "Staging")]
        Staging = 3,
        //[Display(Name = "Test")]
        //Test  = 4,
    }
}

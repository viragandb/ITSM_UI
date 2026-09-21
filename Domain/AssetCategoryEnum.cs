using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum AssetCategoryEnum : int
    {
        [Display(Name = "Hardware")]
        ITAsset = 1,

        [Display(Name = "Application/System")]
        System = 2,

        [Display(Name = "Software")]
        Software = 4,

        //[Display(Name = "Website")]
        //Website = 6,

        [Display(Name = "Network/Telco")]
        Telco = 3,

        //[Display(Name = "Data Center")]
        //DataCenter = 5,

        [Display(Name = "Service")]
        Service = 8

        
    }
}

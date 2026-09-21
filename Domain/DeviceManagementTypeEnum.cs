using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum DeviceManagementTypeEnum : int
    {

        [Display(Name = "Managed")]
        Managed = 0,

        [Display(Name = "Unmanaged")]
        Unmanaged = 1
    }
}

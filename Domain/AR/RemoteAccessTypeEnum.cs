using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
     public enum RemoteAccessTypeEnum : int
    {

        [Display(Name = "Global Protect")]
        GlobalProtect = 1,

        [Display(Name = "Citrix")]
        Citrix = 2

    }
}

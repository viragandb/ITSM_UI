using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public enum UserAccessTypeEnum : int
    {

        [Display(Name = "New Id")]
        NewId= 1,

        [Display(Name = "Renew Id")]
        RenewId= 2,
        [Display(Name = "Existing User")]
        ExistingUser = 3,
    }
}

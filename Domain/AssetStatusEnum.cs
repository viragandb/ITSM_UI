using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum AssetStatusEnum : int
    {
        [Display(Name = "Opened")]
        Opened = 1,

        [Display(Name = "Unassigned")]
        Unassigned = 2,


        [Display(Name = "Assigned")]
        Assigned = 4,

        [Display(Name = "In Transit")]
        InTransit = 5,

        [Display(Name = "Under Repair")]
        UnderRepair = 6,

        [Display(Name = "Out Of Service")]
        OutOfService = 15,

        [Display(Name = "To Be Disposed")]
        ToBeDisposed = 18,

        [Display(Name = "Disposed")]
        Disposed = 20,
    }
}

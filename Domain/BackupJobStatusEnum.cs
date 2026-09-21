using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
      public enum BackupJobStatusEnum : int
    {
        [Display(Name = "Pending ")]
        Pending= 1,

        [Display(Name = "Completed")]
        Completed = 2,

        [Display(Name = "Assigned")]
        Finalized = 3,

        [Display(Name = "Reviewed")]
        Reviewed = 4,

        [Display(Name = "Approved")]
        Approved = 5,
        [Display(Name = "Rejected")]
        Rejected = 9
    }

}

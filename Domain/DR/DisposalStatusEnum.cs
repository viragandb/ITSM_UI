using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DR
{
    public enum DisposalStatusEnum : int
    {

        [Display(Name = "Initiated")]
        Initiated = 1,

        [Display(Name = "Pending Approval")]
        PendingApproval = 2,

        [Display(Name = "Approved")]
        Approved = 3,
        
        [Display(Name = "In Transit")]
        InTransit = 4,
        
        [Display(Name = "Completed")]
        Completed = 5,

        [Display(Name = "Return")]
        Return = 6,

        [Display(Name = "Reject")]
        Reject = 7,

        [Display(Name = "Amend and Resubmit")]
        AmendAndResubmit = 8,


    }
}

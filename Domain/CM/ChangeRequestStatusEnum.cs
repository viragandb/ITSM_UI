using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CM
{
    public enum ChangeRequestStatusEnum : int
    {

        [Display(Name = "Initiated")]
        Initiated = 1,


        [Display(Name = "Pending")]
        Pending = 5,

        [Display(Name = "Pending Approval")]
        PendingApproval = 10,

        [Display(Name = "Pending Approval AVP")]
        PendingApprovalAVP = 15,

        [Display(Name = "Pending Approval VP")]
        PendingApprovalVP = 16,

        [Display(Name = "Pending Approval CMC")]
        PendingApprovalCMC = 17,

        [Display(Name = "Approved")]
        Approved = 20,

        [Display(Name = "Completed")]
        Completed = 25,


        [Display(Name = "Closed")]
        Closed = 30,

        [Display(Name = "Rejected")]
        Rejected = 35,



    }
}

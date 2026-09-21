using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DM
{
    public enum DeviceManagementStatusEnum : int
    {
    
        [Display(Name = "Pending Approval")]
        Pending = 1,

        [Display(Name = "Pending IT Approval")]
        PendingApprovalIT = 5,

        [Display(Name = "Pending Approval AVP")]
        PendingApprovalAVP = 10,

        [Display(Name = "Pending Approval VP")]
        PendingApprovalVP = 15,

        [Display(Name = "Approved")]
        Approved = 20,

        [Display(Name = "Completed")]
        Completed = 25,


        [Display(Name = "Rejected")]
        Rejected = 35,
    }
}

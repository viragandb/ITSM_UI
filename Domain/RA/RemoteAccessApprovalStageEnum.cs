using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RA
{
    public enum RemoteAccessApprovalStageEnum : int
    {
        [Display(Name = "Supervisor Approval Stage")]
        SupervisorApprovalStage = 1,

        [Display(Name = "VP/AVP Approval Stage")]
        VPOrAVPApprovalStage = 2,

        [Display(Name = "Team Approval Stage")]
        TeamApprovalStage = 3,

        [Display(Name = "Completed Stage")]
        CompletedStage = 4,

        [Display(Name = "Rejected Stage")]
        RejectedStage = 5,
        
        [Display(Name = "Revoked Stage")]
        RevokedStage = 6,
        
        [Display(Name = "Expired Stage")]
        ExpiredStage = 7,
    }
    
    
}

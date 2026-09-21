using Domain.AR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RA
{
    public class RemoteAccessRequest
    {
        public long RemoteAccessRequestId { get; set; }


        [Display(Name = "Department")]
        public long DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }


        [Display(Name = "Location")]
        public long BranchId { get; set; }
        public virtual Branch Branch { get; set; }


        [Required]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User CreatedUser { get; set; }


        [Required]
        [Display(Name = "Requested User")]
        public string RequestedBy { get; set; }
        [ForeignKey("RequestedBy")]
        public virtual User RequestedUser { get; set; }


        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; }

        //storing supervisor
        [Display(Name = "Approved Supervisor")]
        public string ApprovedSupervisor { get; set; }
        [ForeignKey("ApprovedSupervisor")]
        public virtual User ApprovedSupervisorUser { get; set; }


        //storing avp/vp
        [Display(Name = "Approved AVP/VP")]
        public string ApprovedAVPOrVP { get; set; }
        [ForeignKey("ApprovedAVPOrVP")]
        public virtual User ApprovedAVPOrVPUser { get; set; }


        //final approver
        [Required]
        [Display(Name = "Final Approval User")]
        public string FinalApprovalBy { get; set; }
        [ForeignKey("FinalApprovalBy")]
        public virtual User FinalApprovalByUser { get; set; }


        [Required]
        [Display(Name = "Business Justification")]
        public String BusinessJustification { get; set; }


        [Required]
        [Display(Name = "Business Impact")]
        public String BusinessImpact { get; set; }


        [Display(Name = "Planned Schedule")]
        public PlannedSheduleEnum PlannedSchedule { get; set; }


        [Display(Name = "Asset")]
        public long AssetId { get; set; }
        public virtual Asset Asset { get; set; }//Laptop/PC Number
        

        //Address
        [Display(Name = "Remote Work Location")]
        public String RemoteWorkLocation{ get; set; }


        [Display(Name = "Access Type ")]
        public RemoteAccessTypeEnum RemoteAccessType { get; set; }


        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }


        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }


        //AM Below Requster
        [Display(Name = "Below AM Justification")]
        public String BelowAMJustification { get; set; }


        [Display(Name = "Tolerance Limit Exceeded Justification")]
        public String ToleranceLimitExceededJustification { get; set; }


        public bool IsToleranceLimitExceeded { get; set; }


        [Display(Name = "Reason For HR Clarification")]
        public string ReasonForHRClarification { get; set; }



        [Display(Name = "HR Clarification")]
        public string HRClarification { get; set; }


        public bool? IsClarificationNeeded { get; set; } 



        [Display(Name = "Status")]
        public AccessRequestStatusEnum Status { get; set; }


        public RemoteAccessApprovalStageEnum? ApprovalStage { get; set; }



        [Display(Name = "Pending Team")]//current Team
        public long? TeamId { get; set; }
        public virtual Team Team { get; set; }



        // team requesting clarification
        [Display(Name = "Clarification Requested By")]
        public long? ClarificationRequestedByTeamId { get; set; }


        public virtual Team ClarificationRequestedByTeam { get; set; }


        // team responding to clarification
        [Display(Name = "Clarification Assigned To")]
        public long? ClarificationAssignedToTeamId { get; set; }


        public virtual Team ClarificationAssignedToTeam { get; set; }


        [Display(Name = "Clarification Type")]
        public ClarificationType? ClarificationType { get; set; }


        [Display(Name = "Resolve Time")]
        public double Resolve { get; set; }


        [Display(Name = "Spent Time")]
        public double SpentTime { get; set; }

        public double SpentTimeStatus { get; set; }
        public DateTime SpentTimeSync { get; set; }



        public bool IsTimeViolated { get; set; }

        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public bool IsAddedToGroup { get; set; }
        public bool IsWifiAccessAllow { get; set; }
        public bool IsCertificateProvided { get; set; }


        public bool IsRequesterAVPOrVP { get; set; }

        public virtual ICollection<RemoteAccessRequestUpdate> RemoteAccessRequestUpdates { get; set; }

        // One request → many logs
        public virtual ICollection<RemoteAccessRequestLog> RemoteAccessRequestLogs { get; set; }

        //requests -> required systems
        public virtual ICollection<RemoteAccessRequiredSystem> RemoteAccessRequiredSystems { get; set; }

        public virtual ICollection<RemoteAgreement> RemoteAgreements { get; set; }

    }
}

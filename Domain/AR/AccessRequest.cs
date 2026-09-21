using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class AccessRequest
    {
        public long AccessRequestId { get; set; }

        [Required]
        [Display(Name = "Request Type")]
        public long AccessRequestTypeId { get; set; }
        public virtual AccessRequestType AccessRequestType { get; set; }

        [Required]
        [Display(Name = "Workflow")]
        public long WorkflowId { get; set; }
        public virtual Workflow Workflow { get; set; }

        [Display(Name = "Next Workflow Level")]
        public int NextWorkflowlevelNo { get; set; }

        [Display(Name = "Level Type")]
        public WorkflowLevelTypeEnum WorkflowLevelType { get; set; }

        [Display(Name = "Team")]
        public long? TeamId { get; set; }
        public virtual Team Team { get; set; }

        [Display(Name = "Status")]
        public AccessRequestStatusEnum Status { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Justification")]
        public string Justification { get; set; }

        [Display(Name = "Location")]
        public long BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        
        [Display(Name = "Department")]
        public long DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User CreatedUser { get; set; }

        [Required]
        [Display(Name = "Requested User")]
        public string RequestedBy { get; set; }
        [ForeignKey("RequestedBy")]
        public virtual User RequestedUser { get; set; }


        [Required]
        [Display(Name = "Approval User")]
        public string ApprovalBy { get; set; }
        [ForeignKey("ApprovalBy")]
        public virtual User ApprovalByUser { get; set; }

        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; }


        [Display(Name = "Duration")]
        public AssignedTypeEnum DurationType { get; set; }

        [Display(Name = "Expire Date")]
        public DateTime ExpireDate { get; set; }

        [Display(Name = "User Type")]
        public AllocatedTypeEnum AccessType { get; set; }

        [Display(Name = "User Name")]
        public string ExternalUser { get; set; }

        [Display(Name = "Legal ID (NIC/Passport No.)")]
        public string LegalId { get; set; }

        [Display(Name = "Organization")]
        public string Organization { get; set; }

        [Display(Name = "Resolve Time")]
        public double Resolve { get; set; }


        [Display(Name = "Spent Time")]
        public double SpentTime { get; set; }

        public double SpentTimeStatus { get; set; }
        public DateTime SpentTimeSync { get; set; }



        public bool IsTimeViolated { get; set; }


        [Display(Name = "Approval from AVP/VP Attached")]
        public bool IsApprovalTerm { get; set; }

        [Display(Name = "Approval Attachment")]
        public string ApprovalAttachment { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<AccessRequestUpdate> AccessRequestUpdates { get; set; }
        public virtual ICollection<AccessRequestLog> AccessRequestLogs { get; set; }



        [Display(Name = "System Access")]
        public long? SystemAccessId { get; set; }
        [ForeignKey("SystemAccessId")]
        public virtual SystemAccess SystemAccess { get; set; }


        [Display(Name = "User Privilege Access")]
        public long? UserPrivilegeAccessId { get; set; }
        [ForeignKey("UserPrivilegeAccessId")]
        public virtual UserPrivilegeAccess UserPrivilegeAccess { get; set; }


        [Display(Name = "Physical Access")]
        public long? PhysicalAccessId { get; set; }
        [ForeignKey("PhysicalAccessId")]
        public virtual PhysicalAccess PhysicalAccess { get; set; }

        [Display(Name = "User Access")]
        public long? UserAccessId { get; set; }
        [ForeignKey("UserAccessId")]
        public virtual UserAccess UserAccess { get; set; }

        [Display(Name = "Device Access")]
        public long? DeviceAccessId { get; set; }
        [ForeignKey("DeviceAccessId")]
        public virtual DeviceAccess DeviceAccess { get; set; }

        [Display(Name = "Remote Access")]
        public long? RemoteAccessId { get; set; }
        [ForeignKey("RemoteAccessId")]
        public virtual RemoteAccess RemoteAccess { get; set; }

    }
}

using Domain;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class NewAccessRequestVM
    {
        public long AccessRequestId { get; set; }

        [Required]
        [Display(Name = "Request Type")]
        public long AccessRequestTypeId { get; set; }

        public string AccessRequestName { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Business Justification")]
        public string Justification { get; set; }

        [Display(Name = "Location")]
        public long BranchId { get; set; }

        [Display(Name = "Department")]
        public long DepartmentId { get; set; }

        [Display(Name = "Requested User")]
        public string RequestedBy { get; set; }
        public virtual User RequestedUser { get; set; }

        //[Required]
        [Display(Name = "Approval User")]
        public string ApprovalBy { get; set; }
        public string ApprovalByUserName { get; set; }

        [Display(Name = "Duration")]
        public AssignedTypeEnum DurationType { get; set; }

        [Display(Name = "Expire Date")]
        public String ExpireDate { get; set; }

        [Display(Name = "User Type")]
        public AllocatedTypeEnum AccessType { get; set; }
        public int AccessTypeId { get; set; }

        [Display(Name = "User Name")]
        public string ExternalUser { get; set; }

        [Display(Name = "Legal ID (NIC/Passport No.)")]
        public string LegalId { get; set; }

        [Display(Name = "Organization")]
        public string Organization { get; set; }

        [Display(Name = "Instructions")]
        public string Instructions { get; set; }

        //******************* System Access
        public List<SystemAccessItem> SystemAccessItems { get; set; }

        //******************* Firewall Change
        public FirewallChangeAccess FirewallChangeAccess { get; set; }

        //******************* User Privilege Access
        public List<UserPrivilegeAccessItem> UserPrivilegeAccessItems { get; set; }

        //******************* Physical Area Access
        public List<PhysicalAccessItem> PhysicalAccessItems { get; set; }


        //******************* User Access

        [Display(Name = "Legal Id Document")]
        public string LegalIdDoc { get; set; }

        [Display(Name = "Non-Employee Acknowledgement")]
        public string NonEmpAcknowledgementDoc { get; set; }

        [Display(Name = "Type")]
        public UserAccessTypeEnum UserAccessType { get; set; }
        public List<UserAccessItem> UserAccessItems { get; set; }


        //******************* Device Access

        [Display(Name = "Asset")]
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public List<DeviceAccessItem> DeviceAccessItems { get; set; }


        //******************* Remote Access

        [Display(Name = "Business Impact if remote access is not granted")]
        public string Impact { get; set; }

        [Display(Name = "Type")]
        public RemoteAccessTypeEnum RemoteAccessType { get; set; }

        [Display(Name = "Asset (Notebook / PC)")]
        public long RemoteAssetId { get; set; }
        public string RemoteAssetName { get; set; }
        public List<RemoteAccessItem> RemoteAccessItems { get; set; }

    }
}
using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class NewDeviceManagementVM
    {

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }


        [Display(Name = "Requested User")]
        public string RequestedBy { get; set; }
        public virtual User RequestedUser { get; set; }

        [Display(Name = "Approval User")]
        public string ApprovalBy { get; set; }
        public string ApprovalByUserName { get; set; }

        [Display(Name = "Device Management Type")]
        public DeviceManagementTypeEnum DeviceManagementType { get; set; }

        [Display(Name = "Duration")]
        public AssignedTypeEnum DurationType { get; set; }

        [Display(Name = "Expire Date")]
        public String ExpireDate { get; set; }


        [Display(Name = "Asset")]
        public long? AssetId { get; set; }
        public virtual Asset Asset { get; set; }

    }
}
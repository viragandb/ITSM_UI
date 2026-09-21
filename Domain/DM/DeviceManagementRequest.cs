using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DM
{
    public class DeviceManagementRequest
    {
        public long DeviceManagementRequestId { get; set; }

        [Display(Name = "Status")]
        public DeviceManagementStatusEnum Status { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Requested For")]
        public string RequestedFor { get; set; }
        [ForeignKey("RequestedFor")]
        public virtual User RequestedForUser { get; set; }

        [Required]
        [Display(Name = "Requested User")]
        public string RequestedBy { get; set; }
        [ForeignKey("RequestedBy")]
        public virtual User RequestedByUser { get; set; }


        [Required]
        [Display(Name = "Approval User")]
        public string ApprovalBy { get; set; }
        [ForeignKey("ApprovalBy")]
        public virtual User ApprovalByUser { get; set; }

        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; }

        [Display(Name = "Device Management Type")]
        public DeviceManagementTypeEnum DeviceManagementType { get; set; }

        [Display(Name = "Duration")]
        public AssignedTypeEnum DurationType { get; set; }

        [Display(Name = "Expire Date")]
        public DateTime ExpireDate { get; set; }

        [Display(Name = "Asset")]
        public long? AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        //[Required]
        //[Display(Name = "Team")]
        //public long TeamId { get; set; }
        //public virtual Team Team { get; set; }

        
        [Display(Name = "Resolve Time")]
        public double Resolve { get; set; }


        [Display(Name = "Spent Time")]
        public double SpentTime { get; set; }

        public double SpentTimeStatus { get; set; }
        public DateTime SpentTimeSync { get; set; }

        public bool IsTimeViolated { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<DeviceManagementRequestUpdate> DeviceManagementRequestUpdates { get; set; }
        public virtual ICollection<DeviceManagementRequestLog> DeviceManagementRequestLogs { get; set; }
    }
}

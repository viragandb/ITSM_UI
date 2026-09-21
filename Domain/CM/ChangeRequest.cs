using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CM
{
    public class ChangeRequest
    {
        public long ChangeRequestId { get; set; }

        //[Required]
        //[Display(Name = "Change Area")]
        //public long ChangeAreaId { get; set; }
        //public virtual ChangeArea ChangeArea { get; set; }

      

        [Display(Name = "Status")]
        public ChangeRequestStatusEnum Status { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Justification")]
        public string Justification { get; set; }

        [Required]
        [Display(Name = "Change Request Category")]
        public long ChangeRequestCategoryId { get; set; }
        public virtual ChangeRequestCategory ChangeRequestCategory { get; set; }


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

        [Required]
        [Display(Name = "Team")]
        public long TeamId { get; set; }
        public virtual Team Team { get; set; }

        [Display(Name = "Implement User")]
        public string ImplementedBy { get; set; }
        [ForeignKey("ImplementedBy")]
        public virtual User ImplementedByUser { get; set; }

        [Display(Name = "Change Manager")]
        public string ChangeApprovedBy { get; set; }

        [ForeignKey("ChangeApprovedBy")]
        public virtual User ChangeApprovedByUser { get; set; }


        [Display(Name = "Resolve Time")]
        public double Resolve { get; set; }


        [Display(Name = "Spent Time")]
        public double SpentTime { get; set; }

        public double SpentTimeStatus { get; set; }
        public DateTime SpentTimeSync { get; set; }


        public long? ChangeImplementDataId { get; set; }
        [ForeignKey("ChangeImplementDataId")]
        public virtual ChangeImplementData ChangeImplementData { get; set; }

        public bool IsTimeViolated { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<ChangeRequestUpdate> ChangeRequestUpdates { get; set; }
        public virtual ICollection<ChangeRequestLog> ChangeRequestLogs { get; set; }
        public virtual ICollection<ChangeRequestDoc> ChangeRequestDocs { get; set; }
        public virtual ICollection<ChangeRequestAsset> Assets { get; set; }
    }
}


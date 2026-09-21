using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Change
    {
        public long ChangeId { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }


        [Required]
        [Display(Name = "Priority")]
        public PriorityEnum Priority { get; set; }

        [Required]
        [Display(Name = "Change Type")]
        public ChangeTypeEnum ChangeType { get; set; }

        [Required]
        [Display(Name = "Risk")]
        public RiskTypeEnum Risk { get; set; }

        [Display(Name = "Request Type")]
        public long ChangeRequestTypeId { get; set; }

        [Display(Name = "Asset Type")]
        public long AssetTypeId { get; set; }

        [Required]
        [Display(Name = "Planned Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Planned End Date")]
        public DateTime EndDate { get; set; }


        [Display(Name = "Status")]
        public ChangeStatusEnum Status { get; set; }

        public string RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }


        [Display(Name = "Reason")]
        public string Reason { get; set; }

        [Display(Name = "Impact")]
        public string Impact { get; set; }

        [Display(Name = "Roll Out Plan")]
        public string RollOutPlan { get; set; }

        [Display(Name = "Back Out Plan")]
        public string BackOutPlan { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ChangeRequestType ChangeRequestType { get; set; }
        public virtual AssetType AssetType { get; set; }

        [ForeignKey("RequestedBy")]
        public virtual User RequestedUser { get; set; }

        [NotMapped]
        public virtual ICollection<ItemLog> ItemLogs { get; set; }
        [NotMapped]
        public virtual ICollection<ItemDoc> ItemsDocs { get; set; }
        [NotMapped]
        public virtual ICollection<ItemAsset> ItemAssets { get; set; }

        [NotMapped]
        public virtual ICollection<Ticket> Tickets { get; set; }

        [NotMapped]
        public virtual ICollection<Problem> Problems { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Release
    {
        public long ReleaseId { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }


        [Required]
        [Display(Name = "Priority")]
        public PriorityEnum Priority { get; set; }

        [Required]
        [Display(Name = "Release Type")]
        public ReleaseTypeEnum ReleaseType { get; set; }


        //[Display(Name = "Request Type")]
        //public long RequestTypeId { get; set; }

        [Display(Name = "Request Type")]
        public long ChangeRequestTypeId { get; set; }

        [Display(Name = "Asset Type")]
        public long AssetTypeId { get; set; }

        [Required]
        [Display(Name = "Release Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Release End Date")]
        public DateTime EndDate { get; set; }


        [Display(Name = "Status")]
        public ReleaseStatusEnum Status { get; set; }

        public string RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }


        [Display(Name = "Build Plan")]
        public string BuildPlan { get; set; }

        [Display(Name = "Test Plan")]
        public string TestPlan { get; set; }

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
        public virtual ICollection<Change> Changes { get; set; }
    }
}

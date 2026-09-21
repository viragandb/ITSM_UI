using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class AssetVerificationRequest
    {

        public long AssetVerificationRequestId { get; set; }

        [Display(Name = "Location")]
        public long BranchId { get; set; }
        public virtual Branch Branch { get; set; }

        [Display(Name = "Department")]
        public long DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        [Display(Name = "Planned Verification Date")]
        public DateTime VerificationDate { get; set; }

        [Display(Name = "Asset Category")]
        public AssetCategoryEnum AssetCategory { get; set; }

        [Display(Name = "Status")]
        public AssetVerificationStatusEnum Status { get; set; }

        [Required]
        [Display(Name = "Requested By")]
        public string RequestedBy { get; set; }
        [ForeignKey("RequestedBy")]
        public virtual User RequestedUser { get; set; }

        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; }

        [Display(Name = "Verified By")]
        public string VerifiedBy { get; set; }
        [ForeignKey("VerifiedBy")]
        public virtual User VerifiedUser { get; set; }

        [Display(Name = "Verified Date")]
        public DateTime VerifiedDate { get; set; }

        [Display(Name = "Final Comment")]
        public string Comment { get; set; }

        [Display(Name = "Verified Document")]
        public string VerifiedDocName { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<AssetVerificationItem> Assets { get; set; }

    }
}

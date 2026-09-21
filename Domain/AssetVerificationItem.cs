using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class AssetVerificationItem
    {
        public long AssetVerificationItemId { get; set; }

        [Display(Name = "Asset Verification Request")]
        public long AssetVerificationRequestId { get; set; }
        public virtual AssetVerificationRequest AssetVerificationRequest { get; set; }

        [Required]
        [Display(Name = "Asset")]
        public long AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        [Display(Name = "Availability")]
        public bool IsAvailable { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }



    }
}

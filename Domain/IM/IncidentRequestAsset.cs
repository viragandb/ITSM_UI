using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IM
{
    public class IncidentRequestAsset
    {
        public long IncidentRequestAssetId { get; set; }

        [Required]
        public long IncidentRequestId { get; set; }
        public virtual IncidentRequest IncidentRequest { get; set; }

        [Required]
        [Display(Name = "Asset")]
        public long AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}

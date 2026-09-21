using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CM
{
    public class ChangeRequestAsset
    {
        public long ChangeRequestAssetId { get; set; }

        [Required]
        public long ChangeRequestId { get; set; }
        public virtual ChangeRequest ChangeRequest { get; set; }

        [Required]
        [Display(Name = "Asset")]
        public long AssetId { get; set; }
        public virtual Asset Asset { get; set; }
    }
}

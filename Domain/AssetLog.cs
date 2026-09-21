using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class AssetLog
    {
        public long AssetLogId { get; set; }

        [Required]
        [Display(Name = "Asset")]
        public long AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        [Required]
        [Display(Name = "Status")]
        public AssetStatusEnum Status { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }
    }
}

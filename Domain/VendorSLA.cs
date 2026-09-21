using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class VendorSLA
    {
        public long VendorSLAId { get; set; }

        [Display(Name = "Vendor")]
        public long VendorId { get; set; }

        public virtual Vendor Vendor { get; set; }

        [Display(Name = "Asset Type")]
        public long AssetTypeId { get; set; }

        public virtual AssetType AssetType { get; set; }

        [Display(Name = "SLA (Hrs.)")]
        public double SLA { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }




    }
}

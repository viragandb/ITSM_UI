using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.RA;

namespace Domain
{
    public class AssetType
    {
        public long AssetTypeId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string AssetTypeName { get; set; }

        [Display(Name = "Asset Category")]
        public AssetCategoryEnum AssetCategory { get; set; }


        [Display(Name = "Accessory")]
        public bool IsAccessory { get; set; }

        [Display(Name = "Icon")]
        public string Icon { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public virtual ICollection<RequestType> RequestTypes { get; set; }

        [NotMapped]
        public virtual ICollection<SubCategory> SubCategories { get; set; }
        public virtual ICollection<Item> Items { get; set; }

        public virtual ICollection<RemoteAccessRequiredSystem> RemoteAccessRequiredSystems { get; set; }

    }
}

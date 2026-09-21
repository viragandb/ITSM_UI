using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ItemId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string ItemName { get; set; }

        [Display(Name = "Make")]
        public string ItemMake { get; set; }

        [Display(Name = "Bar Code")]
        public string ItemBarCode { get; set; }

        [Display(Name = "Model No")]
        public string ItemModelNo { get; set; }

        [Required]
        [Display(Name = "Serial No")]
        public string ItemSerialNo { get; set; }

        [Required]
        [Display(Name = "Fixed Asset No")]
        public string ItemFixedAssetNo { get; set; }

        [Display(Name = "Asset Cost")]
        public string ItemCost { get; set; }

        [Display(Name = "Purchase Date")]
        public DateTime? ItemPurchaseDate { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime? ItemExpiryDate { get; set; }

        [Display(Name = "Input Date")]
        public DateTime? ItemInputDate { get; set; }


        public long CompanyId { get; set; }
        public long LocationId { get; set; }
        public long DepartmentId { get; set; }
        public long VendorId { get; set; }
        public long AssetTypeId { get; set; }

        [NotMapped]
        [Display(Name = "Category")]
        public long CategoryId { get; set; }


        public virtual Department Department { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual AssetType AssetType { get; set; }

    }
}

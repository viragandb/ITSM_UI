using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ItemVM
    {
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

        [Required]
        [Display(Name = "Location Name")]
        public string LocationName { get; set; }

        [Required]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }
        [Required]
        [Display(Name = "Remark")]
        public string Remark { get; set; }

        public long TransferLocationId { get; set; }
        public long TransferDepartmentId { get; set; }


    }
}
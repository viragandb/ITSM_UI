using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AssetVM
    {
        public long AssetId { get; set; }
              

        [Display(Name = "Asset Category")]
        public long AssetCategoryId { get; set; }
        public string AssetCategoryName { get; set; }

        [Display(Name = "Asset Type")]
        public long AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }


        [Display(Name = "Make")]
        public long AssetMakeId { get; set; }
        public string AssetMakeName { get; set; }

        [Display(Name = "Model")]
        public string ModelName { get; set; }



        [Required]
        [Display(Name = "Asset #")]
        public string AssetNo { get; set; }

        [Display(Name = "Barcode")]
        public string Barcode { get; set; }

        [Display(Name = "Serial #")]
        public string SerialNo { get; set; }


        [Display(Name = "Purchased Price")]
        public double PurchasePrice { get; set; }


        [Display(Name = "Warranty")]
        public int WarrantyPeriod { get; set; }

        [Display(Name = "To Be Returned")]
        public int ToBeReturned { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Critical")]
        public bool IsCritical { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedToUserId { get; set; }
        public string AssignedToUserName { get; set; }
        public string AssetName { get; set; }

        public string Availability { get; set; }
        public string Comment { get; set; }

    }
}
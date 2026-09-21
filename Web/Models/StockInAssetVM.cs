using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class StockInAssetVM
    {
        [Required]
        [Display(Name = "GRN Number")]
        public string GRNNumber { get; set; }

        [Required]
        [Display(Name = "PO Number")]
        public string PONumber { get; set; }

        [Required]
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

        [Required]
        [Display(Name = "Purchase Date")]
        public string PurchaseDate { get; set; }

        [Required]
        [Display(Name = "Received Date")]
        public string ReceivedDate { get; set; }

        [Display(Name = "Comment")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Vendor")]
        public long VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }

        [Display(Name = "Ownership")]
        public AllocatedTypeEnum AllocatedType { get; set; }
        public List<TempAsset> Assets { get; set; }


    }
}
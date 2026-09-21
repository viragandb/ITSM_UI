using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class GRNote
    {
        public long GRNoteId { get; set; }


        [Required]
        [Display(Name = "GRN Number")]
        public string GRNNumber { get; set; }

        [Required]
        [Display(Name = "PO Number")]
        public string PONumber { get; set; }


        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }

        [Display(Name = "Received Date")]
        public DateTime ReceivedDate { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Vendor")]
        public long VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }


        public virtual ICollection<Asset> Assets { get; set; }

        [NotMapped]
        public virtual ICollection<ItemDoc> ItemDocs { get; set; }


        
    }
}

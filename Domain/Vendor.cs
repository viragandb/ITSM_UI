using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Vendor
    {

        public long VendorId { get; set; }


        [Required]
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }

        [Display(Name = "Contact No.")]
        public string ContactNo { get; set; }


        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<Item> Items { get; set; }

    }
}

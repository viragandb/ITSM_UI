using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Category
    {
        public long CategoryId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string CategoryName { get; set; }

        [Display(Name = "Icon")]
        public string Icon { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<RequestType> RequestTypes { get; set; }
        [NotMapped]
        public virtual ICollection<AssetType> AssetTypes { get; set; }

    }
}

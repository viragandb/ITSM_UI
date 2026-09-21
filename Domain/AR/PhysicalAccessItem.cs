using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class PhysicalAccessItem
    {
        public long PhysicalAccessItemId { get; set; }

        [Display(Name = "Physical Access")]
        public long PhysicalAccessId { get; set; }
        public virtual PhysicalAccess PhysicalAccess { get; set; }

        [Required]
        [Display(Name = "Physical Area")]
        public long PhysicalAreaId { get; set; }
        public virtual PhysicalArea PhysicalArea { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}

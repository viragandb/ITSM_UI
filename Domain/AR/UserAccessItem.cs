using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class UserAccessItem
    {
        public long UserAccessItemId { get; set; }

        [Display(Name = "User Access")]
        public long UserAccessId { get; set; }
        public virtual UserAccess UserAccess { get; set; }

        [Required]
        [Display(Name = "User Access Type")]
        public long UserAccessItemTypeId { get; set; }
        public virtual UserAccessItemType UserAccessItemType { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Justification")]
        public string Justification { get; set; }

      
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
